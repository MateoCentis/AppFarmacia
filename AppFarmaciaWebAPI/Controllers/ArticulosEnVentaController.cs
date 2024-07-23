using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AutoMapper;
using AppFarmaciaWebAPI.ModelsDTO;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosEnVentaController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public ArticulosEnVentaController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ArticuloEnVenta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloEnVentaDTO>>> GetArticulosEnVenta()
        {
            var articulosEnVenta = await _context.ArticulosEnVenta.ToListAsync();
            var articulosEnVentaDTO = _mapper.Map<IEnumerable<ArticuloEnVentaDTO>>(articulosEnVenta);
            return Ok(articulosEnVentaDTO);
        }

        // GET: api/ArticuloEnVenta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticuloEnVentaDTO>> GetArticuloEnVenta(int id)
        {
            var articuloEnVenta = await _context.ArticulosEnVenta.FirstOrDefaultAsync(a => a.IdArticuloVenta == id);

            if (articuloEnVenta == null)
            {
                return NotFound();
            }

            var articuloEnVentaDTO = _mapper.Map<ArticuloEnVentaDTO>(articuloEnVenta);
            return Ok(articuloEnVentaDTO);
        }

        // PUT: api/ArticuloEnVenta/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditArticuloEnVenta(int id, [FromBody] ArticuloEnVentaDTO articuloEnVentaDTO)
        {
            // Verificar si el ID del artículo en venta en el DTO coincide con el ID de la ruta
            if (id != articuloEnVentaDTO.IdArticuloVenta)
            {
                return BadRequest("El ID del artículo en venta en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            // Verificar si el artículo en venta con el ID especificado existe en la base de datos
            var articuloEnVentaExistente = await _context.ArticulosEnVenta.FindAsync(id);
            if (articuloEnVentaExistente == null)
            {
                return NotFound($"No se encontró un artículo en venta con el ID {id}.");
            }

            // Actualizar el artículo en venta existente con los datos del DTO
            _mapper.Map(articuloEnVentaDTO, articuloEnVentaExistente);

            // Marcar la entidad como modificada
            _context.Entry(articuloEnVentaExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticuloEnVentaExists(id))
                {
                    return NotFound($"No se encontró un artículo en venta con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST: api/ArticuloEnVenta
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ArticuloEnVentaDTO>> PostArticuloEnVenta([FromBody] ArticuloEnVentaDTO articuloEnVentaDTO)
        {
            var articuloEnVenta = _mapper.Map<ArticuloEnVenta>(articuloEnVentaDTO);

            _context.ArticulosEnVenta.Add(articuloEnVenta);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ArticuloEnVentaExists(articuloEnVenta.IdArticuloVenta))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdArticuloEnVentaDTO = _mapper.Map<ArticuloEnVentaDTO>(articuloEnVenta);

            return CreatedAtAction(nameof(GetArticuloEnVenta), new { id = createdArticuloEnVentaDTO.IdArticuloVenta }, createdArticuloEnVentaDTO);
        }

        // DELETE: api/ArticuloEnVenta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticuloEnVenta(int id)
        {
            var articuloEnVenta = await _context.ArticulosEnVenta.FirstOrDefaultAsync(a => a.IdArticuloVenta == id);
            if (articuloEnVenta == null)
            {
                return NotFound($"No se encontró un artículo en venta con el ID {id}.");
            }

            _context.ArticulosEnVenta.Remove(articuloEnVenta);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar el artículo en venta: {ex.Message}");
            }

            return NoContent();
        }

        private bool ArticuloEnVentaExists(int id)
        {
            return _context.ArticulosEnVenta.Any(e => e.IdArticuloVenta == id);
        }
    }
}
