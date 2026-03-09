using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalovetAPI.Data;
using SalovetAPI.Models;

namespace SalovetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotasController : ControllerBase
    {
        private readonly SalovetDbContext _context;

        public MascotasController(SalovetDbContext context)
        {
            _context = context;
        }

        // GET: api/mascotas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mascota>>> GetMascotas()
        {
            return await _context.Mascotas
                .Include(m => m.Cliente)
                .ToListAsync();
        }

        // GET: api/mascotas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mascota>> GetMascota(int id)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.IdMascota == id);

            if (mascota == null)
                return NotFound(new { mensaje = "Mascota no encontrada" });

            return mascota;
        }

        // GET: api/mascotas/cliente/5
        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<Mascota>>> GetMascotasByCliente(int idCliente)
        {
            var mascotas = await _context.Mascotas
                .Include(m => m.Cliente)
                .Where(m => m.IdCliente == idCliente)
                .ToListAsync();

            return mascotas;
        }

        // GET: api/mascotas/especie/Perro
        [HttpGet("especie/{especie}")]
        public async Task<ActionResult<IEnumerable<Mascota>>> GetMascotasByEspecie(string especie)
        {
            var mascotas = await _context.Mascotas
                .Include(m => m.Cliente)
                .Where(m => m.Especie.ToLower() == especie.ToLower())
                .ToListAsync();

            return mascotas;
        }

        // POST: api/mascotas
        [HttpPost]
        public async Task<ActionResult<Mascota>> PostMascota(Mascota mascota)
        {
            // Verificar que el cliente existe
            if (!await _context.Clientes.AnyAsync(c => c.IdCliente == mascota.IdCliente))
                return BadRequest(new { mensaje = "El cliente especificado no existe" });

            _context.Mascotas.Add(mascota);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMascota), new { id = mascota.IdMascota }, mascota);
        }

        // PUT: api/mascotas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMascota(int id, Mascota mascota)
        {
            if (id != mascota.IdMascota)
                return BadRequest(new { mensaje = "ID no coincide" });

            _context.Entry(mascota).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MascotaExists(id))
                    return NotFound(new { mensaje = "Mascota no encontrada" });
                throw;
            }

            return NoContent();
        }

        // DELETE: api/mascotas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMascota(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null)
                return NotFound(new { mensaje = "Mascota no encontrada" });

            _context.Mascotas.Remove(mascota);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MascotaExists(int id)
        {
            return _context.Mascotas.Any(e => e.IdMascota == id);
        }
    }
}
