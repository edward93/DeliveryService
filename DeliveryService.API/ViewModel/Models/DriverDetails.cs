﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Entities;
using DAL.Enums;

namespace DeliveryService.API.ViewModel.Models
{
    public class DriverDetails
    {
        public List<Address> Addresses { get; set; }
        public List<DriverDocumentModel> DriverDocuments { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Sex Sex { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }

    }
}