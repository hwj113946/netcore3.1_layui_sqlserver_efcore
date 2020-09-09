using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using netcore.Models;

namespace netcore.Controllers.Public
{
    public class PublicController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<PublicController> logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        public PublicController(netcore_databaseContext _context, ILogger<PublicController> _logger, IWebHostEnvironment _hostingEnvironment)
        {
            context = _context;
            logger = _logger;
            hostingEnvironment = _hostingEnvironment;
        }

        [CheckCustomer]
        public IActionResult ExcelImport()
        {
            var uploadUrl= HttpContext.Request.Query["UploadUrl"].ToString() ?? "";
            ViewBag.UploadUrl = uploadUrl == "" ? "" : uploadUrl.Replace("{xg}", "/");
            ViewBag.ExcelTempUrl = HttpContext.Request.Query["ExcelTempUrl"].ToString() ?? "";
            return View();
        }

        #region 获取有效状态的公司
        [HttpGet]
        public async Task<IActionResult> GetCorp()
        {
            try
            {
                var list = await context.AppCorps.Where(u => u.Status == "有效").ToListAsync();
                return Json(new { code = 0, msg = "获取成功", data = list });
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "查询公司。" + ex.Message);
                return Json(new { code = 1, msg = "查询时出错，请联系管理员", data = new { } });
            }
        } 
        #endregion
    }
}
