using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalovetAPI.Data;
using SalovetAPI.Models;

namespace SalovetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrosController : ControllerBase
    {
        private readonly SalovetDbContext _context;

        public RegistrosController(SalovetDbContext context)
        {
            _context = context;
        }

        // GET: api/registros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Registro>>> GetRegistros()
        {
            return await _context.Registros
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }

        // GET: api/registros/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Registro>> GetRegistro(int id)
        {
            var registro = await _context.Registros.FindAsync(id);

            if (registro == null)
                return NotFound(new { mensaje = "Registro no encontrado" });

            return registro;
        }

        // GET: api/registros/tipo/Factura creada
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<Registro>>> GetRegistrosByTipo(string tipo)
        {
            return await _context.Registros
                .Where(r => r.TipoActividad == tipo)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }

        // GET: api/registros/fecha/2025-02-10
        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<IEnumerable<Registro>>> GetRegistrosByFecha(DateTime fecha)
        {
            return await _context.Registros
                .Where(r => r.Fecha.Date == fecha.Date)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }

        // GET: api/registros/recientes/10
        [HttpGet("recientes/{cantidad}")]
        public async Task<ActionResult<IEnumerable<Registro>>> GetRegistrosRecientes(int cantidad)
        {
            return await _context.Registros
                .OrderByDescending(r => r.Fecha)
                .Take(cantidad)
                .ToListAsync();
        }

        // POST: api/registros
        [HttpPost]
        public async Task<ActionResult<Registro>> PostRegistro(Registro registro)
        {
            _context.Registros.Add(registro);
            await _context.SaveChangesAsync();

            // Nota: El trigger podría crear otro registro automáticamente

            return CreatedAtAction(nameof(GetRegistro), new { id = registro.IdRegistro }, registro);
        }

        // DELETE: api/registros/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistro(int id)
        {
            var registro = await _context.Registros.FindAsync(id);
            if (registro == null)
                return NotFound(new { mensaje = "Registro no encontrado" });

            _context.Registros.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/registros/limpiar
        [HttpDelete("limpiar")]
        public async Task<IActionResult> LimpiarRegistrosAntiguos([FromQuery] int diasAntiguedad = 30)
        {
            var fechaLimite = DateTime.Now.AddDays(-diasAntiguedad);

            var registrosAntiguos = await _context.Registros
                .Where(r => r.Fecha < fechaLimite)
                .ToListAsync();

            _context.Registros.RemoveRange(registrosAntiguos);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Se eliminaron {registrosAntiguos.Count} registros" });
        }
    }
}
