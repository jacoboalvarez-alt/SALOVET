

using MECAGOENELTFG.Views;
using MECAGOENELTFG.Views2;

namespace MECAGOENELTFG
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            
            //REGISTRO
            Routing.RegisterRoute("Registro", typeof(Registro));

            //PROFESIONALES
            Routing.RegisterRoute("ProfDashBoard", typeof(ProfDashBoard));
            Routing.RegisterRoute("MascotasPageGeneral", typeof(MascotasGeneralPage));

            Routing.RegisterRoute("clienteform", typeof(ClienteFormPage));
            Routing.RegisterRoute("mascotaform", typeof(MascotaFormPage));

            Routing.RegisterRoute("ClientesPage",typeof(ClientesPage));
            Routing.RegisterRoute("CitasPage",typeof(CitasPage));

            Routing.RegisterRoute("MascotasPage", typeof(MascotasPage));
            Routing.RegisterRoute("MedicamentosPage", typeof(MedicamentosPage));

            Routing.RegisterRoute("AsistenteVetProf", typeof(AsistenteVetProf));
            Routing.RegisterRoute("FacturasPage", typeof(FacturasPage));

            Routing.RegisterRoute("RegistroMascota", typeof(RegistroMascotasPage));
            Routing.RegisterRoute("RegistroMascotaForm", typeof(RegistroMascotaFormPage));

            Routing.RegisterRoute("CitasForm", typeof(CitaFormPage));
            Routing.RegisterRoute("MedicamentoForm", typeof(MedicamentoFormPage));

            Routing.RegisterRoute("EditarCita", typeof(EditarCitaPage));
            Routing.RegisterRoute("FacturasFormPage", typeof(FacturaFormPage));
            

            //CLIENTES
            Routing.RegisterRoute("ClientDashBoard", typeof(ClientDashBoard));
            Routing.RegisterRoute("PerfilPage", typeof(PerfilPage));
            Routing.RegisterRoute("CitasPageClient", typeof(CitasPageClient));
            Routing.RegisterRoute("ChatAsistente",typeof(AsistentePageClient));
            Routing.RegisterRoute("AgregarMascotaClient", typeof(AgregarMascotaClient));
            Routing.RegisterRoute("ClienteEditarPerfilPage", typeof(ClienteEditarPerfilPage));
            Routing.RegisterRoute("EditarMascotaClient", typeof(EditarMascotaClient));
            Routing.RegisterRoute("FormCitasCliente", typeof(FormCitasCliente));
            Routing.RegisterRoute("ModificarCitaPageClient", typeof(ModificarCitasPageClient));

            InitializeComponent();
        }
    }
}
