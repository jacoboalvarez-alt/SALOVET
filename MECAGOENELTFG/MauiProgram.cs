using MECAGOENELTFG.Views;
using MECAGOENELTFG.Services;
using MECAGOENELTFG.ViewModels;
using Microsoft.Extensions.Logging;

namespace MECAGOENELTFG
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
                    fonts.AddFont("Euphorigenic.otf", "Euphorigenic");
                    fonts.AddFont("Nunito-Regular.ttf", "Nunito");
                    fonts.AddFont("Notedry.ttf", "Notedry");
                    fonts.AddFont("PrinceOutfitDemoRegular.ttf", "PriceOut");
                    fonts.AddFont("MiranoExtendedFreebie-Light", "Mirano");
                });
            builder.Services.AddSingleton<ClienteApiService>();
            builder.Services.AddSingleton<MascotaApiService>();


            builder.Services.AddTransient<ClientesViewModel>();
            builder.Services.AddTransient<MascotasViewModel>();
            builder.Services.AddTransient<ClienteFormViewModel>();

            builder.Services.AddTransient<ClientesPage>();
            builder.Services.AddTransient<MascotasPage>();
            builder.Services.AddTransient<ClienteFormPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
