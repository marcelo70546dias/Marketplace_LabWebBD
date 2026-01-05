using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace_LabWebBD.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToFiltroFavorito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quilometragem_Min",
                table: "Filtro_Favorito",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cor",
                table: "Filtro_Favorito",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Lotacao_Min",
                table: "Filtro_Favorito",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Lotacao_Max",
                table: "Filtro_Favorito",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quilometragem_Min",
                table: "Filtro_Favorito");

            migrationBuilder.DropColumn(
                name: "Cor",
                table: "Filtro_Favorito");

            migrationBuilder.DropColumn(
                name: "Lotacao_Min",
                table: "Filtro_Favorito");

            migrationBuilder.DropColumn(
                name: "Lotacao_Max",
                table: "Filtro_Favorito");
        }
    }
}
