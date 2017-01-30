using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryService.ViewModels.Business
{
    public class BusinessListItem
    {
        public BusinessListItem(DAL.Entities.Business business)
        {
            Id = business.Id;
            BusinessEmail = business.BusinessEmail;
            BusinessName = business.BusinessName;
            ContactPersonPhoneNumber = business.ContactPersonPhoneNumber;
            Approved = business.Approved;
            RatingAverageScore = business.Rating.AverageScore;
        }

        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessEmail { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public bool Approved { get; set; }
        public decimal RatingAverageScore { get; set; }
    }
}