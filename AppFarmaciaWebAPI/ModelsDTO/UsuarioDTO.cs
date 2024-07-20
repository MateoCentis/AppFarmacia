namespace AppFarmaciaWebAPI.ModelsDTO
{
    // Se accede a los campos de usuario : Idusuario, Nombre y Password
    // Del privilegio solo su Id, donde en el controlador se obtiene con este el privilegio
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int IdPrivilegio { get; set; }
        //public string NombrePrivilegio { get; set; };
    }
}
