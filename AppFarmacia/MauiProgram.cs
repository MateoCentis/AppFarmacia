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

            //Views
            builder.Services.AddSingleton<PaginaVentas>();
            builder.Services.AddSingleton<PaginaArticulos>();
            builder.Services.AddTransient<PaginaDetalleVenta>();

            //ViewModels
            builder.Services.AddSingleton<PaginaVentasViewModel>();
            builder.Services.AddSingleton<PaginaArticulosViewModel>();
            builder.Services.AddTransient<PaginaDetalleVentaViewModel>();

            //Servicios
            builder.Services.AddSingleton<VentasService>();
            builder.Services.AddSingleton<ArticulosService>();
            builder.Services.AddSingleton<ArticulosFinalesService>();
            builder.Services.AddSingleton<CategoriasService>();
            builder.Services.AddSingleton<PreciosService>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
