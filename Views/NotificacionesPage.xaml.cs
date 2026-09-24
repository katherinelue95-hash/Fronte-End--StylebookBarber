using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class NotificacionesPage : ContentPage
{
	public NotificacionesPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<NotificacionesViewModel>();
	}
}