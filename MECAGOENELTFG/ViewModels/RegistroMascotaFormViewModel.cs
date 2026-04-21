using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MECAGOENELTFG.ViewModels
{
     public partial class RegistroMascotaFormViewModel :ObservableObject
    {
        private readonly RegistroMascotaService _service;

        [ObservableProperty]
        private int idMascota;

        [ObservableProperty]
        private int idRegistro;

        [ObservableProperty]
        private string descripcion;

        [ObservableProperty]
        private DateTime fechaInicio = DateTime.Now;

        [ObservableProperty]
        private DateTime fechaFinal = DateTime.Now;

        [ObservableProperty]
        private bool tieneFechaFinal;

        public ICommand VolverCommand { get; }

        public bool EsEdicion => IdRegistro > 0;

        public string Titulo => EsEdicion ? "Editar Regisro" : "Crear Registro";

        public RegistroMascotaFormViewModel(int idMascota, RegistroMascota registro = null)
        {
            _service = new RegistroMascotaService();
            IdMascota = idMascota;
            VolverCommand = new Command(Volver);

            if (registro != null)
            {
                IdRegistro = registro.IdRegistro;
                Descripcion = registro.Descripcion;
                FechaInicio = registro.FechaInicio;
                if (registro.FechaFinal.HasValue)
                {
                    TieneFechaFinal = true;
                    FechaFinal = registro.FechaFinal.Value;
                }
            }
        }

        [RelayCommand]
        public async Task Guardar()
        {
            bool ok;

            if (EsEdicion)
            {
                var actualizado = new RegistroMascota
                {
                    IdRegistro = IdRegistro,  
                    IdMascota = IdMascota,
                    Descripcion = Descripcion,
                    FechaInicio = FechaInicio,
                    FechaFinal = TieneFechaFinal ? FechaFinal : null
                };
                ok = await _service.Actualizar(IdRegistro, actualizado);
            }
            else
            {
                var nuevo = new RegistroMascota
                {
                    IdMascota = IdMascota,
                    Descripcion = Descripcion,
                    FechaInicio = FechaInicio,
                    FechaFinal = TieneFechaFinal ? FechaFinal : null
                };
                ok = await _service.Crear(nuevo);
            }

            if (ok)
            {
                await Shell.Current.DisplayAlert("Exito", "Registro ha sido guardado correctamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se ha podido agregar el nuevo registro", "OK");
            }
        }

        public async void Volver() 
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
