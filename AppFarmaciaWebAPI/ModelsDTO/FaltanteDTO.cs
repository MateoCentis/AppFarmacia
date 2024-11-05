namespace AppFarmaciaWebAPI.ModelsDTO;

public class FaltanteDTO
{
    public int IdFaltante { get; set; }

    public DateTime Fecha { get; set; }

    public int CantidadFaltante { get; set; }

    public int IdArticulo { get; set; }
}
