namespace AppFarmacia.Models
{
    public partial class Privilegio
    {
        public int IdPrivilegio { get; set; }
        public string Nombre { get; set; } = null!;
        public ICollection<Usuario> Usuarios { get; set; } = [];
    }
}