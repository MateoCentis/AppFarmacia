namespace AppFarmaciaWebAPI.Models
{
    public class Notificacion
    {
        public int IdNotificacion { get; set; }
        public string Titulo { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public bool Leido { get; set; }
        public bool Enviado { get; set; }

        public DateTime Fecha { get; set; }
    }
}
