using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace h5_2o_MAIN.Migrations
{
    /// <inheritdoc />
    public partial class correctModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "measurements");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "measurements",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "measurements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "measurements");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "measurements");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "measurements",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
