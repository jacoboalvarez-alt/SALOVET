using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalovetAPI.Data;
using SalovetAPI.Models;

namespace SalovetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfesionalesController : ControllerBase
    {
        private readonly SalovetDbContext _context;

        public ProfesionalesController(SalovetDbContext context)
        {
            _context = context;
        }
        // GET: api/profesionales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profesional>>> GetProfesionales()
        {
            return await _context.Profesionales.ToListAsync();
        }

        // GET: api/profesionales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Profesional>> GetProfesional(int id)
        {
            var profesional = await _context.Profesionales.FindAsync(id);

            if (profesional == null)
                return NotFound(new { mensaje = "Profesional no encontrado" });

            return profesional;
        }

        // GET: api/profesionales/grado/VETERINARIO
        [HttpGet("grado/{grado}")]
        public async Task<ActionResult<IEnumerable<Profesional>>> GetProfesionalesByGrado(string grado)
        {
            if (!Enum.TryParse<GradoProfesional>(grado.ToUpper(), out var gradoEnum))
                return BadRequest(new { mensaje = "Grado inválido. Valores permitidos: VETERINARIO, AYUDANTE, PROFESIONAL" });

            var profesionales = await _context.Profesionales
                .Where(p => p.Grado == gradoEnum)
                .ToListAsync();

            return profesionales;
        }

        // POST: api/profesionales
        [HttpPost]
        public async Task<ActionResult<Profesional>> PostProfesional(Profesional profesional)
        {
            _context.Profesionales.Add(profesional);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProfesional), new { id = profesional.IdProf }, profesional);
        }

        // PUT: api/profesionales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfesional(int id, Profesional profesional)
        {
            if (id != profesional.IdProf)
                return BadRequest(new { mensaje = "ID no coincide" });

            _context.Entry(profesional).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfesionalExists(id))
                    return NotFound(new { mensaje = "Profesional no encontrado" });
                throw;
            }

            return NoContent();
        }

        // DELETE: api/profesionales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfesional(int id)
        {
            var profesional = await _context.Profesionales.FindAsync(id);
            if (profesional == null)
                return NotFound(new { mensaje = "Profesional no encontrado" });

            _context.Profesionales.Remove(profesional);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProfesionalExists(int id)
        {
            return _context.Profesionales.Any(e => e.IdProf == id);
        }
    }
}
