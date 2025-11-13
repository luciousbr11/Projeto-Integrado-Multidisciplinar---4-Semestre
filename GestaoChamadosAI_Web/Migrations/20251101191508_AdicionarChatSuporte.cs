using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoChamadosAI_Web.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarChatSuporte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Usuarios_UsuarioId",
                table: "Chamados");

            migrationBuilder.AddColumn<int>(
                name: "SuporteResponsavelId",
                table: "Chamados",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MensagensChamados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChamadoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LidaPorCliente = table.Column<bool>(type: "bit", nullable: false),
                    LidaPorSuporte = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensagensChamados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensagensChamados_Chamados_ChamadoId",
                        column: x => x.ChamadoId,
                        principalTable: "Chamados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MensagensChamados_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_SuporteResponsavelId",
                table: "Chamados",
                column: "SuporteResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChamados_ChamadoId",
                table: "MensagensChamados",
                column: "ChamadoId");

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChamados_UsuarioId",
                table: "MensagensChamados",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Usuarios_SuporteResponsavelId",
                table: "Chamados",
                column: "SuporteResponsavelId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Usuarios_UsuarioId",
                table: "Chamados",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Usuarios_SuporteResponsavelId",
                table: "Chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Usuarios_UsuarioId",
                table: "Chamados");

            migrationBuilder.DropTable(
                name: "MensagensChamados");

            migrationBuilder.DropIndex(
                name: "IX_Chamados_SuporteResponsavelId",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "SuporteResponsavelId",
                table: "Chamados");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Usuarios_UsuarioId",
                table: "Chamados",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
