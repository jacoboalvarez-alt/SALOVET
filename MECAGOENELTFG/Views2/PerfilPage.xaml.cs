using MECAGOENELTFG.ViewModels2;

namespace MECAGOENELTFG.Views2;

public partial class PerfilPage : ContentPage
{
    public PerfilPageViewModel vm = new PerfilPageViewModel();
	public PerfilPage()
	{
		InitializeComponent();
        BindingContext = vm;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine(">>> OnAppearing ejecutado");
        if (BindingContext is PerfilPageViewModel viewModel)
            await viewModel.CargarDatosAsync();
    }
    private async void OnChatTapped(object? sender, TappedEventArgs e)
    {
        // Animación de salida hacia la izquierda
        await Shell.Current.GoToAsync("ChatAsistente");
    }

    private void OnNavPerfilTapped(object? sender, TappedEventArgs e)
    {
        // Ya estamos en el perfil, no hace nada
    }
    private async void OnNavCitasTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("CitasPageClient");
    }
    private async void OnInicioTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ClientDashBoard");
    }
}