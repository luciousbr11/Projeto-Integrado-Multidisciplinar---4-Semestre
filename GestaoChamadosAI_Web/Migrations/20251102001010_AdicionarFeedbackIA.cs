using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoChamadosAI_Web.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarFeedbackIA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFeedback",
                table: "Chamados",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FeedbackResolvido",
                table: "Chamados",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFeedback",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "FeedbackResolvido",
                table: "Chamados");
        }
    }
}
