using Microsoft.Extensions.Logging;
using AppFarmacia.Services;
using AppFarmacia.ViewModels;
using AppFarmacia.Views;

namespace AppFarmacia
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

            builder.Services.AddSingleton<PaginaVentas>();
            builder.Services.AddSingleton<PaginaArticulos>();
            builder.Services.AddSingleton<PaginaVentasViewModel>();
            builder.Services.AddSingleton<PaginaArticulosViewModel>();
            builder.Services.AddSingleton<VentasService>();
            builder.Services.AddSingleton<ArticulosService>();
#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
