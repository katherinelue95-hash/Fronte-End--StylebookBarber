using Microsoft.Extensions.Logging;
using StyleBookBarberApp.Services;
using StyleBookBarberApp.ViewModels;

namespace StyleBookBarberApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    // UI / texto general
                    fonts.AddFont("PlusJakartaSans.ttf", "PlusJakartaSans");
                    // Display / headlines editoriales
                    fonts.AddFont("BodoniModa.ttf", "BodoniModa");
                    // Iconografía del diseño (Material Symbols Outlined)
                    fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbolsOutlined");
                    // Respaldo
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(Helpers.ApiConstants.BaseUrl),
                Timeout = TimeSpan.FromSeconds(20)
            });

            // Servicios (backend)
            builder.Services.AddScoped<UsuariosServices>();
            builder.Services.AddScoped<CitasServices>();
            builder.Services.AddScoped<ResenasServices>();
            builder.Services.AddScoped<ServiciosServices>();
            builder.Services.AddScoped<BarberosServices>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<ServiciosViewModel>();
            builder.Services.AddTransient<HorariosViewModel>();
            builder.Services.AddTransient<ConfirmacionViewModel>();
            builder.Services.AddTransient<BarberosViewModel>();
            builder.Services.AddTransient<NotificacionesViewModel>();
            builder.Services.AddTransient<PerfilViewModel>();
            builder.Services.AddTransient<CitasViewModel>();
            builder.Services.AddTransient<AdminViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}