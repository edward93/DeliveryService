using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAL.Entities;
using DAL.Enums;

namespace DeliveryService.ViewModels.Business
{
    public class RegisterBusinessModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        public string ContactPersonFirstName { get; set; }
        [Required]
        public string ContactPersonLastName { get; set; }
        public Sex Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }

        #region Address
        [Required]
        public string AddressLine1 { get; set; }
        public string Addressline2 { get; set; }
        [Required]
        public Country Country { get; set; }
        [Required]
        public string City { get; set; }
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        #endregion

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

        [Required]
        [Display(Name = "Email")]
        public string BusinessEmail { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
    }
}