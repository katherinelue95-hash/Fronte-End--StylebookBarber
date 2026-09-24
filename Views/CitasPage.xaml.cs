using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class CitasPage : ContentPage
{
	public CitasPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<CitasViewModel>();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is CitasViewModel vm)
			_ = vm.RefrescarCitasAsync();
	}
}