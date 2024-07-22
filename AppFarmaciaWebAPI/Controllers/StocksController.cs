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
    public class StocksController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public StocksController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDTO>>> GetStocks()
        {
            var stock = await _context.Stocks.ToListAsync();
            var stockDTO = _mapper.Map<IEnumerable<StockDTO>>(stock);
            return Ok(stockDTO);
        }

        // GET: api/Stocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockDTO>> GetStock(int id)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.IdStock == id);

            if (stock == null)
            {
                return NotFound();
            }
            var stockDTO = _mapper.Map<StockDTO>(stock);
            return Ok(stockDTO);
        }

        // PUT: api/Stocks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditStock(int id, [FromBody] StockDTO stockDTO)
        {
            // Verificar si el ID del Stock en el DTO coincide con el ID de la ruta
            if (id != stockDTO.IdStock)
            {
                return BadRequest("El ID del stock en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            // Verificar si el stock con el ID especificado existe en la base de datos
            var stockExistente = await _context.Stocks.FindAsync(id);
            if (stockExistente == null)
            {
                return NotFound($"No se encontró un stock con el ID {id}.");
            }

            // Actualizar el stock existente con los datos del DTO
            _mapper.Map(stockDTO, stockExistente);

            // Marcar la entidad como modificada
            _context.Entry(stockExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
                {
                    return NotFound($"No se encontró un stock con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST: api/Stocks
        [HttpPost]
        public async Task<ActionResult<StockDTO>> PostStock([FromBody] StockDTO stockDTO)
        {
            var stock = _mapper.Map<Stock>(stockDTO);

            _context.Stocks.Add(stock);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StockExists(stock.IdStock))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdStockDTO = _mapper.Map<StockDTO>(stock);

            return CreatedAtAction(nameof(GetStock), new { id = createdStockDTO.IdStock }, createdStockDTO);
        }

        // DELETE: api/Stocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.IdStock == id);
            if (stock == null)
            {
                return NotFound($"No se encontró un stock con el ID {id}.");
            }

            _context.Stocks.Remove(stock);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar el stock: {ex.Message}");
            }

            return NoContent();
        }


        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.IdStock == id);
        }
    }
}
