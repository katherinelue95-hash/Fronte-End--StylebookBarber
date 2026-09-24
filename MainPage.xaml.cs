namespace StyleBookBarberApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Preferences.Default.ContainsKey("UserId"))
            {
                await Shell.Current.GoToAsync("//MainTab");
            }
            else
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}