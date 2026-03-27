using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class MedicamentosPage : ContentPage
{
	public MedicamentosPage()
	{
		InitializeComponent();
		BindingContext = new MedicamentosViewModel();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Obtener el ViewModel del BindingContext (que se creó en XAML)
        var viewModel = BindingContext as MedicamentosViewModel;

        if (viewModel != null)
        {
            await viewModel.CargarMedicamentos();
        }
    }
}