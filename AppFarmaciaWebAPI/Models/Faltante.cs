namespace AppFarmaciaWebAPI.Models;

public partial class Faltante
{
    public int IdFaltante { get; set; }

    public DateTime Fecha { get; set; }

    public int CantidadFaltante {get; set; }

    public int IdArticulo { get; set; }

    public virtual Articulo IdArticuloNavigation { get; set; } = null!;
}
