using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp.Views;

public partial class BarberosPage : ContentPage
{
	public BarberosPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<BarberosViewModel>();
	}
}