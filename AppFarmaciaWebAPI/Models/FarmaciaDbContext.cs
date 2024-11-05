using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AppFarmaciaWebAPI.Models;
using AutoMapper;
using AppFarmaciaWebAPI.ModelsDTO;

namespace AppFarmaciaWebAPI.Models
{
    public partial class FarmaciaDbContext : DbContext
    {
        public FarmaciaDbContext()
        {
        }

        public FarmaciaDbContext(DbContextOptions<FarmaciaDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Articulo> Articulos { get; set; }
        public virtual DbSet<ArticuloEnVenta> ArticulosEnVenta { get; set; }
        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Precio> Precios { get; set; }
        public virtual DbSet<Privilegio> Privilegios { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Vencimiento> Vencimientos { get; set; }
        public virtual DbSet<Venta> Ventas { get; set; }
        public virtual DbSet<Compra> Compras { get; set; }
        public virtual DbSet<Faltante> Faltantes { get; set; }
        public virtual DbSet<ArticuloEnCompra> ArticulosEnCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articulo>(entity =>
            {
                entity.HasKey(e => e.IdArticulo);

                entity.ToTable("ARTICULO");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Descripcion).IsUnicode(false);
                entity.Property(e => e.Marca)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Articulos)
                    .HasForeignKey(d => d.IdCategoria)
                    .HasConstraintName("FK_ARTICULO_CATEGORIA");
            });

            modelBuilder.Entity<ArticuloEnVenta>(entity =>
            {
                entity.HasKey(e => e.IdArticuloVenta);

                entity.ToTable("ARTICULO_EN_VENTA");

                entity.Property(e => e.Precio).HasColumnType("decimal(16, 2)");

                entity.HasOne(d => d.IdArticuloNavigation).WithMany(p => p.ArticulosEnVenta) // Actualizado aquí
                    .HasForeignKey(d => d.IdArticulo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ARTICULO_EN_VENTA_ARTICULO");

                entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.ArticuloEnVenta)
                    .HasForeignKey(d => d.IdVenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ARTICULO_EN_VENTA_VENTA");
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.IdCategoria);

                entity.ToTable("CATEGORIA");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Precio>(entity =>
            {
                entity.HasKey(e => e.IdPrecio);

                entity.ToTable("PRECIO");

                entity.Property(e => e.Fecha).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Valor).HasColumnType("decimal(16, 2)");

                entity.HasOne(d => d.IdArticuloNavigation).WithMany(p => p.Precios)
                    .HasForeignKey(d => d.IdArticulo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PRECIO_ARTICULO");
            });

            modelBuilder.Entity<Privilegio>(entity =>
            {
                entity.HasKey(e => e.IdPrivilegio);

                entity.ToTable("PRIVILEGIO");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(e => e.IdStock);

                entity.ToTable("STOCK");

                entity.Property(e => e.Fecha)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdArticuloNavigation).WithMany(p => p.Stocks)
                    .HasForeignKey(d => d.IdArticulo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_STOCK_ARTICULO");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.ToTable("USUARIO");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdPrivilegioNavigation).WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdPrivilegio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USUARIO_PRIVILEGIO");
            });

            modelBuilder.Entity<Vencimiento>(entity =>
            {
                entity.HasKey(e => e.IdVencimiento);

                entity.ToTable("VENCIMIENTO");

                entity.HasOne(d => d.IdArticuloNavigation).WithMany(p => p.Vencimientos)
                    .HasForeignKey(d => d.IdArticulo)
                    .HasConstraintName("FK_VENCIMIENTO_ARTICULO");
            });

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => e.IdVenta);

                entity.ToTable("VENTA");

                entity.Property(e => e.Fecha)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Compra>(entity =>
            {
                entity.HasKey(e => e.IdCompra);

                entity.ToTable("COMPRA");

                entity.Property(e => e.Fecha)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.Property(e => e.Proveedor)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

            });

            modelBuilder.Entity<Faltante>(entity =>
            {
                entity.HasKey(e => e.IdFaltante);

                entity.ToTable("FALTANTE");

                entity.HasOne(d => d.IdArticuloNavigation).WithMany(p => p.Faltantes)
                    .HasForeignKey(d => d.IdArticulo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FALTANTE_ARTICULO");
            });

            modelBuilder.Entity<ArticuloEnCompra>(entity =>
            {
                entity.HasKey(e => e.IdArticuloCompra);

                entity.ToTable("ARTICULO_EN_COMPRA");

                entity.Property(e => e.MotivoCompra)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdArticuloNavigation).WithMany(p => p.ArticulosEnCompra) // Actualizado aquí
                    .HasForeignKey(d => d.IdArticulo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ARTICULO_EN_COMPRA_ARTICULO");

                entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.ArticulosEnCompra)
                    .HasForeignKey(d => d.IdCompra)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ARTICULO_EN_COMPRA_COMPRA");
            });



            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
