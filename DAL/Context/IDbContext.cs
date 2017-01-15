using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Context
{
    public interface IDbContext : IDisposable
    {
        DbSet<Person> Persons { get; set; }
        DbSet<Card> Cards { get; set; }
        DbSet<Driver> Drivers { get; set; }
        DbSet<DriverUpload> DriverUploads { get; set; }
        DbSet<Feedback> Feedbacks { get; set; }
        DbSet<Rating> Ratings { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<Business> Businesses { get; set; }
        DbSet<DriverLocation> DriverLocations { get; set; }
        DbSet<GeoLocation> GeoLocations { get; set; }
        DbSet<DeviceOrientation> DeviceOrientations { get; set; }
        DbSet<Order> Orders { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
        DbSet<T> Set<T>() where T : class;
        int SaveChanges();
        Database Database { get; }
    }
}