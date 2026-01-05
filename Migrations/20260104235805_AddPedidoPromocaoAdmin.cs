using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace_LabWebBD.Migrations
{
    /// <inheritdoc />
    public partial class AddPedidoPromocaoAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PedidoPromocaoAdmin",
                columns: table => new
                {
                    ID_Pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Utilizador = table.Column<int>(type: "int", nullable: false),
                    Tipo_Utilizador_Atual = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Data_Pedido = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Estado = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Pendente"),
                    Data_Resposta = table.Column<DateTime>(type: "datetime", nullable: true),
                    ID_Admin_Resposta = table.Column<int>(type: "int", nullable: true),
                    Justificacao = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    Observacoes_Admin = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pedido_P__C5A30BC2", x => x.ID_Pedido);
                    table.ForeignKey(
                        name: "FK_PedidoPromocaoAdmin_AspNetUsers_ID_Admin_Resposta",
                        column: x => x.ID_Admin_Resposta,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PedidoPromocaoAdmin_AspNetUsers_ID_Utilizador",
                        column: x => x.ID_Utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoPromocaoAdmin_ID_Admin_Resposta",
                table: "PedidoPromocaoAdmin",
                column: "ID_Admin_Resposta");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoPromocaoAdmin_ID_Utilizador",
                table: "PedidoPromocaoAdmin",
                column: "ID_Utilizador");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidoPromocaoAdmin");
        }
    }
}
