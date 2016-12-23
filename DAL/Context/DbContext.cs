using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DAL.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DAL.Context
{
    public class DbContext : IdentityDbContext<User>, IDbContext
    {
        public DbContext()
            : base("DeliveryService", throwIfV1Schema: false)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DriverUpload> DriverUploads { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public static DbContext Create()
        {
            return new DbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }
    }
}