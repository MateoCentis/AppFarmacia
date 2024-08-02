using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AutoMapper;
using AppFarmaciaWebAPI.ModelsDTO;

namespace AppFarmaciaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly FarmaciaDbContext _context;
        private readonly IMapper _mapper;

        public CategoriasController(IMapper mapper, FarmaciaDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Categoria
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategorias()
        {
            var categorias = await _context.Categorias.ToListAsync();
            var categoriaDTOs = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
            return Ok(categoriaDTOs);
        }

        // GET: api/Categoria/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDTO>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(p => p.IdCategoria == id);

            if (categoria == null)
            {
                return NotFound();
            }

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
            return Ok(categoriaDTO);
        }

        // PUT: api/Categoria/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaDTO categoriaDTO)
        {
            if (id != categoriaDTO.IdCategoria)
            {
                return BadRequest("El ID de la categoría en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }

            var categoriaExistente = await _context.Categorias.FindAsync(id);
            if (categoriaExistente == null)
            {
                return NotFound($"No se encontró una categoría con el ID {id}.");
            }

            _mapper.Map(categoriaDTO, categoriaExistente);

            _context.Entry(categoriaExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
                {
                    return NotFound($"No se encontró una categoría con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categoria
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(CategoriaDTO categoriaDTO)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDTO);

            _context.Categorias.Add(categoria);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CategoriaExists(categoria.IdCategoria))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var createdCategoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return CreatedAtAction(nameof(GetCategoria), new { id = createdCategoriaDTO.IdCategoria }, createdCategoriaDTO);
        }

        // DELETE: api/Categoria/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.IdCategoria == id);
            if (categoria == null)
            {
                return NotFound($"No se encontró una categoría con el ID {id}.");
            }

            if (categoria.Articulos.Any())
            {
                return BadRequest("No se puede eliminar la categoría porque tiene artículos asignados.");
            }

            _context.Categorias.Remove(categoria);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Ocurrió un error al intentar eliminar la categoría: {ex.Message}");
            }

            return NoContent();
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.IdCategoria == id);
        }
    }
}
