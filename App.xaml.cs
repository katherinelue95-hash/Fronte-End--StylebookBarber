using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp
{
    public partial class App : Application
    {
        public App(IServiceProvider services)
        {
            InitializeComponent();
            ServiceHelper.Initialize(services);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}