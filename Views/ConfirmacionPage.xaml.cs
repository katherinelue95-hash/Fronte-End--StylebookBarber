using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class ConfirmacionPage : ContentPage
{
	public ConfirmacionPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<ConfirmacionViewModel>();
	}
}