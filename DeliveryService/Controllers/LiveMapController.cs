using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Enums;
using DeliveryService.Helpers.DataTableHelper;
using DeliveryService.Models.ViewModels;
using DeliveryService.ViewModels.Business;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using ServiceLayer.Service;

namespace DeliveryService.Controllers
{
    [Authorize(Roles = Roles.Business)]
    public class LiveMapController : BaseController
    {
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IBusinessService> _businessService;
        private readonly Lazy<IRiderService> _driverService;
        private readonly Lazy<IBusinessPenaltyService> _businessPenaltyService;
        private readonly Lazy<IDriverLocationService> _driverLocationService;
        private DataTable<BusinessOrder> _ordersDataTable;
        private readonly string _signalRConnection;

        public LiveMapController(IConfig config,
            IDbContext context,
            IOrderService orderService,
            IPersonService personService,
            IBusinessService businessService,
            IRiderService driverService,
            IDriverLocationService driverLocationService,
            IBusinessPenaltyService businessPenaltyService) : base(config, context)
        {
            _businessPenaltyService = new Lazy<IBusinessPenaltyService>(() => businessPenaltyService);
            _signalRConnection = $"{Config.SignalRServerUrl}:{Config.SignalRServerPort}/";
            _driverLocationService = new Lazy<IDriverLocationService>(() => driverLocationService);
            _driverService = new Lazy<IRiderService>(() => driverService);
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _personService = new Lazy<IPersonService>(() => personService);
            _orderService = new Lazy<IOrderService>(() => orderService);

        }
        public async Task<ActionResult> Map()
        {
            var contPerson = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
            var currBusiness = await _businessService.Value.GetBusinessByPersonId(contPerson.Id);

            ViewBag.BusinessAddress =
                $"{currBusiness.Addresses.FirstOrDefault()?.AddressLine1} {currBusiness.Addresses.FirstOrDefault()?.City}";

            return View();
        }

        public async Task<ActionResult> GetOnlineRiders()
        {
            var serviceResult = new ServiceResult();
            try
            {
                var contPerson = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                var currBusiness = await _businessService.Value.GetBusinessByPersonId(contPerson.Id);

                var onlineRiders = await _driverService.Value.GetOnlineDriversAsync();

                serviceResult.Success = true;
                serviceResult.Messages.AddMessage(MessageType.Info, "Success");
                serviceResult.Data = onlineRiders;
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
            }

            return Json(serviceResult);
        }

        public async Task<ActionResult> GetBusinessRiders()
        {
            var serviceResult = new ServiceResult();
            try
            {
                var contPerson = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                var currBusiness = await _businessService.Value.GetBusinessByPersonId(contPerson.Id);

                var businessRiders = await _driverService.Value.GetBusinessDriversAsync(currBusiness.Id);

                serviceResult.Success = true;
                serviceResult.Messages.AddMessage(MessageType.Info, "Success");
                serviceResult.Data = businessRiders;
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
            }

            return Json(serviceResult);
        }
    }
}