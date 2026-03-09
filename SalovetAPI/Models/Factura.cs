namespace SalovetAPI.Models
{
    public enum EstadoPago
    {
        PENDIENTE,
        PAGADO
    }

    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdCita { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaEmision { get; set; } = DateTime.Now;
        public EstadoPago EstadoPago { get; set; } = EstadoPago.PENDIENTE;

        // Navegación
        public Cita Cita { get; set; } = null!;
    }
}
