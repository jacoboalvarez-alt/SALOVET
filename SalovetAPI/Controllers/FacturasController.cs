using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalovetAPI.Data;
using SalovetAPI.Models;

namespace SalovetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
        private readonly SalovetDbContext _context;

        public FacturasController(SalovetDbContext context)
        {
            _context = context;
        }

        // GET: api/facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            return await _context.Facturas
                .Include(f => f.Cita)
                    .ThenInclude(c => c.Cliente)
                .Include(f => f.Cita)
                    .ThenInclude(c => c.Mascota)
                .ToListAsync();
        }

        // GET: api/facturas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> GetFactura(int id)
        {
            var factura = await _context.Facturas
                .Include(f => f.Cita)
                    .ThenInclude(c => c.Cliente)
                .Include(f => f.Cita)
                    .ThenInclude(c => c.Mascota)
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (factura == null)
                return NotFound(new { mensaje = "Factura no encontrada" });

            return factura;
        }

        // GET: api/facturas/cita/5
        [HttpGet("cita/{idCita}")]
        public async Task<ActionResult<Factura>> GetFacturaByCita(int idCita)
        {
            var factura = await _context.Facturas
                .Include(f => f.Cita)
                    .ThenInclude(c => c.Cliente)
                .FirstOrDefaultAsync(f => f.IdCita == idCita);

            if (factura == null)
                return NotFound(new { mensaje = "No se encontró factura para esta cita" });

            return factura;
        }

        // GET: api/facturas/pendientes
        [HttpGet("pendientes")]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturasPendientes()
        {
            return await _context.Facturas
                .Include(f => f.Cita)
                    .ThenInclude(c => c.Cliente)
                .Where(f => f.EstadoPago == EstadoPago.PENDIENTE)
                .ToListAsync();
        }

        // GET: api/facturas/pagadas
        [HttpGet("pagadas")]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturasPagadas()
        {
            return await _context.Facturas
                .Include(f => f.Cita)
                    .ThenInclude(c => c.Cliente)
                .Where(f => f.EstadoPago == EstadoPago.PAGADO)
                .ToListAsync();
        }

        // POST: api/facturas
        [HttpPost]
        public async Task<ActionResult<Factura>> PostFactura(Factura factura)
        {
            // Verificar que la cita existe
            if (!await _context.Citas.AnyAsync(c => c.IdCita == factura.IdCita))
                return BadRequest(new { mensaje = "La cita especificada no existe" });

            // Verificar que no exista ya una factura para esta cita
            if (await _context.Facturas.AnyAsync(f => f.IdCita == factura.IdCita))
                return Conflict(new { mensaje = "Ya existe una factura para esta cita" });

            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            // Nota: El trigger de MySQL creará automáticamente un registro

            return CreatedAtAction(nameof(GetFactura), new { id = factura.IdFactura }, factura);
        }

        // PUT: api/facturas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFactura(int id, Factura factura)
        {
            if (id != factura.IdFactura)
                return BadRequest(new { mensaje = "ID no coincide" });

            _context.Entry(factura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(id))
                    return NotFound(new { mensaje = "Factura no encontrada" });
                throw;
            }

            return NoContent();
        }

        // PATCH: api/facturas/5/pagar
        [HttpPatch("{id}/pagar")]
        public async Task<IActionResult> MarcarComoPagada(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
                return NotFound(new { mensaje = "Factura no encontrada" });

            factura.EstadoPago = EstadoPago.PAGADO;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/facturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
                return NotFound(new { mensaje = "Factura no encontrada" });

            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.IdFactura == id);
        }
    }
}
