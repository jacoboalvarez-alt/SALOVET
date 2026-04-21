using MECAGOENELTFG.Services;
using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class MedicamentoFormPage : ContentPage
{
	public MedicamentoFormPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var medicamento = SessionService.MedicamentoEdicion;
        SessionService.MedicamentoEdicion = null;
        BindingContext = new MedicamentoFormViewModel(medicamento);
    }
}