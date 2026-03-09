

using MECAGOENELTFG.Views;

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

            InitializeComponent();
        }
    }
}
