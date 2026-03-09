using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalovetAPI.Data;
using SalovetAPI.Models;

namespace SalovetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentosController : ControllerBase
    {

        private readonly SalovetDbContext _context;

        public MedicamentosController(SalovetDbContext context)
        {
            _context = context;
        }

        // GET: api/medicamentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicamento>>> GetMedicamentos()
        {
            return await _context.Medicamentos.ToListAsync();
        }

        // GET: api/medicamentos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Medicamento>> GetMedicamento(int id)
        {
            var medicamento = await _context.Medicamentos.FindAsync(id);

            if (medicamento == null)
                return NotFound(new { mensaje = "Medicamento no encontrado" });

            return medicamento;
        }

        // GET: api/medicamentos/disponibles
        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<Medicamento>>> GetMedicamentosDisponibles()
        {
            return await _context.Medicamentos
                .Where(m => m.Estado == true && m.Stock > 0)
                .ToListAsync();
        }

        // GET: api/medicamentos/bajo-stock/10
        [HttpGet("bajo-stock/{limite}")]
        public async Task<ActionResult<IEnumerable<Medicamento>>> GetMedicamentosBajoStock(int limite)
        {
            return await _context.Medicamentos
                .Where(m => m.Stock <= limite)
                .ToListAsync();
        }

        // POST: api/medicamentos
        [HttpPost]
        public async Task<ActionResult<Medicamento>> PostMedicamento(Medicamento medicamento)
        {
            _context.Medicamentos.Add(medicamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicamento), new { id = medicamento.IdMedica }, medicamento);
        }

        // PUT: api/medicamentos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicamento(int id, Medicamento medicamento)
        {
            if (id != medicamento.IdMedica)
                return BadRequest(new { mensaje = "ID no coincide" });

            _context.Entry(medicamento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicamentoExists(id))
                    return NotFound(new { mensaje = "Medicamento no encontrado" });
                throw;
            }

            return NoContent();
        }

        // PATCH: api/medicamentos/5/stock
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> ActualizarStock(int id, [FromBody] int nuevoStock)
        {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null)
                return NotFound(new { mensaje = "Medicamento no encontrado" });

            medicamento.Stock = nuevoStock;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/medicamentos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicamento(int id)
        {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null)
                return NotFound(new { mensaje = "Medicamento no encontrado" });

            _context.Medicamentos.Remove(medicamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicamentoExists(int id)
        {
            return _context.Medicamentos.Any(e => e.IdMedica == id);
        }
    }
}
