using MECAGOENELTFG.ViewModels2;

namespace MECAGOENELTFG.Views2;

public partial class AgregarMascotaClient : ContentPage
{
    private readonly AgregarMascotaClientViewModel _vm;

    private readonly Color _accentColor = Color.FromArgb("#2EBBB0");
    private readonly Color _accentBg = Color.FromArgb("#E8F8F7");
    private readonly Color _neutralBg = Color.FromArgb("#F5F5F0");
    private readonly Color _neutralText = Color.FromArgb("#888780");
    public AgregarMascotaClient()
	{
		InitializeComponent();
        _vm = new AgregarMascotaClientViewModel();
        BindingContext = _vm;
	}

    // ??? Selección de tipo de mascota ????????????????????????????????

    private async void OnDogTapped(object? sender, TappedEventArgs e)
    {
        if (_vm.SelectedType == "dog") return;
        bool wasVisible = _vm.IsFormVisible;
        await AnimateCard(CardDog);
        ResetCards();
        ActivateCard(CardDog, DogLabel);
        _vm.SelectDogCommand.Execute(null);
        if (!wasVisible) await ShowForm();
        else await AnimateConditionalFields();
    }

    private async void OnCatTapped(object? sender, TappedEventArgs e)
    {
        if (_vm.SelectedType == "cat") return;
        bool wasVisible = _vm.IsFormVisible;
        await AnimateCard(CardCat);
        ResetCards();
        ActivateCard(CardCat, CatLabel);
        _vm.SelectCatCommand.Execute(null);
        if (!wasVisible) await ShowForm();
        else await AnimateConditionalFields();
    }

    private async void OnOtherTapped(object? sender, TappedEventArgs e)
    {
        if (_vm.SelectedType == "other") return;
        bool wasVisible = _vm.IsFormVisible;
        await AnimateCard(CardOther);
        ResetCards();
        ActivateCard(CardOther, OtherLabel);
        _vm.SelectOtherCommand.Execute(null);
        if (!wasVisible) await ShowForm();
        else await AnimateConditionalFields();
    }

    // ??? Botones de tamaño (ahora son Border) ????????????????????????

    private void OnSizeSmall(object? sender, TappedEventArgs e) => SelectSize(BtnSmall, "Pequeño");
    private void OnSizeMedium(object? sender, TappedEventArgs e) => SelectSize(BtnMedium, "Mediano");
    private void OnSizeLarge(object? sender, TappedEventArgs e) => SelectSize(BtnLarge, "Grande");

    private void SelectSize(Border active, string tamano)
    {
        foreach (var btn in new[] { BtnSmall, BtnMedium, BtnLarge })
        {
            btn.BackgroundColor = _neutralBg;
            // Actualizamos el color del Label hijo
            if (btn.Content is Label lbl)
                lbl.TextColor = _neutralText;
        }
        active.BackgroundColor = _accentBg;
        if (active.Content is Label activeLbl)
            activeLbl.TextColor = _accentColor;

        _vm.SelectTamanoCommand.Execute(tamano);
    }

    // ??? Switch vacunado ?????????????????????????????????????????????

    private void OnVaccinatedToggled(object? sender, ToggledEventArgs e)
    {
        VaccinatedLabel.Text = e.Value ? "Vacunado ?" : "No vacunado";
        VaccinatedLabel.TextColor = e.Value ? _accentColor : _neutralText;
    }

    // ??? Guardar / Volver ?????????????????????????????????????????????

    private async void OnVolverClicked(object? sender, TappedEventArgs e)
        => await _vm.VolverCommand.ExecuteAsync(null);

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        _vm.NombreMascota = EntryName.Text ?? string.Empty;
        _vm.Edad = EntryAge.Text ?? string.Empty;
        _vm.Raza = EntryBreed.Text ?? string.Empty;
        _vm.Especie = EntrySpecies.Text ?? string.Empty;
        _vm.Sexo = PickerSex.SelectedItem?.ToString() ?? string.Empty;
        _vm.Color = EntryColor.Text ?? string.Empty;
        _vm.TipoPelo = PickerHair.SelectedItem?.ToString() ?? string.Empty;
        _vm.Notas = EditorNotes.Text ?? string.Empty;
        _vm.EstaVacunado = SwitchVaccinated.IsToggled;

        await _vm.GuardarCommand.ExecuteAsync(null);
    }

    // ??? Animaciones ?????????????????????????????????????????????????

    private static async Task AnimateCard(Border card)
    {
        await card.ScaleTo(0.93, 80, Easing.CubicOut);
        await card.ScaleTo(1.0, 80, Easing.CubicIn);
    }

    private async Task ShowForm()
    {
        FormBody.IsVisible = true;
        FormBody.Opacity = 0;
        FormBody.TranslationY = 20;
        await Task.WhenAll(
            FormBody.FadeTo(1, 350, Easing.CubicOut),
            FormBody.TranslateTo(0, 0, 350, Easing.CubicOut)
        );
    }

    private async Task AnimateConditionalFields()
    {
        var hideTargets = new[] { SizeField, HairField, SpeciesField }
            .Where(f => f.IsVisible)
            .ToList();

        if (hideTargets.Any())
        {
            await Task.WhenAll(hideTargets.Select(f => f.FadeTo(0, 150, Easing.CubicIn)));
            foreach (var f in hideTargets) f.IsVisible = false;
        }

        var showTargets = new List<VerticalStackLayout>();
        if (_vm.SizeFieldVisible) showTargets.Add(SizeField);
        if (_vm.HairFieldVisible) showTargets.Add(HairField);
        if (_vm.SpeciesFieldVisible) showTargets.Add(SpeciesField);

        foreach (var f in showTargets) { f.Opacity = 0; f.IsVisible = true; }

        if (showTargets.Any())
            await Task.WhenAll(showTargets.Select(f => f.FadeTo(1, 250, Easing.CubicOut)));
    }

    private void ResetCards()
    {
        foreach (var (card, label) in new[] {
            (CardDog, DogLabel), (CardCat, CatLabel), (CardOther, OtherLabel) })
        {
            card.BackgroundColor = _neutralBg;
            card.Stroke = Color.FromArgb("#E0E0D8");
            card.StrokeThickness = 1.5;
            label.TextColor = _neutralText;
        }
    }

    private void ActivateCard(Border card, Label label)
    {
        card.BackgroundColor = _accentBg;
        card.Stroke = _accentColor;
        card.StrokeThickness = 2;
        label.TextColor = _accentColor;
    }

    // ??? Cursor en Windows ????????????????????????????????????????????

    private void OnCardPointerEntered(object? sender, PointerEventArgs e)
    {
#if WINDOWS
        var view = (sender as Element)?.Handler?.PlatformView
                   as Microsoft.UI.Xaml.FrameworkElement;
        if (view is null) return;
        var cursor = Microsoft.UI.Input.InputSystemCursor.Create(
            Microsoft.UI.Input.InputSystemCursorShape.Hand);
        typeof(Microsoft.UI.Xaml.UIElement)
            .GetProperty("ProtectedCursor",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            ?.SetValue(view, cursor);
#endif
    }

    private void OnCardPointerExited(object? sender, PointerEventArgs e)
    {
#if WINDOWS
        var view = (sender as Element)?.Handler?.PlatformView
                   as Microsoft.UI.Xaml.FrameworkElement;
        if (view is null) return;
        var cursor = Microsoft.UI.Input.InputSystemCursor.Create(
            Microsoft.UI.Input.InputSystemCursorShape.Arrow);
        typeof(Microsoft.UI.Xaml.UIElement)
            .GetProperty("ProtectedCursor",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            ?.SetValue(view, cursor);
#endif
    }
}