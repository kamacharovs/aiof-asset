using System;

using Microsoft.EntityFrameworkCore;

namespace aiof.asset.data
{
    public class AssetContext : DbContext
    {
        public readonly ITenant Tenant;

        public virtual DbSet<AssetType> AssetTypes { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<AssetStock> AssetsStock { get; set; }
        public virtual DbSet<AssetHome> AssetsHome { get; set; }
        public virtual DbSet<AssetSnapshot> AssetSnapshots { get; set; }

        public AssetContext(DbContextOptions<AssetContext> options, ITenant tenant)
            : base(options)
        {
            Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("asset");

            modelBuilder.Entity<AssetType>(e =>
            {
                e.ToTable(Keys.Entity.AssetType);

                e.HasKey(x => x.Name);

                e.Property(x => x.Name).HasSnakeCaseColumnName().HasMaxLength(100).IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
            });

            modelBuilder.Entity<Asset>(e =>
            {
                e.ToTable(Keys.Entity.Asset);

                e.HasKey(x => x.Id);

                e.HasQueryFilter(x => x.UserId == Tenant.UserId
                    && !x.IsDeleted);

                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Name).HasSnakeCaseColumnName().HasMaxLength(100).IsRequired();
                e.Property(x => x.TypeName).HasSnakeCaseColumnName().HasMaxLength(100).IsRequired();
                e.Property(x => x.Value).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.UserId).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Created).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.IsDeleted).HasSnakeCaseColumnName().IsRequired();

                e.HasOne(x => x.Type)
                    .WithMany()
                    .HasForeignKey(x => x.TypeName)
                    .IsRequired();

                e.HasMany(x => x.Snapshots)
                    .WithOne()
                    .HasForeignKey(x => x.AssetId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AssetStock>(e =>
            {
                e.ToTable(Keys.Entity.AssetStock);

                e.Property(x => x.TickerSymbol).HasSnakeCaseColumnName().HasMaxLength(50).IsRequired();
                e.Property(x => x.Shares).HasSnakeCaseColumnName();
                e.Property(x => x.ExpenseRatio).HasSnakeCaseColumnName();
                e.Property(x => x.DividendYield).HasSnakeCaseColumnName();
            });

            modelBuilder.Entity<AssetHome>(e =>
            {
                e.ToTable(Keys.Entity.AssetHome);

                e.Property(x => x.HomeType).HasSnakeCaseColumnName().HasMaxLength(100).IsRequired();
                e.Property(x => x.LoanValue).HasSnakeCaseColumnName();
                e.Property(x => x.MonthlyMortgage).HasSnakeCaseColumnName();
                e.Property(x => x.MortgageRate).HasSnakeCaseColumnName();
                e.Property(x => x.DownPayment).HasSnakeCaseColumnName();
                e.Property(x => x.AnnualInsurance).HasSnakeCaseColumnName();
                e.Property(x => x.AnnualPropertyTax).HasSnakeCaseColumnName();
                e.Property(x => x.ClosingCosts).HasSnakeCaseColumnName();
                e.Property(x => x.IsRefinanced).HasSnakeCaseColumnName();
            });

            modelBuilder.Entity<AssetSnapshot>(e =>
            {
                e.ToTable(Keys.Entity.AssetSnapshot);

                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.AssetId).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Name).HasSnakeCaseColumnName().HasMaxLength(100);
                e.Property(x => x.TypeName).HasSnakeCaseColumnName().HasMaxLength(100);
                e.Property(x => x.Value).HasSnakeCaseColumnName();
                e.Property(x => x.ValueChange).HasSnakeCaseColumnName();
                e.Property(x => x.Created).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
            });
        }
    }
}
