using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infrastructure.Config;
using ServiceLayer.Service;

namespace DeliveryService.API.Controllers
{
    public class DriverFilesController : BaseApiController
    {

        private readonly IDriverUploadService _driverUploadService;
        public DriverFilesController(IConfig config,IDriverUploadService service) : base(config)
        {
            _driverUploadService = service;
        }



    }
}
