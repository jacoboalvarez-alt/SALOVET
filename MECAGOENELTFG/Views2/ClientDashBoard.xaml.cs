namespace MECAGOENELTFG.Views;

public partial class ClientDashBoard : ContentPage
{
    public ClientDashBoard()
    {
        InitializeComponent();
        CargarNoticiasEjemplo();
    }

    private IDispatcherTimer? _carruselTimer;

    private void CargarNoticiasEjemplo()
    {
        var noticias = new List<NoticiaItem>
    {
        new() { Titulo = "Campaña de vacunación primavera 2026", Imagen = "noticia1.png" },
        new() { Titulo = "Nuevos servicios disponibles",       Imagen = "noticia2.png" },
        new() { Titulo = "Consejos para el verano",              Imagen = "noticia3.png" },
    };

        NoticiasCarousel.ItemsSource = noticias;

        // 1. Limpieza total del timer antes de crear uno nuevo
        if (_carruselTimer != null)
        {
            _carruselTimer.Stop();
            _carruselTimer = null;
        }

        // 2. Crear el timer con un intervalo prudente (5 segundos es lo estándar)
        _carruselTimer = Dispatcher.CreateTimer();
        _carruselTimer.Interval = TimeSpan.FromSeconds(15);
        _carruselTimer.Tick += (s, e) =>
        {
            Thread.Sleep(5000);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (noticias.Count > 0)
                {
                    // Calculamos la siguiente posición
                    var siguiente = (NoticiasCarousel.Position + 1) % noticias.Count;

                    // Cambiamos la posición de forma segura
                    NoticiasCarousel.Position = siguiente;
                }
            });
        };

        _carruselTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _carruselTimer?.Stop();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _carruselTimer?.Start();
    }

    //---------------------------------------------------------------
    //FUNCIONES PARA EL MOVIMIENTOS ENTRE LAS VISTAS DE LA APLICACION
    //---------------------------------------------------------------

    private async void OnSalirClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Login");
    }

    private void OnInicioTapped(object? sender, TappedEventArgs e)
    {
        // Ya estamos en inicio, no hace nada
    }

    private async void OnNavCitasTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("CitasPageClient");
    }

    private async void OnNavPerfilTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("PerfilPage");
    }


    private async void OnChatTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ChatAsistente");
    }
}

public class NoticiaItem
{
    public string Titulo { get; set; } = string.Empty;
    public string Imagen { get; set; } = string.Empty;
}