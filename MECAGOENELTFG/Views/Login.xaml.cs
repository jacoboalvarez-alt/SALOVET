using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
		BindingContext = new LoginViewModel();
	}
}