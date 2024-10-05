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
            var stocks = await _context.Stocks.ToListAsync();
            var stockDTOs = _mapper.Map<IEnumerable<StockDTO>>(stocks);
            return Ok(stockDTOs);
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

        // GET: api/Stocks/GetUltimoStockPorArticulo/1
        [HttpGet("GetUltimoStockPorArticulo/{id_articulo}")]
        public async Task<ActionResult<StockDTO>> GetUltimoStockPorArticulo(int id_articulo)
        {
            // Obtener el último registro de stock para el artículo específico
            var stock = await _context.Stocks
                .Where(s => s.IdArticulo == id_articulo)
                .OrderByDescending(s => s.Fecha) // Ordenar por fecha descendente
                .FirstOrDefaultAsync(); // Obtener el primer registro (el más reciente)

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
            if (id != stockDTO.IdStock)
            {
                return BadRequest("El ID del stock en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            var stockExistente = await _context.Stocks.FindAsync(id);
            if (stockExistente == null)
            {
                return NotFound($"No se encontró un stock con el ID {id}.");
            }

            _mapper.Map(stockDTO, stockExistente);

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
