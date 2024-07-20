namespace AppFarmaciaWebAPI.Models;

public partial class Privilegio
{
    public int IdPrivilegio { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
