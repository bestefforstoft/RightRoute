using Microsoft.Extensions.Logging;
using RightRoute.Views;
using RightRoute.ViewModels;
using RightRoute.Services;

namespace RightRoute
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                // Register Services
                .Services
                    .AddSingleton<DatabaseService>()
                    .AddSingleton<OsrmService>()
                    .AddSingleton<NavigationService>()
                    // Register ViewModels
                    .AddTransient<RouteEditorViewModel>()
                    // Register Pages
                    .AddTransient<RouteEditorPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
