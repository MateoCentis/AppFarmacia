namespace AppFarmaciaWebAPI.Models;

public partial class ArticuloEnCompra
{
    public int IdArticuloCompra { get; set; }

    public int Cantidad { get; set; }

    public int IdArticulo { get; set; }

    public int IdCompra { get; set; }

    public string MotivoCompra { get; set; } = null!;

    public virtual Articulo IdArticuloNavigation { get; set; } = null!;

    public virtual Compra IdCompraNavigation { get; set; } = null!;

}
