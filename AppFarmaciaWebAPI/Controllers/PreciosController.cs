using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AppFarmaciaWebAPI.ModelsDTO;
using AutoMapper;

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
            var precio = await _context.Precios.ToListAsync();
            var precioDTO = _mapper.Map<IEnumerable<PrecioDTO>>(precio);
            return Ok(precioDTO);

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

        // PUT: api/Precio/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPrecio(int id, [FromBody] PrecioDTO precioDTO)
        {
            if (id != precioDTO.IdPrecio)
            {
                return BadRequest("El ID del precio no se encuentra en la base de datos");
            }
            //Se cambian solo fecha y valor
            var precioExistente = await _context.Precios.FindAsync(id);
            if (precioExistente == null)
            {
                return NotFound($"No se encontró un precio con el ID {id}.");
            }

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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PrecioDTO>> PostPrecio(PrecioDTO precioDTO)
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
