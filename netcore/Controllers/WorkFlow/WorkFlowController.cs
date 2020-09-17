using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using netcore.Models;
using Helper;
using netcore_ef.Models;
using Microsoft.Data.SqlClient;
using NPOI.HSSF.Record.Chart;

namespace netcore.Controllers.WorkFlow
{
    public class WorkFlowController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<WorkFlowController> logger;
        public WorkFlowController(netcore_databaseContext _context, ILogger<WorkFlowController> _logger)
        {
            context = _context;
            logger = _logger;
        }
        public IActionResult Index()
        {
            return View();
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
