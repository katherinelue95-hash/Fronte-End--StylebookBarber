using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<DashboardViewModel>();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is DashboardViewModel vm)
			vm.RefrescarSesion();
	}
}