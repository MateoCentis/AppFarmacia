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

    public virtual ICollection<ArticuloFinal> ArticuloFinals { get; set; } = new List<ArticuloFinal>();

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();
}
