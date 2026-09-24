using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class ServiciosPage : ContentPage
{
	public ServiciosPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<ServiciosViewModel>();
	}
}