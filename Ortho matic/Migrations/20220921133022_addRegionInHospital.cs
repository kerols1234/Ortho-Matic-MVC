using Microsoft.EntityFrameworkCore.Migrations;

namespace Ortho_matic.Migrations
{
    public partial class addRegionInHospital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Hospitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_RegionId",
                table: "Hospitals",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_Regions_RegionId",
                table: "Hospitals",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetDefault);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_Regions_RegionId",
                table: "Hospitals");

            migrationBuilder.DropIndex(
                name: "IX_Hospitals_RegionId",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Hospitals");
        }
    }
}
