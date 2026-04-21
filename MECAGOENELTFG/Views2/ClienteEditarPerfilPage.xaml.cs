using MECAGOENELTFG.ViewModels2;

namespace MECAGOENELTFG.Views2;

public partial class ClienteEditarPerfilPage : ContentPage
{
	public ClienteEditarPerfilPage()
	{
		InitializeComponent();
		BindingContext = new ClienteEditarPefilViewModel();
	}
}