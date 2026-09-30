using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<RegisterViewModel>();
	}
}