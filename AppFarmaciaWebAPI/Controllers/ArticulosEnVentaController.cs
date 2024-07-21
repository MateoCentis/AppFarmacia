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
    public class ArticulosEnVentaController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;

        public ArticulosEnVentaController(FarmaciaDbContext context)
        {
            _context = context;
        }

        // GET: api/ArticuloEnVenta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloEnVenta>>> GetArticulosEnVenta()
        {
            return await _context.ArticulosEnVenta.ToListAsync();
        }

        // GET: api/ArticuloEnVenta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticuloEnVenta>> GetArticuloEnVenta(int id)
        {
            var articuloEnVenta = await _context.ArticulosEnVenta.FindAsync(id);

            if (articuloEnVenta == null)
            {
                return NotFound();
            }

            return articuloEnVenta;
        }

        // PUT: api/ArticuloEnVenta/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticuloEnVenta(int id, ArticuloEnVenta articuloEnVenta)
        {
            if (id != articuloEnVenta.IdArticuloVenta)
            {
                return BadRequest();
            }

            _context.Entry(articuloEnVenta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticuloEnVentaExists(id))
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

        // POST: api/ArticuloEnVenta
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ArticuloEnVenta>> PostArticuloEnVenta(ArticuloEnVenta articuloEnVenta)
        {
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

            return CreatedAtAction("GetArticuloEnVenta", new { id = articuloEnVenta.IdArticuloVenta }, articuloEnVenta);
        }

        // DELETE: api/ArticuloEnVenta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticuloEnVenta(int id)
        {
            var articuloEnVenta = await _context.ArticulosEnVenta.FindAsync(id);
            if (articuloEnVenta == null)
            {
                return NotFound();
            }

            _context.ArticulosEnVenta.Remove(articuloEnVenta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticuloEnVentaExists(int id)
        {
            return _context.ArticulosEnVenta.Any(e => e.IdArticuloVenta == id);
        }
    }
}
