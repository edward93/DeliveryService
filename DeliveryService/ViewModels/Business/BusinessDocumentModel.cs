using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Enums;

namespace DeliveryService.ViewModels.Business
{
    public class BusinessDocumentModel
    {
        public string FileName { get; set; }
        public DateTime ExpireDate { get; set; }
        public BusinessUploadType UploadType { get; set; }
        public int DocumentId { get; set; }
        public bool IsFileExist { get; set; }
    }
}