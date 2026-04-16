using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace h5_2o_MAIN.Migrations
{
    /// <inheritdoc />
    public partial class CapitalToLowercase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "measurements",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "measurements",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "measurements",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "value",
                table: "measurements",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "measurements",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "measurements",
                newName: "Id");
        }
    }
}
