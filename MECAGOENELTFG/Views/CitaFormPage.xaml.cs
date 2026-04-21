using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class CitaFormPage : ContentPage
{
	public CitaFormPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing() 
	{
		base.OnAppearing();
		var vm = new CitasFormPageViewModel();
		BindingContext = vm;
		await vm.CargarDatosAsync();
	}
}