﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppFarmaciaWebAPI.Models;

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

    public virtual DbSet<ArticuloFinal> ArticulosFinales { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Precio> Precios { get; set; }

    public virtual DbSet<Privilegio> Privilegios { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.IdArticulo);

            entity.ToTable("ARTICULO");

            entity.Property(e => e.IdArticulo).ValueGeneratedNever();
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

            entity.Property(e => e.IdArticuloVenta).ValueGeneratedNever();
            entity.Property(e => e.Precio).HasColumnType("decimal(16, 2)");

            entity.HasOne(d => d.IdArticuloFinalNavigation).WithMany(p => p.ArticuloEnVenta)
                .HasForeignKey(d => d.IdArticuloFinal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ARTICULO_EN_VENTA_ARTICULO_FINAL");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.ArticuloEnVenta)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ARTICULO_EN_VENTA_VENTA");
        });

        modelBuilder.Entity<ArticuloFinal>(entity =>
        {
            entity.HasKey(e => e.IdArticuloFinal);

            entity.ToTable("ARTICULO_FINAL");

            entity.Property(e => e.IdArticuloFinal).ValueGeneratedNever();

            entity.HasOne(d => d.IdArticuloNavigation).WithMany(p => p.ArticuloFinals)
                .HasForeignKey(d => d.IdArticulo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ARTICULO_FINAL_ARTICULO");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria);

            entity.ToTable("CATEGORIA");

            entity.Property(e => e.IdCategoria).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Precio>(entity =>
        {
            entity.HasKey(e => e.IdPrecio);

            entity.ToTable("PRECIO");

            entity.Property(e => e.IdPrecio).ValueGeneratedNever();
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

            entity.Property(e => e.IdPrivilegio).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.IdStock);

            entity.ToTable("STOCK");

            entity.Property(e => e.IdStock).ValueGeneratedNever();
            entity.Property(e => e.Fecha).HasColumnType("datetime");

            entity.HasOne(d => d.IdArticuloFinalNavigation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdArticuloFinal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_STOCK_ARTICULO_FINAL");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable("USUARIO");

            entity.Property(e => e.IdUsuario).ValueGeneratedNever();
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

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta);

            entity.ToTable("VENTA");

            entity.Property(e => e.IdVenta).ValueGeneratedNever();
            entity.Property(e => e.Fecha).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
