using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.ViewModels
{
    public partial class FacturasViewModel : ObservableObject
    {
        private readonly FacturasService _facturasService;

        [ObservableProperty]
        private ObservableCollection<FacturaItemViewModel> _facturas = [];

        [ObservableProperty]
        private bool _isRefreshing = false;

        public FacturasViewModel(FacturasService fac) 
        {
            _facturasService = fac;
            CargarFacturasCommand.Execute(null);
           
        }
        [RelayCommand]
        private async Task CargarFacturas()
        {
            try
            {
                IsRefreshing = true;

                var datos = await _facturasService.ObtenerTodasFacturas();
                var items = datos.Select(f => new FacturaItemViewModel(f));

                Facturas = new ObservableCollection<FacturaItemViewModel>(items);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task Refrescar()
        {
            await CargarFacturas();
        }

        [RelayCommand]
        private async Task AgregarFactura()
        {
            // Navega al formulario — ajusta la ruta según tu Shell
            await Shell.Current.GoToAsync("FacturasFormPage");
        }

        // CERRAR → marca la factura como PAGADO
        [RelayCommand]
        private async Task CerrarFactura(FacturaItemViewModel item)
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cerrar factura",
                $"¿Confirmas que la factura de {item.NombreMascota} ha sido pagada?",
                "Sí, marcar como pagada",
                "Cancelar");

            if (!confirmar) return;

            // ← Ahora usa PATCH /pagar en lugar de PUT
            bool ok = await _facturasService.MarcarComoPagada(item.Factura.IdFactura);

            if (ok)
            {
                item.Factura.EstadoPago = EstadoPago.PAGADO;
                item.RefrescarEstado();
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "No se pudo actualizar la factura. Inténtalo de nuevo.",
                    "Aceptar");
            }
        }

        // CANCELAR → elimina la factura
        [RelayCommand]
        private async Task CancelarFactura(FacturaItemViewModel item)
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cancelar factura",
                $"¿Seguro que quieres eliminar la factura de {item.NombreMascota}? Esta acción no se puede deshacer.",
                "Sí, eliminar",
                "Volver");

            if (!confirmar) return;

            bool ok = await _facturasService.Eliminar(item.Factura.IdFactura);

            if (ok)
                Facturas.Remove(item);
            else
                await Shell.Current.DisplayAlert("Error", "No se pudo eliminar la factura. Inténtalo de nuevo.", "Aceptar");
        }

        [RelayCommand]
        private async Task Volver() 
        {
            await Shell.Current.GoToAsync("..");
        }
    }

}
