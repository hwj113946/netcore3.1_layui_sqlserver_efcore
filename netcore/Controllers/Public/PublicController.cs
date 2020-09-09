using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace netcore.Controllers.Public
{
    public class PublicController : Controller
    {
        
        [CheckCustomer]
        public IActionResult ExcelImport()
        {
            var uploadUrl= HttpContext.Request.Query["UploadUrl"].ToString() ?? "";
            ViewBag.UploadUrl = uploadUrl == "" ? "" : uploadUrl.Replace("{xg}", "/");
            ViewBag.ExcelTempUrl = HttpContext.Request.Query["ExcelTempUrl"].ToString() ?? "";
            return View();
        }
    }
}
