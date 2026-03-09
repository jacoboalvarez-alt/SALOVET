using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace MECAGOENELTFG.ViewModels
{
    internal partial class ProfDashBoardModelView : ObservableObject
    {
        [RelayCommand]
        public async Task IrAClientes()
        {
            await Shell.Current.GoToAsync("ClientesPage");
        }

        [RelayCommand]
        public async Task IrAMascotas()
        {
            await Shell.Current.GoToAsync("MascotasPageGeneral");
        }

        [RelayCommand]
        public async Task IrACitas()
        {
            await Shell.Current.GoToAsync("CitasPage");
        }

        [RelayCommand]
        public async Task IrAMedicamentos()
        {
            await Shell.Current.GoToAsync("MedicamentosPage");
        }
    }
}
