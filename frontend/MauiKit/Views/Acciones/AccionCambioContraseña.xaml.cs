using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;

namespace MauiKit.Views.Acciones;

public partial class AccionCambioContraseña : PopupPage
{
	public AccionCambioContraseña()
	{
		InitializeComponent();
	}
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}