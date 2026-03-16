using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class CitasPage : ContentPage
{
	public CitasPage()
	{
		InitializeComponent();
		BindingContext = new CitasViewModel();
	}
}