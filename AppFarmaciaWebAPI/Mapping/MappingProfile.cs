using AutoMapper;
using AppFarmaciaWebAPI.Models;
using AppFarmaciaWebAPI.ModelsDTO;

namespace AppFarmaciaWebAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo entre Articulo y ArticuloDTO
            CreateMap<Articulo, ArticuloDTO>()
                .ForMember(dest => dest.PreciosDTO, opt => opt.MapFrom(src => src.Precios))
                .ForMember(dest => dest.VencimientosDTO, opt => opt.MapFrom(src => src.Vencimientos))
                .ForMember(dest => dest.StocksDTO, opt => opt.MapFrom(src => src.Stocks))
                .ForMember(dest => dest.ArticulosEnVentaDTO, opt => opt.MapFrom(src => src.ArticulosEnVenta));

            CreateMap<ArticuloDTO, Articulo>()
                .ForMember(dest => dest.Precios, opt => opt.MapFrom(src => src.PreciosDTO))
                .ForMember(dest => dest.Vencimientos, opt => opt.MapFrom(src => src.VencimientosDTO))
                .ForMember(dest => dest.Stocks, opt => opt.MapFrom(src => src.StocksDTO))
                .ForMember(dest => dest.ArticulosEnVenta, opt => opt.MapFrom(src => src.ArticulosEnVentaDTO));

            // Mapeo entre ArticuloEnVenta y ArticuloEnVentaDTO
            CreateMap<ArticuloEnVenta, ArticuloEnVentaDTO>()
                .ForMember(dest => dest.NombreArticulo, opt => opt.MapFrom(src => src.IdArticuloNavigation.Nombre));

            // Mapeo entre ArticuloEnVentaDTO y ArticuloEnVenta
            CreateMap<ArticuloEnVentaDTO, ArticuloEnVenta>();

            // Mapeo entre Categoria y CategoriaDTO
            CreateMap<Categoria, CategoriaDTO>()
                .ForMember(dest => dest.ArticulosDTO, opt => opt.MapFrom(src => src.Articulos));

            CreateMap<CategoriaDTO, Categoria>()
                .ForMember(dest => dest.Articulos, opt => opt.MapFrom(src => src.ArticulosDTO));

            // Mapeo entre Precio y PrecioDTO
            CreateMap<Precio, PrecioDTO>().ReverseMap();

            // Mapeo entre Privilegio y PrivilegioDTO
            CreateMap<Privilegio, PrivilegioDTO>()
                .ForMember(dest => dest.UsuariosDTO, opt => opt.MapFrom(src => src.Usuarios));

            CreateMap<PrivilegioDTO, Privilegio>()
                .ForMember(dest => dest.Usuarios, opt => opt.MapFrom(src => src.UsuariosDTO));

            // Mapeo entre Stock y StockDTO
            CreateMap<Stock, StockDTO>().ReverseMap();

            // Mapeo entre Usuario y UsuarioDTO
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();

            // Mapeo entre Vencimiento y VencimientoDTO
            CreateMap<Vencimiento, VencimientoDTO>().ReverseMap();

            // Mapeo entre Venta y VentaDTO
            CreateMap<Venta, VentaDTO>()
                //.ForMember(dest => dest.ArticulosEnVentaDTO, opt => opt.MapFrom(src => src.ArticuloEnVenta))
                .ForMember(dest => dest.MontoTotal, opt => opt.MapFrom(src => src.ArticuloEnVenta.Sum(a => a.Precio * a.Cantidad)));

            CreateMap<VentaDTO, Venta>();
            //.ForMember(dest => dest.ArticuloEnVenta, opt => opt.MapFrom(src => src.ArticulosEnVentaDTO));
        }
    }
}