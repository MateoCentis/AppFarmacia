namespace AppFarmaciaWebAPI.ModelsDTO
{
    // Se pueden acceder a las características de privilegio
    // Además, de sus usuarios
    public class PrivilegioDTO
    {
        public int IdPrivilegio { get; set; }
        public string Nombre { get; set; } = null!;
        public ICollection<UsuarioDTO> UsuariosDTO { get; set; } = [];
    }
}
