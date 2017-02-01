using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryService.Helpers.DataTableHelper.Models
{
    public class DataParam
    {
        public string Search { get; set; }
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public int? SortColumn { get; set; }
        public string SortDirection { get; set; }

    }
}