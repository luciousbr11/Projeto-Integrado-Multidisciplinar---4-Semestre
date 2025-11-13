using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoChamadosAI_Web.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRespostaIA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RespostaIA",
                table: "Chamados",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RespostaIA",
                table: "Chamados");
        }
    }
}
