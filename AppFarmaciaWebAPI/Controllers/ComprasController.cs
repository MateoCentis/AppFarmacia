using AppFarmaciaWebAPI.Models;
using AppFarmaciaWebAPI.ModelsDTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ComprasController> _logger;
        
        public ComprasController(FarmaciaDbContext context, IMapper mapper, ILogger<ComprasController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/<ComprasController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDTO>>> GetCompras([FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
        {
            // Valores por defecto
            fechaInicio ??= new DateTime(2017, 6, 1); // 01/06/2017
            fechaFin ??= DateTime.Now; // Fecha actual

            var compras = await _context.Compras
                .Where(c => c.Fecha >= fechaInicio && c.Fecha <= fechaFin)
                .ToListAsync();

            var comprasDTO = _mapper.Map<IEnumerable<CompraDTO>>(compras);
            return Ok(comprasDTO);
        }

        // GET api/<ComprasController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompraDTO>> GetCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.ArticulosEnCompra)
                .FirstOrDefaultAsync(c => c.IdCompra == id);
            
            if (compra == null)
            {
                return NotFound();
            }

            var compraDTO = _mapper.Map<CompraDTO>(compra);
            return Ok(compraDTO);
        }

        // PUT api/<ComprasController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCompra(int id, [FromBody] CompraDTO compraDTO)
        {
            // Verificar si el ID de la Compra en el DTO coincide con el ID de la ruta
            if (id != compraDTO.IdCompra)
            {
                return BadRequest("El ID de la compra en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            // Verificar si la compra con el ID especificado existe en la base de datos
            // Cargar con los artículos para verificar si se está confirmando
            var compraExistente = await _context.Compras
                .Include(c => c.ArticulosEnCompra)
                .FirstOrDefaultAsync(c => c.IdCompra == id);
            
            if (compraExistente == null)
            {
                return NotFound($"No se encontró una compra con el ID {id}.");
            }

            // IMPORTANTE: Verificar ANTES del mapeo si se está confirmando
            // Guardar la colección de artículos ANTES del mapeo porque AutoMapper podría afectarla
            var articulosEnCompraAntes = compraExistente.ArticulosEnCompra?.ToList() ?? new List<ArticuloEnCompra>();
            
            bool estabaConfirmada = compraExistente.CompraConfirmada;
            bool compraDTOConfirmada = compraDTO.CompraConfirmada;
            bool seEstaConfirmando = !estabaConfirmada && compraDTOConfirmada;

            _logger.LogWarning($"[ComprasController] EditCompra - ID: {id}");
            _logger.LogWarning($"[ComprasController] Estado ANTES del mapeo:");
            _logger.LogWarning($"[ComprasController]   - compraExistente.CompraConfirmada: {estabaConfirmada}");
            _logger.LogWarning($"[ComprasController]   - compraDTO.CompraConfirmada: {compraDTOConfirmada}");
            _logger.LogWarning($"[ComprasController]   - seEstaConfirmando: {seEstaConfirmando}");
            _logger.LogWarning($"[ComprasController]   - ArticulosEnCompra.Count ANTES: {articulosEnCompraAntes.Count}");

            // Actualizar manualmente solo los campos necesarios para evitar que AutoMapper afecte la colección
            compraExistente.Fecha = compraDTO.Fecha;
            compraExistente.Proveedor = compraDTO.Proveedor;
            compraExistente.Descripcion = compraDTO.Descripcion;
            compraExistente.CompraConfirmada = compraDTO.CompraConfirmada;
            // NO actualizar ArticulosEnCompra - mantener la colección original cargada de la BD
            
            _logger.LogWarning($"[ComprasController] Estado DESPUÉS del mapeo:");
            _logger.LogWarning($"[ComprasController]   - compraExistente.CompraConfirmada: {compraExistente.CompraConfirmada}");
            _logger.LogWarning($"[ComprasController]   - ArticulosEnCompra.Count DESPUÉS: {compraExistente.ArticulosEnCompra?.Count ?? 0}");

            // Si se está confirmando la compra, actualizar el stock de cada artículo
            _logger.LogWarning($"[ComprasController] Evaluando condición para actualizar stock:");
            _logger.LogWarning($"[ComprasController]   - seEstaConfirmando: {seEstaConfirmando}");
            _logger.LogWarning($"[ComprasController]   - ArticulosEnCompra.Any(): {compraExistente.ArticulosEnCompra?.Any() ?? false}");
            _logger.LogWarning($"[ComprasController]   - ArticulosEnCompra.Count: {compraExistente.ArticulosEnCompra?.Count ?? 0}");
            
            if (seEstaConfirmando)
            {
                _logger.LogWarning($"[ComprasController] ✓ Entrando al bloque de confirmación de compra");
                
                if (compraExistente.ArticulosEnCompra == null || !compraExistente.ArticulosEnCompra.Any())
                {
                    _logger.LogWarning($"[ComprasController] ⚠ No hay artículos en la compra, no se actualizará el stock");
                }
                else
                {
                    _logger.LogWarning($"[ComprasController] Confirmando compra - Artículos en compra: {compraExistente.ArticulosEnCompra.Count}");
                    
                    foreach (var articuloEnCompra in compraExistente.ArticulosEnCompra)
                    {
                        _logger.LogWarning($"[ComprasController] Procesando artículo - ID: {articuloEnCompra.IdArticulo}, Cantidad: {articuloEnCompra.Cantidad}");
                        
                        // Obtener el último stock del artículo
                        var ultimoStock = await _context.Stocks
                            .Where(s => s.IdArticulo == articuloEnCompra.IdArticulo)
                            .OrderByDescending(s => s.Fecha)
                            .FirstOrDefaultAsync();

                        int cantidadAnterior = ultimoStock?.CantidadActual ?? 0;
                        int nuevaCantidad = cantidadAnterior + articuloEnCompra.Cantidad;

                        _logger.LogWarning($"[ComprasController] Stock anterior: {cantidadAnterior}, Nueva cantidad: {nuevaCantidad}");

                        // Crear un nuevo registro de stock
                        var nuevoStock = new Stock
                        {
                            IdArticulo = articuloEnCompra.IdArticulo,
                            Fecha = DateTime.Now,
                            CantidadActual = nuevaCantidad
                        };

                        _context.Stocks.Add(nuevoStock);
                        _logger.LogWarning($"[ComprasController] Stock agregado al contexto - ID Artículo: {nuevoStock.IdArticulo}, Cantidad: {nuevoStock.CantidadActual}, Fecha: {nuevoStock.Fecha}");
                    }
                }
            }
            else
            {
                _logger.LogWarning($"[ComprasController] ✗ NO se está confirmando la compra");
                _logger.LogWarning($"[ComprasController]   - estabaConfirmada: {estabaConfirmada}");
                _logger.LogWarning($"[ComprasController]   - compraDTOConfirmada: {compraDTOConfirmada}");
                _logger.LogWarning($"[ComprasController]   - Tiene artículos: {compraExistente.ArticulosEnCompra?.Any() ?? false}");
            }

            // Marcar la entidad como modificada
            _context.Entry(compraExistente).State = EntityState.Modified;
            try
            {
                // Guardar todos los cambios (compra y stocks nuevos)
                _logger.LogWarning($"[ComprasController] Guardando cambios en la base de datos...");
                int cambiosGuardados = await _context.SaveChangesAsync();
                _logger.LogWarning($"[ComprasController] Cambios guardados: {cambiosGuardados}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompraExists(id))
                {
                    return NotFound($"No se encontró una compra con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST api/<ComprasController>
        [HttpPost]
        public async Task<ActionResult<CompraDTO>> PostCompra([FromBody] CompraDTO compraDTO)
        {
            var compra = _mapper.Map<Compra>(compraDTO);

            _context.Compras.Add(compra);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CompraExists(compra.IdCompra))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdCompraDTO = _mapper.Map<CompraDTO>(compra);

            return CreatedAtAction(nameof(GetCompra), new { id = createdCompraDTO.IdCompra }, createdCompraDTO);
        }


        // DELETE api/<ComprasController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompra(int id)
        {
            var compra = await _context.Compras.Include(v => v.ArticulosEnCompra).FirstOrDefaultAsync(v => v.IdCompra == id);
            if (compra == null)
            {
                return NotFound($"No se encontró una compra con el ID {id}.");
            }

            // Verificar si hay artículos asociados a la compra
            if (compra.ArticulosEnCompra.Any())
            {
                return BadRequest("No se puede eliminar la compra porque tiene artículos asignados.");
            }

            _context.Compras.Remove(compra);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar la compra: {ex.Message}");
            }

            return NoContent();
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.IdCompra == id);
        }
    }
}
