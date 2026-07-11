using Newtonsoft.Json;
using MauiKit.Entidades;
using Backend.Entidades;
using System.Text;

namespace MauiKit.Views.PanelUsuario;

public partial class frmCitasUsuario : BasePage
{
	public frmCitasUsuario()
	{
		InitializeComponent();
        cargarDatosCitas();
    }
    private async void cargarDatosCitas()
    {
        try
        {
            ReqListarCitasUsuario req = new ReqListarCitasUsuario();
            req.idUsuario = Sesion.usuario.idUsuario;
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "cita/listarCitasUsuario", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResListarCitasUsuario res = JsonConvert.DeserializeObject<ResListarCitasUsuario>(responseContent);
                if (res.resultado)
                {
                    citasUsuarioCollectionView.ItemsSource = res.cita;
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

    private async void btnEliminarCita_Clicked(object sender, EventArgs e)
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


        bool confirmar = await DisplayAlert("Confirmar", "¿Está seguro que desea eliminar esta cita?", "Sí", "No");

        if (confirmar)
        {
            try
            {

                ReqEliminarCita req = new ReqEliminarCita();
                req.idCita = IDCita;
                HttpResponseMessage respuestaHttp = new HttpResponseMessage();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync(App.API_URL + "cita/eliminar", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    ResEliminarCita res = JsonConvert.DeserializeObject<ResEliminarCita>(responseContent);

                    if (res.resultado)
                    {
                        await DisplayAlert("Éxito", "Cita eliminada correctamente", "Aceptar");
                        cargarDatosCitas();
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
}