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
    public class PrivilegioController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;

        public PrivilegioController(FarmaciaDbContext context)
        {
            _context = context;
        }

        // GET: api/Privilegio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Privilegio>>> GetPrivilegios()
        {
            return await _context.Privilegios.ToListAsync();
        }

        // GET: api/Privilegio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Privilegio>> GetPrivilegio(int id)
        {
            var privilegio = await _context.Privilegios.FindAsync(id);
            
            if (privilegio == null)
            {
                return NotFound();
            }

            return privilegio;
        }

        // PUT: api/Privilegio/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrivilegio(int id, Privilegio privilegio)
        {
            if (id != privilegio.IdPrivilegio)
            {
                return BadRequest();
            }

            _context.Entry(privilegio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrivilegioExists(id))
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

        // POST: api/Privilegio
        [HttpPost]
        public async Task<ActionResult<Privilegio>> PostPrivilegio(Privilegio privilegio)
        {
            _context.Privilegios.Add(privilegio);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PrivilegioExists(privilegio.IdPrivilegio))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPrivilegio", new { id = privilegio.IdPrivilegio }, privilegio);
        }
        
        // DELETE: api/Privilegio/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrivilegio(int id)
        {
            var privilegio = await _context.Privilegios.FindAsync(id);
            if (privilegio == null)
            {
                return NotFound();
            }

            _context.Privilegios.Remove(privilegio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrivilegioExists(int id)
        {
            return _context.Privilegios.Any(e => e.IdPrivilegio == id);
        }
    }
}
