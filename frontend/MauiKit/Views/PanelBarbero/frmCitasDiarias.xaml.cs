using Backend.Entidades;
using MauiKit.Entidades;
using Newtonsoft.Json;
using System.Text;
namespace MauiKit.Views.PanelBarbero;

public partial class frmCitasDiarias : BasePage
{
    public frmCitasDiarias()
    {
        InitializeComponent();
        cargarDatosCitasDia();
        lblFecha.Text = DateTime.UtcNow.AddHours(-6).ToString("dddd, dd MMMM yyyy", new CultureInfo("es-ES"));
    }
    private async void cargarDatosCitasDia()
    {
        try
        {
            ReqListarCitasDiaBarbero req = new ReqListarCitasDiaBarbero();
            req.idBarbero = Sesion.usuario.idUsuario;
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "cita/listarCitasDiaBarbero", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResListarCitasDiaBarbero res = JsonConvert.DeserializeObject<ResListarCitasDiaBarbero>(responseContent);
                if (res.resultado)
                {
                    citasDiaBarberoCollectionView.ItemsSource = res.cita;
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

    private async void btnMarcarTerminada_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Obtener el botón y validar el CommandParameter
            Button button = sender as Button;
            if (button == null || button.CommandParameter == null)
            {
                await DisplayAlert("Error", "No se pudo obtener el ID de la cita", "Aceptar");
                return;
            }

            // Convertir CommandParameter a int de manera segura
            if (!int.TryParse(button.CommandParameter.ToString(), out int IDCita))
            {
                await DisplayAlert("Error", "El ID de la cita no es válido", "Aceptar");
                return;
            }

            // Obtener la cita completa
            var citaSeleccionada = (citasDiaBarberoCollectionView.ItemsSource as List<Cita>)
                                  ?.FirstOrDefault(c => c.idCita == IDCita);

            if (citaSeleccionada == null)
            {
                await DisplayAlert("Error", "No se encontró la cita seleccionada", "Aceptar");
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

                    if (citasDiaBarberoCollectionView.ItemsSource is List<Cita> listaCitas)
                    {
                        var citaTerminada = listaCitas.FirstOrDefault(c => c.idCita == IDCita);
                        if (citaTerminada != null)
                        {
                            listaCitas.Remove(citaTerminada);
                            citasDiaBarberoCollectionView.ItemsSource = new List<Cita>(listaCitas); 
                        }
                    }
                    await EnviarFactura(IDCita, (int)citaSeleccionada.usuario.idUsuario);
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

    private async Task EnviarFactura(int idCita, int idUsuario)
    {
        try
        {
            ReqInsertarFacturaCita req = new ReqInsertarFacturaCita();
            req.idCita = idCita;
            req.idUsuario = idUsuario;

            var jsonContent = new StringContent(JsonConvert.SerializeObject(req),
                                             Encoding.UTF8,
                                             "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                var respuestaHttp = await httpClient.PostAsync(App.API_URL + "factura/insertarCita", jsonContent);

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "Factura generada correctamente", "Aceptar");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo generar la factura", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al generar factura: {ex.Message}", "Aceptar");
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

                    if (citasDiaBarberoCollectionView.ItemsSource is List<Cita> listaCitas)
                    {
                        var citaTerminada = listaCitas.FirstOrDefault(c => c.idCita == IDCita);
                        if (citaTerminada != null)
                        {
                            listaCitas.Remove(citaTerminada);
                            citasDiaBarberoCollectionView.ItemsSource = new List<Cita>(listaCitas); // Refrescar la lista
                        }
                    }
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