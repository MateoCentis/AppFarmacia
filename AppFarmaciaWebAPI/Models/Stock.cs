using System;
using System.Collections.Generic;

namespace AppFarmaciaWebAPI.Models;

public partial class Stock
{
    public int IdStock { get; set; }

    public int IdArticulo { get; set; }

    public DateTime Fecha { get; set; }

    public int CantidadActual { get; set; }

    public virtual Articulo IdArticuloNavigation { get; set; } = null!;
}
