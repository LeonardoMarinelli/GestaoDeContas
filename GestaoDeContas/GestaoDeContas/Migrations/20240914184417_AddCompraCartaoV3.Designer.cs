﻿// <auto-generated />
using System;
using GestaoDeContas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestaoDeContas.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240914184417_AddCompraCartaoV3")]
    partial class AddCompraCartaoV3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GestaoDeContas.Models.Cartao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Anotacoes")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Banco")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("character varying(120)");

                    b.Property<string>("FinalCartao")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)");

                    b.Property<decimal>("LimiteCartao")
                        .HasColumnType("numeric");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<decimal>("TotalGastos")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Cartoes");
                });

            modelBuilder.Entity("GestaoDeContas.Models.CompraCartao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CartaoId")
                        .HasColumnType("integer");

                    b.Property<string>("Categoria")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("DataCompra")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("NumeroParcelas")
                        .HasColumnType("integer");

                    b.Property<bool>("Parcelada")
                        .HasColumnType("boolean");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Valor")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("CartaoId");

                    b.ToTable("ComprasCartao");
                });

            modelBuilder.Entity("GestaoDeContas.Models.ContaPagar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Categoria")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("DataVencimento")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Valor")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("ContasPagar");
                });

            modelBuilder.Entity("GestaoDeContas.Models.ContaReceber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Categoria")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("DataRecebimento")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Valor")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("ContasReceber");
                });

            modelBuilder.Entity("GestaoDeContas.Models.CompraCartao", b =>
                {
                    b.HasOne("GestaoDeContas.Models.Cartao", "Cartao")
                        .WithMany()
                        .HasForeignKey("CartaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cartao");
                });
#pragma warning restore 612, 618
        }
    }
}