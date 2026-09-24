using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class HorariosPage : ContentPage
{
	public HorariosPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<HorariosViewModel>();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is HorariosViewModel vm)
			_ = vm.RecargarResenasAsync();
	}
}