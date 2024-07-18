using System;
using System.Collections.Generic;

namespace AppFarmaciaWebAPI.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdPrivilegio { get; set; }

    public virtual Privilegio IdPrivilegioNavigation { get; set; } = null!;
}
