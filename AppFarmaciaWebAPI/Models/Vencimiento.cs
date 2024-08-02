using System;
using System.Collections.Generic;

namespace AppFarmaciaWebAPI.Models;

public partial class Vencimiento
{
    public int IdVencimiento { get; set; }

    public DateOnly Fecha { get; set; }

    public int? IdArticulo { get; set; }

    public virtual Articulo? IdArticuloNavigation { get; set; }
}
