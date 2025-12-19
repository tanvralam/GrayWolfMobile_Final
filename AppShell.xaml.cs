using GrayWolf.Views;

namespace GrayWolf
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("home", typeof(HomePage));

            // Delay navigation to ensure Shell is fully initialized
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100); // Small delay to prevent issues
                await Shell.Current.GoToAsync("//home");
            });

        }


    }


}
