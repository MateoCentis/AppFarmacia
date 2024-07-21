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
            var categoria = await _context.Categorias.ToListAsync();
            var categoriaDTO = _mapper.Map<IEnumerable<CategoriaDTO>>(categoria);
            return Ok(categoriaDTO);
        }

        // GET: api/Categoria/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDTO>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(p => p.IdCategoria == id);

            if (categoria == null)
            {
                return NotFound();
            }
            var CategoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
            return Ok(CategoriaDTO);
        }

        // PUT: api/Categoria/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaDTO categoriaDTO)
        {
            // Verificar si el ID del categoria en el DTO coincide con el ID de la ruta
            if (id != categoriaDTO.IdCategoria)
            {
                return BadRequest("El ID del categoria en el cuerpo de la solicitud no coincide con el ID de la ruta.");
            }
            // Verificar si el categoria con el ID especificado existe en la base de datos
            var categoriaExistente = await _context.Categorias.FindAsync(id);
            if (categoriaExistente == null)
            {
                return NotFound($"No se encontró un categoria con el ID {id}.");
            }
            
            _mapper.Map(categoriaDTO, categoriaExistente);


            // Marcar la entidad como modificada
            _context.Entry(categoriaExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
                {
                    return NotFound($"No se encontró un privilegio con el ID {id}.");
                }
                else
                {
                    throw;
                }
            }
            // Devolver una respuesta 204 No Content para indicar que la actualización fue exitosa
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
                return NotFound($"No se encontró un categoria con el ID {id}.");
            }

            // Verificar si hay usuarios asociados al categoria
            if (categoria.Articulos.Any())
            {
                return BadRequest("No se puede eliminar el categoria porque tiene Articulos asignados.");
            }

            _context.Categorias.Remove(categoria);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejar excepciones de actualización de base de datos
                return StatusCode(500, $"Ocurrió un error al intentar eliminar el categoria: {ex.Message}");
            }

            return NoContent();
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.IdCategoria == id);
        }
    }
}
