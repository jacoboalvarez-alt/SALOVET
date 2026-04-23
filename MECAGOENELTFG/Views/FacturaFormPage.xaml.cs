using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class FacturaFormPage : ContentPage
{
	public FacturaFormPage(NuevaFacturaViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}