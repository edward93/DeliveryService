using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace DeliveryService.Helpers
{
    public class FileUpload
    {

        public FilesHelper FilesHelper;
        private readonly FileUploadConfig _config = new FileUploadConfig()
        {
            DeleteType = "GET",
            DeleteUrl = "/Profile/DeleteFile/?file=",
            ServerMapPath = "~/Documents/userImages/",
            StorageRoot = Path.Combine(HostingEnvironment.MapPath("~/Documents/userImages/")),
            TempPath = "~/userImages/",
            UrlBase = "/Documents/userImages/"
        };

        public FileUpload()
        {
            FilesHelper = new FilesHelper(_config);
        }

        public FileUpload(FileUploadConfig config)
        {
            config.StorageRoot = Path.Combine(HostingEnvironment.MapPath(config.ServerMapPath));

            FilesHelper = new FilesHelper(config);
        }

    }

    public class FileUploadConfig
    {
        public string DeleteUrl { get; set; }
        public string DeleteType { get; set; }
        public string StorageRoot { get; set; }
        public string UrlBase { get; set; }
        public string TempPath { get; set; }
        public string ServerMapPath { get; set; }
    }
}