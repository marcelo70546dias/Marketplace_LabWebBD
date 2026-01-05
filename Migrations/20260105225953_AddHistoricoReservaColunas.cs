using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace_LabWebBD.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricoReservaColunas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Data_Resposta",
                table: "Historico_Reserva",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motivo_Recusa",
                table: "Historico_Reserva",
                type: "nvarchar(max)",
                nullable: true);

            // Colunas de Filtro_Favorito já existem na base de dados, comentadas para evitar erro
            // migrationBuilder.AddColumn<string>(name: "Cor", table: "Filtro_Favorito", ...);
            // migrationBuilder.AddColumn<int>(name: "Lotacao_Max", table: "Filtro_Favorito", ...);
            // migrationBuilder.AddColumn<int>(name: "Lotacao_Min", table: "Filtro_Favorito", ...);
            // migrationBuilder.AddColumn<int>(name: "Quilometragem_Min", table: "Filtro_Favorito", ...);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data_Resposta",
                table: "Historico_Reserva");

            migrationBuilder.DropColumn(
                name: "Motivo_Recusa",
                table: "Historico_Reserva");

            // Colunas de Filtro_Favorito não serão removidas pois já existiam antes
        }
    }
}
