using Microsoft.EntityFrameworkCore.Migrations;

namespace Ortho_matic.Migrations
{
    public partial class addRegionInClinic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Clinics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_RegionId",
                table: "Clinics",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clinics_Regions_RegionId",
                table: "Clinics",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetDefault);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clinics_Regions_RegionId",
                table: "Clinics");

            migrationBuilder.DropIndex(
                name: "IX_Clinics_RegionId",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Clinics");
        }
    }
}
