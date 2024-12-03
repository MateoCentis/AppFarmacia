using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFarmacia.Models;

public class Notificacion
{
    public int IdNotificacion { get; set; }
    public string Titulo { get; set; }
    public string Detalle { get; set; }
    public bool Leido { get; set; }
}
