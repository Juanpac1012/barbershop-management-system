using RGPopup.Maui.Pages;
using Backend.Entidades;
using MauiKit.Entidades;
using RGPopup.Maui.Services;
namespace MauiKit.Views.Acciones;

public partial class AccionBienvenida : PopupPage
{
	public AccionBienvenida()
	{
		InitializeComponent();
        lblWelcomeTitle.Text = "¡Bienvenido, " + Sesion.usuario.nombre;

        if (Sesion.usuario?.idRol == EnumRoles.Cliente)
        {
            lblWelcomeSubtitle.Text = "Tu experiencia de barbería está a punto de comenzar.";
        }
        else if (Sesion.usuario?.idRol == EnumRoles.Admin)
        {
            lblWelcomeSubtitle.Text = "Panel de administración listo para usar.";
        }
        else if (Sesion.usuario?.idRol == EnumRoles.Barbero)
        {
            lblWelcomeSubtitle.Text = "¡Listo para atender a tus clientes!";
        }
        else
        {
            lblWelcomeSubtitle.Text = "Has iniciado sesión correctamente.";
        }
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }

}