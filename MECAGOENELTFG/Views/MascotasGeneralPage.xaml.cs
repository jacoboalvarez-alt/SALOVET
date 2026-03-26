using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class MascotasGeneralPage : ContentPage
{
	public MascotasGeneralPage()
	{
        InitializeComponent();
		BindingContext = new MascotasGeneralPageViewModel();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Obtener el ViewModel del BindingContext (que se creó en XAML)
        var viewModel = BindingContext as MascotasGeneralPageViewModel;

        if (viewModel != null)
        {
            await viewModel.CargarMascotasCommand.ExecuteAsync(null);
        }
    }

}