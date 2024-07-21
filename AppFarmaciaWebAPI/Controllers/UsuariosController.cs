using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AppFarmaciaWebAPI.ModelsDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly FarmaciaDbContext _context;

        public UsuariosController(IMapper mapper, FarmaciaDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Usuario
        [HttpGet]
        public ActionResult<IEnumerable<UsuarioDTO>> GetUsuarios()
        {
            var usuarios = _context.Usuarios.Include(u => u.IdPrivilegioNavigation).ToList();
            var usuarioDTOs = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
            return Ok(usuarioDTOs);
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.IdPrivilegioNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);
            return Ok(usuarioDTO);
        }

        // PUT: api/Usuario/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditUsuario(int id, [FromBody] UsuarioDTO usuarioDTO)
        {
            // Verificar si el ID del usuario en el DTO coincide con el ID de la ruta
            if (id != usuarioDTO.IdUsuario)
            {
                return BadRequest("El ID del usuario en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }
            // Verificar si el usuario con el ID especificado existe en la base de datos
            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound($"No se encontró un usuario con el ID {id}.");
            }
            // Mapear los datos del DTO al modelo de entidad
            _mapper.Map(usuarioDTO, usuarioExistente);
            // Marcar la entidad como modificada
            _context.Entry(usuarioExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound($"No se encontró un usuario con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }
            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST: api/Usuario
        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> AddUsuario([FromBody] UsuarioDTO usuarioDTO)
        {
            var usuario = _mapper.Map<Usuario>(usuarioDTO);
            
            _context.Usuarios.Add(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UsuarioExists(usuario.IdUsuario))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdUsuarioDTO = _mapper.Map<UsuarioDTO>(usuario);

            return CreatedAtAction(nameof(GetUsuario), new { id = createdUsuarioDTO.IdUsuario }, createdUsuarioDTO);
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound($"No se encontró un usuario con el ID {id}.");
            }
            _context.Usuarios.Remove(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, $"Ocurrió un error al querer borrar el usuario");
            }
            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}
