using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AppFarmaciaWebAPI.ModelsDTO;
using AutoMapper;
using System.Data;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionController : Controller
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public NotificacionController(FarmaciaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Notificacion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificacionDTO>>> GetNotificaciones([FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null, [FromQuery] int? cantidad = null)
        {
            try
            {
                fechaInicio ??= new DateTime(2017, 6, 1); // 01/06/2017
                fechaFin ??= DateTime.Now; // Fecha actual

                var query = _context.Notificaciones
                    .Where(n => n.Fecha >= fechaInicio.Value && n.Fecha <= fechaFin.Value)
                    .OrderByDescending(n => n.Fecha) as IQueryable<AppFarmaciaWebAPI.Models.Notificacion>; // Ordenar por fecha descendente

                // Si se especifica cantidad, limitar los resultados
                if (cantidad.HasValue && cantidad.Value > 0)
                {
                    query = query.Take(cantidad.Value);
                }

                var notificaciones = await query.ToListAsync();
                var notificacionesDTO = _mapper.Map<IEnumerable<NotificacionDTO>>(notificaciones);
                return Ok(notificacionesDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get por busqueda de titulo
        // GET: api/Notificacion/BuscarTitulo
        [HttpGet("BuscarTitulo")]
        public async Task<ActionResult<IEnumerable<NotificacionDTO>>> GetNotificacionesPorTitulo([FromQuery] string titulo)
        {
            try
            {
                var notificaciones = await _context.Notificaciones.Where(n => n.Titulo.Contains(titulo)).ToListAsync();
                var notificacionesDTO = _mapper.Map<IEnumerable<NotificacionDTO>>(notificaciones);
                return Ok(notificacionesDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Cambiar el campo leido de notificaicones
        // PUT: api/Notificacion/5

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotificacionLeida(int id)
        {
            try
            {
                // Buscar la notifiacion a modificar
                var notificacion = await _context.Notificaciones.FindAsync(id);
                if (notificacion == null)
                {
                    return NotFound();
                }

                notificacion.Leido = true;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificacionExists(id))
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

        private bool NotificacionExists(int id)
        {
            throw new NotImplementedException();
        }
    }
}
