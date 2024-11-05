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

    public string? Clasificacion { get; set; }
    public int? DemandaAnual { get; set; }
    public int? PuntoReposicion { get; set; }
    public int? CantidadAPedir { get; set; }
    public int? DemandaAnualHistorica { get; set; }
    public string? NombresDrogas { get; set; }

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual ICollection<ArticuloEnVenta> ArticulosEnVenta { get; set; } = new List<ArticuloEnVenta>();

    public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<Vencimiento> Vencimientos { get; set; } = new List<Vencimiento>();

    public virtual ICollection<ArticuloEnCompra> ArticulosEnCompra { get; set; } = [];

    public virtual ICollection<Faltante> Faltantes { get; set; } = [];
}
