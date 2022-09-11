using Microsoft.EntityFrameworkCore.Migrations;

namespace Ortho_matic.Migrations
{
    public partial class alterTableDoctor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "Doctors");

            migrationBuilder.AddColumn<int>(
                name: "DoctorSpecialty",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorSpecialty",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
