using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public partial class FacturaItemViewModel : ObservableObject
    {
        // Referencia al modelo real para pasarlo al service al actualizar/eliminar
        public Factura Factura { get; }

        public FacturaItemViewModel(Factura factura)
        {
            Factura = factura;
            RefrescarEstado();
        }

        // --- Datos de la Cita ---
        public string NombreMascota => Factura.Cita?.Mascota?.NombreMasc ?? $"Cita #{Factura.IdCita}";
        public string NombreCliente => Factura.Cita?.Cliente != null
            ? $"{Factura.Cita.Cliente.NombreCli} {Factura.Cita.Cliente.ApeCli}".Trim()
            : "Cliente desconocido";
        public DateTime FechaHora => Factura.Cita?.FechaHora ?? Factura.FechaEmision;
        public string? Descripcion => Factura.Cita?.Descripcion;
        public bool TieneDescripcion => !string.IsNullOrWhiteSpace(Descripcion);

        // --- Datos de la Factura ---
        public decimal Monto => Factura.Monto;

        // --- Estado de pago reactivo ---
        [ObservableProperty]
        private string _estadoPagoTexto = string.Empty;

        [ObservableProperty]
        private string _colorEstadoPago = string.Empty;

        [ObservableProperty]
        private bool _puedeCerrarse;

        // Llamado desde el ViewModel tras actualizar para refrescar la UI
        public void RefrescarEstado()
        {
            EstadoPagoTexto = Factura.EstadoPago == EstadoPago.PAGADO ? "PAGADO" : "PENDIENTE";
            ColorEstadoPago = Factura.EstadoPago == EstadoPago.PAGADO ? "#0F6E56" : "#B45309";
            PuedeCerrarse = Factura.EstadoPago == EstadoPago.PENDIENTE;
        }
    }
}
