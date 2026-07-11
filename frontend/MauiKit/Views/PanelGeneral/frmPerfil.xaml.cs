using Backend.Entidades;
using RGPopup.Maui.Services;
using MauiKit.Views.Acciones;
using MauiKit.ViewModels.BarberiaModel;
namespace MauiKit.Views.PanelGeneral;

public partial class frmPerfil : BasePage
{
	public frmPerfil()
	{
		InitializeComponent();
        CargarDatos();
        BindingContext = new PerfilModel();
    }
    private async void CerrarSesion_Clicked(object sender, EventArgs e) //Revisar
    {
        var popup = new AccionCerrarSesion();
        bool confirmado = false;

        popup.DecisionMade += (s, result) =>
        {
            confirmado = result;
        };

        try
        {
            await PopupNavigation.Instance.PushAsync(popup);

            // Esperar a que el popup se cierre
            while (PopupNavigation.Instance.PopupStack.Contains(popup))
            {
                await Task.Delay(50);
            }

            if (confirmado)
            {

                Sesion.usuario = null;
                Sesion.estado = EnumEstadoSesion.cerrada;
                await Navigation.PopToRootAsync();
                await Task.Delay(100);
                Application.Current.MainPage = new NavigationPage(new frmLogin()); //revisar
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
            Application.Current.MainPage = new NavigationPage(new frmLogin());
        }
    } 

    private void CargarDatos()
    {
        lblNombreApellido.Text = Sesion.usuario.nombre + " " + Sesion.usuario.apellido;
        lblCorreo.Text = Sesion.usuario.correoElectronico;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarDatos();
    }
}