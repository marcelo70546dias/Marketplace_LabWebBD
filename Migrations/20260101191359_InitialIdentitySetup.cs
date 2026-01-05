using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace_LabWebBD.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentitySetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tabelas do Identity e tabelas existentes já foram criadas manualmente
            // Esta migration apenas marca o estado inicial como aplicado
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Não remover tabelas pois já existiam antes desta migration
        }
    }
}
