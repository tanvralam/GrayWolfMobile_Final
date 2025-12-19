using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Services;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace GrayWolf.Utility
{
    public static class ZipUtility
    {
        /// <summary>
        /// TODO : To Create Zip File for the Collection of files...
        /// </summary>
        /// <param name="filesToZip"></param>
        /// <param name="destinationZipFullPath"></param>
        /// <returns></returns>
        public static async Task<string> ArchiveIntoLczAsync(IEnumerable<string> filesToZip, string filename, ISettingsService settings, GrayWolf.Interfaces.IFileSystem fileSystem)
        {
            try
            {
                // Get a temporary cache directory
                var exportZipTempDirectory = await fileSystem.GetOrCreateFolderByNameAsync("Export");

                var directoryExists = await fileSystem.IsDirectoryExistAsync(exportZipTempDirectory);
                if (directoryExists)
                {
                    await fileSystem.DeleteDirectoryAsync(exportZipTempDirectory, true);
                }

                // Get a timestamped filename
                var exportZipFilename = $"{filename}.lcz";
                exportZipTempDirectory = await fileSystem.GetOrCreateFolderByPathAsync(exportZipTempDirectory);

                var exportZipFilePath = Path.Combine(exportZipTempDirectory, exportZipFilename);

                var fileExists = await fileSystem.IsFileExistAsync(exportZipFilePath);
                if (fileExists)
                {
                    await fileSystem.DeleteAsync(exportZipFilePath);
                }

                var password = await GetPasswordForZipAsync(settings);
                using (var zip = new ZipFile(exportZipFilePath))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(password))
                        {
                            zip.Encryption = EncryptionAlgorithm.WinZipAes128;
                            zip.Password = password;
                        }
                        foreach(var file in filesToZip)
                        {
                            zip.AddFile(file, "");
                        }
                        zip.Save();
                    }
                    catch (Exception ex) 
                    {

                    }
                }
                return exportZipFilePath;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                throw e;
            }
        }


        /// <summary>
        /// TODO : To Create Zip File for the Collection of files...
        /// </summary>
        /// <param name="filesToZip"></param>
        /// <param name="destinationZipFullPath"></param>
        /// <returns></returns>
        public static async Task<string> ZipAndShare(IEnumerable<string> filesToZip, string fileName, Dictionary<string, string> attachmentFiles, ISettingsService settings, ExportLogOptions option = ExportLogOptions.Share)
        {
            try
            {
                var fileSystem = Ioc.Default.GetService<GrayWolf.Interfaces.IFileSystem>();
                var archivePath = await ArchiveIntoLczAsync(filesToZip, fileName, settings, fileSystem);

                switch (option)
                {
                    case ExportLogOptions.Share:
                        {
                            var files = new List<ShareFile>
                            {
                                new ShareFile(archivePath, "application/lcz"),
                            };
                            files.AddRange(attachmentFiles.Select(x => new ShareFile(x.Key, x.Value)));
                            var archiveFileName = Path.GetFileName(archivePath);
                            var archiveFolderPath = archivePath.Replace(archiveFileName, "");
                            var instructionsPath = Path.Combine(archiveFolderPath, "instructions.txt");
                            await fileSystem.WriteAllTextAsync(instructionsPath, Localization.Localization.Share_InstructionsFileText);
                            files.Add(new ShareFile(instructionsPath));
                            if (Device.RuntimePlatform == DevicePlatform.iOS.ToString())
                            {

                                switch (Device.Idiom)
                                {
                                    case TargetIdiom.Phone:
                                        // To share file immediately for testing purpose...
                                        await Share.RequestAsync(new ShareMultipleFilesRequest
                                        {
                                            Title = "GrayWolf Location File",
                                            Files = files
                                        });
                                        break;
                                    case TargetIdiom.Tablet:
                                        DisplayInfo ScreenInfo = DeviceDisplay.MainDisplayInfo;
                                        // Height (in pixels)
                                        var height = ScreenInfo.Height;
                                        var width = ScreenInfo.Width;
                                        int midX = Convert.ToInt32((height / 4));
                                        int midY = Convert.ToInt32(width / 3);
                                        // To share file immediately for testing purpose...
                                        await Share.RequestAsync(new ShareMultipleFilesRequest
                                        {
                                            Title = "GrayWolf Location File",
                                            Files = files,
                                            PresentationSourceBounds = new Rect(midX, midY, 1, 1)
                                        });
                                        break;
                                }
                            }
                            else
                            {
                                // To share file immediately for testing purpose...
                                await Share.RequestAsync(new ShareMultipleFilesRequest
                                {
                                    Title = "GrayWolf Location File",
                                    Files = files
                                });
                            }
                        }
                        break;
                    case ExportLogOptions.SaveToFile:
                        {
                            await SaveToFileAsync(archivePath, attachmentFiles.Keys);
                            break;
                        }
                    case ExportLogOptions.SendByEmail:
                        {
                            var sendEmail = DependencyService.Get<ISendEmail>();
                            var messageBody = Localization.Localization.Email_MessageBody;
                            messageBody = Regex.Unescape(messageBody);
                            await sendEmail.ComposeEmail(
                                Localization.Localization.Email_MessageHeader, 
                                Localization.Localization.Email_LogFileMessageSubject,
                                messageBody, 
                                archivePath,
                                attachmentFiles.Keys);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(option), option, null);
                }


                return string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                throw e;
            }
        }
                
        public static async Task SaveToFileAsync(string archivePath, IEnumerable<string> attachments)
        {
            var selectFolder = Ioc.Default.GetService<ISaveFile>();
            if (selectFolder == null)
            {
                await AlertService.Instance.ShowAlert("Folder selection not supported on this platform");
                throw new NotSupportedException();
            }

            await selectFolder.SaveFileAsync(archivePath, "application/lcz", attachments);
        } 

        private static Task<string> GetPasswordForZipAsync(ISettingsService settings)
        {
            switch (settings.ProtectionMode)
            {
                case ZipProtectionMode.CustomPassword:
                    return settings.GetCustomZipPasswordAsync();
                case ZipProtectionMode.DefaultPassword:
                    return Task.FromResult(Constants.DEFAULT_LCZ_PASSWORD);
                default:
                    return Task.FromResult("");
            }
        }
    }
}
