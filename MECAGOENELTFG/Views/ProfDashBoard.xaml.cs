using MECAGOENELTFG.ViewModels;

namespace MECAGOENELTFG.Views;

public partial class ProfDashBoard : ContentPage
{
	public ProfDashBoard()
	{
		InitializeComponent();
		BindingContext = new ProfDashBoardModelView();
	}

    private async void OnBtn_PointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            await Task.WhenAll(
                btn.ScaleTo(1.10, 150, Easing.CubicOut),
                btn.TranslateTo(0, -5, 150, Easing.CubicOut)
            );
        }
    }

    private async void OnBtn_PointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            await Task.WhenAll(
                btn.ScaleTo(1.0, 150, Easing.CubicOut),
                btn.TranslateTo(0, 0, 150, Easing.CubicOut)
            );
        }
    }
}