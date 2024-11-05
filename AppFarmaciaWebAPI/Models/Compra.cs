using System;
using System.Collections.Generic;

namespace AppFarmaciaWebAPI.Models;

public partial class Compra
{
    public int IdCompra { get; set; }
    public DateTime Fecha { get; set; }
    public string Proveedor { get; set; } = null!;
    public string Descripcion { get; set; } = null!;

    public virtual ICollection<ArticuloEnCompra> ArticulosEnCompra { get; set; } = [];
}