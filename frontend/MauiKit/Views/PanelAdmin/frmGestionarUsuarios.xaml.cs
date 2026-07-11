using Backend.Entidades;
using MauiKit.Entidades;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
namespace MauiKit.Views.PanelAdmin;

public partial class frmGestionarUsuarios : BasePage
{
    private Int64? barberoSeleccionadoId;
    private Int64? adminSeleccionadoId;
    public frmGestionarUsuarios()
	{
		InitializeComponent();
        cargarBarberos();
        cargarAdmins();
        LimpiarCampos();

        cmbBarberos.SelectedIndexChanged += (s, e) =>
        {
            var barberoSeleccionado = cmbBarberos.SelectedItem as Usuario;
            if (barberoSeleccionado != null && barberoSeleccionado.idUsuario != 0)
            {
                barberoSeleccionadoId = barberoSeleccionado.idUsuario;
                txtNombreBarbero.Text = barberoSeleccionado.nombre;
                txtApellidoBarbero.Text = barberoSeleccionado.apellido;
                txtCorreoBarbero.Text = barberoSeleccionado.correoElectronico;
                txtTelefonoBarbero.Text = barberoSeleccionado.telefono;
            }
            else
            {
                barberoSeleccionado = null;
                LimpiarCampos();
            }
        };

        cmbAdministradores.SelectedIndexChanged += (s, e) =>
        {
            var adminSeleccionado = cmbAdministradores.SelectedItem as Usuario;
            if (adminSeleccionado != null && adminSeleccionado.idUsuario != 0)
            {
                adminSeleccionadoId = adminSeleccionado.idUsuario;
                txtNombreAdmin.Text = adminSeleccionado.nombre;
                txtApellidoAdmin.Text = adminSeleccionado.apellido;
                txtCorreoAdmin.Text = adminSeleccionado.correoElectronico;
                txtTelefonoAdmin.Text = adminSeleccionado.telefono;
            }
            else
            {
                adminSeleccionado = null;
                LimpiarCampos();
            }
        };
    }

    #region Barberos
    private async void cargarBarberos()
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
                    var listaBarberos = new List<Usuario> { new Usuario { idUsuario = 0, nombre = "     " } };
                    listaBarberos.AddRange(res.barberos);
                    cmbBarberos.ItemsSource = listaBarberos;
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron barberos.", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    private async void btnAgregarBarbero_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarUsuario(txtNombreBarbero.Text, txtApellidoBarbero.Text, txtCorreoBarbero.Text, txtTelefonoBarbero.Text, txtPasswordBarbero.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error", string.Join("\n", errores), "Aceptar");
            return;
        }

        try
        {
            ReqInsertarBarbero req = new ReqInsertarBarbero();
            req.usuario = new Usuario();
            req.usuario.nombre = txtNombreBarbero.Text;
            req.usuario.apellido = txtApellidoBarbero.Text;
            req.usuario.correoElectronico = txtCorreoBarbero.Text;
            req.usuario.telefono = txtTelefonoBarbero.Text;
            req.usuario.contraseña = txtPasswordBarbero.Text;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "barbero/insertar", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResInsertarBarbero res = JsonConvert.DeserializeObject<ResInsertarBarbero>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "Barbero agregado correctamente, el barbero debe activar su cuenta y cambiar la contraseña", "Aceptar");
                    cargarBarberos();
                    LimpiarCampos();
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

    private async void btnActualizarBarbero_Clicked(object sender, EventArgs e)
    {
        if (barberoSeleccionadoId == 0)
        {
            await DisplayAlert("Error", "Debe seleccionar un barbero para actualizar.", "Aceptar");
            return;
        }

        var errores = ValidarUsuario(txtNombreBarbero.Text, txtApellidoBarbero.Text, txtCorreoBarbero.Text, txtTelefonoBarbero.Text, txtPasswordBarbero.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error", string.Join("\n", errores), "Aceptar");
            return;
        }

        try
        {
            ReqActualizarBarbero req = new ReqActualizarBarbero();
            req.usuario = new Usuario();
            req.usuario.idUsuario = barberoSeleccionadoId.Value;
            req.usuario.nombre = txtNombreBarbero.Text;
            req.usuario.apellido = txtApellidoBarbero.Text;
            req.usuario.correoElectronico = txtCorreoBarbero.Text;
            req.usuario.telefono = txtTelefonoBarbero.Text;
            req.usuario.contraseña = txtPasswordBarbero.Text;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "barbero/actualizar", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResActualizarBarbero res = JsonConvert.DeserializeObject<ResActualizarBarbero>(responseContent);
                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "Barbero actualizado correctamente.", "Aceptar");
                    cargarBarberos();
                    LimpiarCampos();
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

    private async void btnEliminarBarbero_Clicked(object sender, EventArgs e)
    {
        if (barberoSeleccionadoId == 0)
        {
            await DisplayAlert("Error", "Debe seleccionar un barbero para eliminar.", "Aceptar");
            return;
        }
        bool confirmar = await DisplayAlert("Confirmar", "¿Está seguro que desea eliminar este barbero?", "Sí, eliminar", "Cancelar");
        if (confirmar)
        {
            try
            {
                ReqEliminarBarbero req = new ReqEliminarBarbero();
                req.idBarbero = barberoSeleccionadoId.Value;

                HttpResponseMessage respuestaHttp = new HttpResponseMessage();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync(App.API_URL + "barbero/eliminar", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    ResEliminarBarbero res = JsonConvert.DeserializeObject<ResEliminarBarbero>(responseContent);

                    if (res.resultado)
                    {
                        await DisplayAlert("Éxito", "Barbero eliminado correctamente.", "Aceptar");
                        cargarBarberos();
                        LimpiarCampos();
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

    #endregion

    #region Administradores
    private async void cargarAdmins()
    {
        try
        {
            ReqListarAdmins req = new ReqListarAdmins();
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "admin/listar", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResListarAdmins res = JsonConvert.DeserializeObject<ResListarAdmins>(responseContent);

                if (res.resultado)
                {
                    var listaAdmins = new List<Usuario> { new Usuario { idUsuario = 0, nombre = "     " } };
                    listaAdmins.AddRange(res.admins);
                    cmbAdministradores.ItemsSource = listaAdmins;
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron administradores.", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    private async void btnAgregarAdmin_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarUsuario(txtNombreAdmin.Text, txtApellidoAdmin.Text, txtCorreoAdmin.Text, txtTelefonoAdmin.Text, txtPasswordAdmin.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error", string.Join("\n", errores), "Aceptar");
            return;
        }

        try
        {
            ReqInsertarAdmin req = new ReqInsertarAdmin();
            req.usuario = new Usuario();
            req.usuario.nombre = txtNombreAdmin.Text;
            req.usuario.apellido = txtApellidoAdmin.Text;
            req.usuario.correoElectronico = txtCorreoAdmin.Text;
            req.usuario.telefono = txtTelefonoAdmin.Text;
            req.usuario.contraseña = txtPasswordAdmin.Text;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "admin/insertar", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResInsertarAdmin res = JsonConvert.DeserializeObject<ResInsertarAdmin>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "Admin agregado correctamente, el admin debe activar su cuenta y cambiar la contraseña", "Aceptar");
                    cargarBarberos();
                    LimpiarCampos();
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

    private async void btnActualizarAdmin_Clicked(object sender, EventArgs e)
    {
        if (adminSeleccionadoId == 0)
        {
            await DisplayAlert("Error", "Debe seleccionar un administrador para actualizar.", "Aceptar");
            return;
        }

        var errores = ValidarUsuario(txtNombreAdmin.Text, txtApellidoAdmin.Text, txtCorreoAdmin.Text, txtTelefonoAdmin.Text, txtPasswordAdmin.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error", string.Join("\n", errores), "Aceptar");
            return;
        }

        try
        {
            ReqActualizarAdmin req = new ReqActualizarAdmin();
            req.usuario = new Usuario();
            req.usuario.idUsuario = adminSeleccionadoId.Value;
            req.usuario.nombre = txtNombreAdmin.Text;
            req.usuario.apellido = txtApellidoAdmin.Text;
            req.usuario.correoElectronico = txtCorreoAdmin.Text;
            req.usuario.telefono = txtTelefonoAdmin.Text;
            req.usuario.contraseña = txtPasswordAdmin.Text;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "admin/actualizar", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResActualizarAdmin res = JsonConvert.DeserializeObject<ResActualizarAdmin>(responseContent);
                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "Administrador actualizado correctamente.", "Aceptar");
                    cargarAdmins();
                    LimpiarCampos();
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

    private async void btnEliminarAdmin_Clicked(object sender, EventArgs e)
    {
        if (adminSeleccionadoId == 0)
        {
            await DisplayAlert("Error", "Debe seleccionar un administrador para eliminar.", "Aceptar");
            return;
        }
        bool confirmar = await DisplayAlert("Confirmar", "¿Está seguro que desea eliminar este administrador?", "Sí, eliminar", "Cancelar");
        if (confirmar)
        {
            try
            {
                ReqEliminarAdmin req = new ReqEliminarAdmin();
                req.idAdmin = adminSeleccionadoId.Value;

                HttpResponseMessage respuestaHttp = new HttpResponseMessage();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync(App.API_URL + "admin/eliminar", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    ResEliminarAdmin res = JsonConvert.DeserializeObject<ResEliminarAdmin>(responseContent);

                    if (res.resultado)
                    {
                        await DisplayAlert("Éxito", "Administrador eliminado correctamente.", "Aceptar");
                        cargarAdmins();
                        LimpiarCampos();
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

    #endregion
    private List<string> ValidarUsuario(string nombre, string apellido, string correo, string telefono, string password)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            errores.Add("El campo 'Nombre' es obligatorio. Por favor ingrese el nombre del usuario.");
        }
        if (string.IsNullOrWhiteSpace(apellido))
        {
            errores.Add("El campo 'Apellido' es obligatorio. Por favor ingrese el apellido del usuario.");
        }
        if (string.IsNullOrWhiteSpace(correo))
        {
            errores.Add("Debe proporcionar un correo electrónico para registrar al usuario.");
        }
        else if (!helpers.EsCorreoValido(correo))
        {
            errores.Add("El correo electrónico ingresado no tiene un formato válido. " +
                      "Por favor ingrese una dirección de correo válida como: ejemplo@dominio.com");
        }
        if (string.IsNullOrWhiteSpace(telefono))
        {
            errores.Add("Se requiere un número de teléfono para completar el registro.");
        }
        else if (telefono.Length != 8 || !telefono.All(char.IsDigit))
        {
            errores.Add("El número de teléfono debe contener exactamente 8 dígitos numéricos. " +
                       "No incluya guiones, espacios ni otros caracteres.");
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            errores.Add("Debe establecer una contraseña para el usuario.");
        }
        else if (!helpers.EsPasswordSeguro(password))
        {
            errores.Add("La contraseña no cumple con los requisitos de seguridad. Debe incluir:\n" +
                       "- Mínimo 8 caracteres\n" +
                       "- Al menos 1 letra mayúscula\n" +
                       "- Al menos 1 letra minúscula\n" +
                       "- Al menos 1 número\n" +
                       "- Al menos 1 carácter especial (ej: !@#$%^&*)");
        }

        return errores;
    }
    private void LimpiarCampos()
    {

        barberoSeleccionadoId = null;
        cmbBarberos.SelectedIndex = -1;
        txtNombreBarbero.Text = "";
        txtApellidoBarbero.Text = "";
        txtCorreoBarbero.Text = "";
        txtTelefonoBarbero.Text = "";
        txtPasswordBarbero.Text = "";

        adminSeleccionadoId = null;
        cmbAdministradores.SelectedIndex = -1;
        txtNombreAdmin.Text = "";
        txtApellidoAdmin.Text = "";
        txtCorreoAdmin.Text = "";
        txtTelefonoAdmin.Text = "";
        txtPasswordAdmin.Text = "";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LimpiarCampos();
        cargarBarberos();
        cargarAdmins();
    }
}