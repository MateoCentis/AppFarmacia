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
    public class ArticulosFinalesController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public ArticulosFinalesController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ArticuloFinal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloFinalDTO>>> GetArticulosFinales()
        {
            var articuloFinal = await _context.ArticulosFinales.ToListAsync();
            var articuloFinalDTO = _mapper.Map<IEnumerable<ArticuloFinalDTO>>(articuloFinal);
            return Ok(articuloFinalDTO);
        }

        // GET: api/ArticuloFinal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticuloFinalDTO>> GetArticuloFinal(int id)
        {
            var articuloFinal = await _context.ArticulosFinales
                .FirstOrDefaultAsync(a => a.IdArticuloFinal == id);

            if (articuloFinal == null)
            {
                return NotFound();
            }
            var articuloFinalDTO = _mapper.Map<ArticuloFinalDTO>(articuloFinal);
            return Ok(articuloFinalDTO);
        }

        // PUT: api/ArticuloFinal/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticuloFinal(int id, ArticuloFinalDTO articuloFinalDTO)
        {
            // Verificar si el ID del Privilegio en el DTO coincide con el ID de la ruta
            if (id != articuloFinalDTO.IdArticuloFinal)
            {
                return BadRequest("El ID del articuloFinal en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }
            // Verificar si el privilegio con el ID especificado existe en la base de datos
            var articuloFinalExistente = await _context.ArticulosFinales.FindAsync(id);
            if (articuloFinalExistente == null)
            {
                return NotFound($"No se encontró un articuloFinal con el ID {id}.");
            }
            // Cuando Editamos un privilegio solo nos interesa cambia el nombre
            //privilegioExistente.Nombre = privilegioDTO.Nombre;

            // Tambien podemos hacer otra DTO llama EditPrivilegioDTO y quitarle el vector de usuarios, esta clase se usaria como [FromBody]
            _mapper.Map(articuloFinalDTO, articuloFinalExistente);


            // Marcar la entidad como modificada
            _context.Entry(articuloFinalExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticuloFinalExists(id))
                {
                    return NotFound($"No se encontró un articuloFinal con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }
            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST: api/ArticuloFinal
        [HttpPost]
        public async Task<ActionResult<ArticuloFinal>> PostArticuloFinal(ArticuloFinalDTO articuloFinalDTO)
        {
            var articuloFinal = _mapper.Map<ArticuloFinal>(articuloFinalDTO);

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

            var createdArticuloFinalDTO = _mapper.Map<ArticuloFinalDTO>(articuloFinal);

            return CreatedAtAction(nameof(GetArticuloFinal), new { id = createdArticuloFinalDTO.IdArticuloFinal }, createdArticuloFinalDTO);
        }

        // DELETE: api/ArticuloFinal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticuloFinal(int id)
        {
            var articuloFinal = await _context.ArticulosFinales
                .Include(p => p.Stocks)
                .Include(p => p.ArticuloEnVenta)
                .FirstOrDefaultAsync(p => p.IdArticuloFinal == id);
            if (articuloFinal == null)
            {
                return NotFound($"No se encontró un articuloFinal con el ID {id}.");
            }

            // Verificar si hay usuarios asociados al privilegio
            if (articuloFinal.Stocks.Any())
            {
                return BadRequest("No se puede eliminar el articuloFinal porque tiene stock asignados.");
            }
            if (articuloFinal.ArticuloEnVenta.Any())
            {
                return BadRequest("No se puede eliminar el articuloFinal porque tiene articuloEnVenta asignados.");
            }

            _context.ArticulosFinales.Remove(articuloFinal);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar el articuloFinal: {ex.Message}");
            }

            return NoContent();
        }

        private bool ArticuloFinalExists(int id)
        {
            return _context.ArticulosFinales.Any(e => e.IdArticuloFinal == id);
        }
    }
}
