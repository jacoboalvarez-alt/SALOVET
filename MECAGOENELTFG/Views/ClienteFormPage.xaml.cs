using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class ClienteFormPage : ContentPage
{
	public ClienteFormPage()
	{
		InitializeComponent();
		BindingContext = new ClienteFormViewModel();
	}

}