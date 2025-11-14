using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AutoMapper;
using AppFarmaciaWebAPI.ModelsDTO;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public ArticulosController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Articulos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloDTO>>> GetArticulos(int size = 0)
        {
            // Cargar todas las categorías en un diccionario para mapeo eficiente
            var categoriasDict = await _context.Categorias
                .AsNoTracking()
                .ToDictionaryAsync(c => c.IdCategoria, c => c.Nombre);

            // Consulta principal de artículos
            var query = _context.Articulos
                .Select(a => new ArticuloDTO
                {
                    IdArticulo = a.IdArticulo,
                    Nombre = a.Nombre,
                    Marca = a.Marca,
                    Descripcion = a.Descripcion,
                    Codigo = a.Codigo,
                    IdCategoria = a.IdCategoria,
                    NombreCategoria = null, // Se asignará después
                    Activo = a.Activo,
                    Clasificacion = a.Clasificacion,
                    DemandaAnual = a.DemandaAnual,
                    PuntoReposicion = a.PuntoReposicion,
                    CantidadAPedir = a.CantidadAPedir,
                    DemandaAnualHistorica = a.DemandaAnualHistorica,
                    NombresDrogas = a.NombresDrogas,

                    // Optimización: Usar subconsultas para obtener los últimos valores
                    UltimoVencimiento = a.Vencimientos.Any()
                        ? (DateOnly?)a.Vencimientos
                            .OrderByDescending(v => v.Fecha)
                            .Select(v => v.Fecha)
                            .FirstOrDefault()
                        : null,
                    UltimoPrecio = a.Precios
                        .OrderByDescending(p => p.Fecha)
                        .Select(p => p.Valor)
                        .FirstOrDefault(),
                    UltimoStock = a.Stocks
                        .OrderByDescending(s => s.Fecha)
                        .Select(s => s.CantidadActual)
                        .FirstOrDefault()
                })
                .AsNoTracking();

            // Limitar el tamaño de los resultados si se especifica
            if (size > 0)
            {
                query = query.Take(size);
            }

            var articuloDTOs = await query.ToListAsync();

            // Asignar nombres de categorías desde el diccionario
            foreach (var articulo in articuloDTOs)
            {
                if (articulo.IdCategoria.HasValue && categoriasDict.TryGetValue(articulo.IdCategoria.Value, out var nombreCategoria))
                {
                    articulo.NombreCategoria = nombreCategoria;
                }
            }

            return Ok(articuloDTOs);
        }


        [HttpGet("ArticulosSugeridosParaComprar")]
        public async Task<ActionResult<IEnumerable<ArticuloDTO>>> GetArticulosSugeridosParaComprar()
        {
            try
            {
                // Obtén los artículos junto con su último stock
                var articulos = await _context.Articulos
                    .Include(a => a.Stocks)
                    .Where(a => a.Stocks.Any()) // Solo los artículos que tienen algún historial de stock
                    .Select(a => new
                    {
                        Articulo = a,
                        UltimoStock = a.Stocks.OrderByDescending(s => s.Fecha).FirstOrDefault() // Obtener el último stock (el más reciente)
                    })
                    .Where(a => a.UltimoStock != null && a.UltimoStock.CantidadActual <= a.Articulo.PuntoReposicion) // Verifica que el último stock sea menor al ROP
                    .ToListAsync();

                // Si no hay artículos que cumplir con la condición
                if (articulos.Count == 0)
                {
                    return Ok(new List<ArticuloDTO>());
                }

                // Mapear los resultados a DTOs
                var articuloDTOs = _mapper.Map<IEnumerable<ArticuloDTO>>(articulos.Select(a => a.Articulo));

                return Ok(articuloDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al intentar obtener los artículos sugeridos para comprar: {ex.Message}");
            }
        }

            // GET: api/Articulos/5
            [HttpGet("{id}")]
        public async Task<ActionResult<ArticuloDTO>> GetArticulo(int id)
        {
            var articulo = await _context.Articulos
                .FirstOrDefaultAsync(a => a.IdArticulo == id);

            if (articulo == null)
            {
                return NotFound();
            }
            var articuloDTO = _mapper.Map<ArticuloDTO>(articulo);
            return Ok(articuloDTO);
        }

        //Obtiene las demandas mensuales de un determinado artículo
        // GET: api/Articulos/{id}/Demandas/{year}
        [HttpGet("{id}/Demandas/{year}")]
        public async Task<ActionResult<List<int>>> GetDemandaMensual(int id, int year)
        {
            // Nos traemos las ventas que tienen artículos en venta con ese id de artículo
            var ventas = await _context.Ventas
                .Where(v => _context.ArticulosEnVenta.Any(aev => aev.IdArticulo == id && aev.IdVenta == v.IdVenta))
                .Where(v => v.Fecha.Year == year)
                .ToListAsync();

            if (ventas == null || ventas.Count == 0)
            {
                return NotFound();
            }

            var demandaMensual = Enumerable.Repeat(0, 12).ToList();

            var articulosEnVenta = await _context.ArticulosEnVenta
                .Where(aev => aev.IdArticulo == id && ventas.Select(v => v.IdVenta).Contains(aev.IdVenta))
                .ToListAsync();

            // Agrupa los artículos en venta por mes de la fecha de la venta y cuenta la cantidad de artículos vendidos en cada mes
            var cantidadesPorMes = articulosEnVenta
                .Join(ventas, aev => aev.IdVenta, v => v.IdVenta, (aev, v) => new { aev, v.Fecha })
                .GroupBy(av => av.Fecha.Month)
                .Select(g => new { Mes = g.Key, Cantidad = g.Count() })
                .ToList();

            // Actualiza la lista de demanda mensual con las cantidades
            foreach (var cantidad in cantidadesPorMes)
            {
                demandaMensual[cantidad.Mes - 1] = cantidad.Cantidad; // Mes - 1 porque la lista es 0-indexada
            }

            return Ok(demandaMensual);
        }


        // PUT: api/Articulos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticulo(int id, ArticuloDTO articuloDTO)
        {
            // Verificar si el ID del articulo en el DTO coincide con el ID de la ruta
            if (id != articuloDTO.IdArticulo)
            {
                return BadRequest("El ID del articulo en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }
            // Verificar si el articulo con el ID especificado existe en la base de datos
            var articuloExistente = await _context.Articulos.FindAsync(id);
            if (articuloExistente == null)
            {
                return NotFound($"No se encontró un articulo con el ID {id}.");
            }

            _mapper.Map(articuloDTO, articuloExistente);

            // Marcar la entidad como modificada
            _context.Entry(articuloExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticuloExists(id))
                {
                    return NotFound($"No se encontró un articulo con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }
            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST: api/Articulos
        [HttpPost]
        public async Task<ActionResult<ArticuloDTO>> AddArticulo(ArticuloDTO articuloDTO)
        {
            var articulo = _mapper.Map<Articulo>(articuloDTO);

            _context.Articulos.Add(articulo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ArticuloExists(articulo.IdArticulo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdArticuloDTO = _mapper.Map<ArticuloDTO>(articulo);

            return CreatedAtAction(nameof(GetArticulo), new { id = createdArticuloDTO.IdArticulo }, createdArticuloDTO);
        }

        // DELETE: api/Articulos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticulo(int id)
        {
            var articulo = await _context.Articulos
                .Include(a => a.Precios)
                .FirstOrDefaultAsync(a => a.IdArticulo == id);

            if (articulo == null)
            {
                return NotFound($"No se encontró un articulo con el ID {id}.");
            }

            if (articulo.Stocks.Count > 0)
            {
                return BadRequest("No se puede eliminar el articulo porque tiene stocks asignados.");
            }
            // Verificar si el articulo tiene precios asociados
            if (articulo.Precios.Count > 0)
            {
                return BadRequest("No se puede eliminar el articulo porque tiene precios asignados.");
            }

            _context.Articulos.Remove(articulo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar el Articulo: {ex.Message}");
            }

            return NoContent();
        }

        private bool ArticuloExists(int id)
        {
            return _context.Articulos.Any(e => e.IdArticulo == id);
        }
    }
}
