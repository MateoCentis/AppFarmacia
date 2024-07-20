namespace AppFarmaciaWebAPI.Models;

public partial class ArticuloFinal
{
    public int IdArticuloFinal { get; set; }

    public int IdArticulo { get; set; }

    public DateOnly Vencimiento { get; set; }

    public virtual ICollection<ArticuloEnVenta> ArticuloEnVenta { get; set; } = new List<ArticuloEnVenta>();

    public virtual Articulo IdArticuloNavigation { get; set; } = null!;

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
