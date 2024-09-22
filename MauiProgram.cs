using Microsoft.Extensions.Logging;
using Kamata.Views;
using Microsoft.Maui.LifecycleEvents;
using Kamata.ViewModels;
using Kamata.Interfaces;
using Kamata.Services;

namespace Kamata
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
       

        #if DEBUG
                    builder.Logging.AddDebug();
        #endif
            

            // Pages
            builder.Services.AddTransient<SearchHeaderView>();
            builder.Services.AddTransient<RecentPlaceView>();
            builder.Services.AddTransient<PriceView>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<ServicesPage>();
            builder.Services.AddTransient<ActvivityPage>();
            builder.Services.AddTransient<MainLandingPage>();
            builder.Services.AddTransient<LoginPage>();
            

            //API
            builder.Services.AddTransient<IGoogleMapsApiService, GoogleMapsApiService>();
            

            // ViewModels
            builder.Services.AddTransient<BaseViewModel>();
            builder.Services.AddTransient<HomePageViewModel>();

            return builder.Build();
        }
    }
}
