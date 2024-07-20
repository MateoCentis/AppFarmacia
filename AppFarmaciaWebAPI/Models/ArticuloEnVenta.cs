namespace AppFarmaciaWebAPI.Models;

public partial class ArticuloEnVenta
{
    public int IdArticuloVenta { get; set; }

    public int Cantidad { get; set; }

    public int IdArticuloFinal { get; set; }

    public int IdVenta { get; set; }

    public decimal Precio { get; set; }

    public virtual ArticuloFinal IdArticuloFinalNavigation { get; set; } = null!;

    public virtual Venta IdVentaNavigation { get; set; } = null!;
}
