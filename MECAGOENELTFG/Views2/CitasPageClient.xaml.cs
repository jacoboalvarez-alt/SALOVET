using MECAGOENELTFG.ViewModels2;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Views2;

public partial class CitasPageClient : ContentPage
{
	public CitasPageClient()
	{
		InitializeComponent();
		BindingContext = new ClienteCitasViewModel();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ClienteCitasViewModel vm) 
            await vm.CargarDatosAsync();
        
    }

    //---------------------------------------------------------------
    //FUNCIONES PARA EL MOVIMIENTOS ENTRE LAS VISTAS DE LA APLICACION
    //---------------------------------------------------------------

    private async void OnSalirClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Login");
    }

    private async void OnInicioTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ClientDashBoard");
    }

    private void OnNavCitasTapped(object? sender, TappedEventArgs e)
    {
        // Ya estamos en inicio, no hace nada
    }

    private async void OnNavPerfilTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("PerfilPage");
    }


    private async void OnChatTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ChatAsistente");
    }
}