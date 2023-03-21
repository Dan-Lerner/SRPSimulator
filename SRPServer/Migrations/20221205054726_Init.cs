using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SRPServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DriveConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    NominalN = table.Column<double>(type: "REAL", nullable: false),
                    PolesN = table.Column<int>(type: "INTEGER", nullable: false),
                    SlipIdle = table.Column<double>(type: "REAL", nullable: false),
                    SlipNominal = table.Column<double>(type: "REAL", nullable: false),
                    GearRatio = table.Column<double>(type: "REAL", nullable: false),
                    SmallPulleyD = table.Column<double>(type: "REAL", nullable: false),
                    LargePulleyD = table.Column<double>(type: "REAL", nullable: false),
                    Direction = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriveConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FluidConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OilDensity = table.Column<double>(type: "REAL", nullable: false),
                    WaterDensity = table.Column<double>(type: "REAL", nullable: false),
                    WaterHoldup = table.Column<double>(type: "REAL", nullable: false),
                    GasFactor = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FluidConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PUnitConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SizeR = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeA = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeC = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeI = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeG = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeH = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeP = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeK = table.Column<int>(type: "INTEGER", nullable: false),
                    Counterweight = table.Column<int>(type: "INTEGER", nullable: false),
                    CrankWidth = table.Column<int>(type: "INTEGER", nullable: false),
                    BeamWidth = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PUnitConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RodConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RodConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SRPConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Anchor = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlungerD = table.Column<double>(type: "REAL", nullable: false),
                    Filling = table.Column<double>(type: "REAL", nullable: false),
                    ZatrubP = table.Column<double>(type: "REAL", nullable: false),
                    PipeP = table.Column<double>(type: "REAL", nullable: false),
                    FluidH = table.Column<double>(type: "REAL", nullable: false),
                    FrictionPlunger = table.Column<double>(type: "REAL", nullable: false),
                    FrictionSeal = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SRPConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TubingConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ModuleJung = table.Column<double>(type: "REAL", nullable: false),
                    Density = table.Column<double>(type: "REAL", nullable: false),
                    Length = table.Column<double>(type: "REAL", nullable: false),
                    InnerD = table.Column<double>(type: "REAL", nullable: false),
                    OuterD = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TubingConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RodSectionConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ModuleJung = table.Column<double>(type: "REAL", nullable: false),
                    Density = table.Column<double>(type: "REAL", nullable: false),
                    Length = table.Column<double>(type: "REAL", nullable: false),
                    D = table.Column<double>(type: "REAL", nullable: false),
                    RodConfigId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RodSectionConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RodSectionConfig_RodConfig_RodConfigId",
                        column: x => x.RodConfigId,
                        principalTable: "RodConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DriveConfig",
                columns: new[] { "Id", "Direction", "GearRatio", "LargePulleyD", "Name", "NominalN", "PolesN", "SlipIdle", "SlipNominal", "SmallPulleyD" },
                values: new object[,]
                {
                    { 1, true, 0.0, 1.6000000000000001, "Drive1", 1.1000000000000001, 12, 1.3, 1.3999999999999999, 1.5 },
                    { 2, true, 0.0, 2.6000000000000001, "Drive2", 2.1000000000000001, 22, 2.2999999999999998, 2.3999999999999999, 2.5 },
                    { 3, true, 0.0, 3.6000000000000001, "Drive3", 3.1000000000000001, 32, 3.2999999999999998, 3.3999999999999999, 3.5 }
                });

            migrationBuilder.InsertData(
                table: "FluidConfig",
                columns: new[] { "Id", "GasFactor", "Name", "OilDensity", "WaterDensity", "WaterHoldup" },
                values: new object[,]
                {
                    { 1, 1.3999999999999999, "Fluid1", 1.1000000000000001, 1.2, 1.3 },
                    { 2, 2.3999999999999999, "Fluid2", 2.1000000000000001, 2.2000000000000002, 2.2999999999999998 },
                    { 3, 3.3999999999999999, "Fluid3", 3.1000000000000001, 3.2000000000000002, 3.2999999999999998 }
                });

            migrationBuilder.InsertData(
                table: "PUnitConfig",
                columns: new[] { "Id", "BeamWidth", "Counterweight", "CrankWidth", "Name", "SizeA", "SizeC", "SizeG", "SizeH", "SizeI", "SizeK", "SizeP", "SizeR" },
                values: new object[,]
                {
                    { 1, 44, 0, 55, "Unit1", 2000, 2000, 750, 3800, 2000, 2000, 3000, 1200 },
                    { 2, 45, 0, 56, "Unit2", 2001, 2001, 751, 3801, 2001, 2001, 3001, 1201 },
                    { 3, 46, 0, 57, "Unit3", 2002, 2002, 752, 3802, 2002, 2000, 3002, 1202 }
                });

            migrationBuilder.InsertData(
                table: "SRPConfig",
                columns: new[] { "Id", "Anchor", "Filling", "FluidH", "FrictionPlunger", "FrictionSeal", "Name", "PipeP", "PlungerD", "ZatrubP" },
                values: new object[,]
                {
                    { 1, false, 1.3, 1.3999999999999999, 1.3999999999999999, 1.3999999999999999, "SRP1", 1.3999999999999999, 1.2, 1.3999999999999999 },
                    { 2, false, 2.2999999999999998, 2.3999999999999999, 2.3999999999999999, 2.3999999999999999, "SRP2", 2.3999999999999999, 2.2000000000000002, 2.3999999999999999 },
                    { 3, false, 3.2999999999999998, 3.3999999999999999, 3.3999999999999999, 3.3999999999999999, "SRP3", 3.3999999999999999, 3.2000000000000002, 3.3999999999999999 }
                });

            migrationBuilder.InsertData(
                table: "TubingConfig",
                columns: new[] { "Id", "Density", "InnerD", "Length", "ModuleJung", "Name", "OuterD" },
                values: new object[,]
                {
                    { 1, 1.2, 1.3999999999999999, 1.3999999999999999, 1.1000000000000001, "Tubing1", 1.3999999999999999 },
                    { 2, 2.2000000000000002, 2.3999999999999999, 2.3999999999999999, 2.1000000000000001, "Tubing2", 2.3999999999999999 },
                    { 3, 3.2000000000000002, 3.3999999999999999, 3.3999999999999999, 3.1000000000000001, "Tubing3", 3.3999999999999999 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RodSectionConfig_RodConfigId",
                table: "RodSectionConfig",
                column: "RodConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriveConfig");

            migrationBuilder.DropTable(
                name: "FluidConfig");

            migrationBuilder.DropTable(
                name: "PUnitConfig");

            migrationBuilder.DropTable(
                name: "RodSectionConfig");

            migrationBuilder.DropTable(
                name: "SRPConfig");

            migrationBuilder.DropTable(
                name: "TubingConfig");

            migrationBuilder.DropTable(
                name: "RodConfig");
        }
    }
}
