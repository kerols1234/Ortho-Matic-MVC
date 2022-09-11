using Microsoft.EntityFrameworkCore.Migrations;

namespace Ortho_matic.Migrations
{
    public partial class addPhoneNumberToHospital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Hospitals",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Hospitals");
        }
    }
}
