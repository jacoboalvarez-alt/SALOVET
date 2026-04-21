using MECAGOENELTFG.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MECAGOENELTFG.Views;

[QueryProperty(nameof(IdMascota), "idMascota")]
[QueryProperty(nameof(Nombre), "nombre")]
public partial class RegistroMascotasPage : ContentPage
{
    public int IdMascota { get; set; }
    public string Nombre { get; set; }
    public RegistroMascotasPage()
	{
		InitializeComponent();
	}


    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new RegistroMascotaViewModel(IdMascota, Nombre);
    }
}