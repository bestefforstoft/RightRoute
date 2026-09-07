using Microsoft.Extensions.DependencyInjection;

namespace RightRoute
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Navigate to the routeeditorpage after app starts
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("///routeeditorpage");
            });
        }
    }
}