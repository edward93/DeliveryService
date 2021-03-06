﻿using System;
using System.ComponentModel.DataAnnotations;
using DAL.Entities;
using DAL.Enums;
using Newtonsoft.Json;

namespace DeliveryService.API.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string VehicleRegistrationNumber { get; set; }
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public Sex Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }

        public VehicleType VehicleType { get; set; }

#region Address
        public string AddressLine1 { get; set; }
        public string Addressline2 { get; set; }
        public Country Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
#endregion

        //public Address Address { get; set; }
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
        public Person GetPerson(User user)
        {
            return new Person
            {
                IsDeleted = false,
                CreatedBy = 2,
                CreatedDt = DateTime.UtcNow,
                DateOfBirth = DateOfBirth,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Sex = Sex,
                Phone = Phone,
                UpdatedBy = 2,
                UpdatedDt = DateTime.UtcNow,
                UserId = user.Id
            };
        }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
