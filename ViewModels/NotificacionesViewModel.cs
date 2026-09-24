using System.Collections.ObjectModel;
using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.Models;

namespace StyleBookBarberApp.ViewModels;

/// <summary>Centro de notificaciones del usuario (ícono campana de los headers).</summary>
public partial class NotificacionesViewModel : BaseViewModel
{
    public ObservableCollection<NotificacionItem> Notificaciones { get; } = new();

    public NotificacionesViewModel()
    {
        Title = "Notificaciones";

        Notificaciones.Add(new NotificacionItem
        {
            Titulo = "Turno confirmado",
            Cuerpo = "Tu cita con Mateo Valenzuela el Mié 22 las 10:30 AM fue confirmada.",
            Hora = "Hace 5 min",
            Icono = IconGlyphs.EventAvailable,
            EsNueva = true,
        });
        Notificaciones.Add(new NotificacionItem
        {
            Titulo = "Recordatorio de cita",
            Cuerpo = "Te esperamos este viernes 24 de Octubre a las 4:30 PM con Carlos Mendoza.",
            Hora = "Hace 1 h",
            Icono = IconGlyphs.NotificationsActive,
            EsNueva = true,
        });
        Notificaciones.Add(new NotificacionItem
        {
            Titulo = "Nuevo servicio disponible",
            Cuerpo = "Ritual Spa Capilar: tratamiento de 45 minutos para una recuperación profunda.",
            Hora = "Ayer",
            Icono = IconGlyphs.Spa,
        });
        Notificaciones.Add(new NotificacionItem
        {
            Titulo = "Promoción exclusiva",
            Cuerpo = "Combo Corte & Barba a $40.00 este mes. agéndalo desde el catálogo.",
            Hora = "Hace 2 días",
            Icono = IconGlyphs.VerifiedUser,
        });
        Notificaciones.Add(new NotificacionItem
        {
            Titulo = "Pausa recomendada",
            Cuerpo = "Espacio de 15 minutos entre el servicio de Andrés y Gabriel.",
            Hora = "Hace 3 días",
            Icono = IconGlyphs.Timelapse,
        });
    }
}