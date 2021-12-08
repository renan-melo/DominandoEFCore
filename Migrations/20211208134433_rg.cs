using Microsoft.EntityFrameworkCore.Migrations;

namespace DominandoEFCore.Migrations
{
    public partial class rg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RG",
                table: "Funcionarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RG",
                table: "Funcionarios");
        }
    }
}
