
using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Enums;
using GrayWolf.Extensions;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using GrayWolf.Views.Popups;
using Newtonsoft.Json;
using RGPopup.Maui.Extensions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Maui.Controls.Compatibility.Chart;


namespace GrayWolf.ViewModels
{
    public class ChartPageViewModel : BaseViewModel
    {
        private const string TAG = "ChartPageViewModel";

        #region variables
        private GrayWolfDevice _device;
        public GrayWolfDevice Device
        {
            get => _device;
            private set => SetProperty(ref _device, value);
        }

        private GraphParameter _parameter;
        public GraphParameter Parameter
        {
            get => _parameter;
            set => SetProperty(ref _parameter, value, onChanged: OnParameterChanged);
        }

        private List<GraphParameter> _parameters;
        public List<GraphParameter> Parameters
        {
            get => _parameters;
            private set => SetProperty(ref _parameters, value);
        }

        private GraphTimeOption _timeAxisOption;
        public GraphTimeOption TimeAxisOption
        {
            get => _timeAxisOption;
            set => SetProperty(ref _timeAxisOption, value, onChanged: OnTimeAxisOptionChanged);
        }

        //private DateTimeIntervalType _intervalType;
        //public DateTimeIntervalType IntervalType
        //{
        //    get => _intervalType;
        //    private set => SetProperty(ref _intervalType, value);
        //}


        private TimeInterval _intervalType;
        public TimeInterval IntervalType
        {
            get => _intervalType;
            private set => SetProperty(ref _intervalType, value);
        }

        private int _interval;
        public int Interval
        {
            get => _interval;
            private set => SetProperty(ref _interval, value);
        }

        private DateTime _minimumDate;
        public DateTime MinimumDate
        {
            get => _minimumDate;
            private set => SetProperty(ref _minimumDate, value);
        }

        private DateTime _maximumDate;
        public DateTime MaximumDate
        {
            get => _maximumDate;
            private set => SetProperty(ref _maximumDate, value);
        }

        private double _minValue;
        public double MinValue
        {
            get => _minValue;
            private set => SetProperty(ref _minValue, value);
        }

        private double _maxValue;
        public double MaxValue
        {
            get => _maxValue;
            private set => SetProperty(ref _maxValue, value);
        }

        private string _dateTimeLabelFormat;
        public string DateTimeLabelFormat
        {
            get => _dateTimeLabelFormat;
            set => SetProperty(ref _dateTimeLabelFormat, value);
        }
        
        private ObservableCollection<LineSeries> _seriesSource;
        public ObservableCollection<LineSeries> SeriesSource
        {
            get => _seriesSource;
            private set => SetProperty(ref _seriesSource, value);
        }

        private ChartPageScene _scene;
        public ChartPageScene Scene
        {
            get => _scene;
            private set => SetProperty(ref _scene, value);
        }

        private List<string> Lines { get; set; }

        private LogFile File { get; }
        #endregion

        #region services
        private ISensorsService SensorsService { get; }
        #endregion

        #region commands
        public ICommand SelectTimeOptionCommand { get; }

        public ICommand SelectParameterCommand { get; }
        #endregion

        public ChartPageViewModel(LogFile file)
        {
            SelectTimeOptionCommand = new Command(SelectTimeOption);
            SelectParameterCommand = new Command(SelectParameter);
            SensorsService = Ioc.Default.GetService<ISensorsService>();
            TimeAxisOption = Settings.GraphRange;
            File = file;
            Init(file);
        }

        #region override
        public override void OnAppearing()
        {
            base.OnAppearing();
            MessengerInstance.Register<LogRowAddedMessage>(this, OnLogRowAdded);
            MessengerInstance.Register<LogStatusChangedMessage>(this, OnLogStatusChanged);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            MessengerInstance.Unregister(this);
        }
        #endregion

        public async void Init(LogFile file)
        {
            try
            {
                UpdateTitle(file);
                SeriesSource = new ObservableCollection<LineSeries>();
                var ljhReadTask = FileSystem.ReadAllTextAsync(file.LjhFilePath);
                var lcvReadTask = FileSystem.ReadAllLinesAsync(file.LcvFilePath);
                await Task.WhenAll(lcvReadTask, ljhReadTask);

                var holder = JsonConvert.DeserializeObject<LJH_Holder>(ljhReadTask.Result);
                var lines = lcvReadTask.Result.ToList();

                OnLjhHolderRead(holder);
                OnLcvLinesRead(lines, true);
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await OnBacksAsync();
            }
        }

        private void OnLogRowAdded(LogRowAddedMessage message)
        {
            try
            {
                if(Lines != null)
                {
                    Lines.Add(message.Line);
                }
                OnLjhHolderRead(message.Holder);
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
        }

        private float _previousVersion = -1;
        private void OnLjhHolderRead(LJH_Holder holder)
        {
            if(holder.Version == _previousVersion)
            {
                OnLcvLinesRead(Lines, false);
                return;
            }
            _previousVersion = holder.Version;
            var parameters = new List<GraphParameter>();
            foreach(var set in holder.Sets)
            {
                foreach(var column in set.Columns)
                {
                    var columnIndex = set.Columns.IndexOf(column);
                    if (parameters.FirstOrDefault(x => SensorsService.IsSameParameterAndUnit(column.Code, column.UnitCode, (int)x.Sensor, (int)x.Unit)) is GraphParameter existingParameter)
                    {
                        if (!existingParameter.InfoList.Any(x => x.SetId == set.SetID && x.ColumnIndex == columnIndex))
                        {
                            var index = existingParameter.InfoList.Count;
                            existingParameter.InfoList.Add(CreateParameterInfo(set.SetID, columnIndex, column.SerialNumber, column.Code, index));
                        }
                        continue;
                    }

                    parameters.Add(new GraphParameter
                    {
                        Sensor = (SensorType)column.Code,
                        SensorName = column.Sensor,
                        Unit = (Enums.SensorUnit)column.UnitCode,
                        UnitName = column.Unit,
                        InfoList = new List<GraphParameterInfo>
                        {
                            CreateParameterInfo(set.SetID, columnIndex, column.SerialNumber, column.Code, 0)
                        }
                    });
                }
            }
            var previousSelected = Parameter;
            parameters.RemoveAll(IsLocationParameter);
            Parameters = parameters.OrderBy(x => x.DisplayName).ToList();
            
            if(previousSelected != null)
            {
                Parameter = parameters.FirstOrDefault(x => SensorsService.IsSameParameterAndUnit((int)previousSelected.Sensor, (int)previousSelected.Unit, (int)x.Sensor, (int)x.Unit));
            }
            else
            {
                Parameter = Parameters.FirstOrDefault(x => x.Id == Settings.SelectedGraphParameterId) ?? Parameters.FirstOrDefault();
            }

            var names = Parameters.Select(x => x.DisplayName).ToList();
        }

        private GraphParameterInfo CreateParameterInfo(int setId, int columnIndex, string serialNumber, int sensorCode, int index)
        {
            return new GraphParameterInfo
            {
                SetId = setId,
                SerialNumber = serialNumber,
                ColumnIndex = columnIndex,
                Color = SensorsService.GetSensorColor(sensorCode, index)
            };
        }

        private bool IsLocationParameter(GraphParameter parameter)
        {
            return (parameter.Sensor == SensorType.PRBSEN_LATITUDE || parameter.Sensor == SensorType.PRBSEN_LONGITUDE) && parameter.Unit == Enums.SensorUnit.PRBUNT_DEG;
        }



private void OnLcvLinesRead(List<string> lines, bool parameterChanged)
    {
        if (lines == null)
        {
            return;
        }

        Lines = lines.ToList();
        if (Parameter == null)
        {
            return;
        }

        try
        {
            var source = new ObservableCollection<LineSeries>(); // Replace ChartSeriesCollection with List<ChartSeries>
            UpdateRange();

            foreach (var info in Parameter.InfoList)
            {
                bool isNew = false;

                // Find existing series by checking DisplayName
                //var series = source.FirstOrDefault(x => x.DisplayName == info.SerialNumber) as LineSeries;

                  if (!(source.FirstOrDefault(x => x.DisplayName == info.SerialNumber) is LineSeries series))
                   {
                    series = new LineSeries
                    {
                        DisplayName = info.SerialNumber,
                        ItemsSource = new ObservableCollection<ChartEntry>(),
                        ValueBinding = new PropertyNameDataPointBinding(nameof(ChartEntry.Value)),
                        CategoryBinding = new PropertyNameDataPointBinding(nameof(ChartEntry.TimeStamp)),
                        Stroke =info.Color,                        
                        StrokeThickness = 4
                    };
                    isNew = true;
                }

                var setLines = lines.Where(x => x.StartsWith($"{info.SetId},")).ToList();

                if (setLines.Any())
                {
                    var firstLine = setLines.FirstOrDefault();
                    OnReadLine(firstLine, info, series, parameterChanged);
                }

                foreach (var line in setLines)
                {
                    OnReadLine(line, info, series, false);
                }

                if (isNew)
                {
                    source.Add(series);
                }
            }

            if (SeriesSource != null)
            {
                SeriesSource.Clear();
            }

            foreach (var series in source)
            {
                var seriesItemsSource = series.ItemsSource as ObservableCollection<ChartEntry>;
                var sorted = seriesItemsSource.OrderBy(x => x.TimeStamp).ToObservableCollection();

                if (LogService.IsLogging && sorted.LastOrDefault() is ChartEntry entry)
                {
                    if (entry.TimeStamp.CompareTo(MaximumDate) > 0)
                    {
                        MaximumDate = entry.TimeStamp;
                    }
                }

                series.ItemsSource = sorted;
            }

            SeriesSource = source;
        }
        catch (Exception ex)
        {
            AnalyticsService.TrackError(ex);
        }
    }

    //private void OnLcvLinesRead1(List<string> lines, bool parameterChanged)
    //    {
    //        if (lines == null)
    //        {
    //            return;
    //        }
    //        Lines = lines.ToList();
    //        if(Parameter == null)
    //        {
    //            return;
    //        }

    //        try
    //        {  
    //            var source = new ChartSeriesCollection();
    //            UpdateRange();
    //            foreach (var info in Parameter.InfoList)
    //            {
    //                var isNew = false;

                   
    //                if(!(source.FirstOrDefault(x => ((FastLineSeries)x).Label == info.SerialNumber) is FastLineSeries series))
    //                {
                       
    //                    series = new FastLineSeries
    //                    {
    //                        Label = info.SerialNumber,
    //                        ItemsSource = new ObservableCollection<ChartEntry>(),
    //                        XBindingPath = nameof(ChartEntry.TimeStamp),
    //                        YBindingPath = nameof(ChartEntry.Value),
    //                        //Color = info.Color,
    //                        Fill=new SolidColorBrush(info.Color),
    //                        StrokeWidth = 4,
    //                    };
    //                    isNew = true;
    //                }
    //                var setLines = lines.Where(x => x.StartsWith($"{info.SetId},")).ToList();

    //                if (setLines.Any())
    //                {
    //                    var firstLine = setLines.FirstOrDefault();
    //                    OnReadLine(firstLine, info, series, parameterChanged);
    //                }

    //                foreach (var line in setLines)
    //                {
    //                    OnReadLine(line, info, series, false);
    //                }
    //                if (isNew)
    //                {
    //                    source.Add(series);
    //                }
    //            }

    //            if(SeriesSource != null)
    //            {
    //                SeriesSource.Clear();
    //            }
    //            foreach(var series in source)
    //            {
    //                var seriesItemsSource = series.ItemsSource as ObservableCollection<ChartEntry>;
    //                var sorted = seriesItemsSource.OrderBy(x => x.TimeStamp).ToObservableCollection();
    //                if(LogService.IsLogging && sorted.LastOrDefault() is ChartEntry entry)
    //                {
    //                    var compare = entry.TimeStamp.CompareTo(MaximumDate);
    //                    if(compare > 0)
    //                    {
    //                        MaximumDate = entry.TimeStamp;
    //                    }
    //                }
    //                series.ItemsSource = sorted;
    //            }
    //            SeriesSource = source;
    //        }
    //        catch (Exception ex)
    //        {
    //            AnalyticsService.TrackError(ex);
    //        }
    //    }

        private void OnReadLine(string line, GraphParameterInfo info, LineSeries series, bool isMinMaxConfiguration)
        {
            var values = line.Split(',').Select(x => x.Trim()).ToList();
            var value = double.Parse(values[info.ColumnIndex + 2]);
            var timeStamp = DateTime.Parse(values[1], null, System.Globalization.DateTimeStyles.RoundtripKind).ToLocalTime();
            var lineSource = series.ItemsSource as ObservableCollection<ChartEntry>;
            lineSource.Add(new ChartEntry
            {
                Value = value,
                TimeStamp = timeStamp
            });

            if (isMinMaxConfiguration && !SetValuesRange())
            {
                MinValue = value;
                MaxValue = value + 1;
            }
            else
            {
                MinValue = MinValue < value ? MinValue : value;
                MaxValue = MaxValue > value ? MaxValue : value;
            }
        }

        private void OnTimeAxisOptionChanged()
        {
            RaisePropertyChanged(nameof(TimeAxisOption));
            Settings.GraphRange = TimeAxisOption;
            UpdateRange();
        }

        private void SetMinutesFormat()
        {
            IntervalType = TimeInterval.Minute;
            Interval = 5;
            DateTimeLabelFormat = "HH:mm";
        }

        private void SetHoursFormat()
        {
            IntervalType = TimeInterval.Hour;
            Interval = 1;
            DateTimeLabelFormat = "HH:mm";
        }

        private void SetDaysFormat()
        {
            IntervalType = TimeInterval.Day;
            Interval = 1;
            DateTimeLabelFormat = "ddd";
        }

        private void OnParameterChanged()
        {
            RaisePropertyChanged(nameof(Parameter));
            OnLcvLinesRead(Lines, true);
            Settings.SelectedGraphParameterId = Parameter?.Id ?? "";
        }

        private bool SetValuesRange()
        {
            SensorsService.DefaultScale((int)Parameter.Sensor, (int)Parameter.Unit, out var minimum, out var maximum);
            if(minimum == -1 && maximum == -1)
            {
                return false;
            }
            MinValue = minimum;
            MaxValue = maximum;
            return true;
        }

        private void UpdateRange()
        {
            if (LogService.IsLogging)
            {
                UpdateRangeForLiveGraph();
            }
            else
            {
                UpdateRangeForReviewGraph();
            }
        }

        private void UpdateRangeForReviewGraph()
        {
            if (Lines?.Any() != true)
            {
                return;
            }
            var dateTimes = Lines
                .Select(x => x.Split(','))
                .Where(x => x.Length > 1)
                .Select(x => x[1].Trim())
                .Select(x => x.ToDateNullableTime())
                .Where(x => x != null)
                .Select(x => (DateTime)x)
                .OrderBy(x => x).ToList();
            MaximumDate = dateTimes.LastOrDefault();
            switch (TimeAxisOption)
            {
                case GraphTimeOption.FifteenMinutes:
                    MinimumDate = MaximumDate.AddMinutes(-15);
                    break;
                case GraphTimeOption.OneHour:
                    MinimumDate = MaximumDate.AddHours(-1);
                    break;
                case GraphTimeOption.EightHours:
                    MinimumDate = MaximumDate.AddHours(-8);
                    break;
                case GraphTimeOption.Day:
                    MinimumDate = MaximumDate.AddDays(-1);
                    break;
                case GraphTimeOption.Everything:
                    MinimumDate = dateTimes.FirstOrDefault();
                    break;
            }
            UpdateIntervalType();
        }

        private void UpdateRangeForLiveGraph()
        {
            if(Lines?.Any() != true)
            {
                return;
            }
            var dateTimes = Lines
                .Select(x => x.Split(','))
                .Select(x => x[1])
                .Select(x => DateTime.Parse(x.Trim()))
                .OrderBy(x => x).ToList();
            MinimumDate = dateTimes.FirstOrDefault();
            var maximumDateFromList = dateTimes.LastOrDefault();
            DateTime expectedMaximumDate;
            switch (TimeAxisOption)
            {
                case GraphTimeOption.FifteenMinutes:
                    expectedMaximumDate = MinimumDate.AddMinutes(15);
                    break;
                case GraphTimeOption.OneHour:
                    expectedMaximumDate = MinimumDate.AddHours(1);
                    break;
                case GraphTimeOption.EightHours:
                    expectedMaximumDate = MinimumDate.AddHours(8);
                    break;
                case GraphTimeOption.Day:
                    expectedMaximumDate = MinimumDate.AddDays(1);
                    break;
                default:
                    expectedMaximumDate = dateTimes.LastOrDefault();
                    break;
            }
            MaximumDate = expectedMaximumDate.CompareTo(maximumDateFromList) == 1 ? expectedMaximumDate : maximumDateFromList;
            UpdateIntervalType();
        }

        private void UpdateIntervalType()
        {
            var diff = MaximumDate.Subtract(MinimumDate);
            if (diff.TotalDays > 1)
            {
                SetDaysFormat();
            }
            else if (diff.TotalHours > 1)
            {
                SetHoursFormat();
            }
            else
            {
                SetMinutesFormat();
            }
        }

        private async void SelectTimeOption()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                var tcs = new TaskCompletionSource<GraphTimeOption>();
                var popup = new SelectChartTimeOptionPopupPage(TimeAxisOption, tcs);
                await Navigation.PushPopupAsync(popup);
                TimeAxisOption = await tcs.Task;
            }
            catch (TaskCanceledException) { }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void SelectParameter()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                var tcs = new TaskCompletionSource<GraphParameter>();
                var popup = new SelectGraphParameterPopupPage(Parameters, Parameter, tcs);
                await Navigation.PushPopupAsync(popup);
                Parameter = await tcs.Task;
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateTitle(LogFile file)
        {
            var format = "";
            if (LogService.IsLogging)
            {
                format = Localization.Localization.Chart_LiveGraph;
            }
            else
            {
                format = Localization.Localization.Chart_ReviewGraph;
            }
            Title = String.Format(format, file.Name);
        }

        private void OnLogStatusChanged(LogStatusChangedMessage message)
        {
            UpdateTitle(File);
        }
    }
}
