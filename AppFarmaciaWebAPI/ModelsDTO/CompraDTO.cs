namespace AppFarmaciaWebAPI.ModelsDTO;

public class CompraDTO
{
    public int IdCompra { get; set; }
    public DateTime Fecha { get; set; }
    public string? Proveedor { get; set; }
    public string? Descripcion { get; set; }
    public ICollection<ArticuloEnCompraDTO> ArticuloEnCompraDTO { get; set; } = [];
}
