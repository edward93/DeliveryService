using System;
using System.Collections.Generic;
using System.Security.Claims;
using DAL.Constants;
using DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;
using DAL.Enums;

namespace DAL.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<Context.DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Context.DbContext context)
        {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            rm.Create(new IdentityRole(Roles.Driver));
            rm.Create(new IdentityRole(Roles.Admin));
            rm.Create(new IdentityRole(Roles.Business));

            var um = new UserManager<User>(new UserStore<User>(context));
            var user = new User
            {
                UserName = "admin@gmail.com",
                EmailConfirmed = true,
                Email = "admin@gmail.com"
            };

            um.Create(user, "password1");
            var currentUser = um.FindByEmail(user.Email);
            um.AddToRole(currentUser.Id, Roles.Admin);
            um.AddClaim(currentUser.Id, new Claim(ClaimTypes.Role, Roles.Admin));

            var adminPerson = new Person
            {
                IsDeleted = false,
                CreatedBy = 0,
                CreatedDt = DateTime.UtcNow,
                DateOfBirth = DateTime.UtcNow.AddYears(-21),
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "Admin",
                Sex = Sex.Male,
                Phone = "123456789",
                UpdatedBy = 0,
                UpdatedDt = DateTime.UtcNow,
                UserId = user.Id
            };

            context.Persons.AddOrUpdate(adminPerson);


            context.Rates.AddRange(new List<Rate> {new Rate
            {
                Amount = 2,
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = 0,
                PaymentType = PaymentType.OrderBookingFee,
                UpdatedBy = 0,
                UpdatedDt = DateTime.UtcNow
            }, new Rate
            {
                Amount = 4,
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = 0,
                PaymentType = PaymentType.DriverFeeFor3Miles,
                UpdatedBy = 0,
                UpdatedDt = DateTime.UtcNow
            }, new Rate
            {
                Amount = new decimal(0.25),
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = 0,
                PaymentType = PaymentType.AotmBox,
                UpdatedBy = 0,
                UpdatedDt = DateTime.UtcNow
            }, new Rate
            {
                Amount = 2,
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = 0,
                PaymentType = PaymentType.DriverPenaltyForRejections,
                UpdatedBy = 0,
                UpdatedDt = DateTime.UtcNow
            }, new Rate
            {
                Amount = new decimal(0.10),
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = 0,
                PaymentType = PaymentType.DriverPenaltyForDelayPerMinute,
                UpdatedBy = 0,
                UpdatedDt = DateTime.UtcNow
            }, new Rate
            {
                Amount = new decimal(0.10),
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = 0,
                PaymentType = PaymentType.DriverWaitingForBusinessPerMinute,
                UpdatedBy = 0,
                UpdatedDt = DateTime.UtcNow
            }
            });
            context.SaveChanges();
        }
    }
}
