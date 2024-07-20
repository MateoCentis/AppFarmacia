﻿namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class CategoriaDTO
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = null!;
        public ICollection<ArticuloDTO> ArticulosDTO { get; set; } = [];
    }
}
