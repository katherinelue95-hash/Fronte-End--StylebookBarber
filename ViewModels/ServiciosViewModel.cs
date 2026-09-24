using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.Models;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// Catálogo de Servicios: todas las fotografías del estudio convertidas en
/// tarjetas seleccionables con precio, calificación y botón "Seleccionar".
/// </summary>
public partial class ServiciosViewModel : BaseViewModel
{
    private sealed record DatosServicio(
        string Imagen, string Titulo, string Categoria, string Icono,
        string Precio, string Duracion, string Rating, string Descripcion,
        bool EsVip, bool EsTendencia, bool EsRitual);

    public IReadOnlyList<ServicioItem> Servicios { get; }
    public IReadOnlyList<FiltroCategoria> Categorias { get; }
    public ObservableCollection<ServicioItem> Visibles { get; } = new();

    [ObservableProperty]
    private bool haySeleccion;

    [ObservableProperty]
    private string elegidoTitulo = string.Empty;

    [ObservableProperty]
    private string elegidoPrecio = string.Empty;

    [ObservableProperty]
    private string busqueda = string.Empty;

    private string _categoriaFiltro = "cortes";

    public ServiciosViewModel()
    {
        Title = "Catálogo";

        Servicios = DatosServicios
            .Select(d => new ServicioItem
            {
                Imagen = d.Imagen,
                Titulo = d.Titulo,
                Categoria = d.Categoria,
                Icono = d.Icono,
                Precio = d.Precio,
                Duracion = d.Duracion,
                Rating = d.Rating,
                Descripcion = d.Descripcion,
                EsVip = d.EsVip,
                EsTendencia = d.EsTendencia,
                EsRitual = d.EsRitual,
            })
            .ToList();

        var cortes = Servicios.Count(s => s.Categoria == "cortes");
        var barba = Servicios.Count(s => s.Categoria == "barba");
        var combos = Servicios.Count(s => s.Categoria == "combos");

        Categorias = new List<FiltroCategoria>
        {
            new() { Nombre = "CORTES", Icono = IconGlyphs.Scissors, Count = cortes, EstaActivo = true },
            new() { Nombre = "BARBA", Icono = IconGlyphs.Face, Count = barba },
            new() { Nombre = "COMBOS", Icono = IconGlyphs.AutoAwesome, Count = combos },
            new() { Nombre = "VER TODOS", Icono = string.Empty, Count = 0 },
        };

        AplicarFiltro(_categoriaFiltro);
    }

    [RelayCommand]
    private void Seleccionar(ServicioItem item)
    {
        foreach (var s in Servicios)
            s.EsSeleccionado = ReferenceEquals(s, item) && !s.EsSeleccionado;

        var elegido = Servicios.FirstOrDefault(s => s.EsSeleccionado);
        HaySeleccion = elegido is not null;
        ElegidoTitulo = elegido?.Titulo ?? string.Empty;
        ElegidoPrecio = elegido?.Precio ?? string.Empty;
    }

    [RelayCommand]
    private void Filtrar(string nombre)
    {
        var categoria = nombre switch
        {
            "CORTES" => "cortes",
            "BARBA" => "barba",
            "COMBOS" => "combos",
            _ => "todos",
        };

        foreach (var c in Categorias)
            c.EstaActivo = c.Nombre == nombre;
        AplicarFiltro(categoria);
    }

    private void AplicarFiltro(string categoria)
    {
        Visibles.Clear();
        var termino = Busqueda?.Trim() ?? string.Empty;
        foreach (var s in Servicios.Where(s =>
            (categoria == "todos" || s.Categoria == categoria) &&
            (termino.Length == 0 || s.Titulo.Contains(termino, StringComparison.OrdinalIgnoreCase))))
            Visibles.Add(s);
    }

    partial void OnBusquedaChanged(string value) => AplicarFiltro(_categoriaFiltro);

    [RelayCommand]
    private async Task ContinuarAsync()
    {
        var elegido = Servicios.FirstOrDefault(s => s.EsSeleccionado);
        Preferences.Default.Set("ReservaServicio", elegido?.Titulo ?? "Corte Degradado Clásico");
        Preferences.Default.Set("ReservaPrecio", elegido?.Precio ?? "$25.00");
        Preferences.Default.Set("ReservaDuracion", elegido?.Duracion ?? "40 min");
        Preferences.Default.Set("ReservaServicioCategoria", elegido?.Categoria ?? "cortes");
        await Shell.Current.GoToAsync("Horarios");
    }

    // ===== Las 25 fotografías del estudio como servicios con precio =====
    private static readonly DatosServicio[] DatosServicios =
    [
        // ---- Cortes ----
        new("cabello1.jpg", "Corte Degradado Clásico", "cortes", IconGlyphs.Scissors,
            "$25.00", "40 min", "4.9", "Desvanecido milimétrico con navaja caliente, lavado purificante y fijación editorial personalizada.",
            false, false, false),
        new("cabello2.jpg", "Corte Texturizado Moderno", "cortes", IconGlyphs.Scissors,
            "$28.00", "35 min", "4.8", "Tijera dentada y acabado con cera mate de arcilla volcánica para volumen, textura y movimiento natural.",
            false, true, false),
        new("cabello3.jpg", "Combo Corte & Barba Completo", "combos", IconGlyphs.AutoAwesome,
            "$40.00", "60 min", "4.9", "La sesión definitiva: diseño capilar a elección, perfilado total de barba, exfoliación y bebida de cortesía.",
            true, false, false),
        new("cabello4.jpg", "Diseño Capilar Artístico", "cortes", IconGlyphs.Scissors,
            "$22.00", "30 min", "4.7", "Trazo creativo con máquina y navaja: patrones, sombreados y degradados de autor.",
            false, false, false),
        new("cabello5.jpg", "Corte Ejecutivo", "cortes", IconGlyphs.Scissors,
            "$27.00", "35 min", "4.8", "Look de oficina impecable: corte estructurado, toques de tijera y fijación natural de larga duración.",
            false, false, false),
        new("cabello6.jpg", "Fade Clásico", "cortes", IconGlyphs.Scissors,
            "$23.00", "35 min", "4.7", "Degradado uniforme de bajo a alto con transición limpia y perfilado con navaja.",
            false, false, false),
        new("cabello7.jpg", "Estilo Moderno", "cortes", IconGlyphs.Scissors,
            "$24.00", "30 min", "4.6", "Corte contemporáneo con textura al frente, temple suave y acabado mate.",
            false, false, false),
        new("cabello8.jpg", "Acabado Fino", "cortes", IconGlyphs.Scissors,
            "$21.00", "30 min", "4.6", "Perfilado detallado con tijera de precisión, contorno limpio y sellado con loción.",
            false, false, false),

        // ---- Barba ----
        new("barba1.jpg", "Ritual de Toalla Caliente", "barba", IconGlyphs.Face,
            "$18.00", "25 min", "4.9", "Doble toalla aromatizada al vapor para abrir el poro, aceite nutriente y masaje facial.",
            false, false, true),
        new("barba2.png", "Perfilado y Afeitado de Barba", "barba", IconGlyphs.Face,
            "$20.00", "30 min", "4.8", "Ritual con doble toalla aromatizada, aceites botánicos y afeitado tradicional a navaja japonesa.",
            false, false, true),
        new("barba3.jpg", "Afeitado a Navaja Tradicional", "barba", IconGlyphs.Face,
            "$20.00", "30 min", "4.8", "Afeitado cerrado a navaja con crema de afeitar en caliente y bálsamo post-afeitado.",
            false, false, true),
        new("barba4.jpg", "Barba Perfilada", "barba", IconGlyphs.Face,
            "$15.00", "20 min", "4.6", "Contorno definido, recorte de longitud y alineado preciso de mejillas y cuello.",
            false, false, false),
        new("barba5.jpg", "Vapor y Loción Astringente", "barba", IconGlyphs.Face,
            "$16.00", "25 min", "4.7", "Vaporización profunda, limpieza de poros y aplicación de loción astringente calmante.",
            false, false, true),
        new("barba6.jpg", "Lineado Preciso de Barba", "barba", IconGlyphs.Face,
            "$17.00", "20 min", "4.7", "Geometría perfecta con navaja: líneas limpias que realzan el rostro.",
            false, false, false),
        new("barba7.jpg", "Perfilado y Vapor", "barba", IconGlyphs.Face,
            "$19.00", "25 min", "4.6", "Combinación de perfilado con vapor previo para un corte de barba más suave.",
            false, false, true),
        new("barba8.jpg", "Afeitado Tradicional", "barba", IconGlyphs.Face,
            "$19.00", "25 min", "4.7", "Ritual de afeitado clásico paso a paso: pre-afeitado, navaja y bálsamo reparador.",
            false, false, true),
        new("barba9.jpg", "Acabado Editorial", "barba", IconGlyphs.Face,
            "$18.00", "20 min", "4.8", "Estilización de barba con cera y peinado de autor, listo para portada.",
            false, false, false),
        new("barba10.jpg", "Pieza del Día", "barba", IconGlyphs.Face,
            "$18.00", "20 min", "4.7", "Servicio sorpresa de la casa: el perfilado estrella elegido por el barbero del día.",
            false, false, false),
        new("barba11.jpg", "Perfilado a Cepillo", "barba", IconGlyphs.Face,
            "$16.00", "20 min", "4.6", "Recorte con cepillado previo para acomodar el pelo y definir una forma natural.",
            false, false, false),
        new("barba12.jpg", "Ritual Barba Premium", "barba", IconGlyphs.Face,
            "$22.00", "30 min", "4.9", "Experiencia completa: vapor, mascarilla de arcilla, afeitado a navaja y aceite premium.",
            true, false, true),
        new("barba13.jpg", "Diseño de Barba Creativo", "barba", IconGlyphs.Face,
            "$18.00", "25 min", "4.7", "Diseño personalizado sobre la barba: degradados, cortes con máquina y estilo único.",
            false, false, false),
        new("barba14.jpg", "Afeitado Clásico", "barba", IconGlyphs.Face,
            "$17.00", "20 min", "4.6", "Afeitado a navaja con técnica clásica de barbería y terminado con agua fría.",
            false, false, true),
        new("barba15.jpg", "Textura y Estilo", "barba", IconGlyphs.Face,
            "$15.00", "20 min", "4.5", "Aplicación de pomada para barba con acabado natural y peinado definido.",
            false, false, false),
        new("barba16.jpg", "Línea Editorial", "barba", IconGlyphs.Face,
            "$18.00", "20 min", "4.7", "Perfilado de precisión con acabado editorial e hidratación final.",
            false, false, false),
        new("barba17.jpg", "Sesión Maestro", "barba", IconGlyphs.Face,
            "$24.00", "35 min", "4.9", "Sesión completa con el maestro barbero: asesoría, afeitado ritual y styling final.",
            true, false, true),
    ];
}