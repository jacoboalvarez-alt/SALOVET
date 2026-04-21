using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalovetAPI.Data;
using SalovetAPI.Models;
using System;

namespace SalovetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroMascotaController : ControllerBase
    {
        private readonly SalovetDbContext _context;

        public RegistroMascotaController(SalovetDbContext context)
        {
            _context = context;
        }

        // GET: api/RegistroMascota
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var registros = await _context.RegistroMascotas
                .Include(r => r.Mascota)
                .ToListAsync();

            return Ok(registros);
        }

        // GET: api/RegistroMascota/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var registro = await _context.RegistroMascotas
                .Include(r => r.Mascota)
                .FirstOrDefaultAsync(r => r.IdRegistro == id);

            if (registro == null)
                return NotFound();

            return Ok(registro);
        }

        // POST: api/RegistroMascota
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegistroMascota registro)
        {
            registro.Mascota = null;
            _context.RegistroMascotas.Add(registro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = registro.IdRegistro }, registro);
        }

        // PUT: api/RegistroMascota/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RegistroMascota registro)
        {
            if (id != registro.IdRegistro)
                return BadRequest();

            _context.Entry(registro).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/RegistroMascota/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var registro = await _context.RegistroMascotas.FindAsync(id);

            if (registro == null)
                return NotFound();

            _context.RegistroMascotas.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/RegistroMascota/mascota/5
        [HttpGet("mascota/{idMascota}")]
        public async Task<IActionResult> GetByMascota(int idMascota)
        {
            var registros = await _context.RegistroMascotas
                .Where(r => r.IdMascota == idMascota)
                .Include(r => r.Mascota)
                .ToListAsync();

            return Ok(registros);
        }

    }
}
