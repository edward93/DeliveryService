using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryService.Models
{
    public class AdminDashBoardModel
    {
        public int AllMembersCount { get; set; }
        public int TodayMembersCount { get; set; }
        public int AllDriversCount { get; set; }
        public int TodayDriversCount { get; set; }
        public int ActivePartnersCount { get; set; }
        public int InactivePartnersCount { get; set; }
        public decimal AllAmount { get; set; }
        public decimal TodayAmount { get; set; }
        public decimal AllEarned { get; set; }
        public decimal TodayEarned { get; set; }
        public decimal AllAddRiderFee { get; set; }
        public decimal TodayAddRiderFee { get; set; }

    }
}