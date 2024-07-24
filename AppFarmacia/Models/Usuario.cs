namespace AppFarmacia.Models
{ 
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int IdPrivilegio { get; set; }
    }
}