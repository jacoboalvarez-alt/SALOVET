using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class ClientesPage : ContentPage
{
    public ClientesPage()
    {
        InitializeComponent();
        // NO inicializar _viewModel aquí si está en el XAML
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Obtener el ViewModel del BindingContext (que se creó en XAML)
        var viewModel = BindingContext as ClientesViewModel;

        if (viewModel != null)
        {
            await viewModel.CargarClientesCommand.ExecuteAsync(null);
        }
    }
}