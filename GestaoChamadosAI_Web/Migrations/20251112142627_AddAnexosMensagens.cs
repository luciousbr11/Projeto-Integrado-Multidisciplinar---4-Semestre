using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoChamadosAI_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAnexosMensagens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnexosMensagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MensagemChamadoId = table.Column<int>(type: "int", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoArquivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnexosMensagens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnexosMensagens_MensagensChamados_MensagemChamadoId",
                        column: x => x.MensagemChamadoId,
                        principalTable: "MensagensChamados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnexosMensagens_MensagemChamadoId",
                table: "AnexosMensagens",
                column: "MensagemChamadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnexosMensagens");
        }
    }
}
