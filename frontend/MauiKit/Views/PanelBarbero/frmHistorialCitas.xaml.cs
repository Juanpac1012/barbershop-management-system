using Backend.Entidades;
using MauiKit.Entidades;
using Newtonsoft.Json;
using System.Text;
namespace MauiKit.Views.PanelBarbero;

public partial class frmHistorialCitas : BasePage
{
    public frmHistorialCitas()
	{
		InitializeComponent();
        cargarDatosCitasBarbero();

    }
    private async void cargarDatosCitasBarbero()
    {
        try
        {
            ReqListarCitasBarbero req = new ReqListarCitasBarbero();
            req.idBarbero = Sesion.usuario.idUsuario;
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "cita/listarCitasBarbero", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResListarCitasBarbero res = JsonConvert.DeserializeObject<ResListarCitasBarbero>(responseContent);

                if (res.resultado)
                {
                    // Ajustar fechas a UTC-6 (Costa Rica)
                    foreach (var cita in res.cita)
                    {
                        if (cita.fechaHora.Kind != DateTimeKind.Unspecified)
                        {
                            cita.fechaHora = cita.fechaHora.ToUniversalTime().AddHours(-6);
                        }
                    }

                    // Filtrar solo citas terminadas y agrupar por fecha
                    var citasAgrupadas = res.cita
                        .Where(c => c.estado == "Terminada") // Filtro para solo citas terminadas
                        .OrderByDescending(c => c.fechaHora)
                        .GroupBy(c => c.fechaHora.Date)
                        .Select(g => new GroupedCitas(g.Key, g.ToList()))
                        .ToList();

                    citasBarberoCollectionView.ItemsSource = citasAgrupadas;

                    // Mostrar mensaje si no hay citas terminadas
                    if (citasAgrupadas.Count == 0)
                    {
                        await DisplayAlert("Información", "No hay citas terminadas en tu historial", "Aceptar");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron citas", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    public class GroupedCitas : List<Cita>
    {
        public DateTime Key { get; private set; }
        public decimal TotalGanancias { get; private set; }

        public GroupedCitas(DateTime key, List<Cita> citas) : base(citas)
        {
            Key = key;
            TotalGanancias = citas.Sum(c => c.servicio.precio);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        cargarDatosCitasBarbero();
    }
}