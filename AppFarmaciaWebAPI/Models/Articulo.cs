using System;
using System.Collections.Generic;

namespace AppFarmaciaWebAPI.Models;

public partial class Articulo
{
    public int IdArticulo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Marca { get; set; }

    public string? Descripcion { get; set; }

    public int? IdCategoria { get; set; }

    public bool Activo { get; set; }

    public string? Codigo { get; set; }

    public virtual ICollection<ArticuloEnVenta> ArticuloEnVenta { get; set; } = new List<ArticuloEnVenta>();

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<Vencimiento> Vencimientos { get; set; } = new List<Vencimiento>();
}
