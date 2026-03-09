using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class MascotasGeneralPage : ContentPage
{
	public MascotasGeneralPage()
	{
		InitializeComponent();
		BindingContext = new MascotasGeneralPageViewModel();
	}
}