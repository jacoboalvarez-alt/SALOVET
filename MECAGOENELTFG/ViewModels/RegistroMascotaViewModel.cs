using CommunityToolkit.Mvvm.ComponentModel;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MECAGOENELTFG.ViewModels
{
    public partial class RegistroMascotaViewModel : ObservableObject
    {
        private readonly RegistroMascotaService _service;

        public int IdMascota {  get; set; }
        public string NombreMascota { get; set; }

        public ObservableCollection<RegistroMascota> Registros { get; set; }

        public ICommand AgregarRegistroCommand { get; }
        public ICommand EditarRegistroCommand { get; }
        public ICommand EliminarRegistroCommand { get; }
        public ICommand VolverCommand { get; }
        public RegistroMascotaViewModel(int idMascota, string nombreMascota)
        {
            IdMascota = idMascota;
            NombreMascota = $"Registros de {nombreMascota}";

            _service = new RegistroMascotaService();
            Registros = new ObservableCollection<RegistroMascota>();

            AgregarRegistroCommand = new Command(AgregarRegistro);
            EditarRegistroCommand = new Command<RegistroMascota>(EditarRegistro);
            EliminarRegistroCommand = new Command<RegistroMascota>(EliminarRegistro);
            VolverCommand = new Command(Volver);

            CargarRegistros();
        }

        private async void CargarRegistros()
        {
            var lista = await _service.ObtenerPorMascota(IdMascota);
            Registros.Clear();

            foreach (var r in lista)
                Registros.Add(r);
        }
        private async void Volver()
        {
            await Shell.Current.GoToAsync("..");
        }
        private async void AgregarRegistro()
        {
            await Shell.Current.GoToAsync($"RegistroMascotaForm?idMascota={IdMascota}");
        }

        private async void EditarRegistro(RegistroMascota registro)
        {
            SessionService.RegistroEdicion = registro;
            await Shell.Current.GoToAsync($"RegistroMascotaForm?idMascota={IdMascota}");
        }

        private async void EliminarRegistro(RegistroMascota registro)
        {
            bool ok = await _service.Eliminar(registro.IdRegistro);

            if (ok)
                Registros.Remove(registro);
        }

        
    }
}
