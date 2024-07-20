namespace AppFarmaciaWebAPI.Models;

public partial class Precio
{
    public int IdPrecio { get; set; }

    public DateOnly Fecha { get; set; }

    public decimal Valor { get; set; }

    public int IdArticulo { get; set; }

    public virtual Articulo IdArticuloNavigation { get; set; } = null!;
}
