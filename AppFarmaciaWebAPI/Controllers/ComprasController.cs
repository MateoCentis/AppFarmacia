using AppFarmaciaWebAPI.Models;
using AppFarmaciaWebAPI.ModelsDTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;
        
        public ComprasController(FarmaciaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<ComprasController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDTO>>> GetCompras([FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
        {
            // Valores por defecto
            fechaInicio ??= new DateTime(2017, 6, 1); // 01/06/2017
            fechaFin ??= DateTime.Now; // Fecha actual

            var compras = await _context.Compras
                .Where(c => c.Fecha >= fechaInicio && c.Fecha <= fechaFin)
                .ToListAsync();

            var comprasDTO = _mapper.Map<IEnumerable<CompraDTO>>(compras);
            return Ok(comprasDTO);
        }

        // GET api/<ComprasController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompraDTO>> GetCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.ArticulosEnCompra)
                .FirstOrDefaultAsync(c => c.IdCompra == id);
            
            if (compra == null)
            {
                return NotFound();
            }

            var compraDTO = _mapper.Map<CompraDTO>(compra);
            return Ok(compraDTO);
        }

        // PUT api/<ComprasController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCompra(int id, [FromBody] CompraDTO compraDTO)
        {
            // Verificar si el ID de la Compra en el DTO coincide con el ID de la ruta
            if (id != compraDTO.IdCompra)
            {
                return BadRequest("El ID de la compra en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            // Verificar si la compra con el ID especificado existe en la base de datos
            var compraExistente = await _context.Compras.FindAsync(id);
            if (compraExistente == null)
            {
                return NotFound($"No se encontró una compra con el ID {id}.");
            }

            // Actualizar la compra existente con los datos del DTO
            _mapper.Map(compraDTO, compraExistente);

            // Marcar la entidad como modificada
            _context.Entry(compraExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompraExists(id))
                {
                    return NotFound($"No se encontró una compra con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST api/<ComprasController>
        [HttpPost]
        public async Task<ActionResult<CompraDTO>> PostCompra([FromBody] CompraDTO compraDTO)
        {
            var compra = _mapper.Map<Compra>(compraDTO);

            _context.Compras.Add(compra);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CompraExists(compra.IdCompra))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdCompraDTO = _mapper.Map<CompraDTO>(compra);

            return CreatedAtAction(nameof(GetCompra), new { id = createdCompraDTO.IdCompra }, createdCompraDTO);
        }


        // DELETE api/<ComprasController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompra(int id)
        {
            var compra = await _context.Compras.Include(v => v.ArticulosEnCompra).FirstOrDefaultAsync(v => v.IdCompra == id);
            if (compra == null)
            {
                return NotFound($"No se encontró una compra con el ID {id}.");
            }

            // Verificar si hay artículos asociados a la compra
            if (compra.ArticulosEnCompra.Any())
            {
                return BadRequest("No se puede eliminar la compra porque tiene artículos asignados.");
            }

            _context.Compras.Remove(compra);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar la compra: {ex.Message}");
            }

            return NoContent();
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.IdCompra == id);
        }
    }
}
