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
namespace netcore.Controllers.Button
{
    public class ButtonController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<ButtonController> logger;
        public ButtonController(netcore_databaseContext _context, ILogger<ButtonController> _logger)
        {
            context = _context;
            logger = _logger;
        }

        [CheckCustomer]
        public IActionResult Button()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult EditButton()
        {
            ViewBag.status = HttpContext.Request.Query["status"].ToString();
            ViewBag.Rowid = (ViewBag.status == "Add" ? "" : HttpContext.Request.Query["Rowid"].ToString());
            return View();
        }

        [CheckCustomer]
        public IActionResult ChooseColor()
        {
            return View();
        }

        #region 获取按钮
        [HttpGet]
        public async Task<IActionResult> GetButton(string BUTTON_NAME, int page, int limit)
        {
            try
            {
                var list = await context.AppButtons.ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(BUTTON_NAME))
                    {
                        list = list.Where(u => u.ButtonName.Contains(BUTTON_NAME)).ToList();
                    }
                    count = Math.Ceiling(Convert.ToDecimal(list.Count) / Convert.ToDecimal(limit));
                    if (page > count)
                    {
                        return Json(new { code = 1, msg = "没有更多数据了。", count = Convert.ToInt32(count), data = new { } });
                    }
                    list = list.Skip((page - 1) * limit).Take(limit).ToList();
                }
                else
                {
                    return Json(new { code = 0, msg = "查询成功，与查询条件相符的数据为0行", count = Convert.ToInt32(count), data = new { } });
                }
                return Json(new { code = 0, msg = "查询成功", count = Convert.ToInt32(count), data = list });
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：查询失败。" + ex.Message);
                return Json(new { code = 0, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 根据按钮ID获取按钮信息
        [HttpGet]
        public async Task<IActionResult> GetButtonById(int id)
        {
            var single = await context.AppButtons.SingleOrDefaultAsync(u => u.ButtonId == id);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该按钮信息" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增
        [HttpPost]
        public async Task<IActionResult> Insert(string button_name, string button_icon, string button_event, double button_sort, string button_color, string button_element_id)
        {
            try
            {
                var role = await context.AppButtons.SingleOrDefaultAsync(u => u.ButtonName == button_name);
                if (role == null)
                {
                    await context.AppButtons.AddAsync(new AppButton()
                    {
                        ButtonName = button_name ?? "",
                        ButtonIcon = button_icon ?? "",
                        ButtonEvent = button_event ?? "",
                        ButtonSort = button_sort,
                        ButtonColor = button_color ?? "",
                        ButtonElementId = button_element_id ?? ""
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增按钮成功。");
                    return Json(new { code = 200, msg = "新增成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "按钮名称已存在。");
                    return Json(new { code = 300, msg = "按钮名称已存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增按钮失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑
        [HttpPost]
        public async Task<IActionResult> Modify(string button_name, string button_icon, string button_event, double button_sort, string button_color, int Rowid, string button_element_id)
        {
            try
            {
                var modify = await context.AppButtons.SingleOrDefaultAsync(u => u.ButtonId == Rowid);
                if (modify != null)
                {
                    modify.ButtonName = button_name ?? "";
                    modify.ButtonIcon = button_icon ?? "";
                    modify.ButtonEvent = button_event ?? "";
                    modify.ButtonSort = button_sort;
                    modify.ButtonColor = button_color ?? "";
                    modify.ButtonElementId = button_element_id ?? "";
                    context.AppButtons.Update(modify);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "编辑按钮成功。");
                    return Json(new { code = 200, msg = "编辑成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "按钮不存在。");
                    return Json(new { code = 300, msg = "按钮不存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑按钮失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除
        [HttpPost]
        public async Task<IActionResult> Delete(int[] id)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var list = new List<AppButton>();
                    for (int i = 0; i < id.Length; i++)
                    {
                        var single = await context.AppButtons.SingleOrDefaultAsync(u => u.ButtonId == id[i]);
                        if (single != null)
                        {
                            list.Add(single);
                        }
                    }
                    if (list.Count > 0)
                    {
                        context.AppButtons.RemoveRange(list);
                        context.SaveChanges();
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除按钮成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除按钮失败。" + ex.Message);
                    return Json(new { code = 300, msg = "删除失败" });
                }
            }
        }
        #endregion
    }
}
