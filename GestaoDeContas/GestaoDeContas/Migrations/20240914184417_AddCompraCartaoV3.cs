using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoDeContas.Migrations
{
    /// <inheritdoc />
    public partial class AddCompraCartaoV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumeroParcelas",
                table: "ComprasCartao",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Parcelada",
                table: "ComprasCartao",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LimiteCartao",
                table: "Cartoes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalGastos",
                table: "Cartoes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroParcelas",
                table: "ComprasCartao");

            migrationBuilder.DropColumn(
                name: "Parcelada",
                table: "ComprasCartao");

            migrationBuilder.DropColumn(
                name: "LimiteCartao",
                table: "Cartoes");

            migrationBuilder.DropColumn(
                name: "TotalGastos",
                table: "Cartoes");
        }
    }
}
