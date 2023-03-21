using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SRPServer.Migrations
{
    /// <inheritdoc />
    public partial class DB2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Modified",
                table: "TubingConfig",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Modified",
                table: "SRPConfig",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Modified",
                table: "RodSectionConfig",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Modified",
                table: "RodConfig",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Modified",
                table: "PUnitConfig",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Modified",
                table: "FluidConfig",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Modified",
                table: "DriveConfig",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "DriveConfig",
                keyColumn: "Id",
                keyValue: 1,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "DriveConfig",
                keyColumn: "Id",
                keyValue: 2,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "DriveConfig",
                keyColumn: "Id",
                keyValue: 3,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "FluidConfig",
                keyColumn: "Id",
                keyValue: 1,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "FluidConfig",
                keyColumn: "Id",
                keyValue: 2,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "FluidConfig",
                keyColumn: "Id",
                keyValue: 3,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "PUnitConfig",
                keyColumn: "Id",
                keyValue: 1,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "PUnitConfig",
                keyColumn: "Id",
                keyValue: 2,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "PUnitConfig",
                keyColumn: "Id",
                keyValue: 3,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "SRPConfig",
                keyColumn: "Id",
                keyValue: 1,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "SRPConfig",
                keyColumn: "Id",
                keyValue: 2,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "SRPConfig",
                keyColumn: "Id",
                keyValue: 3,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "TubingConfig",
                keyColumn: "Id",
                keyValue: 1,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "TubingConfig",
                keyColumn: "Id",
                keyValue: 2,
                column: "Modified",
                value: false);

            migrationBuilder.UpdateData(
                table: "TubingConfig",
                keyColumn: "Id",
                keyValue: 3,
                column: "Modified",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modified",
                table: "TubingConfig");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "SRPConfig");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "RodSectionConfig");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "RodConfig");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "PUnitConfig");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "FluidConfig");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "DriveConfig");
        }
    }
}
