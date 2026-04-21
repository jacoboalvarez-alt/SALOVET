using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class MascotaFormPage : ContentPage
{
	public MascotaFormPage()
	{
		InitializeComponent();
		BindingContext = new MascotaFormViewModel();
	}
}