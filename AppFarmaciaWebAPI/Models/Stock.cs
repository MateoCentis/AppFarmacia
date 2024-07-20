namespace AppFarmaciaWebAPI.Models;

public partial class Stock
{
    public int IdStock { get; set; }

    public int IdArticuloFinal { get; set; }

    public DateTime Fecha { get; set; }

    public int CantidadActual { get; set; }

    public virtual ArticuloFinal IdArticuloFinalNavigation { get; set; } = null!;
}
