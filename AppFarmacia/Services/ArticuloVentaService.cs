﻿using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class ArticuloVentaService
    {
        private List<ArticuloEnVenta>? ArticulosEnVenta = [];
        private readonly HttpClient httpClient;

        public ArticuloVentaService()
        {
            this.httpClient = new HttpClient();
        }

        // Te da la lista de artículos en venta dado un id de venta
        public async Task<List<ArticuloEnVenta>> GetArticulosEnVentasPorIdVenta(int idVenta)
        {
            // Si ya tengo ventas cargadas no llamo a la API
            if (this.ArticulosEnVenta?.Count > 0)
            {
                return ArticulosEnVenta;
            }

            // Obtengo respuesta           //Cambiar a localhost para usar en ambas PC's
            var respuesta = await httpClient.GetAsync($"http://LocalHost:83/api/ArticulosEnVenta/PorVentaId/{idVenta}");


            // Si la respuesta es exitosa
            if (respuesta.IsSuccessStatusCode)
                this.ArticulosEnVenta = await respuesta.Content.ReadFromJsonAsync<List<ArticuloEnVenta>>() ?? [];

            return this.ArticulosEnVenta!;
        }

        // Te da un artículo en venta por su id
        public async Task<List<ArticuloEnVenta>> GetArticulosEnVentaArticulo(int id)
        {
            var respuesta = await httpClient.GetAsync($"http://localhost:83/api/ArticulosEnVenta/Articulos/{id}");
            List<ArticuloEnVenta> aev_articulos = [];

            if (respuesta.IsSuccessStatusCode)
                aev_articulos = await respuesta.Content.ReadFromJsonAsync<List<ArticuloEnVenta>>() ?? [];

            return aev_articulos!;
        }

    }
}
