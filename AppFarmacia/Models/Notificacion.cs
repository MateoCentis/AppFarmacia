using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFarmacia.Models;

public class Notificacion : ObservableObject
{
    public int IdNotificacion { get; set; }
    public string Titulo { get; set; } = null!;
    public string Detalle { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public bool Enviado { get; set; }

    private bool leido;
    public bool Leido
    {
        get => leido;
        set => SetProperty(ref leido, value);
    }
}
