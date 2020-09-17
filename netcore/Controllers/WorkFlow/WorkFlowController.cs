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

        #region 审批流类型
        [CheckCustomer]
        public IActionResult ApprType()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult ApprTypeEdit()
        {
            ViewBag.status = HttpContext.Request.Query["status"];
            ViewBag.ApprTypeId = ViewBag.status == "add" ? "" : HttpContext.Request.Query["Rowid"].ToString();
            return View();
        }

        #region 获取审批流类型列表
        [HttpGet]
        public async Task<IActionResult> GetApprTypeList(string ApprTypeCode, string ApprTypeName, string Status, int page, int limit)
        {
            try
            {
                var list = await context.ApprTypes.ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Status))
                    {
                        if (Status != "全部")
                        {
                            list = list.Where(u => u.Status == Status).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(ApprTypeCode))
                    {
                        list = list.Where(u => u.ApprTypeCode.Contains(ApprTypeCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(ApprTypeName))
                    {
                        list = list.Where(u => u.ApprTypeName.Contains(ApprTypeName)).ToList();
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
                logger.LogInformation(HttpContext.Session.GetString("who") + "：查询审批流类型失败。" + ex.Message);
                return Json(new { code = 1, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 根据ID获取审批流类型
        [HttpGet]
        public async Task<IActionResult> GetApprTypeById(int ApprTypeId)
        {
            var single = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeId == ApprTypeId);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该审批流类型" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增审批流类型
        [HttpPost]
        public async Task<IActionResult> InsertApprType(
            string ApprTypeCode, string ApprTypeName, string TableName, string TablePkName, string TableStatusName,
            string TableApprIdName, string ApprStartStatus, string ApprEndStatus, string ApprCancelStatus,
            string PageViewUrl, string TransProcName)
        {
            try
            {
                var single = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeCode == ApprTypeCode);
                if (single == null)
                {
                    single = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeCode == ApprTypeCode);
                    if (single == null)
                    {
                        await context.ApprTypes.AddAsync(new ApprType()
                        {
                            ApprTypeCode = ApprTypeCode ?? "",
                            ApprTypeName = ApprTypeName ?? "",
                            TableName = TableName ?? "",
                            TablePkName = TablePkName ?? "",
                            TableStatusName = TableStatusName ?? "",
                            TableApprIdName = TableApprIdName ?? "",
                            ApprStartStatus = ApprStartStatus ?? "",
                            ApprEndStatus = ApprEndStatus ?? "",
                            ApprCancelStatus = ApprCancelStatus ?? "",
                            PageViewUrl = PageViewUrl ?? "",
                            TransProcName = TransProcName ?? "",
                            Status = "有效",
                            CreationDate = DateTime.Now,
                            CreationUser = HttpContext.Session.GetInt32("user_id")
                        });
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "新增审批流类型成功。");
                        return Json(new { code = 200, msg = "新增成功" });
                    }
                    else
                    {
                        return Json(new { code = 300, msg = "审批流类型名称已存在" });
                    }
                }
                else
                {
                    return Json(new { code = 300, msg = "审批流类型代码已存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增审批流类型失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑审批流类型
        [HttpPost]
        public async Task<IActionResult> ModifyApprType(int ApprTypeId, string ApprTypeCode, string ApprTypeName,
                                                        string TableName, string TablePkName, string TableStatusName,
                                                        string TableApprIdName, string ApprStartStatus,
                                                        string ApprEndStatus, string ApprCancelStatus,
                                                        string PageViewUrl, string TransProcName)
        {
            try
            {
                var single = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeCode == ApprTypeCode);
                if (single == null)
                {
                    single = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeName == ApprTypeName);
                    if (single == null)
                    {
                        var modify = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeId == ApprTypeId);
                        if (modify != null)
                        {
                            modify.ApprTypeCode = ApprTypeCode ?? "";
                            modify.ApprTypeName = ApprTypeName ?? "";
                            modify.TableName = TableName ?? "";
                            modify.TablePkName = TablePkName ?? "";
                            modify.TableStatusName = TableStatusName ?? "";
                            modify.TableApprIdName = TableApprIdName ?? "";
                            modify.ApprStartStatus = ApprStartStatus ?? "";
                            modify.ApprEndStatus = ApprEndStatus ?? "";
                            modify.ApprCancelStatus = ApprCancelStatus ?? "";
                            modify.PageViewUrl = PageViewUrl ?? "";
                            modify.TransProcName = TransProcName ?? "";
                            context.ApprTypes.Update(modify);
                            await context.SaveChangesAsync();
                            logger.LogInformation(HttpContext.Session.GetString("who") + "编辑审批流类型成功。");
                            return Json(new { code = 200, msg = "编辑成功" });
                        }
                        else
                        {
                            return Json(new { code = 300, msg = "审批流类型不存在" });
                        }
                    }
                    else
                    {
                        if (single.ApprTypeId != ApprTypeId)
                        {
                            return Json(new { code = 300, msg = "审批流类型名称已存在" });
                        }
                        else
                        {
                            var modify = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeId == ApprTypeId);
                            if (modify != null)
                            {
                                modify.ApprTypeCode = ApprTypeCode ?? "";
                                modify.ApprTypeName = ApprTypeName ?? "";
                                modify.TableName = TableName ?? "";
                                modify.TablePkName = TablePkName ?? "";
                                modify.TableStatusName = TableStatusName ?? "";
                                modify.TableApprIdName = TableApprIdName ?? "";
                                modify.ApprStartStatus = ApprStartStatus ?? "";
                                modify.ApprEndStatus = ApprEndStatus ?? "";
                                modify.ApprCancelStatus = ApprCancelStatus ?? "";
                                modify.PageViewUrl = PageViewUrl ?? "";
                                modify.TransProcName = TransProcName ?? "";
                                context.ApprTypes.Update(modify);
                                await context.SaveChangesAsync();
                                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑审批流类型成功。");
                                return Json(new { code = 200, msg = "编辑成功" });
                            }
                            else
                            {
                                return Json(new { code = 300, msg = "审批流类型不存在" });
                            }
                        }
                    }
                }
                else
                {
                    if (single.ApprTypeId != ApprTypeId)
                    {
                        return Json(new { code = 300, msg = "审批流类型代码已存在" });
                    }
                    else
                    {
                        single = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeName == ApprTypeName);
                        if (single == null)
                        {
                            var modify = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeId == ApprTypeId);
                            if (modify != null)
                            {
                                modify.ApprTypeCode = ApprTypeCode ?? "";
                                modify.ApprTypeName = ApprTypeName ?? "";
                                modify.TableName = TableName ?? "";
                                modify.TablePkName = TablePkName ?? "";
                                modify.TableStatusName = TableStatusName ?? "";
                                modify.TableApprIdName = TableApprIdName ?? "";
                                modify.ApprStartStatus = ApprStartStatus ?? "";
                                modify.ApprEndStatus = ApprEndStatus ?? "";
                                modify.ApprCancelStatus = ApprCancelStatus ?? "";
                                modify.PageViewUrl = PageViewUrl ?? "";
                                modify.TransProcName = TransProcName ?? "";
                                context.ApprTypes.Update(modify);
                                await context.SaveChangesAsync();
                                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑审批流类型成功。");
                                return Json(new { code = 200, msg = "编辑成功" });
                            }
                            else
                            {
                                return Json(new { code = 300, msg = "审批流类型不存在" });
                            }
                        }
                        else
                        {
                            if (single.ApprTypeId != ApprTypeId)
                            {
                                return Json(new { code = 300, msg = "审批流类型名称已存在" });
                            }
                            else
                            {
                                var modify = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeId == ApprTypeId);
                                if (modify != null)
                                {
                                    modify.ApprTypeCode = ApprTypeCode ?? "";
                                    modify.ApprTypeName = ApprTypeName ?? "";
                                    modify.TableName = TableName ?? "";
                                    modify.TablePkName = TablePkName ?? "";
                                    modify.TableStatusName = TableStatusName ?? "";
                                    modify.TableApprIdName = TableApprIdName ?? "";
                                    modify.ApprStartStatus = ApprStartStatus ?? "";
                                    modify.ApprEndStatus = ApprEndStatus ?? "";
                                    modify.ApprCancelStatus = ApprCancelStatus ?? "";
                                    modify.PageViewUrl = PageViewUrl ?? "";
                                    modify.TransProcName = TransProcName ?? "";
                                    context.ApprTypes.Update(modify);
                                    await context.SaveChangesAsync();
                                    logger.LogInformation(HttpContext.Session.GetString("who") + "编辑审批流类型成功。");
                                    return Json(new { code = 200, msg = "编辑成功" });
                                }
                                else
                                {
                                    return Json(new { code = 300, msg = "审批流类型不存在" });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑审批流类型失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除审批流类型
        [HttpPost]
        public async Task<IActionResult> DeleteApprType(int[] id)
        {
            try
            {
                var list = new List<ApprType>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeId == id[i]);
                    if (single != null)
                    {
                        list.Add(single);
                    }
                }
                if (list.Count > 0)
                {
                    context.ApprTypes.RemoveRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除审批流类型成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除审批流类型失败：审批流类型不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "审批流类型不存在或未勾选数据" });
                }

            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "删除审批流类型失败。" + ex.Message);
                return Json(new { code = 300, msg = "删除失败" });
            }
        }
        #endregion

        #region 生效审批流类型
        [HttpPost]
        public async Task<IActionResult> EnableStatusForApprType(int[] id)
        {
            try
            {
                var list = new List<ApprType>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeId == id[i]);
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
                    context.ApprTypes.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效审批流类型成功。");
                    return Json(new { code = 200, msg = "生效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效审批流类型失败，审批流类型不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "审批流类型不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "生效审批流类型失败。" + ex.Message);
                return Json(new { code = 300, msg = "生效失败" });
            }
        }
        #endregion

        #region 失效
        [HttpPost]
        public async Task<IActionResult> FailureStatusForApprType(int[] id)
        {
            try
            {
                var list = new List<ApprType>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeId == id[i]);
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
                    context.ApprTypes.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效审批流类型成功。");
                    return Json(new { code = 200, msg = "失效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效审批流类型失败，审批流类型不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "审批流类型不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "失效审批流类型失败。" + ex.Message);
                return Json(new { code = 300, msg = "失效失败" });
            }
        }
        #endregion

        #endregion

        #region 审批流
        [CheckCustomer]
        public IActionResult ApprFlow()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult ApprFlowEdit()
        {
            ViewBag.status = HttpContext.Request.Query["status"];
            ViewBag.ApprFlowId = ViewBag.status == "add" ? "" : HttpContext.Request.Query["Rowid"].ToString() ?? "";
            return View();
        }

        [CheckCustomer]
        public IActionResult ApprFlowSetting()
        {
            ViewBag.ApprFlowId = HttpContext.Request.Query["Rowid"].ToString()??"";
            return View();
        }

        #region 获取有效审批流类型
        [HttpGet]
        public async Task<IActionResult> GetApprType()
        {
            try
            {
                var list = await context.ApprTypes.Where(u => u.Status == "有效").ToListAsync();
                return Json(new { code = 0, msg = "获取成功", data = list });
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "获取有效审批流类型出错。" + ex.Message);
                return Json(new { code = 1, msg = "查询时出错，请联系管理员", data = new { } });
            }
        }
        #endregion

        #region 获取审批流列表
        [HttpGet]
        public async Task<IActionResult> GetApprFlowList(int ApprTypeId, string ApprFlowName, int page, int limit)
        {
            try
            {
                var list = await (from f in context.ApprFlows
                                  join t in context.ApprTypes
                                  on f.ApprTypeId equals t.ApprTypeId
                                  select new
                                  {
                                      f.ApprFlowId,
                                      f.ApprTypeId,
                                      f.ApprFlowName,
                                      t.ApprTypeName,
                                      f.Note
                                  }).ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    if (ApprTypeId != -99)
                    {
                        list = list.Where(u => u.ApprTypeId == ApprTypeId).ToList();
                    }
                    if (!string.IsNullOrEmpty(ApprFlowName))
                    {
                        list = list.Where(u => u.ApprFlowName.Contains(ApprFlowName)).ToList();
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
                logger.LogInformation(HttpContext.Session.GetString("who") + "：查询审批流失败。" + ex.Message);
                return Json(new { code = 1, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 根据ID获取审批流
        [HttpGet]
        public async Task<IActionResult> GetApprFlowById(int ApprFlowId)
        {
            var single = await context.ApprFlows.SingleOrDefaultAsync(u => u.ApprFlowId == ApprFlowId);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该审批流" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增审批流
        [HttpPost]
        public async Task<IActionResult> InsertApprFlow(int ApprTypeId, string ApprFlowName, string Note)
        {
            try
            {
                var single = await context.ApprFlows.SingleOrDefaultAsync(u => u.ApprFlowName == ApprFlowName);
                if (single == null)
                {
                    await context.ApprFlows.AddAsync(new ApprFlow()
                    {
                        ApprTypeId = ApprTypeId,
                        ApprFlowName = ApprFlowName ?? "",
                        Note = Note ?? "",
                        CreationDate = DateTime.Now,
                        CreationUser = HttpContext.Session.GetInt32("user_id")
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增审批流成功。");
                    return Json(new { code = 200, msg = "新增成功" });
                }
                else
                {
                    return Json(new { code = 300, msg = "审批流名称已存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增审批流失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑审批流
        [HttpPost]
        public async Task<IActionResult> ModifyApprFlow(int ApprFlowId,int ApprTypeId,  string ApprFlowName,string Note)
        {
            try
            {
                var single = await context.ApprFlows.SingleOrDefaultAsync(u => u.ApprFlowId == ApprFlowId);
                if (single == null)
                {
                    return Json(new { code = 300, msg = "审批流不存在" });
                }
                else
                {
                    var check = await context.ApprFlows.SingleOrDefaultAsync(u => u.ApprFlowName == ApprFlowName);
                    if (check != null)
                    {
                        if (single.ApprFlowId != check.ApprFlowId)
                        {
                            return Json(new { code = 300, msg = "审批流名称已存在" });
                        }
                        else
                        {
                            single.ApprTypeId = ApprTypeId ;
                            single.ApprFlowName = ApprFlowName ?? "";
                            single.Note = Note ?? "";
                            context.ApprFlows.Update(single);
                            await context.SaveChangesAsync();
                            logger.LogInformation(HttpContext.Session.GetString("who") + "编辑审批流成功。");
                            return Json(new { code = 200, msg = "编辑成功" });
                        }
                    }
                    else
                    {
                        single.ApprTypeId = ApprTypeId;
                        single.ApprFlowName = ApprFlowName ?? "";
                        single.Note = Note ?? "";
                        context.ApprFlows.Update(single);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "编辑审批流成功。");
                        return Json(new { code = 200, msg = "编辑成功" });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑审批流类型失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除审批流
        [HttpPost]
        public async Task<IActionResult> DeleteApprFlow(int[] id)
        {
            try
            {
                var list = new List<ApprFlow>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.ApprFlows.SingleOrDefaultAsync(u => u.ApprFlowId == id[i]);
                    if (single != null)
                    {
                        list.Add(single);
                    }
                }
                if (list.Count > 0)
                {
                    context.ApprFlows.RemoveRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除审批流成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除审批流失败：审批流不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "审批流不存在或未勾选数据" });
                }

            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "删除审批流失败。" + ex.Message);
                return Json(new { code = 300, msg = "删除失败" });
            }
        }
        #endregion

        #endregion
    }
}
