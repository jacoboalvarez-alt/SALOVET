using MECAGOENELTFG.Services;
using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

[QueryProperty(nameof(IdMascota),"idMascota")]
public partial class RegistroMascotaFormPage : ContentPage
{
    public int IdMascota { get; set; }
	public RegistroMascotaFormPage()
	{
		InitializeComponent();
	}
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var registro = SessionService.RegistroEdicion;
        SessionService.RegistroEdicion = null;

        BindingContext = new RegistroMascotaFormViewModel(IdMascota,registro);
    }
}