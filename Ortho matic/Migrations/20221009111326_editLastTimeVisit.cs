using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ortho_matic.Migrations
{
    public partial class editLastTimeVisit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTimeOfVisitation",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "LastTimeOfVisitation",
                table: "Clinics");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeOfVisitation",
                table: "DoctorHospitals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeOfVisitation",
                table: "DoctorClinics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTimeOfVisitation",
                table: "DoctorHospitals");

            migrationBuilder.DropColumn(
                name: "LastTimeOfVisitation",
                table: "DoctorClinics");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeOfVisitation",
                table: "Hospitals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeOfVisitation",
                table: "Clinics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
