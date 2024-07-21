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
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosFinalesController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;

        public ArticulosFinalesController(FarmaciaDbContext context)
        {
            _context = context;
        }

        // GET: api/ArticuloFinal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloFinal>>> GetArticulosFinales()
        {
            return await _context.ArticulosFinales.ToListAsync();
        }

        // GET: api/ArticuloFinal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticuloFinal>> GetArticuloFinal(int id)
        {
            var articuloFinal = await _context.ArticulosFinales.FindAsync(id);

            if (articuloFinal == null)
            {
                return NotFound();
            }

            return articuloFinal;
        }

        // PUT: api/ArticuloFinal/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticuloFinal(int id, ArticuloFinal articuloFinal)
        {
            if (id != articuloFinal.IdArticuloFinal)
            {
                return BadRequest();
            }

            _context.Entry(articuloFinal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticuloFinalExists(id))
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

        // POST: api/ArticuloFinal
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ArticuloFinal>> PostArticuloFinal(ArticuloFinal articuloFinal)
        {
            _context.ArticulosFinales.Add(articuloFinal);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ArticuloFinalExists(articuloFinal.IdArticuloFinal))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetArticuloFinal", new { id = articuloFinal.IdArticuloFinal }, articuloFinal);
        }

        // DELETE: api/ArticuloFinal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticuloFinal(int id)
        {
            var articuloFinal = await _context.ArticulosFinales.FindAsync(id);
            if (articuloFinal == null)
            {
                return NotFound();
            }

            _context.ArticulosFinales.Remove(articuloFinal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticuloFinalExists(int id)
        {
            return _context.ArticulosFinales.Any(e => e.IdArticuloFinal == id);
        }
    }
}
