using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAL.Entities;
using DAL.Enums;

namespace DeliveryService.ViewModels.Business
{
    public class PreviewBusinessModel
    {
        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public Sex Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string Addressline2 { get; set; }
        public Country Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string AddressForDisplay => AddressLine1 + " " + City + " " + State + " " + ZipCode;

        public Person GetPerson(User user, Person adminUser)
        {
            return new Person
            {
                IsDeleted = false,
                CreatedBy = adminUser.Id,
                CreatedDt = DateTime.UtcNow,
                DateOfBirth = DateTime.Now.AddYears(-21),
                Email = BusinessEmail,
                FirstName = ContactPersonFirstName,
                LastName = ContactPersonLastName,
                Sex = Sex,
                Phone = Phone,
                UpdatedBy = adminUser.Id,
                UpdatedDt = DateTime.UtcNow,
                UserId = user.Id
            };
        }
        public Address GetAddress()
        {
            return new Address
            {
                CreatedDt = DateTime.UtcNow,
                AddressLine1 = AddressLine1,
                AddressLine2 = Addressline2,
                City = City,
                Country = Country,
                CreatedBy = 2,
                IsDeleted = false,
                UpdatedBy = 2,
                State = State,
                ZipCode = ZipCode,
                UpdatedDt = DateTime.UtcNow
            };
        }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessEmail { get; set; }
        public string ContactPersonPhoneNumber { get; set; }

    }
}