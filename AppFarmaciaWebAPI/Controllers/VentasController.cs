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
    public class VentasController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public VentasController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Venta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDTO>>> GetVentas()
        {
            var ventas = await _context.Ventas.ToListAsync();
            var ventasDTO = _mapper.Map<IEnumerable<VentaDTO>>(ventas);
            return Ok(ventasDTO);
        }

        // GET: api/Venta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDTO>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.ArticuloEnVenta)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            var ventaDTO = _mapper.Map<VentaDTO>(venta);
            return Ok(ventaDTO);
        }

        // PUT: api/Venta/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditVenta(int id, [FromBody] VentaDTO ventaDTO)
        {
            // Verificar si el ID de la Venta en el DTO coincide con el ID de la ruta
            if (id != ventaDTO.IdVenta)
            {
                return BadRequest("El ID de la venta en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            // Verificar si la venta con el ID especificado existe en la base de datos
            var ventaExistente = await _context.Ventas.FindAsync(id);
            if (ventaExistente == null)
            {
                return NotFound($"No se encontró una venta con el ID {id}.");
            }

            // Actualizar la venta existente con los datos del DTO
            _mapper.Map(ventaDTO, ventaExistente);

            // Marcar la entidad como modificada
            _context.Entry(ventaExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VentaExists(id))
                {
                    return NotFound($"No se encontró una venta con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST: api/Venta
        [HttpPost]
        public async Task<ActionResult<VentaDTO>> PostVenta([FromBody] VentaDTO ventaDTO)
        {
            var venta = _mapper.Map<Venta>(ventaDTO);

            _context.Ventas.Add(venta);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VentaExists(venta.IdVenta))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdVentaDTO = _mapper.Map<VentaDTO>(venta);

            return CreatedAtAction(nameof(GetVenta), new { id = createdVentaDTO.IdVenta }, createdVentaDTO);
        }

        // DELETE: api/Venta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Ventas.Include(v => v.ArticuloEnVenta).FirstOrDefaultAsync(v => v.IdVenta == id);
            if (venta == null)
            {
                return NotFound($"No se encontró una venta con el ID {id}.");
            }

            // Verificar si hay artículos asociados a la venta
            if (venta.ArticuloEnVenta.Any())
            {
                return BadRequest("No se puede eliminar la venta porque tiene artículos asignados.");
            }

            _context.Ventas.Remove(venta);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar la venta: {ex.Message}");
            }

            return NoContent();
        }


        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.IdVenta == id);
        }
    }
}
