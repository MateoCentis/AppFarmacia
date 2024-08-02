using System;
using System.Collections.Generic;

namespace AppFarmaciaWebAPI.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public DateTime Fecha { get; set; }

    public virtual ICollection<ArticuloEnVenta> ArticuloEnVenta { get; set; } = new List<ArticuloEnVenta>();
}
