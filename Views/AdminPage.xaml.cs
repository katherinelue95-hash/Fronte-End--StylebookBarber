using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class AdminPage : ContentPage
{
	public AdminPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<AdminViewModel>();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is AdminViewModel vm)
			_ = vm.InicializarAsync();
	}
}