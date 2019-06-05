using core.Domain.Entities.Auth;
using core.Domain.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Data.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuthClient> AuthClients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.Entity<item>().ToTable("item");

            // User
            builder.Entity<ApplicationUser>()
                .HasKey(a => a.Id)
                .ForSqlServerIsClustered(false);

            builder.Entity<ApplicationUser>()
                .Property(a => a.ClusterId)
                .ValueGeneratedOnAdd()
                .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            builder.Entity<ApplicationUser>()
                .HasIndex(c => c.ClusterId)
                .ForSqlServerIsClustered(true)
                .IsUnique(true);

            // Seed Data
            builder.Entity<AuthClient>().HasData(
                new AuthClient()
                {
                    Id = 1,
                    Name = "NuxtSPA",
                    Active = true
                }
            );
        }
    }
}
