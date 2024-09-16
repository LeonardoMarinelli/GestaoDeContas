﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoDeContas.Migrations
{
    /// <inheritdoc />
    public partial class AddCompraCartaoV5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinalCartao",
                table: "ComprasCartao",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
