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
    public class VentasController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public VentasController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDTO>>> GetVentas(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            // si es null se trae todo
            if (!fechaInicio.HasValue)
            {
                fechaInicio = new DateTime(2017, 6, 1); // 01/06/2017
            }

            if (!fechaFin.HasValue)
            {
                fechaFin = DateTime.Now; // Fecha actual
            }

            // Filtrar por fechas
            var query = _context.Ventas.AsQueryable();

            query = query.Where(v => v.Fecha >= fechaInicio.Value && v.Fecha <= fechaFin.Value);

            // Incluir los ArticulosEnVenta
            var ventas = await query
                               .ToListAsync();
                               //.Include(v => v.ArticulosEnVenta) // Asegúrate de incluir los artículos en venta

            //// Calcular el monto total para cada venta
            //foreach (var venta in ventas)
            //{
            //    // Sumar el monto total de los artículos en venta
            //    venta.MontoTotal = venta.ArticuloEnVenta.Sum(a => a.Precio * a.Cantidad);
            //}

            // Mapear a VentaDTO
            var ventasDTO = _mapper.Map<IEnumerable<VentaDTO>>(ventas);

            return Ok(ventasDTO);
        }


        // GET: api/Venta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDTO>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.ArticuloEnVenta)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            var ventaDTO = _mapper.Map<VentaDTO>(venta);
            return Ok(ventaDTO);
        }

        // PUT: api/Venta/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditVenta(int id, [FromBody] VentaDTO ventaDTO)
        {
            // Verificar si el ID de la Venta en el DTO coincide con el ID de la ruta
            if (id != ventaDTO.IdVenta)
            {
                return BadRequest("El ID de la venta en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            // Verificar si la venta con el ID especificado existe en la base de datos
            var ventaExistente = await _context.Ventas.FindAsync(id);
            if (ventaExistente == null)
            {
                return NotFound($"No se encontró una venta con el ID {id}.");
            }

            // Actualizar la venta existente con los datos del DTO
            _mapper.Map(ventaDTO, ventaExistente);

            // Marcar la entidad como modificada
            _context.Entry(ventaExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VentaExists(id))
                {
                    return NotFound($"No se encontró una venta con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
            return NoContent();
        }

        // POST: api/Venta
        [HttpPost]
        public async Task<ActionResult<VentaDTO>> PostVenta([FromBody] VentaDTO ventaDTO)
        {
            var venta = _mapper.Map<Venta>(ventaDTO);

            _context.Ventas.Add(venta);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VentaExists(venta.IdVenta))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdVentaDTO = _mapper.Map<VentaDTO>(venta);

            return CreatedAtAction(nameof(GetVenta), new { id = createdVentaDTO.IdVenta }, createdVentaDTO);
        }

        // DELETE: api/Venta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Ventas.Include(v => v.ArticuloEnVenta).FirstOrDefaultAsync(v => v.IdVenta == id);
            if (venta == null)
            {
                return NotFound($"No se encontró una venta con el ID {id}.");
            }

            // Verificar si hay artículos asociados a la venta
            if (venta.ArticuloEnVenta.Any())
            {
                return BadRequest("No se puede eliminar la venta porque tiene artículos asignados.");
            }

            _context.Ventas.Remove(venta);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar la venta: {ex.Message}");
            }

            return NoContent();
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.IdVenta == id);
        }

        // --------------------------------------MÉTODOS RELACIONADOS A LOS GRÁFICOS--------------------------------------

        // Obtener la lista de cantidades vendidas por mes dado un año determinado
        [HttpGet("cantidad-por-mes/{year}")]
        public async Task<IActionResult> GetCantidadVendidaPorMes(int year)
        {
            // Realizamos la primera agregación en una subconsulta.
            var ventasPorMes = await _context.ArticulosEnVenta
                .Join(_context.Ventas,
                      articulo => articulo.IdVenta,
                      venta => venta.IdVenta,
                      (articulo, venta) => new { Articulo = articulo, Venta = venta })
                .Where(v => v.Venta.Fecha.Year == year)
                .GroupBy(v => v.Venta.Fecha.Month)
                .Select(g => new
                {
                    Mes = g.Key,
                    TotalCantidadVendida = g.Sum(v => v.Articulo.Cantidad)
                })
                .ToListAsync();

            return Ok(ventasPorMes);
        }

        [HttpGet("cantidad-por-hora-semana")]
        public async Task<IActionResult> GetCantidadVendidaPorHoraSemana()
        {
            // Obtener las ventas con los datos básicos desde la base de datos
            var ventasConDetalles = await _context.Ventas
                .Include(v => v.ArticuloEnVenta)  // Incluir los artículos en venta
                .ToListAsync();

            // Realizar la agrupación en memoria
            var ventasPorHoraSemana = ventasConDetalles
                .SelectMany(v => v.ArticuloEnVenta.Select(a => new
                {
                    DiaSemana = (int)v.Fecha.DayOfWeek,  // Día de la semana (0 = Domingo, 1 = Lunes, ...)
                    Hora = v.Fecha.Hour,                 // Hora de la venta
                    a.Cantidad                           // Cantidad vendida del artículo
                }))
                .GroupBy(v => new { v.DiaSemana, v.Hora })  // Agrupar por día de la semana y hora
                .Select(g => new
                {
                    DiaSemana = g.Key.DiaSemana,
                    Hora = g.Key.Hora,
                    CantidadVendida = g.Sum(v => v.Cantidad)  // Sumar la cantidad vendida
                })
                .OrderBy(v => v.DiaSemana)
                .ThenBy(v => v.Hora)
                .ToList();

            // Preparar el formato final para que la respuesta esté organizada por día de la semana
            var resultado = ventasPorHoraSemana
                .GroupBy(v => v.DiaSemana)
                .Select(g => new
                {
                    DiaSemana = g.Key,
                    VentasPorHora = g.Select(v => new { v.Hora, v.CantidadVendida }).ToList()
                })
                .ToList();

            return Ok(resultado);
        }

        [HttpGet("cantidad-por-dia/{year}/{month}")]
        public async Task<IActionResult> GetCantidadVendidaPorDia(int year, int month)
        {
            // Realizar un join entre ArticuloEnVenta y Venta
            var ventasPorDia = await _context.ArticulosEnVenta
                .Join(_context.Ventas,
                      articulo => articulo.IdVenta,  // FK en ArticuloEnVenta
                      venta => venta.IdVenta,             // PK en Venta
                      (articulo, venta) => new { Articulo = articulo, Venta = venta }) // Unión en Join
                .Where(v => v.Venta.Fecha.Year == year && v.Venta.Fecha.Month == month) // Filtrar por año y mes
                .GroupBy(v => v.Venta.Fecha.Day) // Agrupar por día
                .Select(g => new
                {
                    Dia = g.Key,
                    CantidadVendida = g.Sum(v => v.Articulo.Cantidad)  // Sumar las cantidades vendidas por día
                })
                .ToListAsync();

            return Ok(ventasPorDia);
        }

        // Obtener la lista de cantidades vendidas por categoría
        [HttpGet("cantidad-por-categoria")]
        public async Task<IActionResult> GetCantidadVendidaPorCategoria()
        {
            var ventasPorCategoria = await _context.ArticulosEnVenta
                .Include(a => a.IdArticuloNavigation)
                .ThenInclude(a => a.IdCategoriaNavigation)
                .GroupBy(a => a.IdArticuloNavigation.IdCategoriaNavigation.Nombre)
                .Select(g => new
                {
                    Categoria = g.Key,
                    CantidadVendida = g.Sum(a => a.Cantidad)
                })
                .ToListAsync();

            return Ok(ventasPorCategoria);
        }

        [HttpGet("facturacion-mensual/{year}")]
        public async Task<IActionResult> GetFacturacionMensual(int year)
        {
            // Obtener facturación por mes de manera más directa
            var facturacionMensual = await _context.Ventas
                .Where(v => v.Fecha.Year == year) // Filtra ventas por año
                .SelectMany(v => v.ArticuloEnVenta, (venta, articulo) => new
                {
                    venta.Fecha.Month,
                    Facturacion = articulo.Precio * articulo.Cantidad
                }) // Calcular facturación para cada artículo
                .GroupBy(v => v.Month) // Agrupar por mes
                .Select(g => new
                {
                    Mes = g.Key,
                    TotalFacturacion = g.Sum(v => v.Facturacion) // Sumar las facturaciones ya calculadas
                })
                .ToListAsync();

            return Ok(facturacionMensual);
        }

        [HttpGet("articulos-mas-vendidos/{year}/{mes}/{cantidad}")]
        public async Task<IActionResult> GetArticulosMasVendidos(int year, int mes, int cantidad)
        {
            var ventas = await _context.Ventas
                .Where(v => v.Fecha.Year == year && v.Fecha.Month == mes)
                .SelectMany(v => v.ArticuloEnVenta)
                .GroupBy(a => a.IdArticulo)
                .Select(g => new
                {
                    IdArticulo = g.Key,
                    CantidadVendida = g.Sum(a => a.Cantidad)
                })
                .ToListAsync(); // Traer los resultados a memoria

            // Luego realiza la unión en memoria
            var resultado = ventas
                .Join(_context.Articulos,
                      venta => venta.IdArticulo,
                      articulo => articulo.IdArticulo,
                      (venta, articulo) => new
                      {
                          IdArticulo = articulo.IdArticulo,
                          Nombre = articulo.Nombre,
                          CantidadVendida = venta.CantidadVendida
                      })
                .OrderByDescending(a => a.CantidadVendida) // Ordenar por cantidad vendida en orden descendente
                .Take(cantidad) // Tomar solo la cantidad indicada
                .ToList(); // Resultado final en memoria

            return Ok(resultado);
        }

        [HttpGet("cantidad-vendida-historica")]
        public async Task<IActionResult> GetCantidadVendidaHistorica()
        {
            var query = _context.ArticulosEnVenta
                .Join(_context.Ventas,
                      articulo => articulo.IdVenta,
                      venta => venta.IdVenta,
                      (articulo, venta) => new { Articulo = articulo, Venta = venta });

            // Agrupamos por año y mes para obtener la cantidad vendida por mes cada año
            var ventasPorMes = await query
                .GroupBy(v => new { v.Venta.Fecha.Year, v.Venta.Fecha.Month }) // Agrupo por año y mes
                .Select(g => new
                {
                    Año = g.Key.Year,  // Extraigo el año
                    Mes = g.Key.Month,  // Extraigo el mes
                    TotalCantidadVendida = g.Sum(v => v.Articulo.Cantidad)
                })
                .OrderBy(v => v.Año)  // Primero por año
                .ThenBy(v => v.Mes)   // Luego por mes
                .ToListAsync();

            return Ok(ventasPorMes);
        }

        // Lo mismo pero para la facturación
        [HttpGet("facturacion-mensual-historica")]
        public async Task<IActionResult> GetFacturacionMensualHistorica()
        {
            // Filtrar por el año si se proporciona, de lo contrario obtener todas las ventas
            var query = _context.Ventas
                .Join(_context.ArticulosEnVenta,
                      venta => venta.IdVenta,
                      articulo => articulo.IdVenta,
                      (venta, articulo) => new
                      {
                          venta.Fecha,
                          Facturacion = articulo.Precio * articulo.Cantidad
                      });

            // Agrupar por año y mes y sumar la facturación mensual
            var facturacionMensual = await query
                .GroupBy(v => new { v.Fecha.Year, v.Fecha.Month }) // Agrupar por año y mes
                .Select(g => new
                {
                    Año = g.Key.Year,  // Extraigo el año
                    Mes = g.Key.Month,  // Extraigo el mes
                    TotalFacturacion = g.Sum(v => v.Facturacion) // Sumar la facturación de cada mes
                })
                .OrderBy(v => v.Año)  // Ordenamos primero por año
                .ThenBy(v => v.Mes)   // Luego por mes
                .ToListAsync();

            return Ok(facturacionMensual);
        }

    }
}
