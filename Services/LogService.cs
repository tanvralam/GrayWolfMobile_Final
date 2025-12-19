using Acr.UserDialogs;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Extensions;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using GrayWolf.Utility;
using GrayWolf.Views.Popups;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;

using RGPopup.Maui.Extensions;

namespace GrayWolf.Services
{

    public class LogService : ILogService
    {
        public const string PhotoExtension = ".jpg";
        public const string VideoExtension = ".mp4";
        private const string TAG = "LogService";

        private bool AppendSnapshot { get; set; }

        public LogStatus Status { get; private set; }

        public bool IsLogging { get; private set; }

        private bool AnyDeviceSelected { get; set; }

        public int LogFileId => SettingsService.LogFileId;

        private List<DeviceUpdateMsg> latestUpdates;

        public LogFile CurrentFile { get; private set; }

        private LoggerJSON LoggerJSON { get; set; }

        #region services
        private GrayWolf.Interfaces. IFileSystem FileSystem { get; }

        private IAnalyticsService AnalyticsService { get; }

        private static Ioc Container => Ioc.Default;

        public static ILogService Instance => Container.GetService<ILogService>();

        private IAlertSoundService AlertSound => Container.GetService<IAlertSoundService>();

        private IDatabase Database { get; }

        private INavigationService NavigationService { get; }

        private ISettingsService SettingsService { get; }

        private IGeolocationService GeolocationService { get; }

        private IDeviceService DeviceRefreshService { get; }

        private ISensorsService SensorsService { get; }

        private IAlertService AlertService { get; }

        private IUserDialogs UserDialogs { get; }
        #endregion

        public LogService()
        {
            NavigationService = Services.NavigationService.Instance;
            SettingsService = Services.SettingsService.Instance;
            GeolocationService = Container.GetService<IGeolocationService>();
            Database = Container.GetService<IDatabase>();
            DeviceRefreshService = Container.GetService<IDeviceService>();
             FileSystem = Container.GetService<GrayWolf.Interfaces.IFileSystem>();
            AnalyticsService = Container.GetService<IAnalyticsService>();
            SensorsService = Container.GetService<ISensorsService>();
            AlertService = Container.GetService<IAlertService>();
            Messenger.Default.Register<SelectedDevicesMessage>(this, OnSelectedDevicesMessage);
            Messenger.Default.Register<LogButtonMessage>(this, OnDeviceLogButtonClicked);
            UserDialogs= Container.GetService<IUserDialogs>();
            InitSelectedDevices();
            InitCurrentFile();
        }

        private async void InitSelectedDevices()
        {
            //var devices = await DeviceRefreshService.GetSelectedDevicesAsync();
            //OnSelectedDevicesMessage(devices);
        }

        private async void InitCurrentFile()
        {
            if (LogFileId > 0)
            {
                CurrentFile = await GetLogFileAsync(LogFileId);
            }
        }

        public async Task<LogFile> GetLogFileAsync(int id)
        {
            try
            {
                var dbo = await Database.GetItemAsync<LogFileDBO>(id);
                return dbo.ToLogFile(FileSystem);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
                throw ex;
            }
        }

        public async Task<List<LogFile>> GetLogFilesAsync()
        {
            try
            {
                var files = await Database.GetItemsAsync<LogFileDBO>();
                return files.Select(x => x.ToLogFile(FileSystem)).ToList();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
                throw ex;
            }
        }

        public async Task SetFileAsync(int selectedLogId)
        {
            try
            {
                AppendSnapshot = CurrentFile?.Id == selectedLogId && AppendSnapshot;
                var logs = await Database.GetItemsAsync<LogFileDBO>();
                foreach (var log in logs)
                {
                    log.IsSelected = log.Id == selectedLogId;
                }
                await Database.UpdateAllAsync(logs);
                Services.SettingsService.Instance.LogFileId = selectedLogId;
                CurrentFile = await GetLogFileAsync(selectedLogId);

                UpdateStatus();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
        }

        public async Task SetFileAsync(LogFile file)
        {
            AppendSnapshot = CurrentFile?.Id == file.Id && AppendSnapshot;
            var logs = await Database.GetItemsAsync<LogFileDBO>();
            logs.RemoveAll(x => x.Id == file.Id);
            foreach (var log in logs)
            {
                log.IsSelected = false;
            }
            await Database.UpdateAllAsync(logs);
            Services.SettingsService.Instance.LogFileId = file.Id;
            await Database.UpdateAsync(file.ToLogFileDbo());
            CurrentFile = await GetLogFileAsync(file.Id);

            UpdateStatus();
        }
        System.Timers.Timer timer = new System.Timers.Timer();
        public async Task StartLog(LogFile file, LogFileWriteMode writeMode = LogFileWriteMode.Default)
        {
            try
            {
                if (!file.IsGraphAvailable || writeMode == LogFileWriteMode.Overwrite)
                {
                    file.ParameterNameDisplayMode = SettingsService.ParameterNameDisplayMode;
                }

                await SetFileAsync(file);

                if (!(await GetStoragePermissionsAsync()))
                {
                    return;
                }
                if (writeMode == LogFileWriteMode.Overwrite)
                {
                    var tasks = new List<Task>
                    {
                        FileSystem.DeleteAsync(file.LcvFilePath),
                        FileSystem.DeleteAsync(file.LjhFilePath)
                    };
                    await Task.WhenAll(tasks);
                }

                try
                {
                    await GeolocationService.StartListeningAsync();
                }
                catch (Exception ex)
                {
                    // could not start Geo
                    await GeolocationService.StopListeningAsync();
                    GeolocationService.SetNoPosition();
                }

                if(timer != null)
                {
                    timer.Dispose();
                }
                DeviceDisplay.KeepScreenOn = true;
                timer = new System.Timers.Timer();
                timer.Interval = file.LoggingInterval;
                timer.Elapsed += (sender, e) =>
                {
                    var now = DateTime.Now;
                    Debug.WriteLine($"Log - {IsLogging} - {now:T}");
                    if (IsLogging)
                    {
                        Task.Run(async () => await CreateLogRowAsync(file));
                    }
                };
                timer.Start();
                IsLogging = true;
                await PrepareLoggingAsync(file);
                //To store logger details if TrendLoggingActive is true...

                try
                {
                    await Task.Run(async () => await CreateLogRowAsync(file));
                }
                catch (Exception ex) { }

                if (file.LoggingInterval <= 0)
                {
                    throw new Exception("Log interval is not set");
                }
            
                UpdateStatus();
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e, TAG);
                await AlertService.DisplayError(e);
                await GeolocationService.StopListeningAsync();
            }
        }

        private async Task<bool> CreateLogRowAsync(LogFile file)
        {
            try
            {
                var upd = (await DeviceRefreshService.GetSelectedDevicesAsync()).Where(x => x.StatusEnum != ProbeStatus.STABILIZING && x.IsOnline);

                var latestUpdateFromEachSource = new List<DeviceUpdateMsg>();

                var latestCloudDevices = upd.Where(x => x.Source == DeviceSource.Cloud);
                AddDevicesToLatestUpdate(latestUpdateFromEachSource, latestCloudDevices, DeviceSource.Cloud);

                var latestBleDevices = upd.Where(x => x.Source == DeviceSource.Ble);
                AddDevicesToLatestUpdate(latestUpdateFromEachSource, latestBleDevices, DeviceSource.Ble);

                var latestUsbDevices = upd.Where(x => x.Source == DeviceSource.Usb);
                AddDevicesToLatestUpdate(latestUpdateFromEachSource, latestUsbDevices, DeviceSource.Usb);

                var logRow = new LogFileRowDTO
                {
                    Timestamp = DateTime.UtcNow,
                    Sources = latestUpdateFromEachSource.Select(p => new SourceDTO
                    {
                        DataSource = $"{p.Source}",
                        Devices = p.Devices
                            .Select(q => q.ToLogDevice())
                            .ToList()
                    }).ToList(),
                };
                var now = DateTime.Now;
                Debug.WriteLine($"Log row dto created - {now:T}");
                var isLogged = await AppendAsync(logRow, file.ParameterNameDisplayMode);
                if (isLogged)
                {
                    Debug.WriteLine($"Log appended - {now:T}");
                }
                var isChanged = isLogged && !file.IsGraphAvailable;
                if (isChanged)
                {
                    file.IsGraphAvailable = true;
                    await Database.UpdateAsync(file.ToLogFileDbo());
                    CurrentFile = file;
                    UpdateStatus();
                }
                return isLogged;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
                throw ex;
            }
        }

        private void AddDevicesToLatestUpdate(List<DeviceUpdateMsg> update, IEnumerable<GrayWolfDevice> devices, DeviceSource source)
        {
            if (devices != null)
            {
                var message = new DeviceUpdateMsg
                {
                    Devices = devices?.ToList() ?? new List<GrayWolfDevice>(),
                    Source = source,
                    Time = DateTime.UtcNow
                };
                update.Add(message);
                latestUpdates.Add(message);
            }
        }

        public async Task<bool> AppendAsync(LogFileRowDTO row, ParameterNameDisplayOption parameterNameDisplayOption)
        {
            try
            {
                bool status = await LoggerJSON.AppendRowAsync(row, CurrentFile.LjhFilePath, CurrentFile.LcvFilePath, parameterNameDisplayOption);

                return status;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
            return false;
        }

        private async Task<bool> FinalizeAsync()
        {
            try
            {
                LJH_Holder holder = LoggerJSON.Finalize();
                var columns = holder.Sets.SelectMany(x => x.Columns).ToList();
                if (columns.Any())
                {
                    await FileSystem.GetOrCreateFolderByNameAsync(CurrentFile.Name);
                    var holderJson = JsonConvert.SerializeObject(holder);
                    await FileSystem.WriteAllTextAsync(CurrentFile.LjhFilePath, holderJson);
                }
                else
                {
                    await FileSystem.DeleteAsync(CurrentFile.LcvFilePath);
                    await FileSystem.DeleteAsync(CurrentFile.LjhFilePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
            return false;
        }

        public async Task<bool> CreateSnapshotAsync(LogFile file, bool isLogButtonClicked = false)
        {
            try
            {
                if (!isLogButtonClicked)
                {
                    if (!await GetStoragePermissionsAsync())
                    {
                        return false;
                    }
                }
                else { 
                    GetLocationPermission();
                }
                if (!file.IsGraphAvailable)
                {
                    file.ParameterNameDisplayMode = SettingsService.ParameterNameDisplayMode;
                    await Database.UpdateAsync(file.ToLogFileDbo());
                }
                await GeolocationService.GetPositionOnceAsync();
                var isExist = await IsLogFileHasContentAsync(file?.Id);

                if (isLogButtonClicked && isExist)
                {
                    AppendSnapshot = true;
                }

                if (!AppendSnapshot && isExist)
                {
                    try
                    {
                        var snapshotWriteMode = await GetLogWriteModeAsync();
                        AppendSnapshot = snapshotWriteMode == LogFileWriteMode.Append;
                    }
                    catch (TaskCanceledException)
                    {
                        return false;
                    }
                }

                if (!AppendSnapshot && isExist)
                {
                    await Task.WhenAll(new[]
                    {
                        FileSystem.DeleteAsync(file.LcvFilePath),
                        FileSystem.DeleteAsync(file.LjhFilePath)
                    });
                }
                await PrepareLoggingAsync(file);
                var isLogged = await CreateLogRowAsync(file);
                await FinalizeAsync();
                return isLogged;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                return false;
            }
        }

        public async Task StopLog()
        {
            try
            {
                if (!IsLogging)
                {
                    return;
                }
                await FinalizeAsync();
                IsLogging = false;
                DeviceDisplay.KeepScreenOn = false;
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                }
                Messenger.Default.Unregister<DeviceUpdateMsg>(this);
                UpdateStatus();
                latestUpdates.Clear();
                LoggerJSON = null;

                AlertService.Toast(Localization.Localization.LogService_LoggingStopped);
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
                await AlertService.DisplayError(e);
            }
            finally
            {
                App.FlyoutPage.IsPresented = false;
                await GeolocationService.StopListeningAsync();
            }
        }

        private Task PrepareLoggingAsync(LogFile file)
        {
            LoggerJSON = new LoggerJSON(FileSystem, Ioc.Default.GetService<ISensorsService>());
            latestUpdates = new List<DeviceUpdateMsg>();
            return LoggerJSON.InitializeHolderAsync(file.LjhFilePath, file.LcvFilePath);
        }

        public async Task<bool> AddLogFileAsync(string name, int intervalMS)
        {
            try
            {
                if (name.Length > Constants.MAX_FILE_LOG_NAME_LENGTH)
                {
                    var format = Localization.Localization.Log_FileNameTooLongFormat;
                    var message = string.Format(format, Constants.MAX_FILE_LOG_NAME_LENGTH);
                    await AlertService.ShowAlert(message);
                    return false;
                }
                var matches = await Database.GetItemsAsync<LogFileDBO>(x => x.Name == name);
                if (matches.Any())
                {
                    await AlertService.ShowAlert(Localization.Localization.Log_LocationAlreadyExist);
                    return false;
                }
                var folderPath = await FileSystem.GetOrCreateFolderByNameAsync(name);
                var logFile = new LogFileDBO
                {
                    Name = name,
                    LoggingInterval = intervalMS,
                    IsSelected = true,
                    ParameterNamesDisplayMode = (int)SettingsService.ParameterNameDisplayMode
                };
                await Database.InsertAsync(logFile);
                return true;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                throw ex;
            }
        }

        public Task<TAttachment> AddLogAttachmentFileAsync<TAttachment>(TAttachment attachment, bool isNew, int fileId, byte[] bytes = null)
            where TAttachment : IAttachment
        {
            var logAttachmentFile = attachment.ToAttachmentDBO();
            if (attachment is EventAttachment eventAttachment)
            {
                var holder = new EventsDTO
                {
                    Events = eventAttachment.Events.ToList()
                };
                logAttachmentFile.TextContent = JsonConvert.SerializeObject(holder);
            }
            if (attachment is VideoAttachment videoAttachment)
            {
                logAttachmentFile.ThumbnailSource = videoAttachment.ThumbnailPath;
            }
            return AddLogAttachmentFileAsync<TAttachment>(logAttachmentFile, bytes, fileId, isNew);
        }

        private async Task<TAttachment> AddLogAttachmentFileAsync<TAttachment>(AttachmentDBO logAttachmentFile, byte[] bytes, int fileId, bool isNew)
            where TAttachment : IAttachment
        {
            var file = await GetLogFileAsync(fileId);
            logAttachmentFile.LoggerId = fileId;
            var folder = file.FolderPath;
            var fileName = Path.GetFileNameWithoutExtension(file.Name);
            string extension = string.Empty;
            string captionExtension = null;

            var attachmentFileType = AttachmentFileDataType.Binary;

            int index = 0;
            if (logAttachmentFile.IsText)
            {
                fileName = $"{fileName}_text";
                attachmentFileType = AttachmentFileDataType.Text;
                extension = ".txt";
            }
            else if (logAttachmentFile.IsAudio)
            {
                fileName = $"{fileName}_audio";
                extension = ".mp3";
                index = await LastFileNumberAsync(AttachmentType.Sound, fileId);
            }
            else if (logAttachmentFile.IsVideo)
            {
                fileName = $"{fileName}_video";
                extension = VideoExtension;
                index = await LastFileNumberAsync(AttachmentType.Video, fileId);
                captionExtension = ".vid";

            }
            else if (logAttachmentFile.IsMedia)
            {
                fileName = $"{fileName}_photo";
                index = await LastFileNumberAsync(AttachmentType.Photo, fileId);
                extension = PhotoExtension;
                captionExtension = ".cap";
            }
            else if (logAttachmentFile.IsDrawing)
            {
                fileName = $"{fileName}_drawing";
                extension = ".bmp";
            }
            else if (logAttachmentFile.IsEvent)
            {
                fileName = $"{fileName}_event";
                attachmentFileType = AttachmentFileDataType.Text;
                extension = ".evt";
            }

            string indexSuffix = string.Empty;
            if (logAttachmentFile.IsMedia || logAttachmentFile.IsVideo || logAttachmentFile.IsAudio)
            {
                indexSuffix = $"{++index}";
            }

            logAttachmentFile.Name = $"{fileName}{indexSuffix}";
            var path = Path.Combine(folder, $"{fileName}{indexSuffix}{extension}");
            logAttachmentFile.Path = path;
            var captionPath = "";
            if (captionExtension != null)
            {
                captionPath = Path.Combine(folder, $"{file.Name}_caption{indexSuffix}{captionExtension}");
                logAttachmentFile.CaptionPath = captionPath;
            }

            switch (attachmentFileType)
            {
                case AttachmentFileDataType.Binary:
                    await FileSystem.WriteAllBytesAsync(path, bytes);
                    if (captionPath.NotNullOrEmpty())
                    {
                        await FileSystem.WriteAllTextAsync(captionPath, logAttachmentFile.Caption);
                    }
                    break;

                case AttachmentFileDataType.Text:
                    await FileSystem.WriteAllTextAsync(path, logAttachmentFile.TextContent);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (isNew)
            {
                await Database.InsertAsync(logAttachmentFile);
            }
            else
            {
                await Database.UpdateAsync(logAttachmentFile);
            }
            return (TAttachment)logAttachmentFile.ToAttachment(folder);
        }

        public async Task<TextAttachment> GetNoteAsync()
        {
            if (CurrentFile == null)
            {
                return null;
            }

            var files = await GetLogAttachmentFilesAsync(LogFileId);
            var noteFile = files.FirstOrDefault(p => p is TextAttachment) as TextAttachment;
            if (noteFile != null)
            {
                noteFile.TextContent = await FileSystem.ReadAllTextAsync(noteFile.Path);
            }
            return noteFile;
        }

        public async Task<DrawableAttachment> GetCurrentDrawingNoteAsync()
        {
            if (CurrentFile == null)
            {
                return null;
            }
            var files = await GetLogAttachmentFilesAsync(CurrentFile.Id);
            if (!(files.FirstOrDefault(x => x is DrawableAttachment) is DrawableAttachment attachment))
            {
                return null;
            }

            var fileExists = await FileSystem.IsFileExistAsync(attachment.Path);
            if (!fileExists)
            {
                return null;
            }

            return attachment;
        }

        public async Task ExportLogAsync(int logFileID, ExportLogOptions option = ExportLogOptions.Share)
        {
            try
            {
                var permissions = await GetStoragePermissionsAsync();
                if (!permissions)
                {
                    return;
                }
                var logFile = await GetLogFileAsync(logFileID);
                var logFiles = await GetFilesForArchiveAsync(logFile);
                if (option == ExportLogOptions.SaveToFile || !logFiles.Any(IsLargeFile))
                {
                    await OnArchiveEntriesSelectedAsync(logFiles, logFileID, option);
                }
                else
                {
                    var tcs = new TaskCompletionSource<bool>();
                    var popup = new AttachmentsSizeWarningPopupPage(tcs);
                    //await NavigationService.Nav.PushModalAsync(popup);

                    await NavigationService.Nav.PushPopupAsync(popup);
                    await tcs.Task;
                    await FilterArchiveEntriesAsync(logFiles, logFileID, option);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private bool IsLargeFile(ArchiveEntry entry)
        {
            var extension = Path.GetExtension(entry.FilePath);
            return extension == PhotoExtension || extension == VideoExtension;
        }

        public async Task OnArchiveEntriesSelectedAsync(List<ArchiveEntry> entries, int logFileId, ExportLogOptions option)
        {
            try
            {
                var logFile = await GetLogFileAsync(logFileId);
                var csvFiles = SettingsService.IncludeCsvIntoExport ? (await CreateCsvFilesAsync(logFile)) : new Dictionary<string, string>();

                var files = new List<string>();
                foreach (var entry in entries)
                {
                    files.Add(entry.FilePath);
                    if (entry.CaptionPath.NotNullOrEmpty())
                    {
                        files.Add(entry.CaptionPath);
                    }
                }

                await Utility.ZipUtility.ZipAndShare(files, logFile.Name, csvFiles, SettingsService, option);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
                throw ex;
            }
        }

        public async Task FilterArchiveEntriesAsync(List<ArchiveEntry> entries, int logFileId, ExportLogOptions option)
        {
            await NavigationService.ShowPopup(new SelectAttachmentFilesPopup(entries, logFileId, option));
        }

        private async Task<Dictionary<string, string>> CreateCsvFilesAsync(LogFile logFile)
        {
            var existsTasks = new List<Task<bool>>
            {
                FileSystem.IsFileExistAsync(Path.Combine(logFile.FolderPath, logFile.LcvFilePath)),
                FileSystem.IsFileExistAsync(Path.Combine(logFile.FolderPath, logFile.LjhFilePath))
            };
            var isExists = (await Task.WhenAll(existsTasks)).Any(x => x);
            if (!isExists)
            {
                return new Dictionary<string, string>();
            }
            var path = logFile.LcvFilePath;
            var holderJson = await FileSystem.ReadAllTextAsync(logFile.LjhFilePath);
            var holder = JsonConvert.DeserializeObject<LJH_Holder>(holderJson);
            var files = new Dictionary<string, List<string>>();
            var filePaths = new Dictionary<string, string>();

            var folderPath = Path.Combine(logFile.FolderPath, "csv");
            await LoadLinesForCsvAsync(logFile, files, filePaths, holder, path, folderPath);

            await FileSystem.GetOrCreateFolderByPathAsync(folderPath);

            return await WriteCsvLinesAsync(filePaths, files);
        }

        private async Task LoadLinesForCsvAsync(
            LogFile logFile,
            Dictionary<string, List<string>> files,
            Dictionary<string, string> filePaths,
            LJH_Holder holder,
            string path,
            string folderPath)
        {
            var isExist = await FileSystem.IsFileExistAsync(path);
            var lines = await FileSystem.ReadAllLinesAsync(path);
            var allIgnoredColumns = new Dictionary<int, List<int>>();

            foreach (var line in lines)
            {
                var index = line.IndexOf(',');
                if (index == -1 || index == line.Length - 1)
                {
                    continue;
                }
                var setId = line.Substring(0, index);
                var csvValue = line.Substring(index + 1);
                var prefix = holder.IsSimulated ? "SIMULATION-" : "";
                var fileName = $"{prefix}{logFile.Name}-{setId}.csv";
                var filePath = Path.Combine(folderPath, fileName);

                var intSetId = int.Parse(setId);
                if (!allIgnoredColumns.TryGetValue(intSetId, out var ignoredColumns))
                {
                    ignoredColumns = GetIgnoredCsvColumnsForSet(intSetId, holder);
                    allIgnoredColumns[intSetId] = ignoredColumns;
                }

                if (!files.TryGetValue(setId, out var setLines))
                {
                    var firstLine = GetColumnNamesLine(holder, setId, allIgnoredColumns[intSetId]);

                    // BVW - remove UNICODE characters
                    firstLine = GWL_Units.FlattenUnits(firstLine);

                    setLines = new List<string>
                    {
                        firstLine
                    };
                    files[setId] = setLines;
                    filePaths[setId] = filePath;
                }

                // BVW - Change ISO8601Time to Excel-Friendly Time in local time zone
                csvValue = GetCsvValueForLine(csvValue, ignoredColumns);

                setLines.Add(csvValue);
            }
        }

        private List<int> GetIgnoredCsvColumnsForSet(int setId, LJH_Holder holder)
        {
            var set = holder.Sets.FirstOrDefault(x => x.SetID == setId);
            var columns = set
                .Columns
                .Where(x => x.Code.IsCoordinatesSensorCode() || x.Sensor.IsNullOrEmpty())
                .Select(x => set.Columns.IndexOf(x))
                .ToList();
            return columns;
        }

        private string GetCsvValueForLine(string sourceLine, List<int> ignoredColumns)
        {
            var result = "";
            try
            {
                var parts = sourceLine.Split(',').ToList();
                var indexes = ignoredColumns.OrderByDescending(x => x).ToList();

                for (int i = 0; i < indexes.Count; i++)
                {
                    parts.RemoveAt(indexes[i] + 1);
                }
                if (parts.Count > 1)
                {
                    DateTime tm = DateTime.Parse(parts[0]);
                    parts[0] = tm.ToLocalTime().ToString("dd-MMM-yyyy HH:mm:ss", CultureInfo.GetCultureInfo("en-US"));
                    result = string.Join(", ", parts);
                }
                else
                {
                    result = sourceLine.ToString(CultureInfo.GetCultureInfo("en-US"));
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        private string GetColumnNamesLine(LJH_Holder holder, string setId, List<int> ignoredColumns)
        {
            var result = "TimeStamp";
            if (!(holder?.Sets?.FirstOrDefault(x => $"{x.SetID}" == setId.Trim()) is LJH_Set set))
            {
                return result;
            }
            result += ", ";

            var indexes = ignoredColumns.OrderByDescending(x => x).ToList();
            var columns = set.Columns.ToList();

            for (int i = 0; i < indexes.Count; i++)
            {
                columns.RemoveAt(indexes[i]);
            }
            var names = columns.Select(x => $"{x.Sensor} {x.Unit} ({x.SerialNumber})").ToList();

            result += string.Join(", ", names);
            return result;
        }

        private async Task<Dictionary<string, string>> WriteCsvLinesAsync(Dictionary<string, string> filePaths, Dictionary<string, List<string>> fileLines)
        {
            var tasks = new List<Task>();

            foreach (var key in filePaths.Keys)
            {
                if (!fileLines.TryGetValue(key, out var lines))
                {
                    lines = new List<string>();
                }
                var filePath = filePaths[key];
                tasks.Add(FileSystem.WriteAllLinesAsync(filePath, lines));
            }

            await Task.WhenAll(tasks);
            var result = new Dictionary<string, string>();
            foreach (var filePath in filePaths.Values)
            {
                result.Add(filePath, "text/csv");
            }
            return result;
        }

        public async Task<List<ArchiveEntry>> GetFilesForArchiveAsync(LogFile logFile)
        {
            // To zip and share log FileSystem...
            var logFiles = new List<ArchiveEntry>();

            var lcvExists = await FileSystem.IsFileExistAsync(logFile.LcvFilePath);
            if (lcvExists)
            {
                logFiles.Add(new ArchiveEntry
                {
                    FilePath = Path.Combine(logFile.FolderPath, logFile.LcvFilePath),
                    IsSelected = true
                });
            }
            var ljhExists = await FileSystem.IsFileExistAsync(logFile.LjhFilePath);
            if (ljhExists)
            {
                logFiles.Add(new ArchiveEntry
                {
                    FilePath = Path.Combine(logFile.FolderPath, logFile.LjhFilePath),
                    IsSelected = true
                });
            }

            var attachments = await GetLogAttachmentFilesAsync(logFile.Id);
            var entriesTasks = new List<Task<ArchiveEntry>>();
            foreach (var attachment in attachments)
            {
                entriesTasks.Add(GetArchiveEntryFromAttachmentAsync(attachment));
            }
            var result = await Task.WhenAll(entriesTasks);
            logFiles.AddRange(result.Where(x => x != null));

            return logFiles.DistinctBy(x => x.FileName).ToList();
        }

        private async Task<ArchiveEntry> GetArchiveEntryFromAttachmentAsync(IAttachment attachment)
        {
            var pathExistsTask = FileSystem.IsFileExistAsync(attachment.Path);
            var captionPathExistsTask = FileSystem.IsFileExistAsync(attachment.CaptionPath);
            await Task.WhenAll(pathExistsTask, captionPathExistsTask);
            ArchiveEntry entry = null;
            if (pathExistsTask.Result)
            {
                entry = new ArchiveEntry
                {
                    FilePath = attachment.Path,
                    IsSelected = true
                };
            }
            if (captionPathExistsTask.Result && entry != null)
            {
                entry.CaptionPath = attachment.CaptionPath;
            }
            return entry;
        }

        public async Task<bool> IsLogFileExistAsync(int? fileId = null)
        {
            var file = CurrentFile;
            if (fileId != null)
            {
                file = await GetLogFileAsync(fileId.Value);
            }

            return await FileSystem.IsFileExistAsync(file.LcvFilePath);
        }

        public async Task<bool> IsLogFileHasContentAsync(int? fileId = null)
        {
            try
            {
                var file = CurrentFile;
                if (fileId != null)
                {
                    file = await GetLogFileAsync(fileId.Value);
                }
                var csvFile = new System.IO.FileInfo(file.LcvFilePath);
                if (csvFile != null && csvFile.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task DeleteLocationAsync(int logFileId)
        {
            var file = await GetLogFileAsync(logFileId);
            if (file == null)
            {
                return;
            }
            var exist = await FileSystem.IsDirectoryExistAsync(file.FolderPath);
            if (exist)
            {
                await FileSystem.DeleteDirectoryAsync(file.FolderPath, true);
            }

            await Database.DeleteItemAsync<LogFileDBO>(logFileId);
            SettingsService.LogFileId = -1;
            CurrentFile = null;

            UpdateStatus();
        }

        public async Task ClearLocationAsync(int logFileId)
        {
            try
            {
                var file = await GetLogFileAsync(logFileId);
                await FileSystem.DeleteDirectoryAsync(file.FolderPath, true);
                await FileSystem.GetOrCreateFolderByPathAsync(file.FolderPath);
                file.IsGraphAvailable = false;
                file.HasContent = false;
                await Database.UpdateAsync(file.ToLogFileDbo());
                CurrentFile = await GetLogFileAsync(file.Id);
                UpdateStatus();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void OnSelectedDevicesMessage(SelectedDevicesMessage message)
        {
            OnSelectedDevicesMessage(message.Devices);
        }

        private void OnSelectedDevicesMessage(IEnumerable<GrayWolfDevice> devices)
        {
            AnyDeviceSelected = devices.Any();
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (IsLogging)
            {
                Status = LogStatus.Logging;
            }
            else
            {
                Status = AnyDeviceSelected ? LogStatus.DevicesSelected : LogStatus.NoDevicesSelected;
            }
            Messenger.Default.Send(new LogStatusChangedMessage
            {
                Status = Status
            });
        }

        private async void OnDeviceLogButtonClicked(LogButtonMessage msg)
        {
            if (CurrentFile == null)
                return;
            if (IsLogging)
                return;
            if (msg.GrayWolfDevice == null || msg.GrayWolfDevice.StatusEnum == ProbeStatus.STABILIZING)
            {
                AlertSound.PlaySystemSound(true);
                return;
            }
            try
            {
                await CreateSnapshotAsync(CurrentFile, true);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
        }

        public async Task<bool> GetStoragePermissionsAsync()
        {
            switch (Device.RuntimePlatform)
            {

                    case Device.Android:
                    bool statuses = false;

                    statuses = await Ioc.Default.GetService<IStoragePermissionService>().RequestStoragePermissions();

                    return statuses;
                case Device.iOS:
                        var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                        if (status != PermissionStatus.Granted)
                        {
                            return false;
                        }
                        status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                        return status == PermissionStatus.Granted;
                    default:
                        return false;
                }
           
        }

        protected void GetLocationPermission()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Granted)
                    return;

                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // Prompt the user to turn on in settings
                    // On iOS once a permission has been denied it may not be requested again from the application
                    return;
                }

                if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
                {
                    // Prompt the user with additional information as to why the permission is needed
                }

                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            });
        }

        public async Task<PermissionStatus> CheckAndRequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.Camera>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.Camera>();

            return status;
        }

        public async Task<LogFileWriteMode> GetLogWriteModeAsync()
        {
            var options = new[]
            {
                Localization.Localization.LogFileWritingMode_Append,
                Localization.Localization.LogFileWritingMode_Overwrite
            };
            string optionResult = "";
            var cancelButtonText = Localization.Localization.Button_Cancel.ToUpper();
            //Device.BeginInvokeOnMainThread(async () => {
                optionResult = await UserDialogs.ActionSheetAsync(Localization.Localization.LogService_LogAlreadyExistMessage,             
                cancelButtonText, null, null, options);
            



            if (optionResult == cancelButtonText)
                throw new TaskCanceledException();
            else if (optionResult == Localization.Localization.LogFileWritingMode_Overwrite)
            {
                var result = await UserDialogs.ConfirmAsync(
                    Localization.Localization.LogService_OverwriteDialogTitle,
                    Localization.Localization.LogService_OverwriteButton,
                    Localization.Localization.Button_Yes,
                    Localization.Localization.Button_Cancel);
                if (!result)
                {
                    throw new TaskCanceledException();
                }
                return LogFileWriteMode.Overwrite;
            }
            else
                return LogFileWriteMode.Append;
        }

        #region Attachments
        public async Task<List<IAttachment>> GetLogAttachmentFilesAsync(int logFileId)
        {
            var logFile = await GetLogFileAsync(logFileId);
            var attachmentFiles = await Database.GetItemsAsync<AttachmentDBO>(x => x.LoggerId == logFileId);
            return attachmentFiles.Select(x => x.ToAttachment(logFile.FolderPath)).ToList();
        }

        public async Task RecordSound()
        {
            try
            {
                if (CurrentFile == null)
                {
                    await AlertService.ShowAlert(Localization.Localization.LogService_SelectLocationBeforeRecord);
                }
                else
                {
                    var isPermissionsGranted = await GetStoragePermissionsAsync();
                    if (!isPermissionsGranted)
                    {
                        await AlertService.ShowAlert(Localization.Localization.Log_StoragePermissionsMessage);
                        return;
                    }
                    //Show Record Audio Popup;
                    var recordPopup = new RecordSoundPopupPage();
                    await NavigationService.Nav.PushPopupAsync(recordPopup);
                }
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e, TAG);
                await AlertService.DisplayError(e);
            }
            finally
            {
                App.FlyoutPage.IsPresented = false;
            }
        }

        public async Task AddNote()
        {
            try
            {
                var isPermissionsGranted = await GetStoragePermissionsAsync();
                if (!isPermissionsGranted)
                {
                    await AlertService.ShowAlert(Localization.Localization.Log_StoragePermissionsMessage);
                    return;
                }
                if (CurrentFile == null)
                {
                    await AlertService.ShowAlert(Localization.Localization.LogService_SelectLocationBeforeNote);
                    return;
                }

                var logPopup = new LogNotePopupPage();
                await NavigationService.Nav.PushPopupAsync(logPopup);
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e, TAG);
                await AlertService.DisplayError(e);
            }
            finally
            {
                App.FlyoutPage.IsPresented = false;
            }
        }

        public async Task TakePhoto()
        {
            if (CurrentFile == null)
            {
                await AlertService.ShowAlert(Localization.Localization.LogService_SelectLocationBeforePhoto);
                return;
            }

            try
            {
                var isPermissionsGranted = await GetStoragePermissionsAsync();
                if (!isPermissionsGranted)
                {
                    await AlertService.ShowAlert(Localization.Localization.Log_StoragePermissionsMessage);
                    return;
                }

                //var cameraPermission = await CheckAndRequestCameraPermission();
                //if(cameraPermission != PermissionStatus.Granted)
                //{
                //    return;
                //}
                byte[] imageByteData = null;
                string photoPath = string.Empty;
                var selectedOption = "";
                if (SettingsService.AreGalleryAttachmentsEnabled)
                {
                    var options = new[]
                    {
                        Localization.Localization.LogService_CameraSource,
                        Localization.Localization.LogService_GallerySource
                    };

                    selectedOption =
                        await AlertService.DisplayActionSheet(
                            Localization.Localization.Localization_SelectPhotoSource,
                            Localization.Localization.Button_Cancel,
                            null,
                            options);
                    if (!options.Contains(selectedOption))
                        return;
                }
                else
                {
                    selectedOption = Localization.Localization.LogService_CameraSource;
                }


                var cameraOptions = new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Full,
                    AllowCropping = false,
                    CompressionQuality = 100,
                    Directory = "WolfSense",
                    Name = "SelectedLocation_photo.png",
                };
                if (Device.RuntimePlatform == Device.WinUI)
                {
                    MediaFile photo = null;
                    if (selectedOption == Localization.Localization.LogService_GallerySource)
                    {
                        photo = await CrossMedia.Current.PickPhotoAsync();

                    }
                    else if (selectedOption == Localization.Localization.LogService_CameraSource)
                    {
                        photo = await CrossMedia.Current.TakePhotoAsync(cameraOptions);
                    }

                    if (photo != null)
                    {
                        imageByteData = photo.GetStream().ConverteStreamToByteArray();
                    }
                }
                else
                {
                    var mediaService = Ioc.Default.GetService<IMediaService>();

                    if (selectedOption == Localization.Localization.LogService_CameraSource)
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

                        if (status != PermissionStatus.Granted)
                        {
                            var result = await Permissions.RequestAsync<Permissions.Camera>();
                            status = result;
                        }

                        if (status == PermissionStatus.Granted)
                        {
                            if (!CrossMedia.Current.IsCameraAvailable)
                            {
                                UserDialogs.HideLoading();
                                UserDialogs.Alert(
                                    Localization.Localization.LogService_NoCameraAvailableMessage,
                                    null,
                                    Localization.Localization.Button_Ok);
                                return;
                            }

                            if (Device.RuntimePlatform == Device.Android)
                                UserDialogs.ShowLoading(Localization.Localization.Loading);
                            var file = await CrossMedia.Current.TakePhotoAsync(cameraOptions);
                            if (file == null)
                            {
                                UserDialogs.HideLoading();
                                return;
                            }

                            Constants.ImgFilePath = file.Path;
                            photoPath = file.Path;
                            string filepath = file.Path;
                            imageByteData = await mediaService.GetMediaInBytes(filepath);

                            //await UpdateProfilePictureApi();
                            UserDialogs.HideLoading();
                        }
                        else
                        {
                            await AlertService.ShowAlert(Localization.Localization.LogService_CameraPermissionsDenied);
                            UserDialogs.HideLoading();
                        }
                    }
                    else
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.Media>();

                        if (status != PermissionStatus.Granted)
                        {
                            var result = await Permissions.RequestAsync<Permissions.Media>();
                            status = result;
                        }

                        if (status == PermissionStatus.Granted)
                        {
                            if (!CrossMedia.Current.IsPickPhotoSupported)
                            {
                                UserDialogs.HideLoading();
                                await AlertService.ShowAlert(Localization.Localization.LogService_GalleryIsNotAvailable);
                                return;
                            }

                            if (Device.RuntimePlatform == Device.Android)
                                UserDialogs.ShowLoading(Localization.Localization.Loading);
                            var file = await CrossMedia.Current.PickPhotoAsync();
                            if (file == null)
                            {
                                UserDialogs.HideLoading();
                                return;
                            }

                            Constants.ImgFilePath = file.Path;
                            photoPath = file.Path;
                            imageByteData = await mediaService.GetMediaInBytes(photoPath);

                            //await UpdateProfilePictureApi();
                            UserDialogs.HideLoading();
                        }
                        else
                        {
                            await AlertService.ShowAlert(Localization.Localization.LogService_CameraPermissionsDenied);
                            UserDialogs.HideLoading();
                        }
                    }


                    UserDialogs.HideLoading();
                }

                //To save th Photo Details in Local Database...
                if (imageByteData != null)
                {
                    var photo = new PhotoAttachment
                    {
                        Path = photoPath,
                        Caption = DateTime.Now.ToString("O"),
                    };
                    await AddLogAttachmentFileAsync(photo, true, LogFileId, imageByteData);
                    AlertService.Toast(Localization.Localization.LogService_MediaFileLogged);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
                await AlertService.DisplayError(ex);

                UserDialogs.HideLoading();
            }
            finally
            {
                App.FlyoutPage.IsPresented = false;
            }
        }

        private async Task<int> LastFileNumberAsync(AttachmentType type, int locationId)
        {
            var files = await GetLogAttachmentFilesAsync(locationId);

            switch (type)
            {
                case AttachmentType.Note:
                    return files.Count(p => p is TextAttachment);
                case AttachmentType.Photo:
                    return files.Count(p => p is PhotoAttachment);
                case AttachmentType.Sound:
                    return files.Count(p => p is AudioAttachment);
                case AttachmentType.Video:
                    return files.Count(p => p is VideoAttachment);
                case AttachmentType.ImageNote:
                    return files.Count(p => p is DrawableAttachment);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        string thumbnailPath;

        public async Task AddVideo()
        {
            if (CurrentFile == null)
            {
                await AlertService.ShowAlert(Localization.Localization.AddVideo_SelectLoggerMessage);
                return;
            }

            try
            {
                var isPermissionsGranted = await GetStoragePermissionsAsync();
                if (!isPermissionsGranted)
                {
                    await AlertService.ShowAlert(Localization.Localization.Log_StoragePermissionsMessage);
                    return;
                }
                string videoPath = string.Empty;
                var selectedOption = "";

                if (SettingsService.AreGalleryAttachmentsEnabled)
                {
                    var options = new[]
                    {
                        Localization.Localization.LogService_CameraSource,
                        Localization.Localization.LogService_GallerySource
                    };

                    selectedOption =
                        await AlertService.DisplayActionSheet(
                            Localization.Localization.Localization_SelectPhotoSource,
                            Localization.Localization.Button_Cancel,
                            null,
                            options);
                    if (!options.Contains(selectedOption))
                        return;
                }
                else
                {
                    selectedOption = Localization.Localization.LogService_CameraSource;
                }


                if (Device.RuntimePlatform == Device.WinUI)
                {

                    MediaFile video = null;
                    if (selectedOption == Localization.Localization.LogService_GallerySource)
                    {
                        video = await CrossMedia.Current.PickVideoAsync();

                    }
                    else if (selectedOption == Localization.Localization.LogService_CameraSource)
                    {
                        video = await CrossMedia.Current.TakeVideoAsync(
                            new StoreVideoOptions());
                    }

                    if (video != null)
                    {
                        videoPath = video.Path;
                    }
                }
                else
                {
                    if (selectedOption == Localization.Localization.LogService_CameraSource)
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

                        if (status != PermissionStatus.Granted)
                        {
                            var result = await Permissions.RequestAsync<Permissions.Camera>();
                            status = result;
                        }

                        if (status == PermissionStatus.Granted)
                        {
                            if (!CrossMedia.Current.IsCameraAvailable)
                            {
                                UserDialogs.HideLoading();
                                await AlertService.ShowAlert(Localization.Localization.LogService_NoCameraAvailableMessage);
                                return;
                            }

                            if (Device.RuntimePlatform == Device.Android)
                                UserDialogs.ShowLoading(Localization.Localization.Loading);
                            var file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
                            {
                                Directory = "WolfSense",
                                Name = "SelectedLocation_photo.mp4"
                            });
                            if (file == null)
                            {
                                UserDialogs.HideLoading();
                                return;
                            }

                            Constants.ImgFilePath = file.Path;
                            videoPath = file.Path;
                            Stream stream = await GetVideoThumbnailAsync(videoPath);
                            thumbnailPath = SaveStreamAsImage(stream, videoPath);
                            //await UpdateProfilePictureApi();
                            UserDialogs.HideLoading();
                        }
                        else
                        {
                            await AlertService.ShowAlert(Localization.Localization.LogService_CameraPermissionsDenied);
                            UserDialogs.HideLoading();
                        }
                    }
                    else
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.Media>();

                        if (status != PermissionStatus.Granted)
                        {
                            var result = await Permissions.RequestAsync<Permissions.Media>();
                            status = result;
                        }

                        if (status == PermissionStatus.Granted)
                        {
                            if (!CrossMedia.Current.IsPickVideoSupported)
                            {
                                await AlertService.ShowAlert(Localization.Localization.LogService_GalleryIsNotAvailable);
                                UserDialogs.HideLoading();
                                return;
                            }

                            if (Device.RuntimePlatform == Device.Android)
                                UserDialogs.ShowLoading(Localization.Localization.Loading);
                            var file = await CrossMedia.Current.PickVideoAsync();
                            if (file == null)
                            {
                                UserDialogs.HideLoading();
                                return;
                            }

                            Constants.ImgFilePath = file.Path;
                            videoPath = file.Path;
                            Stream stream = await GetVideoThumbnailAsync(videoPath);
                            thumbnailPath = SaveStreamAsImage(stream, videoPath);
                        }
                        else
                        {
                            await AlertService.ShowAlert(Localization.Localization.LogService_CameraPermissionsDenied);
                            UserDialogs.HideLoading();
                        }
                    }

                    UserDialogs.HideLoading();
                }

                if (videoPath == null) return;
                //To save th Photo Details in Local Database...
                var bytes = await FileSystem.ReadAllBytesAsync(videoPath);
                var videoAttachment = new VideoAttachment
                {
                    Path = videoPath,
                    ThumbnailPath = thumbnailPath,
                    Caption = DateTime.Now.ToString("O")
                };
                await AddLogAttachmentFileAsync(videoAttachment, true, LogFileId, bytes);
                AlertService.Toast(Localization.Localization.LogService_VideoFileLogged);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
                await AlertService.DisplayError(ex);

                UserDialogs.HideLoading();
            }
            finally
            {
                App.FlyoutPage.IsPresented = false;
            }
        }

        private string SaveStreamAsImage(Stream stream, string path)
        {
            byte[] bytes = null;
            string localPath = Path.Combine(path.Replace("mp4", "png"));
            try
            {
                MemoryStream ms = (MemoryStream)stream;
                bytes = ms.ToArray();
                File.WriteAllBytes(localPath, bytes);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            return localPath;
        }

        private async Task<Stream> GetVideoThumbnailAsync(string filePath)
        {
            Stream stream = null;
            try
            {
                stream = await Ioc.Default.GetService<IThumbnailService>().GetImageStreamAsync(filePath);
                if (stream != null)
                {
                    //image.Source = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            return stream;
        }

        public Task RemoveAttachmentsAsync(IEnumerable<IAttachment> attachments)
        {
            var tasks = new List<Task>();
            foreach (var attachment in attachments)
            {
                tasks.Add(DeleteAttachmentAsync(attachment));
            }
            return Task.WhenAll(tasks);
        }

        private async Task DeleteAttachmentAsync(IAttachment attachment)
        {
            try
            {
                await Database.DeleteItemAsync<AttachmentDBO>(attachment.Id);
                await FileSystem.DeleteAsync(attachment.Path);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
                Debug.WriteLine($"Failed to delete attachment {attachment.Path}: {ex.Message}");
            }
        }

        public async Task<EventAttachment> GetEventAttachmentAsync()
        {
            if (CurrentFile == null)
            {
                throw new NullReferenceException();
            }

            var attachments = await Database.GetItemsAsync<AttachmentDBO>(x => x.LoggerId == CurrentFile.Id && x.IsEvent);
            var isCreated = attachments.Any();
            var dbo = attachments.FirstOrDefault() ?? new AttachmentDBO
            {
                LoggerId = CurrentFile.Id,
                IsEvent = true,
            };
            var attachment = dbo.ToEventAttachment(isCreated, CurrentFile.FolderPath);
            var events = await GetEventsFromPathAsync(attachment.Path);
            attachment.Events = events.ToObservableCollection();

            return attachment;
        }

        private async Task<List<Event>> GetEventsFromPathAsync(string path)
        {
            var isExist = await FileSystem.IsFileExistAsync(path);
            if (!isExist)
            {
                return new List<Event>();
            }
            var json = await FileSystem.ReadAllTextAsync(path);
            var eventsHolder = JsonConvert.DeserializeObject<EventsDTO>(json);
            return eventsHolder.Events;
        }
        #endregion Attachments
    }
}