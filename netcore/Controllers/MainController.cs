using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netcore.Models;
using Helper;
using System.Text;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace netcore.Controllers
{
    public class MainController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<MainController> logger;
        public MainController(netcore_databaseContext _context, ILogger<MainController> _logger)
        {
            context = _context;
            logger = _logger;
        }

        [CheckCustomer]
        public IActionResult MainIndex()
        {
            ViewBag.user_code = HttpContext.Session.GetString("user_code");
            ViewBag.user_name = HttpContext.Session.GetString("user_name");
            return View();
        }

        [CheckCustomer]
        public IActionResult Index()
        {
            var user = context.AppUsers.SingleOrDefault(u => u.UserId == HttpContext.Session.GetInt32("user_id"));
            if (user.Password == EncodeHelper.MD5Hash("klapp"))
            {
                ViewBag.need = false;
            }
            else
            {
                ViewBag.need = user.ModifyPasswordDate == null || (user.ModifyPasswordDate > DateTime.Now.AddDays(-90));
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetMenuData()
        {
            var data = new
            {
                UseMenuDatas = GetMenuList()
            };
            return Content(data.ToJson());
        }


        public object GetMenuList()
        {
            var menuList = context.AppMenus.FromSqlRaw(@"SELECT  m.*
                FROM   app_menu m JOIN app_role_menu rm ON m.menu_id = rm.menu_id JOIN app_user_role ur
                ON rm.role_id = ur.role_id AND ur.user_id = @user_id",
            new object[] { new SqlParameter("@user_id", HttpContext.Session.GetInt32("user_id")) }).Distinct().ToList();
            var json = ToMenuJson(menuList, 0);
            return json;
        }


        private string ToMenuJson(List<AppMenu> data, int parentId)
        {
            if (data == null)
            {
                return "";
            }
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("[");
            List<AppMenu> entitys = data.FindAll(t => t.ParentMenuId == parentId);
            if (entitys.Count > 0)
            {
                foreach (var item in entitys)
                {
                    string strJson = item.ToJson();
                    strJson = strJson.Insert(strJson.Length - 1, ",\"ChildNodes\":" + ToMenuJson(data, item.MenuId) + "");
                    sbJson.Append(strJson + ",");
                }
                sbJson = sbJson.Remove(sbJson.Length - 1, 1);
            }
            sbJson.Append("]");
            return sbJson.ToString();
        }

        //#region 获取审批通知
        //[HttpGet]
        //public async Task<IActionResult> GetApprNotice()
        //{
        //    string Msg = "";
        //    var ds = DbContextExtensions.RunProcCur(context.Database, "Get_WolkFlow_by_Person",
        //        new object[] { new SqlParameter("v_user_id",HttpContext.Session.GetInt32("user_id"))});
        //    Msg = ds.Tables[0].Rows.Count > 0
        //        ? "{\"code\":0,\"msg\":\"已查询到数据\",\"count\":" + ds.Tables[0].Rows.Count + ",\"data\":[" + JsonTools.DataTableToJson(ds.Tables[0]) + "]}"
        //        : "{\"code\":0,\"msg\":\"未查询到数据\",\"count\":0,\"data\":[]}";
        //    return Content(Msg);
        //}
        //#endregion
    }
}

