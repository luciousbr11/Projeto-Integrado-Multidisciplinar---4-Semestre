using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoChamadosAI_Web.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposIA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoriaIA",
                table: "Chamados",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prioridade",
                table: "Chamados",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SugestaoIA",
                table: "Chamados",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriaIA",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "Prioridade",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "SugestaoIA",
                table: "Chamados");
        }
    }
}
