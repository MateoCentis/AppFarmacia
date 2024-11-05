namespace AppFarmaciaWebAPI.ModelsDTO;

public class ArticuloEnCompraDTO
{
    public int IdArticuloCompra { get; set; }

    public int Cantidad { get; set; }

    public int IdArticulo { get; set; }

    public int IdCompra { get; set; }

    public string? MotivoCompra { get; set; }
}
