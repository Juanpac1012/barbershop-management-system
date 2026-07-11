using Microsoft.Maui.Controls;
using Backend.Entidades;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MauiKit.Views.PanelGeneral;
using MauiKit.Views.PanelUsuario;

namespace MauiKit.ViewModels.BarberiaModel
{
    public class PerfilModel : BaseViewModel
    {
        public ICommand TapCommand { get; private set; }

        public List<MenuItems> _MenuItems = new List<MenuItems>();
        public List<MenuItems> MenuItems
        {
            get { return _MenuItems; }
            set { _MenuItems = value; }
        }

        public PerfilModel()
        {
            PopulateData();
            CommandInit();
        }

        void PopulateData()
        {
            MenuItems.Clear();
            MenuItems.Add(new MenuItems() { Title = "Editar Perfil", Icon = IonIcons.Edit, TargetType = typeof(frmGestionarPerfil) });
            MenuItems.Add(new MenuItems() { Title = "Cambiar Contraseña", Icon = IonIcons.LockCombination, TargetType = typeof(frmCambioContraPerfil1) });


            if (Sesion.usuario.idRol == EnumRoles.Cliente)
            {

                MenuItems.Add(new MenuItems() { Title = "Citas", Icon = IonIcons.AndroidTime, TargetType = typeof(frmCitasUsuario) });
                MenuItems.Add(new MenuItems() { Title = "Facturas", Icon = IonIcons.Card, TargetType = typeof(frmFacturasUsuario) });
            }
        }
        private void CommandInit()
        {
            TapCommand = new Command<MenuItems>(item =>
            {
                if (item.TargetType == null)
                    return;
                Application.Current.MainPage.Navigation.PushAsync(((Page)Activator.CreateInstance(item.TargetType)));
            });
        }

    }
}