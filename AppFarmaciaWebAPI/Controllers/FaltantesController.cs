﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AutoMapper;
using AppFarmaciaWebAPI.ModelsDTO;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaltantesController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public FaltantesController(IMapper mapper, FarmaciaDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Faltante
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FaltanteDTO>>> GetFaltantes()
        {
            var faltantes = await _context.Faltantes.ToListAsync();
            var faltantesDTO = _mapper.Map<IEnumerable<FaltanteDTO>>(faltantes);
            return Ok(faltantesDTO);
        }

        // GET: api/Faltante/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FaltanteDTO>> GetFaltante(int id)
        {
            var faltante = await _context.Faltantes.FindAsync(id);

            if (faltante == null)
            {
                return NotFound();
            }

            var faltanteDTO = _mapper.Map<FaltanteDTO>(faltante);
            return Ok(faltanteDTO);
        }

        // GET: api/ArticulosEnCompra/PorCompraId/5 (Hdp mira como le pusiste)
        [HttpGet("Articulo/{id}")]
        public async Task<ActionResult<int>> GetFaltantePorArticulo(int id)
        {
            try
            {
                // 1. Obtener el último registro de STOCK para el artículo con el idArticulo
                var ultimoStock = await _context.Stocks
                    .Where(s => s.IdArticulo == id)
                    .OrderByDescending(s => s.Fecha)  // Ordenar por fecha descendente
                    .FirstOrDefaultAsync();

                if (ultimoStock == null)
                {
                    return NotFound("No se encontró el último stock para este artículo.");
                }

                // 2. Buscar los faltantes entre la fecha de stock más reciente y la fecha actual
                var faltantes = await _context.Faltantes
                    .Where(f => f.IdArticulo == id && f.Fecha >= ultimoStock.Fecha)
                    .ToListAsync();

                if (faltantes == null || faltantes.Count == 0)
                {
                    return 0;
                }

                // 3. Agrupar los faltantes por IdArticulo y sumar las cantidades faltantes
                var cantidadFaltanteTotal = faltantes
                    .GroupBy(f => f.IdArticulo)
                    .Select(g => new
                    {
                        IdArticulo = g.Key,
                        TotalFaltante = g.Sum(f => f.CantidadFaltante)
                    })
                    .FirstOrDefault();

                if (cantidadFaltanteTotal == null)
                {
                    return 0;
                }

                return cantidadFaltanteTotal.TotalFaltante;
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error al eliminar el faltante: {ex.Message}");
            }
            
        }


        // PUT: api/Faltante/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditFaltante(int id, [FromBody] FaltanteDTO faltanteDTO)
        {
            if (id != faltanteDTO.IdFaltante)
            {
                return BadRequest("El ID del faltante no se encuentra en la base de datos.");
            }

            var faltanteExistente = await _context.Faltantes.FindAsync(id);
            if (faltanteExistente == null)
            {
                return NotFound($"No se encontró un faltante con el ID {id}.");
            }

            // Actualizar solo los campos relevantes
            faltanteExistente.Fecha = faltanteDTO.Fecha;
            faltanteExistente.CantidadFaltante = faltanteDTO.CantidadFaltante;

            _context.Entry(faltanteExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FaltanteExists(id))
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

        // POST: api/Faltante
        [HttpPost]
        public async Task<ActionResult<FaltanteDTO>> PostFaltante([FromBody] FaltanteDTO faltanteDTO)
        {
            var faltante = _mapper.Map<Faltante>(faltanteDTO);

            _context.Faltantes.Add(faltante);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FaltanteExists(faltante.IdFaltante))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdFaltanteDTO = _mapper.Map<FaltanteDTO>(faltante);

            return CreatedAtAction(nameof(GetFaltante), new { id = createdFaltanteDTO.IdFaltante }, createdFaltanteDTO);
        }

        // DELETE: api/Faltante/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaltante(int id)
        {
            var faltante = await _context.Faltantes.FindAsync(id);
            if (faltante == null)
            {
                return NotFound();
            }

            _context.Faltantes.Remove(faltante);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error al eliminar el faltante: {ex.Message}");
            }

            return NoContent();
        }

        private bool FaltanteExists(int id)
        {
            return _context.Faltantes.Any(e => e.IdFaltante == id);
        }
    }
}
