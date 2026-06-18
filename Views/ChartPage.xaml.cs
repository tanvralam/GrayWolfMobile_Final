using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;
using Telerik.Maui.Controls.Compatibility.Chart;

namespace GrayWolf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartPage
    {
        private readonly ChartPageViewModel _viewModel;

        public ChartPage(LogFile file)
        {
            InitializeComponent();

            _viewModel = new ChartPageViewModel(file);
            BindingContext = _viewModel;

            RefreshChart();

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.SeriesSource) ||
    e.PropertyName == nameof(_viewModel.ChartRefreshVersion))
                {
                    RefreshChart();
                }
            };
        }

        private void RefreshChart()
        {
            RefreshAxes();

            chart.Series.Clear();

            if (_viewModel.SeriesSource == null)
            {
                return;
            }

            foreach (var series in _viewModel.SeriesSource)
            {
                chart.Series.Add(series);
            }

            RefreshAxes();
            chart.InvalidateMeasure();
        }
        private void RefreshAxes()
        {
            if (chart.HorizontalAxis is DateTimeContinuousAxis horizontalAxis &&
                _viewModel.MinimumDate < _viewModel.MaximumDate)
            {
                var minimum = _viewModel.MinimumDate;
                var maximum = _viewModel.MaximumDate;

                horizontalAxis.LabelFormat = _viewModel.DateTimeLabelFormat;
                horizontalAxis.MajorStepUnit = _viewModel.IntervalType;
                horizontalAxis.MajorStep = _viewModel.Interval;

                if (minimum > horizontalAxis.Maximum)
                {
                    horizontalAxis.Maximum = maximum;
                    horizontalAxis.Minimum = minimum;
                }
                else if (maximum < horizontalAxis.Minimum)
                {
                    horizontalAxis.Minimum = minimum;
                    horizontalAxis.Maximum = maximum;
                }
                else
                {
                    horizontalAxis.Minimum = minimum;
                    horizontalAxis.Maximum = maximum;
                }
            }

            if (chart.VerticalAxis is NumericalAxis verticalAxis &&
                _viewModel.MinValue < _viewModel.MaxValue)
            {
                var minimum = _viewModel.MinValue;
                var maximum = _viewModel.MaxValue;

                if (minimum > verticalAxis.Maximum)
                {
                    verticalAxis.Maximum = maximum;
                    verticalAxis.Minimum = minimum;
                }
                else if (maximum < verticalAxis.Minimum)
                {
                    verticalAxis.Minimum = minimum;
                    verticalAxis.Maximum = maximum;
                }
                else
                {
                    verticalAxis.Minimum = minimum;
                    verticalAxis.Maximum = maximum;
                }
            }
        }
    }
}