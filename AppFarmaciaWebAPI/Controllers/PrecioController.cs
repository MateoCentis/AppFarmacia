using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PrecioController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;

        public PrecioController(FarmaciaDbContext context)
        {
            _context = context;
        }

        // GET: api/Precio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Precio>>> GetPrecios()
        {
            return await _context.Precios.ToListAsync();
        }

        // GET: api/Precio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Precio>> GetPrecio(int id)
        {
            var precio = await _context.Precios.FindAsync(id);

            if (precio == null)
            {
                return NotFound();
            }

            return precio;
        }

        // PUT: api/Precio/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrecio(int id, Precio precio)
        {
            if (id != precio.IdPrecio)
            {
                return BadRequest();
            }

            _context.Entry(precio).State = EntityState.Modified;

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
        public async Task<ActionResult<Precio>> PostPrecio(Precio precio)
        {
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

            return CreatedAtAction("GetPrecio", new { id = precio.IdPrecio }, precio);
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
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrecioExists(int id)
        {
            return _context.Precios.Any(e => e.IdPrecio == id);
        }
    }
}
