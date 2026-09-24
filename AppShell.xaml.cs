using StyleBookBarberApp.Views;

namespace StyleBookBarberApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("Horarios", typeof(HorariosPage));
            Routing.RegisterRoute("Confirmacion", typeof(ConfirmacionPage));
            Routing.RegisterRoute("Perfil", typeof(PerfilPage));
            Routing.RegisterRoute("Notificaciones", typeof(NotificacionesPage));
            Routing.RegisterRoute("Admin", typeof(AdminPage));
        }
    }
}