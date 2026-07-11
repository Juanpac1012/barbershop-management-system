namespace MauiKit.Views.PanelGeneral;

public partial class frmInicio : BasePage
{
    public frmInicio()
	{
		InitializeComponent();
	}

    private void btnReservar_Clicked(object sender, EventArgs e)
    {
        if (Parent is TabbedPage tabbedPage)
        {
            var AgendarCitapage = tabbedPage.Children.FirstOrDefault(p => p.Title == "Agendar Cita");

            if (AgendarCitapage != null)
            {
                tabbedPage.CurrentPage = AgendarCitapage;
            }
        }

    }

    private void btnServicios_Clicked(object sender, EventArgs e)
    {
        if (Parent is TabbedPage tabbedPage)
        {
            var serviciosPage = tabbedPage.Children.FirstOrDefault(p => p.Title == "Servicios");

            if (serviciosPage != null)
            {
                tabbedPage.CurrentPage = serviciosPage;
            }
        }

    }
}