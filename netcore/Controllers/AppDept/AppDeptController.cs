using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using netcore.Models;
using netcore.Models.Excel;
using netcore.Models.Excel.ExcelClass;
using netcore.Models.Excel.ExcelTitle;

namespace netcore.Controllers.AppDept
{
    public class AppDeptController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<AppDeptController> logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        public AppDeptController(netcore_databaseContext _context, ILogger<AppDeptController> _logger, IWebHostEnvironment _hostingEnvironment)
        {
            context = _context;
            logger = _logger;
            hostingEnvironment = _hostingEnvironment;
        }

        [CheckCustomer]
        public IActionResult AppDept()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult AppDeptEdit()
        {
            ViewBag.status = HttpContext.Request.Query["status"];
            ViewBag.DeptId = ViewBag.status == "add" ? "" : HttpContext.Request.Query["Rowid"].ToString();
            return View();
        }

        #region 获取列表
        [HttpGet]
        public async Task<IActionResult> GetList(int CorpId, string Status,string DeptCode,string DeptName, int page, int limit)
        {
            try
            {
                var list = await (from b in context.AppDepts
                                  join c in context.AppCorps
                                  on b.CorpId equals c.CorpId
                                  select new
                                  {
                                      b.DeptId,
                                      b.DeptCode,
                                      b.DeptName,
                                      CorpName = c.CorpName,
                                      b.CorpId,
                                      b.Note,
                                      b.Status
                                  }).ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    if (CorpId != -99)
                    {
                        list = list.Where(u => u.CorpId == CorpId).ToList();
                    }
                    if (!string.IsNullOrEmpty(Status))
                    {
                        if (Status != "全部")
                        {
                            list = list.Where(u => u.Status == Status).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(DeptCode))
                    {
                        list= list.Where(u => u.DeptCode.Contains(DeptCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(DeptName))
                    {
                        list = list.Where(u => u.DeptName.Contains(DeptName)).ToList();
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
                logger.LogInformation(HttpContext.Session.GetString("who") + "：查询部门失败。" + ex.Message);
                return Json(new { code = 0, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 根据ID获取信息
        [HttpGet]
        public async Task<IActionResult> GetById(int DeptId)
        {
            var single = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptId == DeptId);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该部门信息" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增
        [HttpPost]
        public async Task<IActionResult> Insert(int CorpId, string DeptCode, string DeptName, string Note)
        {
            try
            {
                var single = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptCode == DeptCode && u.CorpId == CorpId);
                if (single == null)
                {
                    var single1 = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptName == DeptName && u.CorpId == CorpId);
                    if (single1 == null)
                    {
                        await context.AppDepts.AddAsync(new Models.AppDept()
                        {
                            CorpId = CorpId,
                            DeptCode = DeptCode ?? "",
                            DeptName = DeptName ?? "",
                            Note = Note,
                            Status = "编辑",
                            CreationDate = DateTime.Now,
                            CreationUser = HttpContext.Session.GetInt32("user_id")
                        });
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "新增部门成功。");
                        return Json(new { code = 200, msg = "新增成功" });
                    }
                    else
                    {
                        return Json(new { code = 300, msg = "该部门名称在该公司中已经存在" });
                    }
                }
                else
                {
                    return Json(new { code = 300, msg = "该部门代码在该公司中已经存在" });
                }

            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增部门失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑
        [HttpPost]
        public async Task<IActionResult> Modify(int DeptId,int CorpId, string DeptCode, string DeptName, string Note)
        {
            try
            {
                var single = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptCode == DeptCode && u.CorpId == CorpId);
                if (single == null)
                {
                    var single1 = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptName == DeptName && u.CorpId == CorpId);
                    if (single1 == null)
                    {
                        var modify = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptId == DeptId);
                        if (modify!=null)
                        {
                            modify.CorpId = CorpId;
                            modify.DeptCode = DeptCode ?? "";
                            modify.DeptName = DeptName ?? "";
                            modify.Note = Note ?? "";
                            modify.LastModifiedDate = DateTime.Now;
                            modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                            context.AppDepts.Update(modify);
                            await context.SaveChangesAsync();
                            logger.LogInformation(HttpContext.Session.GetString("who") + "新增部门成功。");
                            return Json(new { code = 200, msg = "新增成功" });
                        }
                        else
                        {
                            return Json(new { code = 300, msg = "该部门不存在与任何公司" });
                        }
                    }
                    else
                    {
                        return Json(new { code = 300, msg = "该部门名称在该公司中已经存在" });
                    }
                }
                else
                {
                    if (single.DeptId!=DeptId)
                    {
                        return Json(new { code = 300, msg = "该部门代码在该公司中已经存在" });
                    }
                    else
                    {
                        var single1 = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptName == DeptName && u.CorpId == CorpId);
                        if (single1 == null)
                        {
                            var modify = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptId == DeptId);
                            if (modify != null)
                            {
                                modify.CorpId = CorpId;
                                modify.DeptCode = DeptCode ?? "";
                                modify.DeptName = DeptName ?? "";
                                modify.Note = Note ?? "";
                                modify.LastModifiedDate = DateTime.Now;
                                modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                                context.AppDepts.Update(modify);
                                await context.SaveChangesAsync();
                                logger.LogInformation(HttpContext.Session.GetString("who") + "新增部门成功。");
                                return Json(new { code = 200, msg = "新增成功" });
                            }
                            else
                            {
                                return Json(new { code = 300, msg = "该部门不存在与任何公司" });
                            }
                        }
                        else
                        {
                            if (single1.DeptId!=DeptId)
                            {
                                return Json(new { code = 300, msg = "该部门名称在该公司中已经存在" });
                            }
                            else
                            {
                                var modify = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptId == DeptId);
                                if (modify != null)
                                {
                                    modify.CorpId = CorpId;
                                    modify.DeptCode = DeptCode ?? "";
                                    modify.DeptName = DeptName ?? "";
                                    modify.Note = Note ?? "";
                                    modify.LastModifiedDate = DateTime.Now;
                                    modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                                    context.AppDepts.Update(modify);
                                    await context.SaveChangesAsync();
                                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增部门成功。");
                                    return Json(new { code = 200, msg = "新增成功" });
                                }
                                else
                                {
                                    return Json(new { code = 300, msg = "该部门不存在与任何公司" });
                                }
                            }                            
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增部门失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
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
                    for (int i = 0; i < id.Length; i++)
                    {
                        var single = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptId == id[i]);
                        if (single != null)
                        {
                            var list = await context.AppPosts.Where(u=>u.DeptId==id[i]).ToListAsync();
                            if (list.Count>0)
                            {
                                context.AppPosts.RemoveRange(list);
                                context.SaveChanges();
                            }
                            var list4 = await context.AppUsers.Where(u => u.DeptId == id[i]).ToListAsync();
                            if (list4.Count > 0)
                            {
                                for (int k = 0; k < list4.Count; k++)
                                {
                                    list4[i].DeptId = -999;
                                    list4[i].PostId = -999;
                                    list4[i].Status = "失效";
                                    list4[4].LastModifiedDate = DateTime.Now;
                                    list4[i].LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                                    context.AppUsers.Update(list4[i]);
                                    context.SaveChanges();
                                }
                            }
                            context.AppDepts.Remove(single);
                            context.SaveChanges();
                        }
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除部门成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除部门失败。" + ex.Message);
                    return Json(new { code = 300, msg = "删除失败" });
                }
            }
        }
        #endregion

        #region 生效
        [HttpPost]
        public async Task<IActionResult> EnableStatus(int[] id)
        {
            try
            {
                var list = new List<Models.AppDept>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptId == id[i]);
                    if (single != null)
                    {
                        single.Status = "有效";
                        single.LastModifiedDate = DateTime.Now;
                        single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        list.Add(single);
                    }
                }
                if (list.Count > 0)
                {
                    context.AppDepts.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效部门成功。");
                    return Json(new { code = 200, msg = "生效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效部门失败，部门不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "部门不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "生效部门失败。" + ex.Message);
                return Json(new { code = 300, msg = "生效失败" });
            }
        }
        #endregion

        #region 失效
        [HttpPost]
        public async Task<IActionResult> FailureStatus(int[] id)
        {
            try
            {
                var list = new List<Models.AppDept>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptId == id[i]);
                    if (single != null)
                    {
                        single.Status = "失效";
                        single.LastModifiedDate = DateTime.Now;
                        single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        list.Add(single);
                    }
                }
                if (list.Count > 0)
                {
                    context.AppDepts.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效部门成功。");
                    return Json(new { code = 200, msg = "失效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效部门失败，部门不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "部门不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "失效部门失败。" + ex.Message);
                return Json(new { code = 300, msg = "失效失败" });
            }
        }
        #endregion

        #region 恢复编辑
        [HttpPost]
        public async Task<IActionResult> ResetStatus(int[] id)
        {
            try
            {
                var list = new List<Models.AppDept>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptId == id[i]);
                    if (single != null)
                    {
                        single.Status = "编辑";
                        single.LastModifiedDate = DateTime.Now;
                        single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        list.Add(single);
                    }
                }
                if (list.Count > 0)
                {
                    context.AppDepts.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "部门恢复编辑成功。");
                    return Json(new { code = 200, msg = "恢复编辑成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "部门恢复编辑失败，部门不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "部门不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "部门恢复编辑失败。" + ex.Message);
                return Json(new { code = 300, msg = "恢复编辑失败" });
            }
        }
        #endregion

        #region 导出
        [HttpPost]
        public async Task<IActionResult> Export(int CorpId, string Status,string DeptCode,string DeptName)
        {
            try
            {
                if (CorpId == -99)
                {
                    var list = await (from b in context.AppDepts
                                      join c in context.AppCorps
                                      on b.CorpId equals c.CorpId
                                      select new
                                      {
                                          CorpName = c.CorpName,
                                          b.DeptCode,
                                          b.DeptName,
                                          b.Note,
                                          b.Status
                                      }).ToListAsync();
                    if (!string.IsNullOrEmpty(Status))
                    {
                        if (Status != "全部")
                        {
                            list = list.Where(u => u.Status == Status).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(DeptCode))
                    {
                        list = list.Where(u => u.DeptCode.Contains(DeptCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(DeptName))
                    {
                        list = list.Where(u => u.DeptName.Contains(DeptName)).ToList();
                    }
                    byte[] buffer = ExcelHelper.Export(list, "部门信息", ExcelTitle.AppDept).GetBuffer();
                    return File(buffer, "application/ms-excel", "部门信息.xls");
                }
                else
                {
                    var list = await (from b in context.AppDepts
                                      join c in context.AppCorps
                                      on b.CorpId equals c.CorpId
                                      where b.CorpId.Equals(CorpId)
                                      select new
                                      {
                                          CorpName = c.CorpName,
                                          b.DeptCode,
                                          b.DeptName,
                                          b.Note,
                                          b.Status
                                      }).ToListAsync();
                    if (!string.IsNullOrEmpty(Status))
                    {
                        if (Status != "全部")
                        {
                            list = list.Where(u => u.Status == Status).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(DeptCode))
                    {
                        list = list.Where(u => u.DeptCode.Contains(DeptCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(DeptName))
                    {
                        list = list.Where(u => u.DeptName.Contains(DeptName)).ToList();
                    }
                    byte[] buffer = ExcelHelper.Export(list, "部门信息", ExcelTitle.AppDept).GetBuffer();
                    return File(buffer, "application/ms-excel", "部门信息.xls");
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：导出部门信息失败。" + ex.Message);
                return File(new byte[] { }, "application/ms-excel", "部门信息.xls");
            }
        }
        #endregion

        #region 导入
        [HttpPost]
        public async Task<IActionResult> Import([FromForm] IFormFile file)
        {
            if (file.Length > 0)
            {
                long fileSize = file.Length / 5242880;
                if (fileSize > 15)
                {
                    return Json(new { code = 300, msg = "文件不能大于15M", returnMsg = "文件不能大于15M" });
                }
                else
                {
                    var path = hostingEnvironment.WebRootPath + "\\" + Guid.NewGuid() + file.FileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    ExcelHelper excelHelper = new ExcelHelper();
                    List<ExcelAppDept> excel = excelHelper.GetList<ExcelAppDept>(path).ToList();
                    var returnMsg = "";
                    if (excel.Count > 0)
                    {
                        using (var tran = context.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 0; i < excel.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(excel[i].CorpName))
                                    {
                                        returnMsg += "第" + (i + 1) + "行：所属公司不能为空。\r\n";
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(excel[i].DeptCode))
                                        {
                                            returnMsg += "第" + (i + 1) + "行：部门代码不能为空。\r\n";
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(excel[i].DeptName))
                                            {
                                                returnMsg += "第" + (i + 1) + "行：部门名称不能为空。\r\n";
                                            }
                                            else
                                            {
                                                var corp = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpName == excel[i].CorpName);
                                                if (corp != null)
                                                {
                                                    var code = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptCode == excel[i].DeptCode && u.CorpId == corp.CorpId);
                                                    if (code != null)
                                                    {
                                                        returnMsg += "第" + (i + 1) + "行：该部门代码在所属公司下已存在。\r\n";
                                                    }
                                                    else
                                                    {
                                                        var name = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptName == excel[i].DeptName && u.CorpId == corp.CorpId);
                                                        if (name != null)
                                                        {
                                                            returnMsg += "第" + (i + 1) + "行：该部门名称在所属公司下已存在。\r\n";
                                                        }
                                                        else
                                                        {
                                                            context.AppDepts.Add(new Models.AppDept()
                                                            {
                                                                DeptCode = excel[i].DeptCode ?? "",
                                                                DeptName = excel[i].DeptName ?? "",
                                                                CorpId = corp.CorpId,
                                                                Note = excel[i].Note ?? "",
                                                                Status = excel[i].Status ?? "",
                                                                CreationDate = DateTime.Now,
                                                                CreationUser = HttpContext.Session.GetInt32("user_id")
                                                            });
                                                            await context.SaveChangesAsync();
                                                            returnMsg += "第" + (i + 1) + "行【" + excel[i].DeptCode + "】" + excel[i].DeptName + "：执行成功。\r\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    returnMsg += "第" + (i + 1) + "行【" + excel[i].CorpName + "】：公司不存在。\r\n";
                                                }
                                            }
                                        }

                                    }
                                }
                                await tran.CommitAsync();
                                //找到文件，删除
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                return Json(new { code = 200, msg = "执行成功", returnMsg = returnMsg });
                            }
                            catch (Exception ex)
                            {
                                await tran.RollbackAsync();
                                //找到文件，删除
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                return Json(new { code = 300, msg = "执行失败", returnMsg = ex.Message });
                            }
                        }
                    }
                }
                return Json(new { code = 300, msg = "", returnMsg = "" });
            }
            else
            {
                return Json(new { code = 300, msg = "没有文件", returnMsg = "没有文件" });
            }
        }
        #endregion
    }
}
