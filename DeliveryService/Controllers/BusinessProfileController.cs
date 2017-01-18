using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DeliveryService.Helpers;
using Infrastructure.Config;

namespace DeliveryService.Controllers
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessProfileController : BaseController
    {
        // GET: BusinessProfile
        public BusinessProfileController(IConfig config, IDbContext context) : base(config, context)
        {
        }

        public ActionResult BusinessProfile()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Upload()
        {
            try
            {
                string controlId = Request.Params["controlID"];
                var resultList = new List<ViewDataUploadFilesResult>();
                var currentContext = HttpContext;
                FileUpload fileUpload = InitUploader(controlId);
                fileUpload.FilesHelper.UploadAndShowResults(currentContext, resultList);
                JsonFiles files = new JsonFiles(resultList);
                bool isEmpty = !resultList.Any();
                if (isEmpty)
                    return Json("Error ");
                return Json(files);
            }
            catch (Exception)
            {
                return Json("error");
            }
        }

        private FileUpload InitUploader(string controlId)
        {
            FileUploadConfig uploaderConfig = new FileUploadConfig()
            {
                TempPath = "~/VehicleDocs/",
                ServerMapPath = "~/Documents/Vehicle/" + controlId,
                UrlBase = "~/Documents/Vehicle/" + controlId,
                DeleteUrl = "/Vehicles/DeleteFile/?file=",
                DeleteType = "GET",
            };

            return new FileUpload(uploaderConfig);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteFile(string controlId, string file, int id)
        {
            try
            {
                FileUpload fileUpload = InitUploader(controlId);
                fileUpload.FilesHelper.DeleteFile(file);

                if (id != 0)
                {
                    /*  var resultFile = await _vehicleFileService.GetVehicleFileById(id);
                      await _vehicleFileService.DeleteVehicleFile(id);*/
                }

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }
    }
}