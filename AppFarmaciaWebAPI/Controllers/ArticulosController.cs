using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AutoMapper;
using AppFarmaciaWebAPI.ModelsDTO;

namespace AppFarmaciaWebAPI.Controllers  
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public ArticulosController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET'S---

        // GET: api/Articulo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloDTO>>> GetArticulos()
        {
            var articulo = await _context.Articulos.ToListAsync();
            var articuloDTO = _mapper.Map<IEnumerable<ArticuloDTO>>(articulo);
            return Ok(articuloDTO);
        }

        // GET: api/Articulo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticuloDTO>> GetArticulo(int id)
        {
            var articulo = await _context.Articulos
                .FirstOrDefaultAsync(a => a.IdArticulo == id);

            if (articulo == null)
            {
                return NotFound();
            }
            var articuloDTO = _mapper.Map<ArticuloDTO>(articulo);
            return Ok(articuloDTO);
        }

        // GET: api/Articulo/5/precios

        // PUT: api/Articulo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticulo(int id, ArticuloDTO articuloDTO)
        {
            // Verificar si el ID del articulo en el DTO coincide con el ID de la ruta
            if (id != articuloDTO.IdArticulo)
            {
                return BadRequest("El ID del articulo en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }
            // Verificar si el articulo con el ID especificado existe en la base de datos
            var articuloExistente = await _context.Articulos.FindAsync(id);
            if (articuloExistente == null)
            {
                return NotFound($"No se encontró un articulo con el ID {id}.");
            }

            _mapper.Map(articuloDTO, articuloExistente);


            // Marcar la entidad como modificada
            _context.Entry(articuloExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticuloExists(id))
                {
                    return NotFound($"No se encontró un articulo con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }
            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST: api/Articulo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ArticuloDTO>> AddArticulo(ArticuloDTO articuloDTO)
        {
            var articulo = _mapper.Map<Articulo>(articuloDTO);

            _context.Articulos.Add(articulo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ArticuloExists(articulo.IdArticulo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdarticuloDTO = _mapper.Map<ArticuloDTO>(articulo);

            return CreatedAtAction(nameof(GetArticulo), new { id = createdarticuloDTO.IdArticulo }, createdarticuloDTO);
        }

        // DELETE: api/Articulo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticulo(int id)
        {
            var articulo = await _context.Articulos.FirstOrDefaultAsync(p => p.IdArticulo == id);
            if (articulo == null)
            {
                return NotFound($"No se encontró un articulo con el ID {id}.");
            }

            // Verificar si hay usuarios asociados al articulo
            if (articulo.ArticulosFinales.Any())
            {
                return BadRequest("No se puede eliminar el articulo porque tiene usuarios asignados.");
            }
            if (articulo.Precios.Any())
            {
                return BadRequest("No se puede eliminar el articulo porque tiene usuarios asignados.");
            }

            _context.Articulos.Remove(articulo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar el Articulo: {ex.Message}");
            }

            return NoContent();
        }

        private bool ArticuloExists(int id)
        {
            return _context.Articulos.Any(e => e.IdArticulo == id);
        }
    }
}
