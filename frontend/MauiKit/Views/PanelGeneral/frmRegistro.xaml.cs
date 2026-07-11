using Backend.Entidades;
using MauiKit.Entidades;
using Newtonsoft.Json;
using System.Text;
namespace MauiKit.Views.PanelGeneral;

public partial class frmRegistro : ContentPage
{
    private bool isPasswordVisible = true;
    private bool isPasswordConfirVisible = true;
    public frmRegistro()
	{
		InitializeComponent();
	}
    private async void btnRegistro_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarRegistro(txtNombre.Text, txtApellido.Text, txtTelefono.Text, txtCorreo.Text, txtContraseña.Text, txtConfirContraseña.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Datos incompletos", string.Join("\n", errores), "Aceptar");
            return;
        }
        try
        {
            ReqInsertarUsuario req = new ReqInsertarUsuario();
            req.usuario = new Usuario();
            req.usuario.nombre = txtNombre.Text;
            req.usuario.apellido = txtApellido.Text;
            req.usuario.correoElectronico = txtCorreo.Text;
            req.usuario.telefono = txtTelefono.Text;
            req.usuario.contraseña = txtContraseña.Text;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuarioCliente/insertar", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResInsertarUsuario res = new ResInsertarUsuario();
                res = JsonConvert.DeserializeObject<ResInsertarUsuario>(responseContent);

                if (res.resultado)
                {
                    Sesion.usuario = new Usuario();
                    Sesion.usuario.correoElectronico = req.usuario.correoElectronico;
                    await Navigation.PushAsync(new frmVerificacionCodigo());
                }
                else
                {
                    string mensajeErrores = string.Join("\n", res.listaErrores.Select(e => e.Message));
                    await DisplayAlert("Error de registro", mensajeErrores, "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    private async void btnVolverAtras_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void btnActivarCuenta_Clicked(object sender, EventArgs e)
    {
         await Navigation.PushAsync(new frmActivarCuentaCorreo());
    }

    private void btnAlternarContraseña_Clicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;
        txtContraseña.IsPassword = !isPasswordVisible;

        if (btnAlternarContraseña.ImageSource is FontImageSource fontImageSource)
        {
            fontImageSource.Glyph = isPasswordVisible ?
                MauiKitIcons.Eye :
                MauiKitIcons.EyeOff;
        }
    }

    private void btnAlternarContraseñaConfir_Clicked(object sender, EventArgs e)
    {
        isPasswordConfirVisible = !isPasswordConfirVisible;
        txtConfirContraseña.IsPassword = !isPasswordConfirVisible;

        if (btnAlternarContraseñaConfir.ImageSource is FontImageSource fontImageSource)
        {
            fontImageSource.Glyph = isPasswordConfirVisible ?
                MauiKitIcons.Eye :
                MauiKitIcons.EyeOff;
        }
    }

    private List<string> ValidarRegistro(string nombre, string apellido, string telefono, string correo, string contraseña, string confirContraseña)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            errores.Add("Por favor ingrese su nombre.");
        }
        if (string.IsNullOrWhiteSpace(apellido))
        {
            errores.Add("Por favor ingrese su apellido.");
        }
        if (string.IsNullOrWhiteSpace(telefono))
        {
            errores.Add("Por favor ingrese su número telefónico.");
        }
        else if (telefono.Length != 8 || !telefono.All(char.IsDigit))
        {
            errores.Add("El teléfono debe tener 8 dígitos numéricos (sin espacios ni guiones).");
        }
        if (string.IsNullOrWhiteSpace(correo))
        {
            errores.Add("Por favor ingrese su correo electrónico.");
        }
        else if (!helpers.EsCorreoValido(correo))
        {
            errores.Add("Ingrese un correo electrónico válido (ejemplo: usuario@dominio.com).");
        }
        if (string.IsNullOrWhiteSpace(contraseña))
        {
            errores.Add("Por favor cree una contraseña.");
        }
        else if (!helpers.EsPasswordSeguro(contraseña))
        {
            errores.Add("La contraseña debe contener:\n- 8+ caracteres\n- 1 mayúscula\n- 1 minúscula\n- 1 número\n- 1 carácter especial");
        }
        if (confirContraseña != contraseña)
        {
            errores.Add("Las contraseñas no coinciden. Verifique que sean iguales en ambos campos.");
        }

        return errores;
    }
}