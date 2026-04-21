using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace MECAGOENELTFG.Views;

public class ChatMessage
{
    public string Text { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public bool IsUser { get; set; }
}

// ??? Modelos para la API de Gemini ???????????????????????????????????????????

internal record GeminiRequest(List<GeminiContent> contents, GeminiSystemInstruction system_instruction, GeminiGenerationConfig generation_config);
internal record GeminiContent(string role, List<GeminiPart> parts);
internal record GeminiPart(string text);
internal record GeminiSystemInstruction(List<GeminiPart> parts);
internal record GeminiGenerationConfig(double temperature, int maxOutputTokens);
internal record GeminiResponse(List<GeminiCandidate>? candidates);
internal record GeminiCandidate(GeminiContent content);

//  Servicio de IA 

internal class PetAIService
{

    // CONFIGURACIÓN DE LA IA
    // Cambia aquí el comportamiento del asistente antes de mandarlo a Gemini.

    private const string SystemPrompt = """
    Eres "SALOVET IA", el asistente virtual de la clínica veterinaria Salovet.

    ## ROL
    Eres el ayudante de un Veterinario profesional de la empresa, le serviras de ayuda
    para entender problemas sobre las mascotas que lleguen y asistir al Veterinario para poder dar un
    diagnostico correcto.

    ## FUENTES DE INFORMACIÓN
    Basa siempre tus respuestas en estas fuentes, por orden de prioridad:

    Perros y gatos:
    1. msdvetmanual.com
    2. wsava.org
    3. avepa.org
    4. animalshealth.es

    Reptiles (iguanas, tortugas, serpientes, geckos, pogonas):
    1. msdvetmanual.com/exotic-and-laboratory-animals/reptiles
    2. arav.org
    3. reptifiles.com
    4. veterinarypartner.vin.com

    Aves (loros, periquitos, canarios, agapornis, cacatúas):
    1. msdvetmanual.com/exotic-and-laboratory-animals/pet-birds
    2. lafebervet.com
    3. aav.org
    4. veterinarypartner.vin.com

    Roedores (hámsters, cobayas, chinchillas, ratas, ratones, degús):
    1. msdvetmanual.com/exotic-and-laboratory-animals/rodents
    2. oxbowanimalhealth.com
    3. veterinarypartner.vin.com
    4. aspca.org/pet-care/small-pet-care

    ## REGLAS
    - Responde SIEMPRE en español.
    - Sé conciso: máximo 3-4 frases por respuesta, y si es necesario separalo por pasos para su entendimiento.
    - Usa un tono cálido y termina cada mensaje con un emoji de animal.
    - NO diagnostiques enfermedades ni inventes medicamentos o dosis, pero si avisa de la posibilidad.
    - Si el caso parece grave, recomienda visitar al veterinario presencialmente.
    """;

    // API KEY 
    // Mueve este valor a un archivo de configuración seguro (appsettings, 
    // variable de entorno o Secret Manager) antes de publicar la app.

    private const string ApiKey = "AIzaSyCmGPjWoSIT8kJ-3nI-3WH03aa2NgLZDAo";
    private const string Model = "gemini-2.5-flash-lite";
    private const double Temperature = 0.7;  // 0 = determinista, 1 = creativo
    private const int MaxTokens = 100;  // Límite de tokens por respuesta


    private static readonly HttpClient _http = new();

    // Historial de conversación que se envía en cada llamada para mantener contexto
    private readonly List<GeminiContent> _history = new();

    public async Task<string> AskAsync(string userMessage)
    {
        _history.Add(new GeminiContent("user", new List<GeminiPart> { new(userMessage) }));

        var requestBody = new GeminiRequest(
            contents: _history,
            system_instruction: new GeminiSystemInstruction(
                new List<GeminiPart> { new(SystemPrompt) }),
            generation_config: new GeminiGenerationConfig(Temperature, MaxTokens));

        var json = JsonSerializer.Serialize(requestBody);

        //  Lista de modelos a intentar en orden 
        // Si el primero falla por saturación (503), prueba el siguiente.

        var modelsToTry = new[] { "gemini-2.5-flash-lite", "gemini-2.5-flash" };

        foreach (var model in modelsToTry)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={ApiKey}";

            for (int attempt = 1; attempt <= 2; attempt++)
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var parsed = JsonSerializer.Deserialize<GeminiResponse>(responseJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var assistantReply = parsed?.candidates?[0].content.parts[0].text
                                         ?? "No pude generar una respuesta. Inténtalo de nuevo.";

                    _history.Add(new GeminiContent("model", new List<GeminiPart> { new(assistantReply) }));
                    return assistantReply;
                }

                var statusCode = (int)response.StatusCode;

                // Solo reintenta en 503 (servidor saturado)
                if (statusCode == 503 && attempt < 2)
                {
                    await Task.Delay(2000);
                    continue;
                }

                // Cualquier otro error o modelo agotado ? prueba el siguiente modelo
                break;
            }
        }

        throw new Exception("El servicio no está disponible ahora mismo. Inténtalo en unos segundos.");
    }
}


public partial class AsistenteVetProf : ContentPage
{
    private readonly ObservableCollection<ChatMessage> _messages = new();
    private readonly PetAIService _aiService = new();
    private bool _isBotTyping = false;

    public AsistenteVetProf()
    {
        InitializeComponent();
        MessagesCollection.ItemsSource = _messages;
        MessagesCollection.ItemTemplate = new ChatMessageTemplateSelector(
            botTemplate: (DataTemplate)Resources["BotMessageTemplate"],
            userTemplate: (DataTemplate)Resources["UserMessageTemplate"]);

        AddBotMessage("¡Hola! Soy tu asistente de mascotas de Salovet. " +
                      "Puedo ayudarte con vacunas, alimentación, higiene y más. " +
                      "¿En qué puedo ayudarte hoy? ??");
    }
    //  Enviar mensaje 

    private async void OnSendTapped(object? sender, TappedEventArgs e) =>
        await SendMessageAsync(MessageInput.Text?.Trim());

    private async void OnQuickReply1(object? sender, TappedEventArgs e) =>
        await SendMessageAsync("¿Qué vacunas necesita mi mascota?");

    private async void OnQuickReply2(object? sender, TappedEventArgs e) =>
        await SendMessageAsync("¿Cuánto ejercicio necesita al día?");

    private async void OnQuickReply3(object? sender, TappedEventArgs e) =>
        await SendMessageAsync("Dame consejos de higiene para mi mascota");

    private async Task SendMessageAsync(string? text)
    {
        if (string.IsNullOrEmpty(text) || _isBotTyping) return;

        MessageInput.Text = string.Empty;
        AddUserMessage(text);
        await GetBotReplyAsync(text);
    }

    //  Llamada real a la IA 

    private async Task GetBotReplyAsync(string userText)
    {
        if (_isBotTyping) return;
        _isBotTyping = true;
        ShowTypingIndicator(true);

        try
        {
            var reply = await _aiService.AskAsync(userText);
            ShowTypingIndicator(false);
            AddBotMessage(reply);
        }
        catch (Exception ex)
        {
            ShowTypingIndicator(false);
            AddBotMessage($"Ha ocurrido un error al contactar con la IA. " +
                          $"Comprueba tu conexión e inténtalo de nuevo. ({ex.Message})");
        }
        finally
        {
            _isBotTyping = false;
        }
    }

    // Helpers para agregar mensajes

    private void AddUserMessage(string text) =>
        _messages.Add(new ChatMessage
        { Text = text, Time = CurrentTime(), IsUser = true });

    private void AddBotMessage(string text)
    {
        _messages.Add(new ChatMessage
        { Text = text, Time = CurrentTime() + " ??", IsUser = false });
        ScrollToEnd();
    }

    //Indicador "escribiendo..." con animación

    private CancellationTokenSource? _typingAnimCts;

    private void ShowTypingIndicator(bool show)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            TypingIndicator.IsVisible = show;

            if (show)
            {
                StatusLabel.Text = "Escribiendo...";
                StatusLabel.TextColor = Color.FromArgb("#888780");

                _typingAnimCts?.Cancel();
                _typingAnimCts = new CancellationTokenSource();
                var token = _typingAnimCts.Token;

                _ = Task.Run(async () =>
                {
                    var dots = new[] { Dot1, Dot2, Dot3 };
                    while (!token.IsCancellationRequested)
                    {
                        for (int i = 0; i < dots.Length; i++)
                        {
                            if (token.IsCancellationRequested) break;
                            var dot = dots[i];
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await dot.TranslateTo(0, -5, 180, Easing.SinOut);
                                await dot.TranslateTo(0, 0, 180, Easing.SinIn);
                            });
                            await Task.Delay(80, token).ContinueWith(_ => { });
                        }
                        await Task.Delay(200, token).ContinueWith(_ => { });
                    }
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        foreach (var d in dots) d.TranslationY = 0;
                    });
                }, token);
            }
            else
            {
                _typingAnimCts?.Cancel();
                StatusLabel.Text = "En línea";
                StatusLabel.TextColor = Color.FromArgb("#1D9E75");
            }

            ScrollToEnd();
        });
    }

    //Scroll al último mensaje

    private void ScrollToEnd()
    {
        if (_messages.Count > 0)
            MessagesCollection.ScrollTo(_messages[^1],
                position: ScrollToPosition.End, animate: true);
    }

    private static string CurrentTime()
    {
        var n = DateTime.Now;
        return $"{n.Hour}:{n.Minute:D2}";
    }

    private async void OnVolverTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ProfDashBoard");
    }
}

public class ChatMessageTemplateSelector : DataTemplateSelector
{
    private readonly DataTemplate _botTemplate;
    private readonly DataTemplate _userTemplate;

    public ChatMessageTemplateSelector(DataTemplate botTemplate,
                                       DataTemplate userTemplate)
    {
        _botTemplate = botTemplate;
        _userTemplate = userTemplate;
    }

    protected override DataTemplate OnSelectTemplate(object item,
                                                      BindableObject container) =>
        item is ChatMessage { IsUser: true } ? _userTemplate : _botTemplate;

}