using Microsoft.EntityFrameworkCore.Migrations;

namespace Ortho_matic.Migrations
{
    public partial class editPhoneNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Clinics");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Hospitals",
                newName: "Phone3");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Clinics",
                newName: "Phone1");

            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "Visitations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone1",
                table: "Hospitals",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone2",
                table: "Hospitals",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone2",
                table: "Clinics",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone3",
                table: "Clinics",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comment",
                table: "Visitations");

            migrationBuilder.DropColumn(
                name: "Phone1",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Phone2",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Phone2",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "Phone3",
                table: "Clinics");

            migrationBuilder.RenameColumn(
                name: "Phone3",
                table: "Hospitals",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Phone1",
                table: "Clinics",
                newName: "PhoneNumber");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Hospitals",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Hospitals",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Doctors",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Clinics",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Clinics",
                type: "float",
                nullable: true);
        }
    }
}
