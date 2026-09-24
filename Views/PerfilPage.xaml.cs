using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class PerfilPage : ContentPage
{
	public PerfilPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<PerfilViewModel>();
	}
}