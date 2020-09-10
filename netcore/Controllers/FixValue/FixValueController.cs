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

namespace netcore.Controllers.FixValue
{
    public class FixValueController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<FixValueController> logger;
        public FixValueController(netcore_databaseContext _context, ILogger<FixValueController> _logger)
        {
            context = _context;
            logger = _logger;
        }

        [CheckCustomer]
        public IActionResult FixValue()
        {
            return View();
        }

        [CheckCustomer]
        public ActionResult FixValueTypeEdit()
        {
            ViewBag.status = HttpContext.Request.Query["status"];
            ViewBag.type_id = ViewBag.status == "add" ? "" : HttpContext.Request.Query["Rowid"].ToString();
            return View();
        }

        [CheckCustomer]
        public ActionResult FixValueEdit()
        {
            ViewBag.status = HttpContext.Request.Query["status"];
            ViewBag.type_id = HttpContext.Request.Query["type_id"];
            ViewBag.fixvalue_id = ViewBag.status == "add" ? "" : HttpContext.Request.Query["Rowid"].ToString();
            return View();
        }

        #region 获取公共代码类型
        [HttpGet]
        public async Task<IActionResult> GetFixValueType(string fixvalue, int page, int limit)
        {
            try
            {
                var list = await context.AppFixvalueTypes.ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(fixvalue))
                    {
                        list = list.Where(u => u.FixvalueTypeCode.Contains(fixvalue)||u.FixvalueTypeName.Contains(fixvalue)).ToList();
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
                return Json(new { code = 1, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 根据ID获取公共代码类型
        [HttpGet]
        public async Task<IActionResult> GetFixValueTypeById(int type_id)
        {
            var single = await context.AppFixvalueTypes.SingleOrDefaultAsync(u => u.FixvalueTypeId == type_id);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该公共代码类型信息" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增公共代码类型
        [HttpPost]
        public async Task<IActionResult> InsertFixValueType(string code, string name)
        {
            try
            {
                var single = await context.AppFixvalueTypes.SingleOrDefaultAsync(u => u.FixvalueTypeCode == code);
                if (single == null)
                {
                    await context.AppFixvalueTypes.AddAsync(new AppFixvalueType()
                    {
                        FixvalueTypeCode = code ?? "",
                        FixvalueTypeName = name ?? "",
                        CreationDate = DateTime.Now,
                        CreationUser = HttpContext.Session.GetInt32("user_id"),
                        Status = "有效"
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增公共代码类型成功。");
                    return Json(new { code = 200, msg = "新增成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "公共代码类型代码已存在。");
                    return Json(new { code = 300, msg = "公共代码类型代码已存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增公共代码类型失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑公共代码类型
        [HttpPost]
        public async Task<IActionResult> ModifyFixValueType(string code, string name, int type_id)
        {
            try
            {
                var modify = await context.AppFixvalueTypes.SingleOrDefaultAsync(u => u.FixvalueTypeId == type_id);
                if (modify != null)
                {
                    var single = await context.AppFixvalueTypes.SingleOrDefaultAsync(u => u.FixvalueTypeCode == code);
                    if (single!=null)
                    {
                        if (single.FixvalueTypeId==type_id)
                        {
                            modify.FixvalueTypeCode = code ?? "";
                            modify.FixvalueTypeName = name ?? "";
                            modify.LastModifiedDate = DateTime.Now;
                            modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                            context.AppFixvalueTypes.Update(modify);
                            await context.SaveChangesAsync();
                            logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公共代码类型成功。");
                            return Json(new { code = 200, msg = "编辑成功" });
                        }
                        else
                        {
                            logger.LogInformation(HttpContext.Session.GetString("who") + "公共代码类型代码已存在。");
                            return Json(new { code = 300, msg = "公共代码类型代码已存在" });
                        }
                    }
                    else
                    {
                        modify.FixvalueTypeCode = code ?? "";
                        modify.FixvalueTypeName = name ?? "";
                        modify.LastModifiedDate = DateTime.Now;
                        modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        context.AppFixvalueTypes.Update(modify);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公共代码类型成功。");
                        return Json(new { code = 200, msg = "编辑成功" });
                    }
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "公共代码类型不存在。");
                    return Json(new { code = 300, msg = "公共代码类型不存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公共代码类型失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除公共代码类型
        [HttpPost]
        public async Task<IActionResult> DeleteType(int[] type_id)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var list = new List<AppFixvalueType>();
                    var list1 = new List<AppFixvalue>();
                    for (int i = 0; i < type_id.Length; i++)
                    {
                        var single1 = await context.AppFixvalues.Where(u=>u.FixvalueTypeId==type_id[i]).ToListAsync();
                        if (single1.Count>0)
                        {
                            context.AppFixvalues.RemoveRange(single1);
                            context.SaveChanges();
                        }
                        var single = await context.AppFixvalueTypes.SingleOrDefaultAsync(u => u.FixvalueTypeId == type_id[i]);
                        if (single != null)
                        {
                            list.Add(single);
                        }
                    }
                    if (list.Count > 0)
                    {
                        context.AppFixvalueTypes.RemoveRange(list);
                        context.SaveChanges();
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除公共代码类型成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除公共代码类型失败。" + ex.Message);
                    return Json(new { code = 300, msg = "删除失败" });
                }
            }
        }
        #endregion

        #region 更改公共代码类型状态：生效或失效
        [HttpPost]
        public async Task<IActionResult> ChangeFixValueTypeStatus(int[] type_id)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var list = new List<AppFixvalueType>();
                    for (int i = 0; i < type_id.Length; i++)
                    {
                        var single = await context.AppFixvalueTypes.SingleOrDefaultAsync(u => u.FixvalueTypeId == type_id[i]);
                        if (single != null)
                        {
                            single.Status = single.Status=="有效" ? "失效" : "有效";
                            single.LastModifiedDate = DateTime.Now;
                            single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                            list.Add(single);
                        }
                    }
                    if (list.Count > 0)
                    {
                        context.AppFixvalueTypes.UpdateRange(list);
                        context.SaveChanges();
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "更改公共代码类型状态成功。");
                    return Json(new { code = 200, msg = "执行成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "更改公共代码类型状态失败。" + ex.Message);
                    return Json(new { code = 300, msg = "执行失败" });
                }
            }
        }
        #endregion

        #region 获取公共代码
        [HttpGet]
        public async Task<IActionResult> GetFixValue(string fixvalue, int fixvaluetypeid, int page, int limit)
        {
            try
            {
                var list = await context.AppFixvalues.Where(u => u.FixvalueTypeId == fixvaluetypeid).ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(fixvalue))
                    {
                        list = list.Where(u => u.FixvalueCode.Contains(fixvalue) || u.FixvalueName.Contains(fixvalue)).ToList();
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
                return Json(new { code = 1, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 根据ID获取公共代码
        [HttpGet]
        public async Task<IActionResult> GetFixValueById(int fixvalue_id)
        {
            var single = await context.AppFixvalues.SingleOrDefaultAsync(u => u.FixvalueId == fixvalue_id);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该公共代码信息" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增公共代码
        [HttpPost]
        public async Task<IActionResult> InsertFixValue(string code, string name, string note, int type_id)
        {
            try
            {
                var single = await context.AppFixvalues.SingleOrDefaultAsync(u => u.FixvalueCode == code && u.FixvalueTypeId == type_id);
                if (single == null)
                {
                    await context.AppFixvalues.AddAsync(new AppFixvalue()
                    {
                        FixvalueCode = code ?? "",
                        FixvalueName = name ?? "",
                        FixvalueTypeId = type_id,
                        Note = note ?? "",
                        CreationDate = DateTime.Now,
                        CreationUser = HttpContext.Session.GetInt32("user_id"),
                        Status = "有效"
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增公共代码成功。");
                    return Json(new { code = 200, msg = "新增成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "公共代码值已存在。");
                    return Json(new { code = 300, msg = "公共代码值已存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增公共代码失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑公共代码
        [HttpPost]
        public async Task<IActionResult> ModifyFixValue(string code, string name, string note, int fixvalue_id)
        {
            try
            {
                var modify = await context.AppFixvalues.SingleOrDefaultAsync(u => u.FixvalueId == fixvalue_id);
                if (modify != null)
                {
                    var single = await context.AppFixvalues.SingleOrDefaultAsync(u => u.FixvalueCode == code && u.FixvalueTypeId == modify.FixvalueTypeId);
                    if (single != null)
                    {
                        if (single.FixvalueId == fixvalue_id)
                        {
                            modify.FixvalueCode = code ?? "";
                            modify.FixvalueName = name ?? "";
                            modify.Note = note ?? "";
                            modify.LastModifiedDate = DateTime.Now;
                            modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                            context.AppFixvalues.Update(modify);
                            await context.SaveChangesAsync();
                            logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公共代码成功。");
                            return Json(new { code = 200, msg = "编辑成功" });
                        }
                        else
                        {
                            logger.LogInformation(HttpContext.Session.GetString("who") + "公共代码值已存在。");
                            return Json(new { code = 300, msg = "公共代码值已存在" });
                        }
                    }
                    else
                    {
                        modify.FixvalueCode = code ?? "";
                        modify.FixvalueName = name ?? "";
                        modify.Note = note ?? "";
                        modify.LastModifiedDate = DateTime.Now;
                        modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        context.AppFixvalues.Update(modify);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公共代码成功。");
                        return Json(new { code = 200, msg = "编辑成功" });
                    }
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "公共代码不存在。");
                    return Json(new { code = 300, msg = "公共代码不存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公共代码失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除代码
        [HttpPost]
        public async Task<IActionResult> DeleteFixValue(int[] fixvalue_id)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var list = new List<AppFixvalue>();
                    for (int i = 0; i < fixvalue_id.Length; i++)
                    {
                        var single = await context.AppFixvalues.SingleOrDefaultAsync(u => u.FixvalueId == fixvalue_id[i]);
                        if (single != null)
                        {
                            list.Add(single);
                        }
                    }
                    if (list.Count > 0)
                    {
                        context.AppFixvalues.RemoveRange(list);
                        context.SaveChanges();
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除公共代码成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除公共代码失败。" + ex.Message);
                    return Json(new { code = 300, msg = "删除失败" });
                }
            }
        }
        #endregion

        #region 更改公共代码状态：生效或失效
        [HttpPost]
        public async Task<IActionResult> ChangeFixValueStatus(int[] fixvalue_id)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var list = new List<AppFixvalue>();
                    for (int i = 0; i < fixvalue_id.Length; i++)
                    {
                        var single = await context.AppFixvalues.SingleOrDefaultAsync(u => u.FixvalueId == fixvalue_id[i]);
                        if (single != null)
                        {
                            single.Status = single.Status == "有效" ? "失效" : "有效";
                            single.LastModifiedDate = DateTime.Now;
                            single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                            list.Add(single);
                        }
                    }
                    if (list.Count > 0)
                    {
                        context.AppFixvalues.UpdateRange(list);
                        context.SaveChanges();
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "更改公共代码状态成功。");
                    return Json(new { code = 200, msg = "执行成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "更改公共代码状态失败。" + ex.Message);
                    return Json(new { code = 300, msg = "执行失败" });
                }
            }
        }
        #endregion
    }
}
