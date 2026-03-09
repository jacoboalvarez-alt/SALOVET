using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalovetAPI.Data;
using SalovetAPI.Models;

namespace SalovetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly SalovetDbContext _context;

        public CitasController(SalovetDbContext context)
        {
            _context = context;
        }

        // GET: api/citas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitas()
        {
            return await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Profesional)
                .Include(c => c.Mascota)
                .ToListAsync();
        }

        // GET: api/citas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cita>> GetCita(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Profesional)
                .Include(c => c.Mascota)
                .FirstOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
                return NotFound(new { mensaje = "Cita no encontrada" });

            return cita;
        }

        // GET: api/citas/cliente/5
        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitasByCliente(int idCliente)
        {
            return await _context.Citas
                .Include(c => c.Profesional)
                .Include(c => c.Mascota)
                .Where(c => c.IdCliente == idCliente)
                .ToListAsync();
        }

        // GET: api/citas/profesional/5
        [HttpGet("profesional/{idProf}")]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitasByProfesional(int idProf)
        {
            return await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Mascota)
                .Where(c => c.IdProf == idProf)
                .ToListAsync();
        }

        // GET: api/citas/estado/PENDIENTE
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitasByEstado(string estado)
        {
            if (!Enum.TryParse<EstadoCita>(estado.ToUpper(), out var estadoEnum))
                return BadRequest(new { mensaje = "Estado inválido. Valores: PENDIENTE, CONFIRMADA, CANCELADA, COMPLETADA" });

            return await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Profesional)
                .Include(c => c.Mascota)
                .Where(c => c.Estado == estadoEnum)
                .ToListAsync();
        }

        // GET: api/citas/fecha/2025-02-10
        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitasByFecha(DateTime fecha)
        {
            return await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Profesional)
                .Include(c => c.Mascota)
                .Where(c => c.FechaHora.Date == fecha.Date)
                .ToListAsync();
        }

        // POST: api/citas
        [HttpPost]
        public async Task<ActionResult<Cita>> PostCita(Cita cita)
        {
            // Validaciones
            if (!await _context.Clientes.AnyAsync(c => c.IdCliente == cita.IdCliente))
                return BadRequest(new { mensaje = "El cliente no existe" });

            if (!await _context.Profesionales.AnyAsync(p => p.IdProf == cita.IdProf))
                return BadRequest(new { mensaje = "El profesional no existe" });

            if (!await _context.Mascotas.AnyAsync(m => m.IdMascota == cita.IdMascota))
                return BadRequest(new { mensaje = "La mascota no existe" });

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCita), new { id = cita.IdCita }, cita);
        }

        // PUT: api/citas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCita(int id, Cita cita)
        {
            if (id != cita.IdCita)
                return BadRequest(new { mensaje = "ID no coincide" });

            _context.Entry(cita).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitaExists(id))
                    return NotFound(new { mensaje = "Cita no encontrada" });
                throw;
            }

            return NoContent();
        }

        // PATCH: api/citas/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            if (!Enum.TryParse<EstadoCita>(nuevoEstado.ToUpper(), out var estadoEnum))
                return BadRequest(new { mensaje = "Estado inválido" });

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
                return NotFound(new { mensaje = "Cita no encontrada" });

            cita.Estado = estadoEnum;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/citas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
                return NotFound(new { mensaje = "Cita no encontrada" });

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.IdCita == id);
        }
    }
}
