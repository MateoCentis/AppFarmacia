using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AppFarmaciaWebAPI.ModelsDTO;
using AutoMapper;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PrivilegioController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public PrivilegioController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Privilegio
        [HttpGet]
        public ActionResult<IEnumerable<PrivilegioDTO>> GetPrivilegios()
        {
            var privilegio = _context.Privilegios.Include(p => p.Usuarios).ToList();
            var privilegioDTO = _mapper.Map<IEnumerable<PrivilegioDTO>>(privilegio);
            return Ok(privilegioDTO);
        }

        // GET: api/Privilegio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrivilegioDTO>> GetPrivilegio(int id)
        {
            var privilegio = await _context.Privilegios
                .Include(p => p.Usuarios)
                .FirstOrDefaultAsync(p => p.IdPrivilegio == id);
            
            if (privilegio == null)
            {
                return NotFound();
            }
            var privilegioDTO = _mapper.Map<PrivilegioDTO>(privilegio);
            return Ok(privilegioDTO);
        }

        // PUT: api/Privilegio/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPrivilegio(int id, [FromBody] PrivilegioDTO privilegioDTO)
        {
            // Verificar si el ID del Privilegio en el DTO coincide con el ID de la ruta
            if (id != privilegioDTO.IdPrivilegio)
            {
                return BadRequest("El ID del privilegio en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }
            // Verificar si el privilegio con el ID especificado existe en la base de datos
            var privilegioExistente = await _context.Privilegios.FindAsync(id);
            if (privilegioExistente == null)
            {
                return NotFound($"No se encontró un privilegio con el ID {id}.");
            }
            // Cuando Editamos un privilegio solo nos interesa cambia el nombre
            privilegioExistente.Nombre = privilegioDTO.Nombre;

            // Tambien podemos hacer otra DTO llama EditPrivilegioDTO y quitarle el vector de usuarios, esta clase se usaria como [FromBody]
            //_mapper.Map(privilegioDTO, privilegioExistente);


            // Marcar la entidad como modificada
            _context.Entry(privilegioExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrivilegioExists(id))
                {
                    return NotFound($"No se encontró un privilegio con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }
            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST: api/Privilegio
        [HttpPost]
        public async Task<ActionResult<PrivilegioDTO>> PostPrivilegio([FromBody] PrivilegioDTO privilegioDTO)
        {
            var privilegio = _mapper.Map<Privilegio>(privilegioDTO);

            _context.Privilegios.Add(privilegio);

            await _context.SaveChangesAsync();

            var createdPrivilegioDTO = _mapper.Map<PrivilegioDTO>(privilegio);

            return CreatedAtAction(nameof(GetPrivilegio), new { id = createdPrivilegioDTO.IdPrivilegio }, createdPrivilegioDTO);
        }

        // DELETE: api/Privilegio/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrivilegio(int id)
        {
            var privilegio = await _context.Privilegios.Include(p => p.Usuarios).FirstOrDefaultAsync(p => p.IdPrivilegio == id);
            if (privilegio == null)
            {
                return NotFound($"No se encontró un privilegio con el ID {id}.");
            }

            // Verificar si hay usuarios asociados al privilegio
            if (privilegio.Usuarios.Any())
            {
                return BadRequest("No se puede eliminar el privilegio porque tiene usuarios asignados.");
            }

            _context.Privilegios.Remove(privilegio);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar el privilegio: {ex.Message}");
            }

            return NoContent();
        }

        private bool PrivilegioExists(int id)
        {
            return _context.Privilegios.Any(e => e.IdPrivilegio == id);
        }
    }
}
