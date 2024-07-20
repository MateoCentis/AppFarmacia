using AppFarmaciaWebAPI.ModelsDTO;
using AppFarmaciaWebAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace AppFarmaciaWebAPI.Services
{
    public class UsuarioService
    {
        
        //private readonly FarmaciaDbContext _context;

        //public UsuarioService(FarmaciaDbContext context)
        //{
        //    _context = context;
        //}

        //public List<ModelsDTO.UsuarioDTO> GetUsuarios()
        //{
        //    var usuarios = _context.Usuarios.Include(u => u.IdPrivilegioNavigation).ToList();
        //    var privilegio = _context.Privilegios.Find(p => p.Id).ToList();
        //    return usuarios.Select(u => new UsuarioDTO
        //    {
        //        IdUsuario = u.IdUsuario,
        //        Nombre = u.Nombre,
        //        IdPrivilegio = u.IdPrivilegio,
        //        NombrePrivilegio = u.IdPrivilegioNavigation.Nombre
        //    }).ToList();
        //}

        
    }
}
