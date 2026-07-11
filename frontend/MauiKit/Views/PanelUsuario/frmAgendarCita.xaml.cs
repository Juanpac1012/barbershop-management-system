using Backend.Entidades;
using Newtonsoft.Json;
using System.Text;
namespace MauiKit.Views.PanelUsuario;

public partial class frmAgendarCita : BasePage
{
    private int? servicioSeleccionadoId;
    private int? barberoSeleccionadoId;
    public frmAgendarCita()
    {
        InitializeComponent();
        CargarBarberos();
        CargarServicios();

        cmbBarbero.SelectedIndexChanged += (s, e) =>
        {
            var barbero = cmbBarbero.SelectedItem as Usuario;
            barberoSeleccionadoId = (int?)(barbero?.idUsuario ?? 0);
            ActualizarResumen();
        };

        cmbServicio.SelectedIndexChanged += (s, e) =>
        {
            var servicio = cmbServicio.SelectedItem as Servicio;
            servicioSeleccionadoId = servicio?.idServicio ?? 0;
            ActualizarResumen();
        };

        dpFecha.DateSelected += (s, e) => ActualizarResumen();
        tpHora.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "Time") ActualizarResumen();
        };

    }

    private async void btnAgendar_Clicked(object sender, EventArgs e)
    {
        var barberoSeleccionado = cmbBarbero.SelectedItem as Usuario;
        var servicioSeleccionado = cmbServicio.SelectedItem as Servicio;
        DateTime fechaSeleccionada = dpFecha.Date;
        TimeSpan horaSeleccionada = tpHora.Time;
        var errores = ValidarCita(barberoSeleccionado, servicioSeleccionado, fechaSeleccionada, horaSeleccionada);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error de validación", string.Join("\n", errores), "Aceptar");
            return;
        }
        try
        {
            ReqInsertarCita req = new ReqInsertarCita();
            req.cita = new Cita();
            req.cita.usuario = new Usuario();
            req.cita.barbero = new Usuario();
            req.cita.servicio = new Servicio();

            req.cita.usuario.idUsuario = Sesion.usuario.idUsuario;
            req.cita.barbero.idUsuario = barberoSeleccionado.idUsuario;
            req.cita.servicio.idServicio = servicioSeleccionado.idServicio;
            req.cita.fechaHora = fechaSeleccionada.Add(horaSeleccionada);

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "cita/insertar", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResInsertarCita res = new ResInsertarCita();
                res = JsonConvert.DeserializeObject<ResInsertarCita>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "Cita agendada correctamente", "Aceptar");
                    //navergar a facturacion
                }
                else
                {
                    string mensajeErrores = string.Join("\n", res.listaErrores.Select(e => e.Message));
                    await DisplayAlert("Error de inicio de sesión", mensajeErrores, "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }

    }

    private async void CargarServicios()
    {
        try
        {
            ReqListarServicio req = new ReqListarServicio();
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "servicio/listar", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResListarServicio res = JsonConvert.DeserializeObject<ResListarServicio>(responseContent);

                if (res.resultado)
                {
                    var listaServicio = new List<Servicio> { new Servicio { idServicio = 0, nombre = "Seleccione un servicio..." } }; // Opción vacía, revisar
                    listaServicio.AddRange(res.servicio);
                    cmbServicio.ItemsSource = listaServicio;
                    cmbServicio.ItemDisplayBinding = new Binding("nombre");
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron servicios.", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la lista de servicios.", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    private async void CargarBarberos()
    {
        try
        {
            ReqListarBarberos req = new ReqListarBarberos();
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "barbero/listar", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResListarBarberos res = JsonConvert.DeserializeObject<ResListarBarberos>(responseContent);

                if (res.resultado)
                {
                    var listaBarberos = new List<Usuario> { new Usuario { idUsuario = 0, nombre = "Seleccione un barbero..." } };
                    listaBarberos.AddRange(res.barberos);
                    cmbBarbero.ItemsSource = listaBarberos;
                    cmbBarbero.ItemDisplayBinding = new Binding("nombre");
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron servicios.", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la lista de servicios.", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    private void ActualizarResumen()
    {
        var barberoSeleccionado = cmbBarbero.SelectedItem as Usuario;
        var servicioSeleccionado = cmbServicio.SelectedItem as Servicio;
        DateTime fechaSeleccionada = dpFecha.Date;
        TimeSpan horaSeleccionada = tpHora.Time;

        if (barberoSeleccionado == null || servicioSeleccionado == null)
        {
            frmResumen.IsVisible = false;
            return;
        }

        lblResumenBarbero.Text = barberoSeleccionado.nombre;
        lblResumenServicio.Text = servicioSeleccionado.nombre;
        lblResumenFecha.Text = fechaSeleccionada.ToString("dd/MM/yyyy");
        DateTime horaDateTime = DateTime.Today.Add(horaSeleccionada);
        lblResumenHora.Text = horaDateTime.ToString("h:mm tt");

        frmResumen.IsVisible = true;
    }

    private List<string> ValidarCita(object barberoSeleccionado, object servicioSeleccionado, DateTime fecha, TimeSpan hora)
    {
        List<string> errores = new List<string>();
        DateTime fechaHoraCita = fecha.Add(hora);
        DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);
        DateTime apertura = fechaHoraCita.Date.AddHours(8);
        DateTime cierre = fechaHoraCita.Date.AddHours(20);
        if (barberoSeleccionadoId == 0)
        {
            errores.Add("Por favor seleccione el barbero que lo atenderá.");
        }
        if (servicioSeleccionadoId == 0)
        {
            errores.Add("Seleccione el servicio que desea agendar.");
        }
        if (fechaHoraCita <= fechaHoraActual)
        {
            errores.Add("No puede agendar citas en horarios pasados. Por favor seleccione una fecha y hora futuras.");
        }
        if (fechaHoraCita.DayOfWeek == DayOfWeek.Sunday)
        {
            errores.Add("Nuestro horario de atención es de lunes a sábado. No trabajamos los domingos.");
        }
        if (fechaHoraCita < apertura || fechaHoraCita > cierre)
        {
            errores.Add("El horario de atención es de 8:00 AM a 8:00 PM. Seleccione un horario dentro de este rango.");
        }
        return errores;
    }

    protected override void OnAppearing() 
    {
        base.OnAppearing();

        cmbBarbero.SelectedItem = null;
        cmbServicio.SelectedItem = null;
        barberoSeleccionadoId = 0;
        servicioSeleccionadoId = 0;
        dpFecha.Date = DateTime.Today;
        tpHora.Time = DateTime.Now.TimeOfDay;
        frmResumen.IsVisible = false;
        CargarServicios();
        CargarBarberos();
    }
}