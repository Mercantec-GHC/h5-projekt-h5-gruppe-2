using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace h5_2o_MAIN.Migrations
{
    /// <inheritdoc />
    public partial class addedArduinoIdtoMeasurement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "measurements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "measurements");
        }
    }
}
