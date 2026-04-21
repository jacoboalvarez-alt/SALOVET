using MECAGOENELTFG.ViewModels2;

namespace MECAGOENELTFG.Views2;

public partial class FormCitasCliente : ContentPage
{
	public FormCitasCliente()
	{
		InitializeComponent();
		BindingContext = new FormCitasClienteViewModel();
	}

	protected override async void OnAppearing() 
	{ 
		base.OnAppearing();
		if(BindingContext is FormCitasClienteViewModel vm)
			await vm.CargarDatosAsync();
	}
}