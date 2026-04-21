using MECAGOENELTFG.ViewModels2;

namespace MECAGOENELTFG.Views2;

public partial class EditarMascotaClient : ContentPage
{
	public EditarMascotaClient()
	{
		InitializeComponent();
		BindingContext = new EditarMascotaClientViewModel();
	}
}