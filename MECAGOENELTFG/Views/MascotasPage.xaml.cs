using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class MascotasPage : ContentPage
{
    private readonly MascotasViewModel _viewModel;

    public MascotasPage()
    {
        InitializeComponent();
        _viewModel = new MascotasViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarMascotasDelClienteCommand.ExecuteAsync(null);
    }
}