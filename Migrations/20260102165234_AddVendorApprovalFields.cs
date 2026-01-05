using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace_LabWebBD.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Data_Aprovacao_Vendedor",
                table: "AspNetUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ID_Aprovacao_Vendedor",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motivo_Rejeicao_Vendedor",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorApprovalStatus",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data_Aprovacao_Vendedor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ID_Aprovacao_Vendedor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Motivo_Rejeicao_Vendedor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VendorApprovalStatus",
                table: "AspNetUsers");
        }
    }
}
