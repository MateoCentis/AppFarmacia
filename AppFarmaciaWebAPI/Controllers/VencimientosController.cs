using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AppFarmaciaWebAPI.ModelsDTO;
using AutoMapper;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VencimientosController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public VencimientosController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Vencimientos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VencimientoDTO>>> GetVencimientos()
        {
            var vencimientos = await _context.Vencimientos.ToListAsync();
            var vencimientoDTOs = _mapper.Map<IEnumerable<VencimientoDTO>>(vencimientos);
            return Ok(vencimientoDTOs);
        }

        // GET: api/Vencimientos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VencimientoDTO>> GetVencimiento(int id)
        {
            var vencimiento = await _context.Vencimientos.FindAsync(id);

            if (vencimiento == null)
            {
                return NotFound();
            }

            var vencimientoDTO = _mapper.Map<VencimientoDTO>(vencimiento);
            return Ok(vencimientoDTO);
        }

        // GET: api/Vencimientos/Articulos/5 (obtiene todos los vencimientos dado un idArticulo)
        [HttpGet("Articulos/{id}")]
        public async Task<ActionResult<IEnumerable<VencimientoDTO>>> GetVencimientosArticulo(int id)
        {
            var vencimientos = await _context.Vencimientos
                .Where(v => v.IdArticulo == id)
                .ToListAsync();

            if (vencimientos == null || vencimientos.Count == 0)
            {
                return NotFound();
            }

            var vencimientosDTO = _mapper.Map<IEnumerable<VencimientoDTO>>(vencimientos);

            return Ok(vencimientosDTO);
        }

        // PUT: api/Vencimientos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVencimiento(int id, [FromBody] VencimientoDTO vencimientoDTO)
        {
            if (id != vencimientoDTO.IdVencimiento)
            {
                return BadRequest("El ID del vencimiento en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            var vencimientoExistente = await _context.Vencimientos.FindAsync(id);
            if (vencimientoExistente == null)
            {
                return NotFound($"No se encontró un vencimiento con el ID {id}.");
            }

            _mapper.Map(vencimientoDTO, vencimientoExistente);

            _context.Entry(vencimientoExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VencimientoExists(id))
                {
                    return NotFound($"No se encontró un vencimiento con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Vencimientos
        [HttpPost]
        public async Task<ActionResult<VencimientoDTO>> PostVencimiento([FromBody] VencimientoDTO vencimientoDTO)
        {
            var vencimiento = _mapper.Map<Vencimiento>(vencimientoDTO);

            _context.Vencimientos.Add(vencimiento);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VencimientoExists(vencimiento.IdVencimiento))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdVencimientoDTO = _mapper.Map<VencimientoDTO>(vencimiento);

            return CreatedAtAction(nameof(GetVencimiento), new { id = createdVencimientoDTO.IdVencimiento }, createdVencimientoDTO);
        }

        // DELETE: api/Vencimientos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVencimiento(int id)
        {
            var vencimiento = await _context.Vencimientos.FindAsync(id);
            if (vencimiento == null)
            {
                return NotFound($"No se encontró un vencimiento con el ID {id}.");
            }

            _context.Vencimientos.Remove(vencimiento);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Ocurrió un error al intentar eliminar el vencimiento: {ex.Message}");
            }

            return NoContent();
        }

        private bool VencimientoExists(int id)
        {
            return _context.Vencimientos.Any(e => e.IdVencimiento == id);
        }
    }
}
