using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class ProfDashBoard : ContentPage
{
	public ProfDashBoard()
	{
		InitializeComponent();
		BindingContext = new ProfDashBoardModelView();
	}
}