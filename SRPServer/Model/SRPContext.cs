using Microsoft.EntityFrameworkCore;
using SRPConfig;
using System.Collections.Generic;

namespace SRPServer.Model
{
    public class SRPContext : DbContext
    {
        // Names of Tables mut be equal to names of __Config classes
        public DbSet<PUnitConfig> PUnitConfig { get; set; } = null!;
        public DbSet<DriveConfig> DriveConfig { get; set; } = null!;
        public DbSet<FluidConfig> FluidConfig { get; set; } = null!;
        public DbSet<SRPConfig.SRPConfig> SRPConfig { get; set; } = null!;
        public DbSet<TubingConfig> TubingConfig { get; set; } = null!;
        public DbSet<RodConfig> RodConfig { get; set; } = null!;
        public DbSet<RodSectionConfig> RodSectionConfig { get; set; } = null!;

        public SRPContext(DbContextOptions<SRPContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PUnitConfig>().HasData(
                            new PUnitConfig { Id = 1, Name = "Unit1", SizeA = 2000, SizeC = 2000, SizeG = 750, SizeH = 3800, SizeI = 2000, SizeK = 2000, SizeP = 3000, SizeR = 1200, BeamWidth = 44, CrankWidth = 55 },
                            new PUnitConfig { Id = 2, Name = "Unit2", SizeA = 2001, SizeC = 2001, SizeG = 751, SizeH = 3801, SizeI = 2001, SizeK = 2001, SizeP = 3001, SizeR = 1201, BeamWidth = 45, CrankWidth = 56 },
                            new PUnitConfig { Id = 3, Name = "Unit3", SizeA = 2002, SizeC = 2002, SizeG = 752, SizeH = 3802, SizeI = 2002, SizeK = 2000, SizeP = 3002, SizeR = 1202, BeamWidth = 46, CrankWidth = 57 }
            );

            modelBuilder.Entity<DriveConfig>().HasData(
                            new DriveConfig { Id = 1, Name = "Drive1", NominalN = 1.1, PolesN = 12, SlipIdle = 1.3, SlipNominal = 1.4, SmallPulleyD = 1.5, LargePulleyD = 1.6, Direction = true },
                            new DriveConfig { Id = 2, Name = "Drive2", NominalN = 2.1, PolesN = 22, SlipIdle = 2.3, SlipNominal = 2.4, SmallPulleyD = 2.5, LargePulleyD = 2.6, Direction = true },
                            new DriveConfig { Id = 3, Name = "Drive3", NominalN = 3.1, PolesN = 32, SlipIdle = 3.3, SlipNominal = 3.4, SmallPulleyD = 3.5, LargePulleyD = 3.6, Direction = true }
            );

            modelBuilder.Entity<FluidConfig>().HasData(
                            new FluidConfig { Id = 1, Name = "Fluid1", OilDensity = 1.1, WaterDensity = 1.2, WaterHoldup = 1.3, GasFactor = 1.4 },
                            new FluidConfig { Id = 2, Name = "Fluid2", OilDensity = 2.1, WaterDensity = 2.2, WaterHoldup = 2.3, GasFactor = 2.4 },
                            new FluidConfig { Id = 3, Name = "Fluid3", OilDensity = 3.1, WaterDensity = 3.2, WaterHoldup = 3.3, GasFactor = 3.4 }
            );

            modelBuilder.Entity<SRPConfig.SRPConfig>().HasData(
                            new SRPConfig.SRPConfig { Id = 1, Name = "SRP1", Anchor = false, PlungerD = 1.2, Filling = 1.3, ZatrubP = 1.4, PipeP = 1.4, FluidH = 1.4, FrictionPlunger = 1.4, FrictionSeal = 1.4 },
                            new SRPConfig.SRPConfig { Id = 2, Name = "SRP2", Anchor = false, PlungerD = 2.2, Filling = 2.3, ZatrubP = 2.4, PipeP = 2.4, FluidH = 2.4, FrictionPlunger = 2.4, FrictionSeal = 2.4 },
                            new SRPConfig.SRPConfig { Id = 3, Name = "SRP3", Anchor = false, PlungerD = 3.2, Filling = 3.3, ZatrubP = 3.4, PipeP = 3.4, FluidH = 3.4, FrictionPlunger = 3.4, FrictionSeal = 3.4 }
            );

            modelBuilder.Entity<TubingConfig>().HasData(
                            new TubingConfig { Id = 1, Name = "Tubing1", ModuleJung = 1.1, Density = 1.2, Length = 1.4, InnerD = 1.4, OuterD = 1.4 },
                            new TubingConfig { Id = 2, Name = "Tubing2", ModuleJung = 2.1, Density = 2.2, Length = 2.4, InnerD = 2.4, OuterD = 2.4 },
                            new TubingConfig { Id = 3, Name = "Tubing3", ModuleJung = 3.1, Density = 3.2, Length = 3.4, InnerD = 3.4, OuterD = 3.4 }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
