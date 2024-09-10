using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using DelegacjePracownicze.Services;

namespace DelegacjePracownicze
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
                });

#if DEBUG
    		builder.Logging.AddDebug();

#endif
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pl-PL");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pl-PL");
            builder.Services.AddSingleton<UserService>();

            return builder.Build();
        }
    }
}
