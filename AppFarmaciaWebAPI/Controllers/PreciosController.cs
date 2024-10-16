using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AutoMapper;
using AppFarmaciaWebAPI.ModelsDTO;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreciosController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public PreciosController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Precio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrecioDTO>>> GetPrecios()
        {
            var precios = await _context.Precios.ToListAsync();
            var preciosDTO = _mapper.Map<IEnumerable<PrecioDTO>>(precios);
            return Ok(preciosDTO);
        }

        // GET: api/Precio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrecioDTO>> GetPrecio(int id)
        {
            var precio = await _context.Precios.FindAsync(id);

            if (precio == null)
            {
                return NotFound();
            }

            var precioDTO = _mapper.Map<PrecioDTO>(precio);
            return Ok(precioDTO);
        }
        [HttpGet("Articulo/{id}")]
        public async Task<ActionResult<IEnumerable<PrecioDTO>>> GetPreciosArticulo(int id)
        {
            var precios = await _context.Precios
                .Where(p => p.IdArticulo == id)
                .OrderBy(p => p.Fecha) // Ordenar por fecha
                .ToListAsync();

            if (precios == null || precios.Count == 0)
            {
                return NotFound();
            }

            var preciosDTO = _mapper.Map<IEnumerable<PrecioDTO>>(precios);

            return Ok(preciosDTO);
        }


        // PUT: api/Precio/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPrecio(int id, [FromBody] PrecioDTO precioDTO)
        {
            if (id != precioDTO.IdPrecio)
            {
                return BadRequest("El ID del precio no se encuentra en la base de datos.");
            }

            var precioExistente = await _context.Precios.FindAsync(id);
            if (precioExistente == null)
            {
                return NotFound($"No se encontró un precio con el ID {id}.");
            }

            // Actualizar solo los campos relevantes
            precioExistente.Fecha = precioDTO.Fecha;
            precioExistente.Valor = precioDTO.Valor;

            _context.Entry(precioExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrecioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Precio
        [HttpPost]
        public async Task<ActionResult<PrecioDTO>> PostPrecio([FromBody] PrecioDTO precioDTO)
        {
            var precio = _mapper.Map<Precio>(precioDTO);

            _context.Precios.Add(precio);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PrecioExists(precio.IdPrecio))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdPrecioDTO = _mapper.Map<PrecioDTO>(precio);

            return CreatedAtAction(nameof(GetPrecio), new { id = createdPrecioDTO.IdPrecio }, createdPrecioDTO);
        }

        // DELETE: api/Precio/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecio(int id)
        {
            var precio = await _context.Precios.FindAsync(id);
            if (precio == null)
            {
                return NotFound();
            }

            _context.Precios.Remove(precio);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error al eliminar el precio: {ex.Message}");
            }

            return NoContent();
        }

        private bool PrecioExists(int id)
        {
            return _context.Precios.Any(e => e.IdPrecio == id);
        }
    }
}
