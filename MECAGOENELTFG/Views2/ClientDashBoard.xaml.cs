using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;

namespace MECAGOENELTFG.Views;

public partial class ClientDashBoard : ContentPage
{
    private readonly ClienteApiService _service;
    private readonly CitasAPIService _citasService;
    private int id_cliente = SessionService.IdClienteActual;
    private Cliente client = new Cliente();

    public ClientDashBoard()
    {
        InitializeComponent();
        CargarNoticiasEjemplo();
        _service = new ClienteApiService();
        _citasService = new CitasAPIService();
    }

    private IDispatcherTimer? _carruselTimer;

    public async void AsociarID()
    {
        client = await _service.ObtenerPorId(id_cliente);
        NombreClienteLabel.Text = client.NombreCli;
    }

    private async Task CargarCitasAsync()
    {
        if (id_cliente == 0) return;

        var citas = await _citasService.ObtenerCitasPorCliente(id_cliente);
        var ahora = DateTime.Now;

        // Filtramos solo las 2 más próximas para el dashboard
        var proximas = citas
            .Where(c => c.FechaHora >= ahora &&
                        c.Estado is EstadoCita.PENDIENTE or EstadoCita.CONFIRMADA)
            .OrderBy(c => c.FechaHora)
            .Take(2)
            .ToList();

        // Limpiamos el contenedor (quitamos las tarjetas de ejemplo del XAML)
        CitasContainer.Children.Clear();

        if (proximas.Count == 0)
        {
            SinCitasPlaceholder.IsVisible = true;
            CitasContainer.Children.Add(SinCitasPlaceholder);
            return;
        }

        foreach (var c in proximas)
        {
            var card = CrearTarjetaCita(c);
            CitasContainer.Children.Add(card);
        }
    }

    private Border CrearTarjetaCita(Cita c)
    {
        var veterinario = c.Profesional != null
            ? $"{c.Profesional.NomProf} {c.Profesional.ApeProf}"
            : "Veterinario";

        var esProxima = c.Estado is EstadoCita.PENDIENTE or EstadoCita.CONFIRMADA;
        var colorFecha = esProxima ? Color.FromArgb("#E8F8F7") : Color.FromArgb("#F5F5F0");
        var colorTexto = esProxima ? Color.FromArgb("#2EBBB0") : Color.FromArgb("#888780");
        var badgeTexto = esProxima ? "Próxima" : "Pasada";

        var fechaBlock = new Border
        {
            BackgroundColor = colorFecha,
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            WidthRequest = 48,
            HeightRequest = 48,
            Content = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 0,
                Children =
            {
                new Label
                {
                    Text = c.FechaHora.ToString("MMM").ToUpper()[..3],
                    FontSize = 9,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = colorTexto,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = c.FechaHora.Day.ToString("D2"),
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = colorTexto,
                    HorizontalOptions = LayoutOptions.Center
                }
            }
            }
        };
        Grid.SetColumn(fechaBlock, 0);

        var infoBlock = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            Spacing = 3,
            Children =
        {
            new Label
            {
                Text = c.Descripcion ?? "Cita veterinaria",
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                TextColor = Color.FromArgb("#2C2C2A")
            },
            new Label
            {
                Text = $"{c.FechaHora:HH:mm} · {veterinario}",
                FontSize = 12,
                TextColor = Color.FromArgb("#888780")
            }
        }
        };
        Grid.SetColumn(infoBlock, 1);

        var badgeBlock = new Border
        {
            BackgroundColor = colorFecha,
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Padding = new Thickness(8, 4),
            VerticalOptions = LayoutOptions.Center,
            Content = new Label
            {
                Text = badgeTexto,
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                TextColor = colorTexto
            }
        };
        Grid.SetColumn(badgeBlock, 2);

        var grid = new Grid
        {
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Star },
            new ColumnDefinition { Width = GridLength.Auto }
        },
            ColumnSpacing = 12,
            Children = { fechaBlock, infoBlock, badgeBlock }
        };

        return new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
            Padding = new Thickness(16, 14),
            Content = grid
        };
    }

    private void CargarNoticiasEjemplo()
    {
        var noticias = new List<NoticiaItem>
        {
            new() { Titulo = "Campaña de vacunación primavera 2026", Imagen = "noticia1.png" },
            new() { Titulo = "Nuevos servicios disponibles",         Imagen = "noticia2.png" },
            new() { Titulo = "Consejos para el verano",              Imagen = "noticia3.png" },
        };
        NoticiasCarousel.ItemsSource = noticias;

        if (_carruselTimer != null)
        {
            _carruselTimer.Stop();
            _carruselTimer = null;
        }

        _carruselTimer = Dispatcher.CreateTimer();
        _carruselTimer.Interval = TimeSpan.FromSeconds(15);
        _carruselTimer.Tick += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (noticias.Count > 0)
                    NoticiasCarousel.Position = (NoticiasCarousel.Position + 1) % noticias.Count;
            });
        };
        _carruselTimer.Start();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _carruselTimer?.Start();
        AsociarID();
        await CargarCitasAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _carruselTimer?.Stop();
    }

    // Navegación
    private async void OnSalirClicked(object? sender, EventArgs e)
    {
        await Shell.Current.DisplayAlert("Volver al Inicio de Sesión",
            "Vas a volver al Inicio de Sesión, ¿estás seguro?", "OK", "NO");
        await Shell.Current.GoToAsync("//Login");
    }

    private void OnInicioTapped(object? sender, TappedEventArgs e) { }

    private async void OnNavCitasTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("CitasPageClient");

    private async void OnNavPerfilTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("PerfilPage");

    private async void OnChatTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("ChatAsistente");
}

public class NoticiaItem
{
    public string Titulo { get; set; } = string.Empty;
    public string Imagen { get; set; } = string.Empty;
}