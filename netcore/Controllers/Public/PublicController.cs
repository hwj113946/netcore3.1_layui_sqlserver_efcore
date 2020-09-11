using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using netcore.Models;
using Helper;

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

        #region 根据CorpId获取有效状态的部门
        [HttpGet]
        public async Task<IActionResult> GetDept(int CorpId)
        {
            try
            {
                var list = await context.AppDepts.Where(u => u.Status == "有效" && u.CorpId == CorpId).ToListAsync();
                return Json(new { code = 0, msg = "获取成功", data = list });
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "查询公司。" + ex.Message);
                return Json(new { code = 1, msg = "查询时出错，请联系管理员", data = new { } });
            }
        }
        #endregion

        #region 根据DeptId获取有效状态的岗位
        [HttpGet]
        public async Task<IActionResult> GetPost(int DeptId)
        {
            try
            {
                var list = await context.AppPosts.Where(u => u.Status == "有效" && u.DeptId == DeptId).ToListAsync();
                return Json(new { code = 0, msg = "获取成功", data = list });
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "查询公司。" + ex.Message);
                return Json(new { code = 1, msg = "查询时出错，请联系管理员", data = new { } });
            }
        }
        #endregion

        #region 根据部门Id获取岗位
        [HttpGet]
        public async Task<IActionResult> GetPostTree(int DeptId)
        {
            try
            {
                var list = await context.AppPosts.Where(u => u.DeptId == DeptId).ToListAsync();
                if (list.Count > 0)
                {
                    var data = ToPostJson(list, 0);
                    data = data.Replace("PostId", "value").Replace("PostName", "name");
                    return Json(new { code = 0, msg = "查询成功", data = data.ToJson() });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "查询岗位树数据为0行。");
                    return Json(new { code = 0, msg = "未查询到岗位", data = "[]".ToJson() });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "查询岗位树失败。" + ex.Message);
                return Json(new { code = 1, msg = "查询失败，请联系管理员", data = new { } });
            }
        }

        #region 转换成json树结构
        private string ToPostJson(List<Models.AppPost> data, int parentId)
        {
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("[");
            List<Models.AppPost> entitys = data.FindAll(t => t.ParentPostId == parentId);
            if (entitys.Count > 0)
            {
                foreach (var item in entitys)
                {
                    string strJson = item.ToJson();
                    strJson = strJson.Insert(strJson.Length - 1, ",\"children\":" + ToPostJson(data, item.PostId) + "");
                    sbJson.Append(strJson + ",");
                }
                sbJson = sbJson.Remove(sbJson.Length - 1, 1);
            }
            sbJson.Append("]");
            return sbJson.ToString();
        }
        #endregion
        #endregion
    }
}
