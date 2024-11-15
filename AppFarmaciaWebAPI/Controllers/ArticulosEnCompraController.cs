using AppFarmaciaWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AppFarmaciaWebAPI.ModelsDTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosEnCompraController : ControllerBase
    {

        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public ArticulosEnCompraController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<ArticulosEnCompraController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloEnCompraDTO>>> GetArticulosEnCompra()
        {
            var articulosEnCompra = await _context.ArticulosEnCompra.ToListAsync();
            var articulosEnCompraDTO = _mapper.Map<IEnumerable<ArticuloEnCompraDTO>>(articulosEnCompra);
            return Ok(articulosEnCompraDTO);
        }

        // GET api/<ArticulosEnCompraController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticuloEnCompraDTO>> GetArticuloEnCompra(int id)
        {
            var articuloEnCompra = await _context.ArticulosEnCompra.Include(a => a.IdArticuloNavigation).FirstOrDefaultAsync(a => a.IdArticuloCompra == id);

            if (articuloEnCompra == null)
            {
                return NotFound();
            }

            var articuloEnCompraDTO = _mapper.Map<ArticuloEnCompraDTO>(articuloEnCompra);
            return Ok(articuloEnCompraDTO);
        }

        // GET: api/ArticulosEnCompra/PorCompraId/5 (Hdp mira como le pusiste)
        [HttpGet("PorCompraId/{id}")]
        public async Task<ActionResult<IEnumerable<ArticuloEnCompraDTO>>> GetArticulosEnCompraPorCompra(int id)
        {
            var articulosEnCompra = await _context.ArticulosEnCompra
                .Where(a => a.IdCompra == id)
                .Include(a => a.IdArticuloNavigation)
                .ToListAsync();

            if (articulosEnCompra == null || articulosEnCompra.Count == 0)
            {
                return NotFound();
            }

            var articulosEnCompraDTOs = _mapper.Map<IEnumerable<ArticuloEnCompraDTO>>(articulosEnCompra);
            return Ok(articulosEnCompraDTOs);
        }
 

        // POST api/<ArticulosEnCompraController>
        [HttpPost]
        public async Task<IActionResult> PostArticuloEnCompra([FromBody] ArticuloEnCompraDTO articuloEnCompraDTO)
        {
            var articuloEnCompra = _mapper.Map<ArticuloEnCompra>(articuloEnCompraDTO);

            _context.ArticulosEnCompra.Add(articuloEnCompra);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ArticuloEnCompraExists(articuloEnCompra.IdArticuloCompra))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Si no necesitas devolver ningún contenido específico, puedes usar NoContent o Ok
            return Ok(); // O puedes usar: return Ok();
        }

        // PUT api/<ArticulosEnCompraController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditArticuloEnCompra(int id, [FromBody] ArticuloEnCompraDTO articuloEnCompraDTO)
        {
            // Verificar si el ID del artículo en Compra en el DTO coincide con el ID de la ruta
            if (id != articuloEnCompraDTO.IdArticuloCompra)
            {
                return BadRequest("El ID del artículo en Compra en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            // Verificar si el artículo en Compra con el ID especificado existe en la base de datos
            var articuloEnCompraExistente = await _context.ArticulosEnCompra.FindAsync(id);
            if (articuloEnCompraExistente == null)
            {
                return NotFound($"No se encontró un artículo en Compra con el ID {id}.");
            }

            // Actualizar el artículo en Compra existente con los datos del DTO
            _mapper.Map(articuloEnCompraDTO, articuloEnCompraExistente);

            // Marcar la entidad como modificada
            _context.Entry(articuloEnCompraExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticuloEnCompraExists(id))
                {
                    return NotFound($"No se encontró un artículo en Compra con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // DELETE api/<ArticulosEnCompraController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticuloEnCompra(int id)
        {
            var articuloEnCompra = await _context.ArticulosEnCompra.FirstOrDefaultAsync(a => a.IdArticuloCompra == id);
            if (articuloEnCompra == null)
            {
                return NotFound($"No se encontró un artículo en Compra con el ID {id}.");
            }

            _context.ArticulosEnCompra.Remove(articuloEnCompra);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar el artículo en Compra: {ex.Message}");
            }

            return NoContent();
        }

        private bool ArticuloEnCompraExists(int id)
        {
            return _context.ArticulosEnCompra.Any(e => e.IdArticuloCompra == id);
        }

    }
}
