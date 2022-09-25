using Microsoft.EntityFrameworkCore.Migrations;

namespace Ortho_matic.Migrations
{
    public partial class alterTableVisitationAddClinicHospitalFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "Visitations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "Visitations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visitations_ClinicId",
                table: "Visitations",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitations_HospitalId",
                table: "Visitations",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Visitations_Clinics_ClinicId",
                table: "Visitations",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visitations_Hospitals_HospitalId",
                table: "Visitations",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visitations_Clinics_ClinicId",
                table: "Visitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitations_Hospitals_HospitalId",
                table: "Visitations");

            migrationBuilder.DropIndex(
                name: "IX_Visitations_ClinicId",
                table: "Visitations");

            migrationBuilder.DropIndex(
                name: "IX_Visitations_HospitalId",
                table: "Visitations");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Visitations");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "Visitations");
        }
    }
}
