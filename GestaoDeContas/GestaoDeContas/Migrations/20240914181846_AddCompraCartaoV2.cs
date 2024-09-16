using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoDeContas.Migrations
{
    /// <inheritdoc />
    public partial class AddCompraCartaoV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "ComprasCartao",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ComprasCartao");
        }
    }
}
