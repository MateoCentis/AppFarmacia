using Microsoft.Extensions.Logging;
using AppFarmacia.Services;
using AppFarmacia.ViewModels;
using AppFarmacia.Views;
using Microcharts.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AppFarmacia
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("times.ttf", "TimesNewRoman");
                    fonts.AddFont("timesbd.ttf", "TimesNewRomanBold");
                    fonts.AddFont("Lexend-Regular.ttf", "LexendRegular");
                    fonts.AddFont("Lexend-Black.ttf", "LexendBlack");
                    fonts.AddFont("Lexend-Bold.ttf", "LexendBold");
                    fonts.AddFont("Lexend-Medium.ttf", "LexendMedium");

                });

            //Views
            builder.Services.AddSingleton<PaginaVentas>();
            builder.Services.AddSingleton<PaginaArticulos>();
            builder.Services.AddTransient<PaginaDetalleVenta>();
            builder.Services.AddTransient<PaginaArticuloInformacion>();
            builder.Services.AddSingleton<PaginaGraficos>();
            builder.Services.AddSingleton<PaginaPrediccionStock>();
            builder.Services.AddSingleton<PaginaConfiguracion>();

            //ViewModels
            builder.Services.AddSingleton<PaginaVentasViewModel>();
            builder.Services.AddSingleton<PaginaArticulosViewModel>();
            builder.Services.AddTransient<PaginaDetalleVentaViewModel>();
            builder.Services.AddTransient<PaginaArticuloInformacionViewModel>();

            //Servicios
            builder.Services.AddSingleton<VentasService>();
            builder.Services.AddSingleton<ArticulosService>();
            builder.Services.AddSingleton<CategoriasService>();
            builder.Services.AddSingleton<PreciosService>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
