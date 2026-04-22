using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalovetAPI.Data;
using SalovetAPI.Models;

namespace SalovetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly SalovetDbContext _context;

        public ServiciosController(SalovetDbContext context)
        {
            _context = context;
        }

        // GET: api/Servicios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Servicio>>> GetServicios()
        {
            return await _context.Servicios.ToListAsync();
        }

        // GET: api/Servicios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Servicio>> GetServicioPorId(int id)
        {
            var servicio = await _context.Servicios
                .FirstOrDefaultAsync(s => s.IdServicio == id);

            if (servicio == null)
                return NotFound(new { mensaje = "No se ha encontrado el servicio" });

            return servicio;
        }

        // POST: api/Servicios
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Servicio servicio)
        {
            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetServicioPorId),
                new { id = servicio.IdServicio }, servicio);
        }

        // PUT: api/Servicios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Servicio servicio)
        {
            if (id != servicio.IdServicio)
                return BadRequest(new { mensaje = "El ID no coincide" });

            var existe = await _context.Servicios.AnyAsync(s => s.IdServicio == id);
            if (!existe)
                return NotFound(new { mensaje = "No se ha encontrado el servicio" });

            _context.Entry(servicio).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Servicios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
                return NotFound(new { mensaje = "No se ha encontrado el servicio" });

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
