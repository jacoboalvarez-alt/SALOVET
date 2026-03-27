using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class CitasPage : ContentPage
{
	public CitasPage()
	{
		InitializeComponent();
		BindingContext = new CitasViewModel();
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Obtener el ViewModel del BindingContext (que se creó en XAML)
        var viewModel = BindingContext as CitasViewModel;

        if (viewModel != null)
        {
            await viewModel.InitializeAsync();
        }
    }
}