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
        if (BindingContext is MascotasGeneralPageViewModel vm)
            await vm.CargarMascotas();
    }

}