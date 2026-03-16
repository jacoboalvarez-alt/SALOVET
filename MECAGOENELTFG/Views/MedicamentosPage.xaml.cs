using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class MedicamentosPage : ContentPage
{
	public MedicamentosPage()
	{
		InitializeComponent();
		BindingContext = new MedicamentosViewModel();
	}
}