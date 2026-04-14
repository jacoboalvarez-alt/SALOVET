

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
            Routing.RegisterRoute("MedicamentosPage", typeof(MedicamentosPage));
            

            //CLIENTES
            Routing.RegisterRoute("ClientDashBoard", typeof(ClientDashBoard));
            Routing.RegisterRoute("PerfilPage", typeof(PerfilPage));
            Routing.RegisterRoute("CitasPageClient", typeof(CitasPageClient));
            Routing.RegisterRoute("ChatAsistente",typeof(AsistentePageClient));
            Routing.RegisterRoute("AgregarMascotaClient", typeof(AgregarMascotaClient));
            Routing.RegisterRoute("ClienteEditarPerfilPage", typeof(ClienteEditarPerfilPage));

            InitializeComponent();
        }
    }
}
