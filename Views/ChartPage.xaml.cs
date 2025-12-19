using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;
using Syncfusion.Maui.Core.Carousel;


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
                if (e.PropertyName == nameof(_viewModel.SeriesSource))
                {
                    RefreshChart();
                }
            };
        }

        private void RefreshChart()
        {
            chart.Series.Clear();
            foreach (var series in _viewModel.SeriesSource)
            {
                chart.Series.Add(series);
            }
        }
    }
}