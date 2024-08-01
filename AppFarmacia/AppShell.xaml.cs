using AppFarmacia.Views;

namespace AppFarmacia
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(PaginaDetalleVenta), typeof(PaginaDetalleVenta));
            Routing.RegisterRoute(nameof(PaginaArticuloFinal), typeof(PaginaArticuloFinal));
        }
    }
}
