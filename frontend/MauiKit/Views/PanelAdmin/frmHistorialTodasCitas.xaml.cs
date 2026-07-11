using Backend.Entidades;
using MauiKit.Entidades;
using Newtonsoft.Json;
using System.Text;
namespace MauiKit.Views.PanelAdmin;

public partial class frmHistorialTodasCitas : BasePage
{
	public frmHistorialTodasCitas()
	{
		InitializeComponent();
        cargarDatosCitasAdmin();
    }
    private async void cargarDatosCitasAdmin()
    {
        try
        {
            ReqListarTodasCitas req = new ReqListarTodasCitas();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                var respuestaHttp = await httpClient.PostAsync(App.API_URL + "cita/todasCitas", jsonContent);

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResListarTodasCitas>(responseContent);

                    if (res.resultado)
                    {
                        foreach (var cita in res.cita)
                        {
                            if (cita.fechaHora.Kind != DateTimeKind.Unspecified)
                            {
                                cita.fechaHora = cita.fechaHora.ToUniversalTime().AddHours(-6);
                            }
                        }

                        var citasAgrupadas = res.cita
                            .OrderByDescending(c => c.fechaHora)
                            .GroupBy(c => c.fechaHora.Date)
                            .Select(g => new GroupedCitas(g.Key, g.ToList()))
                            .ToList();

                        citasAdminCollectionView.ItemsSource = citasAgrupadas;

                        if (citasAgrupadas.Count == 0)
                        {
                            await DisplayAlert("Información", "No hay citas registradas en el historial", "Aceptar");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se encontraron citas", "Aceptar");
                    }
                }
            }
        }
        catch (Exception)
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
            TotalGanancias = citas
                .Where(c => c.estado != null && c.estado.Equals("Terminada", StringComparison.OrdinalIgnoreCase))
                .Sum(c => c.servicio?.precio ?? 0);
        }
    }


    private async void btnMarcarTerminada_Clicked(object sender, EventArgs e)
    {
        try
        {
            Button button = sender as Button;
            if (button == null || button.CommandParameter == null)
            {
                await DisplayAlert("Error", "No se pudo obtener el ID de la cita", "Aceptar");
                return;
            }

            if (!int.TryParse(button.CommandParameter.ToString(), out int IDCita))
            {
                await DisplayAlert("Error", "El ID de la cita no es válido", "Aceptar");
                return;
            }


            ReqCitaTerminada req = new ReqCitaTerminada();
            req.idBarbero = Sesion.usuario.idUsuario;
            req.idCita = IDCita;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "cita/citaTeminada", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResCitaTerminada res = new ResCitaTerminada();
                res = JsonConvert.DeserializeObject<ResCitaTerminada>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "La cita ha sido marcada como terminada", "Aceptar");
                    cargarDatosCitasAdmin();
                }
                else
                {
                    string mensajeErrores = string.Join("\n", res.listaErrores.Select(e => e.Message));
                    await DisplayAlert("Error", mensajeErrores, "Aceptar");
                }
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }

    }

    private async void btnMarcarNoAsistio_Clicked(object sender, EventArgs e)
    {
        try
        {
            Button button = sender as Button;
            if (button == null || button.CommandParameter == null)
            {
                await DisplayAlert("Error", "No se pudo obtener el ID de la cita", "Aceptar");
                return;
            }

            if (!int.TryParse(button.CommandParameter.ToString(), out int IDCita))
            {
                await DisplayAlert("Error", "El ID de la cita no es válido", "Aceptar");
                return;
            }

            ReqCitaNoAsistio req = new ReqCitaNoAsistio();
            req.idBarbero = Sesion.usuario.idUsuario;
            req.idCita = IDCita;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "cita/citaNoAsistio", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResCitaNoAsistio res = new ResCitaNoAsistio();
                res = JsonConvert.DeserializeObject<ResCitaNoAsistio>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "La cita ha sido marcada como No Asistió", "Aceptar");
                    cargarDatosCitasAdmin();
                }
                else
                {
                    string mensajeErrores = string.Join("\n", res.listaErrores.Select(e => e.Message));
                    await DisplayAlert("Error", mensajeErrores, "Aceptar");
                }
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }

    }
}