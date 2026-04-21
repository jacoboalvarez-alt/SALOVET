using MECAGOENELTFG.Services;
using MECAGOENELTFG.ViewModels;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Views;

public partial class EditarCitaPage : ContentPage
{
	public EditarCitaPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var cita = SessionService.CitaEdicion;
        SessionService.CitaEdicion = null;

        if (cita == null) 
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        var vm = new EditarCitaViewModel(cita);
        BindingContext = vm;
        await vm.CargarDatosAsync();
    }
}