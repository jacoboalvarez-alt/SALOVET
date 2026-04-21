using MECAGOENELTFG.ViewModels2;

namespace MECAGOENELTFG.Views2;

public partial class ModificarCitasPageClient : ContentPage
{
	public ModificarCitasPageClient()
	{
		InitializeComponent();
		BindingContext = new ModificarCitaViewModel();
	}
}