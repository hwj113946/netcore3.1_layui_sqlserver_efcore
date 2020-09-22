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
using Newtonsoft.Json.Linq;

namespace netcore.Controllers.WorkFlow
{
    public class WorkFlowController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<WorkFlowController> logger;
        private List<FlowNode> NewFlowNodes = null;
        private int NextNodeId = -99;
        private string NextNodeName = "";
        private string NextNodeType = "";
        public WorkFlowController(netcore_databaseContext _context, ILogger<WorkFlowController> _logger)
        {
            context = _context;
            logger = _logger;
            NewFlowNodes = new List<FlowNode>();
        }

        [CheckCustomer]
        public IActionResult WorkFlow()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult FlowNodes()
        {
            ViewBag.ApprId = "";
            return View();
        }

        [CheckCustomer]
        public IActionResult ChooseNextNodeApprUser()
        {
            ViewBag.CorpId = HttpContext.Session.GetInt32("CorpId");
            ViewBag.DeptId = HttpContext.Session.GetInt32("DeptId");
            ViewBag.PostId = HttpContext.Session.GetInt32("PostId");
            return View();
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
            ViewBag.FlowId = HttpContext.Request.Query["Rowid"].ToString()??"";
            return View();
        }

        #region 获取有效审批流类型
        [HttpGet]
        public async Task<IActionResult> GetApprType()
        {
            try
            {
                var list = await (from r in (
    (from t in context.ApprTypes
     join f in context.ApprFlows
           on new { t.ApprTypeId, t.Status }
       equals new { ApprTypeId = (int)f.ApprTypeId, Status = "有效" } into f_join
     from f in f_join.DefaultIfEmpty()
     select new
     {
         t.ApprTypeId,
         t.ApprTypeName,
         Status =
       f.ApprFlowId == null ? "未选" : "已选"
     }))
                                  where
                                    r.Status == "未选"
                                  select new
                                  {
                                      r.ApprTypeId,
                                      r.ApprTypeName,
                                      r.Status
                                  }).ToListAsync();
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
            var single = await (from f in context.ApprFlows
                                join t in context.ApprTypes
                                on f.ApprTypeId equals t.ApprTypeId
                                where f.ApprFlowId.Equals(ApprFlowId)
                                select new
                                {
                                    f.ApprFlowId,
                                    f.ApprTypeId,
                                    f.ApprFlowName,
                                    f.Note,
                                    t.ApprTypeName
                                }).SingleOrDefaultAsync();
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

        #region 连接线属性
        [CheckCustomer]
        public IActionResult WorkFlowLineUpdate()
        {
            ViewBag.FlowId = HttpContext.Request.Query["flowid"];
            ViewBag.LineCode = HttpContext.Request.Query["line_code"];
            return View();
        }

        #region 获取列
        [HttpGet]
        public async Task<IActionResult> GetColName(int FlowId)
        {
            try
            {
                var Table = await (from t in context.ApprTypes
                                   join f in context.ApprFlows
                                   on t.ApprTypeId equals f.ApprTypeId
                                   where f.ApprFlowId.Equals(FlowId)
                                   select new
                                   {
                                       t.TableName
                                   }).SingleOrDefaultAsync();
                var sql = @"SELECT 
                             a.name as ColName,
                           Isnull(g.[value], '') as Comments
                    FROM   syscolumns a
                           LEFT JOIN systypes b
                                  ON a.xusertype = b.xusertype
                           INNER JOIN sysobjects d
                                   ON a.id = d.id
                                      AND d.xtype = 'U'
                                      AND d.name <> 'dtproperties'
                           LEFT JOIN syscomments e
                                  ON a.cdefault = e.id
                           LEFT JOIN sys.extended_properties g
                                  ON a.id = g.major_id
                                     AND a.colid = g.minor_id
                           LEFT JOIN sys.extended_properties f
                                  ON d.id = f.major_id
                                     AND f.minor_id = 0
                    WHERE  d.name = @tableName
                    ORDER  BY  a.colorder ";
                var table = context.Database.SqlQuery(sql, new object[] { new SqlParameter("@tableName", Table.TableName) });
                return Json(new { code = 0, msg = "查询成功", count = table.Rows.Count, data = ("[" + JsonTools.DataTableToJson(table) + "]").ToJson() });
            }
            catch (Exception ex)
            {
                return Json(new { code = 1, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 连接线名称更改
        [HttpPost]
        public async Task<IActionResult> WorkFlowLineUpdate(string Code, string Name)
        {
            try
            {
                var modify = await context.FlowLines.SingleOrDefaultAsync(u => u.LineCode == Code);
                if (modify != null)
                {
                    var flowing = await (from t in context.Apprs
                                         where
                                           t.ApprFlowId == modify.ApprFlowId &&
                                           (new string[] { "审批中" }).Contains(t.Status)
                                         select new
                                         {
                                             t
                                         }).ToListAsync();
                    if (flowing.Count > 0)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "检测到该审批流存在审批中的审批，驳回修改请求。");
                        return Json(new { code = 300, msg = "检测到该审批流存在审批中的审批，驳回修改请求" });
                    }
                    else
                    {
                        modify.LineName = Name ?? "";
                        modify.LastModifiedDate = DateTime.Now;
                        modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        context.FlowLines.Update(modify);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "保存连接线名称成功。");
                        return Json(new { code = 200, msg = "保存成功" });
                    }
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "保存连接线名称失败,节点不存在。");
                    return Json(new { code = 300, msg = "连接线不存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "保存连接线失败。" + ex.Message);
                return Json(new { code = 300, msg = "保存失败" });
            }
        }
        #endregion

        #region 获取连接线属性
        [HttpGet]
        public async Task<IActionResult> GetLineProList(int FlowId,string LineCode,int page,int limit)
        {
            try
            {
                var list = await context.FlowLinePros.Where(u => u.FlowId == FlowId && u.LineCode == LineCode).ToListAsync();
                decimal count = 0;
                if (list.Count>0)
                {
                    count = Math.Ceiling(Convert.ToDecimal(list.Count) / Convert.ToDecimal(limit));
                    if (page > count)
                    {
                        return Json(new { code = 1, msg = "没有更多数据了。", count = Convert.ToInt32(count), data = new { } });
                    }
                    list = list.Skip((page - 1) * limit).Take(limit).ToList();
                }
                else
                {
                    return Json(new { code = 1, msg = "暂无数据", data = new { } });
                }
                return Json(new { code = 0, msg = "查询成功", data = list });
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "获取连接线属性出错。" + ex.Message);
                return Json(new { code = 1, msg = "查询出错，请联系管理员", data = new { } });
            }
        }
        #endregion

        #region 新增连接线属性
        [HttpPost]
        public async Task<IActionResult> InsertLinePro(int FlowId, string LineCode, string Type, string Value,
            string ColIf, string ColName)
        {
            try
            {
                var flowing = await (from t in context.Apprs
                                     where
                                       t.ApprFlowId == FlowId &&
                                       (new string[] { "审批中" }).Contains(t.Status)
                                     select new
                                     {
                                         t
                                     }).ToListAsync();
                if (flowing.Count > 0)
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "检测到该审批流存在审批中的审批，驳回修改请求。");
                    return Json(new { code = 300, msg = "检测到该审批流存在审批中的审批，驳回修改请求" });
                }
                else
                {
                    var values = "";
                    var sql1 = "";
                    switch (Type)
                    {
                        case "数字": values = Value; break;
                        case "文本": values = " '" + Value + "'"; break;
                        case "日期(yyyy-MM-dd)": values = " CONVERT(varchar(10),'" + Value + "',120)"; break;
                    }
                    switch (ColIf)
                    {
                        case "like ": sql1 = ColName + " " + ColIf + " %" + values + "% "; break;
                        case "not like ": sql1 = ColName + " " + ColIf + " %" + values + "% "; break;
                        default: sql1 = ColName + " " + ColIf + " " + values; break;
                    }
                    await context.FlowLinePros.AddAsync(new FlowLinePro()
                    {
                        FlowId = FlowId,
                        LineCode = LineCode ?? "",
                        Sql = sql1 ?? "",
                        CreationDate = DateTime.Now,
                        CreationUser = HttpContext.Session.GetInt32("user_id")
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增连接线属性成功。");
                    return Json(new { code = 200, msg = "新增成功" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增连接线属性失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 新增连接线属性：sql
        [HttpPost]
        public async Task<IActionResult> InsertLineProSql(int FlowId, string LineCode, string sqls)
        {
            try
            {
                var flowing = await (from t in context.Apprs
                                     where
                                       t.ApprFlowId == FlowId &&
                                       (new string[] { "审批中" }).Contains(t.Status)
                                     select new
                                     {
                                         t
                                     }).ToListAsync();
                if (flowing.Count > 0)
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "检测到该审批流存在审批中的审批，驳回修改请求。");
                    return Json(new { code = 300, msg = "检测到该审批流存在审批中的审批，驳回修改请求" });
                }
                else
                {
                    await context.FlowLinePros.AddAsync(new FlowLinePro()
                    {
                        FlowId = FlowId,
                        LineCode = LineCode ?? "",
                        Sql = sqls ?? "",
                        CreationDate = DateTime.Now,
                        CreationUser = HttpContext.Session.GetInt32("user_id")
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增连接线属性成功。");
                    return Json(new { code = 200, msg = "新增成功" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增连接线属性失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑连接线属性
        [HttpPost]
        public async Task<IActionResult> ModifyLinePro(int ProId, string sqls)
        {
            try
            {
                var modify = await context.FlowLinePros.SingleOrDefaultAsync(u => u.LineProId == ProId);
                if (modify != null)
                {
                    var flowing = await (from t in context.Apprs
                                         where
                                           t.ApprFlowId == modify.FlowId &&
                                           (new string[] { "审批中" }).Contains(t.Status)
                                         select new
                                         {
                                             t
                                         }).ToListAsync();
                    if (flowing.Count > 0)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "检测到该审批流存在审批中的审批，驳回修改请求。");
                        return Json(new { code = 300, msg = "检测到该审批流存在审批中的审批，驳回修改请求" });
                    }
                    else
                    {
                        modify.Sql = sqls ?? "";
                        modify.LastModifiedDate = DateTime.Now;
                        modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        context.FlowLinePros.Update(modify);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "编辑连接线属性成功。");
                        return Json(new { code = 200, msg = "编辑成功" });
                    }
                }
                else
                {
                    return Json(new { code = 300, msg = "该连接线属性不存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑连接线属性失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除连接线属性
        [HttpPost]
        public async Task<IActionResult> DeleteLinePro(int id)
        {
            try
            {
                var single = await context.FlowLinePros.SingleOrDefaultAsync(u => u.LineProId == id);
                if (single != null)
                {
                    var flowing = await (from t in context.Apprs
                                         where
                                           t.ApprFlowId == single.FlowId &&
                                           (new string[] { "审批中" }).Contains(t.Status)
                                         select new
                                         {
                                             t
                                         }).ToListAsync();
                    if (flowing.Count > 0)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "检测到该审批流存在审批中的审批，驳回修改请求。");
                        return Json(new { code = 300, msg = "检测到该审批流存在审批中的审批，驳回修改请求" });
                    }
                    else
                    {
                        context.FlowLinePros.Remove(single);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "删除连接线属性成功。");
                        return Json(new { code = 200, msg = "删除成功" });
                    }
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除连接线属性失败：连接线属性不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "连接线属性不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "删除连接线属性失败。" + ex.Message);
                return Json(new { code = 300, msg = "删除失败" });
            }
        }
        #endregion

        #endregion

        #region 节点属性
        [CheckCustomer]
        public IActionResult WorkFlowNodeUpdate()
        {
            ViewBag.FlowId = HttpContext.Request.Query["flowid"];
            ViewBag.NodeCode = HttpContext.Request.Query["node_code"];
            ViewBag.NodeName = HttpContext.Request.Query["node_name"];
            return View();
        }

        [CheckCustomer]
        public IActionResult ChooseApprUser()
        {
            ViewBag.CorpId = HttpContext.Session.GetInt32("CorpId");
            ViewBag.DeptId = HttpContext.Session.GetInt32("DeptId");
            ViewBag.PostId = HttpContext.Session.GetInt32("PostId");
            return View();
        }

        #region 获取用户
        [HttpGet]
        public async Task<IActionResult> GetApprUserList(int CorpId, int DeptId, int PostId, string UserName, int page, int limit)
        {
            try
            {
                var iqa = context.AppUsers.Where(u => u.Status == "有效");
                if (DeptId != 0)
                {
                    iqa = context.AppUsers.Where(u => u.DeptId == DeptId);
                }
                else
                {
                    if ( CorpId != 0)
                    {
                        iqa = context.AppUsers.Where(u => u.CorpId == CorpId);
                    }
                }
                if (PostId != 0)
                {
                    iqa = context.AppUsers.Where(u => u.PostId == PostId);
                }
                if (!string.IsNullOrEmpty(UserName))
                {
                    iqa = context.AppUsers.Where(u => u.UserName.Contains(UserName));
                }
                var list = await iqa.ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
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
                logger.LogInformation(HttpContext.Session.GetString("who") + "查询失败。" + ex.Message);
                return Json(new
                {
                    code = 1,
                    msg = "查询失败，请联系管理员",
                    count = 0,
                    data = new { }
                });
            }
        }
        #endregion 

        #region 根据ID获取节点属性信息
        [HttpGet]
        public async Task<IActionResult> GetNodePro(int FlowId,string NodeCode)
        {
            var single = await context.FlowNodePros.SingleOrDefaultAsync(u => u.FlowId == FlowId && u.NodeCode == NodeCode);
            return Json(new { code = single == null ? 0 : 0, msg = single == null ? "查询不到该节点属性信息" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增或编辑节点属性
        [HttpPost]
        public async Task<IActionResult> InsertOrModifyNodePro(int FlowId, string NodeCode, string PageViewUrl,int ApprUserId,
            int ApprCorpId,int ApprDeptId,int ApprPostId)
        {
            try
            {
                var flowing = await (from t in context.Apprs
                                     where
                                       t.ApprFlowId == FlowId &&
                                       (new string[] { "审批中" }).Contains(t.Status)
                                     select new
                                     {
                                         t
                                     }).ToListAsync();
                if (flowing.Count > 0)
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "检测到该审批流存在审批中的审批，驳回修改请求。");
                    return Json(new { code = 300, msg = "检测到该审批流存在审批中的审批，驳回修改请求" });
                }
                else
                {
                    var modify = await context.FlowNodePros.SingleOrDefaultAsync(u => u.FlowId == FlowId && u.NodeCode == u.NodeCode);
                    if (modify != null)
                    {
                        modify.PageViewUrl = PageViewUrl ?? "";
                        modify.ApprUserId = ApprUserId;
                        modify.ApprCorpId = ApprCorpId;
                        modify.ApprDeptId = ApprDeptId;
                        modify.ApprPostId = ApprPostId;
                        modify.LastModifiedDate = DateTime.Now;
                        modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        context.FlowNodePros.Update(modify);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "保存节点属性成功。");
                    }
                    else
                    {
                        await context.FlowNodePros.AddAsync(new FlowNodePro()
                        {
                            FlowId = FlowId,
                            NodeCode = NodeCode ?? "",
                            PageViewUrl = PageViewUrl ?? "",
                            ApprUserId = ApprUserId,
                            ApprCorpId = ApprCorpId,
                            ApprDeptId = ApprDeptId,
                            ApprPostId = ApprPostId,
                            CreationDate = DateTime.Now,
                            CreationUser = HttpContext.Session.GetInt32("user_id")
                        });
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "新增节点属性成功。");
                    }
                    return Json(new { code = 200, msg = "保存成功" });
                }                
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "保存节点属性失败。" + ex.Message);
                return Json(new { code = 300, msg = "保存失败" });
            }
        }
        #endregion

        #region 节点名称更改
        [HttpPost]
        public async Task<IActionResult> WorkFlowNodeUpdate(string Code, string Name)
        {
            try
            {
                var modify = await context.FlowNodes.SingleOrDefaultAsync(u => u.NodeCode == Code);
                if (modify != null)
                {
                    var flowing = await (from t in context.Apprs
                                         where
                                           t.ApprFlowId == modify.ApprFlowId &&
                                           (new string[] { "审批中" }).Contains(t.Status)
                                         select new
                                         {
                                             t
                                         }).ToListAsync();
                    if (flowing.Count > 0)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "检测到该审批流存在审批中的审批，驳回修改请求。");
                        return Json(new { code = 300, msg = "检测到该审批流存在审批中的审批，驳回修改请求" });
                    }
                    else
                    {
                        modify.NodeName = Name ?? "";
                        modify.LastModifiedDate = DateTime.Now;
                        modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        context.FlowNodes.Update(modify);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "保存节点名称成功。");
                        return Json(new { code = 200, msg = "保存成功" });
                    }
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "保存节点名称失败,节点不存在。");
                    return Json(new { code = 300, msg = "节点不存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "保存节点名称失败。" + ex.Message);
                return Json(new { code = 300, msg = "保存失败" });
            }
        } 
        #endregion

        #endregion

        #region 保存设计
        [HttpPost]
        public async Task<IActionResult> SaveWorkFlow(int FlowId, string JsonData)
        {
            var flowing = await (from t in context.Apprs
                              where
                                t.ApprFlowId == FlowId &&
                                (new string[] { "审批中"}).Contains(t.Status)
                              select new
                              {
                                  t
                              }).ToListAsync();
            if (flowing.Count>0)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "检测到该审批流存在审批中的审批，驳回修改请求。");
                return Json(new { code = 300, msg = "检测到该审批流存在审批中的审批，驳回修改请求" });
            }
            JObject j = JObject.Parse(JsonData);
            JToken jn = (JToken)j["nodes"];
            JToken jl = (JToken)j["lines"];
            List<WorkFlow.Node> nodes = new List<WorkFlow.Node>();
            List<WorkFlow.Line> lines = new List<WorkFlow.Line>();
            //节点
            foreach (JProperty jp in jn)
            {
                JObject node = (JObject)jp.Value;
                string n = node.ToString().Remove(node.ToString().Length - 1, 1) + ",\"id\":\"" + jp.Name + "\"}";
                WorkFlow.Node rb = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkFlow.Node>(n);
                nodes.Add(rb);
            }
            //线
            foreach (JProperty jp in jl)
            {
                JObject line = (JObject)jp.Value;
                string n = line.ToString().Remove(line.ToString().Length - 1, 1) + ",\"id\":\"" + jp.Name + "\"}";
                WorkFlow.Line rb = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkFlow.Line>(n);
                lines.Add(rb);
            }
            if (nodes.Count == 0)
            {
                var removeNodePros = await context.FlowNodePros.Where(u => u.FlowId == FlowId).ToListAsync();
                var removeLinePros = await context.FlowLinePros.Where(u => u.FlowId == FlowId).ToListAsync();
                var removeNodes = await context.FlowNodes.Where(u => u.ApprFlowId == FlowId).ToListAsync();
                var removeLines = await context.FlowLines.Where(u => u.ApprFlowId == FlowId).ToListAsync();
                if (removeNodePros.Count>0)
                {
                    context.FlowNodePros.RemoveRange(removeNodePros);
                }
                if (removeLinePros.Count > 0)
                {
                    context.FlowLinePros.RemoveRange(removeLinePros);
                }
                if (removeNodes.Count > 0)
                {
                    context.FlowNodes.RemoveRange(removeNodes);
                }
                if (removeLines.Count > 0)
                {
                    context.FlowLines.RemoveRange(removeLines);
                }
                await context.SaveChangesAsync();
                logger.LogInformation(HttpContext.Session.GetString("who") + "检测到设计中不存在节点，已清空该审批流所有相关设计。");
                return Json(new { code = 200, msg = "检测到设计中不存在节点，已清空该审批流所有相关设计" });
            }
            else
            {
                bool flag = false;
                using (var tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        var ls = await context.FlowLines.Where(u => u.ApprFlowId == FlowId).ToListAsync();
                        if (ls.Count > 0)
                        {
                            context.FlowLines.RemoveRange(ls);
                        }
                        var ns = await context.FlowNodes.Where(u => u.ApprFlowId == FlowId).ToListAsync();
                        if (ls.Count > 0)
                        {
                            context.FlowNodes.RemoveRange(ns);
                        }
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            string type = "";
                            if (nodes[i].type == "start round") { type = "start"; }
                            if (nodes[i].type == "task round") { type = "node"; }
                            if (nodes[i].type == "end round") { type = "end"; }
                            context.FlowNodes.Add(new FlowNode()
                            {
                                ApprFlowId = FlowId,
                                NodeCode = nodes[i].id,
                                NodeName = nodes[i].name,
                                Type = type ?? "",
                                Height = Convert.ToDouble(nodes[i].height),
                                Left = Convert.ToDouble(nodes[i].left),
                                Top = Convert.ToDouble(nodes[i].top),
                                Width = Convert.ToDouble(nodes[i].width),
                                Num = (i + 1),
                                CreationDate = DateTime.Now,
                                CreationUser = HttpContext.Session.GetInt32("user_id")
                            });
                        }
                        for (int i = 0; i < lines.Count; i++)
                        {
                            context.FlowLines.Add(new FlowLine()
                            {
                                ApprFlowId = FlowId,
                                LineCode = lines[i].id,
                                LineName = lines[i].name,
                                Type = "line",
                                Num = (i + 1),
                                From = lines[i].from,
                                To = lines[i].to,
                                CreationDate = DateTime.Now,
                                CreationUser = HttpContext.Session.GetInt32("user_id")
                            });
                        }
                        context.SaveChanges();
                        await tran.CommitAsync();
                        flag = true;
                        logger.LogInformation(HttpContext.Session.GetString("who") + "保存流设计成功。");
                    }
                    catch (Exception ex)
                    {
                        await tran.RollbackAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "保存流设计失败。" + ex.Message);
                    }
                }
                return flag ? Json(new { code = 200, msg = "保存成功" }) : Json(new { code = 200, msg = "保存失败" });
            }
        }
        #endregion

        #region 获取流json
        [HttpGet]
        public async Task<IActionResult> GetWorkFlowJson(int FlowId)
        {
            var flow=await context.ApprFlows.SingleOrDefaultAsync(u => u.ApprFlowId == FlowId);
            var nodes = await context.FlowNodes.Where(u => u.ApprFlowId == FlowId).ToListAsync();
            var lines= await context.FlowLines.Where(u => u.ApprFlowId == FlowId).ToListAsync();
            var n = nodes.Count > lines.Count ? nodes.Count : lines.Count;
            if (n==0)
            {
                return Json(new { title = "", nodes = new { }, lines = new { }, areas = new { }, initNum = 0 });
            }
            string jsonData = "{\"title\":\"" + flow.ApprFlowName + "\",\"nodes\":{";
            for (int i = 0; i < nodes.Count; i++)
            {
                var type = "";
                if (nodes[i].Type=="start")
                {
                    type = "start round";
                }
                if (nodes[i].Type == "node")
                {
                    type = "task round";
                }
                if (nodes[i].Type == "end")
                {
                    type = "end round";
                }
                jsonData += "\"" + nodes[i].NodeCode + "\":{\"name\":\"" + nodes[i].NodeName + "\",\"left\":" + nodes[i].Left
                    + ",\"top\":" + nodes[i].Top + ",\"type\":\"" + type + "\",\"width\":" + nodes[i].Width + ",\"height\":" + nodes[i].Height + "},";
            }
            jsonData = jsonData.Remove(jsonData.Length - 1, 1) + "},\"lines\":{";
            for (int i = 0; i < lines.Count; i++)
            {
                jsonData += "\"" + lines[i].LineCode + "\":{\"type\":\"sl\",\"from\":\"" + lines[i].From + "\",\"to\":\"" + lines[i].To
                    + "\",\"name\":\"" + lines[i].LineName + "\"},";
            }
            jsonData = jsonData.Remove(jsonData.Length - 1, 1) + "},\"areas\":{},\"initNum\":" + (nodes.Count + lines.Count) + "}";
            return Content(jsonData);
        }
        #endregion

        #region 节点：获取节点集合与节点属性
        private async Task<List<FlowLine>> GetLines(int FlowId, string From)
        {
            try
            {
                var lines = await context.FlowLines.Where(u => u.ApprFlowId == FlowId && u.From == From).ToListAsync();
                return lines;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<List<FlowLinePro>> GetLinePros(int FlowId, string From, string LineCode)
        {
            try
            {
                var linePros = await (from f in context.FlowLines
                                      join p in context.FlowLinePros
                                            on new { f.LineCode, f.ApprFlowId, Column1 = (int)f.ApprFlowId, Column2 = f.LineCode, Column3 = f.From }
                                        equals new { p.LineCode, ApprFlowId = p.FlowId, Column1 = FlowId, Column2 = LineCode, Column3 = From }
                                      select new FlowLinePro
                                      {
                                          CreationDate = p.CreationDate,
                                          CreationUser = p.CreationUser,
                                          FlowId = p.FlowId,
                                          LastModifiedDate = p.LastModifiedDate,
                                          LastModifiedUser = p.LastModifiedUser,
                                          LineCode = p.LineCode,
                                          LineProId = p.LineProId,
                                          Sql = p.Sql
                                      }).ToListAsync();
                return linePros;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region 根据开始节点代码、审批流Id、审批类型对象、源Id进行获取所对应走向的节点
        private async Task<string> GetNode(string NodeCode, int FlowId, ApprType apprType, int SourceId)
        {
            var Node = await context.FlowNodes.SingleOrDefaultAsync(u => u.NodeCode == NodeCode && u.ApprFlowId == FlowId);
            var Lines = await GetLines(FlowId, NodeCode);
            if (Lines.Count > 0)
            {
                for (int i = 0; i < Lines.Count; i++)
                {
                    var linePro = await GetLinePros(FlowId, Lines[i].From, Lines[i].LineCode);
                    if (linePro.Count > 0)
                    {
                        var sql = @"select count(*) as n from " + apprType.TableName + " t where " + apprType.TablePkName + " = @SourceId  ";
                        var sqlIf = "";
                        for (int k = 0; k < linePro.Count; k++)
                        {
                            sqlIf += " and ( " + linePro[k].Sql + " )";
                        }
                        sql = sql + sqlIf;
                        var s = context.Database.SqlQuery(sql, new[] { new SqlParameter("@SourceId", SourceId) });
                        if (s.Rows.Count > 0)
                        {
                            if (s.Rows[0]["n"].ToString() != "0")
                            {
                                NewFlowNodes.Add(Node);
                                await GetNode(Lines[i].To, FlowId, apprType, SourceId);
                                break;
                            }
                        }
                    }
                    else
                    {
                        NewFlowNodes.Add(Node);
                        await GetNode(Lines[i].To, FlowId, apprType, SourceId);
                    }
                }
                return "Y";
            }
            else
            {
                if (Node.Type == "end")
                {
                    NewFlowNodes.Add(Node);
                }
                return "Y";
            }
        }
        #endregion

        #region 根据当前节点Id、审批流id、审批流类型、源Id进行获取下一节点Id
        private async Task<string> GetNextNodeIdByCurrentNode(int NodeId, int FlowId, ApprType apprType, int SourceId)
        {
            var Node = await context.FlowNodes.SingleOrDefaultAsync(u => u.NodeId == NodeId && u.ApprFlowId == FlowId);
            var Lines = await context.FlowLines.Where(u => u.From == Node.NodeCode && u.ApprFlowId == FlowId).ToListAsync();
            for (int i = 0; i < Lines.Count; i++)
            {
                var LinePros = await context.FlowLinePros.Where(u => u.LineCode == Lines[i].LineCode && u.FlowId == FlowId).ToListAsync();
                var sql = @"select count(*) as n from " + apprType.TableName + " t where " + apprType.TablePkName + " = @SourceId  ";
                var sqlIf = "";
                for (int k = 0; k < LinePros.Count; k++)
                {
                    sqlIf += " and ( " + LinePros[k].Sql + " )";
                }
                sql = sql + sqlIf;
                var s = context.Database.SqlQuery(sql, new[] { new SqlParameter("@SourceId", SourceId) });
                if (s.Rows.Count > 0)
                {
                    if (s.Rows[0]["n"].ToString() != "0")
                    {
                        var nodeInfo = await context.FlowNodes.SingleOrDefaultAsync(u => u.ApprFlowId == FlowId && u.NodeCode == Lines[i].To);
                        NextNodeId = nodeInfo.NodeId;
                        NextNodeName = nodeInfo.NodeName;
                        NextNodeType = nodeInfo.Type;
                        break;
                    }
                }
            }
            return "Y";
        } 
        #endregion

        #region 获取审批流节点
        [HttpGet]
        public async Task<IActionResult> GetApprNodes(string ApprTypeCode,int SourceId)
        {
            try
            {
                var ApprType = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeCode == ApprTypeCode);
                if (ApprType == null)
                {
                    return Json(new { code = 1, msg = "审批流类型不存在", data = new { } });
                }
                var ApprFlow = await context.ApprFlows.SingleOrDefaultAsync(u => u.ApprTypeId == ApprType.ApprTypeId);
                if (ApprFlow == null)
                {
                    return Json(new { code = 1, msg = "审批流不存在", data = new { } });
                }
                var Appr = await (from apprs in context.Apprs
                                  where
                                    apprs.ApprFlowId == ApprFlow.ApprFlowId &&
                                    apprs.SourceId == SourceId &&
                                    (new string[] { "审批中", "审批退回" }).Contains(apprs.Status)
                                  select new
                                  {
                                      apprs
                                  }).SingleOrDefaultAsync();
                var Nodes = await context.FlowNodes.Where(u => u.ApprFlowId == ApprFlow.ApprFlowId).OrderBy(u => u.Num).ToListAsync();
                if (Nodes.Count == 0)
                {
                    return Json(new { code = 1, msg = "找不到任何审批节点", data = new { } });
                }
                var node = Nodes.Find(u=>u.Type=="start");
                //根据开始节点代码、审批流Id、审批类型对象、源Id进行获取所对应走向的节点
                await GetNode(node.NodeCode, ApprFlow.ApprFlowId, ApprType, SourceId);
                //根据当前节点Id、审批流id、审批流类型、源Id进行获取下一节点Id
                await GetNextNodeIdByCurrentNode(node.NodeId,ApprFlow.ApprFlowId,ApprType,SourceId);
                if (Appr != null)
                {
                    var ApprTran = await (from a in context.ApprTrans
                                          where
                                            a.TranNumber ==
                                              (from b in context.ApprTrans
                                               where
                                             a.ApprId == b.ApprId && b.ApprId == Appr.apprs.ApprId
                                               select new
                                               {
                                                   b.TranNumber
                                               }).Max(p => p.TranNumber)
                                          select new
                                          {
                                              a
                                          }).SingleOrDefaultAsync();
                    if (ApprTran != null)
                    {
                        return Json(new { code = 0, msg = "查询成功", data = NewFlowNodes, current_note = ApprTran.a.SubmitNodeId, NextNodeId = NextNodeId, NextNodeName = NextNodeName, NextNodeType = NextNodeType });
                    }
                }
                return Json(new { code = 0, msg = "查询成功", data = NewFlowNodes, current_note = -99, NextNodeId = NextNodeId, NextNodeName = NextNodeName, NextNodeType = NextNodeType });
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：获取审批流节点失败。" + ex.Message);
                return Json(new { code = 300, msg = "获取审批流节点失败，请联系管理员" });
            }
        }
        #endregion

        #region 获取审批流节点信息
        [HttpGet]
        public async Task<IActionResult> GetNextNodePro(int NodeId)
        {
            try
            {
                var Node = await context.FlowNodes.SingleOrDefaultAsync(u => u.NodeId == NodeId);
                if (Node == null)
                {
                    return Json(new { code = 1, msg = "节点不存在" });
                }
                var Pro = await context.FlowNodePros.SingleOrDefaultAsync(u => u.NodeCode == Node.NodeCode && u.FlowId == Node.ApprFlowId);
                if (Pro == null)
                {
                    return Json(new { code = 1, msg = "节点未设置审批人员信息" });
                }
                if (Pro.ApprUserId == 0)
                {
                    if (Pro.ApprPostId == 0)
                    {
                        if (Pro.ApprDeptId == 0)
                        {
                            if (Pro.ApprCorpId == 0)
                            {
                                return Json(new { code = 1, msg = "节点未设置审批人员信息" });
                            }
                            else
                            {
                                var user = await context.AppUsers.Where(u => u.CorpId == Pro.ApprCorpId && u.Status == "有效").ToListAsync();
                                return Json(new { code = 0, msg = "查询成功", data = user, assign = -99, UserName = "" });
                            }
                        }
                        else
                        {
                            var user = await context.AppUsers.Where(u => u.DeptId == Pro.ApprDeptId && u.Status == "有效").ToListAsync();
                            return Json(new { code = 0, msg = "查询成功", data = user, assign = -99, UserName = "" });
                        }
                    }
                    else
                    {
                        var user = await context.AppUsers.Where(u => u.PostId == Pro.ApprPostId && u.Status == "有效").ToListAsync();
                        return Json(new { code = 0, msg = "查询成功", data = user, assign = -99, UserName = "" });
                    }
                }
                else
                {
                    var UserInfo = await context.AppUsers.SingleOrDefaultAsync(u => u.UserId == Pro.ApprUserId);
                    if (Pro.ApprPostId == 0)
                    {
                        if (Pro.ApprDeptId == 0)
                        {
                            if (Pro.ApprCorpId == 0)
                            {
                                return Json(new { code = 1, msg = "节点未设置审批人员信息" });
                            }
                            else
                            {
                                var user = await context.AppUsers.Where(u => u.CorpId == Pro.ApprCorpId && u.Status == "有效").ToListAsync();
                                return Json(new { code = 0, msg = "查询成功", data = user, assign = Pro.ApprUserId, UserName = UserInfo.UserName });
                            }
                        }
                        else
                        {
                            var user = await context.AppUsers.Where(u => u.DeptId == Pro.ApprDeptId && u.Status == "有效").ToListAsync();
                            return Json(new { code = 0, msg = "查询成功", data = user, assign = Pro.ApprUserId, UserName = UserInfo.UserName });
                        }
                    }
                    else
                    {
                        var user = await context.AppUsers.Where(u => u.PostId == Pro.ApprPostId && u.Status == "有效").ToListAsync();
                        return Json(new { code = 0, msg = "查询成功", data = user, assign = Pro.ApprUserId, UserName = UserInfo.UserName });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：获取审批流节点信息失败。" + ex.Message);
                return Json(new { code = 300, msg = "获取审批流节点信息失败，请联系管理员" });
            }
            
        }
        #endregion

        #region 获取审批事务
        [HttpGet]
        public async Task<IActionResult> GetApprTran(int ApprId)
        {
            try
            {
                var tran = await (from t in context.ApprTrans
                                  join c in context.AppCorps on new { SubmitterCorp = (int)t.SubmitterCorp } equals new { SubmitterCorp = c.CorpId }
                                  join d in context.AppDepts on new { SubmitterDept = (int)t.SubmitterDept } equals new { SubmitterDept = d.DeptId }
                                  join p in context.AppPosts on new { SubmitterPost = (int)t.SubmitterPost } equals new { SubmitterPost = p.PostId }
                                  join u in context.AppUsers
                                        on new { Submitter = (int)t.Submitter, ApprId = (int)t.ApprId }
                                    equals new { Submitter = u.UserId, ApprId = ApprId }
                                  select new
                                  {
                                      t.ApprNote,
                                      t.Status,
                                      t.SubmissionTime,
                                      t.TranNumber,
                                      c.CorpName,
                                      d.DeptName,
                                      p.PostName,
                                      u.UserName
                                  }).ToListAsync();
                if (tran.Count > 0)
                {
                    return Json(new { code = 0, msg = "查询成功", count = tran.Count, data = tran });
                }
                else
                {
                    return Json(new { code = 0, msg = "查询成功", count = 0, data = new { } });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：查询审批事务出错。" + ex.Message);
                return Json(new { code = 1, msg = "查询审批事务出错", count = 0, data = new { } });
            }            
        } 
        #endregion

        #region 创建审批流
        [HttpPost]
        public async Task<IActionResult> WorkFlowStart(
            string ApprTypeCode, int SourceId, string Title, string Note, int NextApprSubmitter, string ApprNote)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var ApprType = await context.ApprTypes.SingleOrDefaultAsync(u => u.ApprTypeCode == ApprTypeCode);
                    if (ApprType == null)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，审批流类型不存在。");
                        return Json(new { code = 300, msg = "创建审批流失败，审批流类型不存在" });
                    }
                    var ApprFlow = await context.ApprFlows.SingleOrDefaultAsync(u => u.ApprTypeId == ApprType.ApprTypeId);
                    if (ApprFlow == null)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，审批流不存在。");
                        return Json(new { code = 300, msg = "创建审批流失败，审批流不存在" });
                    }
                    if (ApprType.Status == "失效")
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，审批流类型已失效。");
                        return Json(new { code = 300, msg = "创建审批流失败，审批流类型已失效" });
                    }
                    var Nodes = await context.FlowNodes.Where(u => u.ApprFlowId == ApprFlow.ApprFlowId).OrderBy(u => u.Num).ToListAsync();
                    if (Nodes.Count == 0)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，节点设置错误，不存在任何节点。");
                        return Json(new { code = 300, msg = "创建审批流失败，节点设置错误，不存在任何节点" });
                    }
                    var nodeStart = Nodes.Find(u => u.Type == "start");
                    if (nodeStart == null)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，节点设置错误，起始节点不存在。");
                        return Json(new { code = 300, msg = "创建审批流失败，节点设置错误，起始节点不存在" });
                    }
                    var nodeEnd = Nodes.Find(u => u.Type == "end");
                    if (nodeEnd == null)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，节点设置错误，结束节点不存在。");
                        return Json(new { code = 300, msg = "创建审批流失败，节点设置错误，结束节点不存在" });
                    }
                    var Lines = await context.FlowLines.Where(u => u.ApprFlowId == ApprFlow.ApprFlowId).ToListAsync();
                    if (Lines.Count == 0)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，连接线设置错误，不存在连接线。");
                        return Json(new { code = 300, msg = "创建审批流失败，连接线设置错误，不存在连接线" });
                    }
                    var LineToEnd = await context.FlowLines.Where(u => u.To == nodeEnd.NodeCode && u.ApprFlowId == ApprFlow.ApprFlowId).ToListAsync();
                    if (LineToEnd.Count == 0)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，连接线设置错误，没有连接线连接到结束节点。");
                        return Json(new { code = 300, msg = "创建审批流失败，连接线设置错误，没有连接线连接到结束节点" });
                    }
                    var LineStartAndEnd = await context.FlowLines.Where(u => u.From == nodeStart.NodeCode && u.To == nodeEnd.NodeCode && u.ApprFlowId == ApprFlow.ApprFlowId).ToListAsync();
                    if (LineStartAndEnd.Count > 0)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，审批设置错误，起始节点不能直接连接结束节点。");
                        return Json(new { code = 300, msg = "创建审批流失败，审批设置错误，起始节点不能直接连接结束节点" });
                    }
                    var NextNode = await context.FlowNodes.SingleOrDefaultAsync(u => u.Num == (nodeStart.Num + 1) && u.ApprFlowId == ApprFlow.ApprFlowId);
                    if (NextNode == null)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，节点设置错误，起始节点后找不到节点。");
                        return Json(new { code = 300, msg = "创建审批流失败，节点设置错误，起始节点后找不到节点" });
                    }
                    var ApprList = await (from apprs in context.Apprs
                                          where
                                            apprs.SourceId == SourceId &&
                                            (new string[] { "审批中", "审批通过" }).Contains(apprs.Status)
                                          select new
                                          {
                                              apprs
                                          }).ToListAsync();
                    if (ApprList.Count > 0)
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败，该项目已经提交审批，无法再次提交。");
                        return Json(new { code = 300, msg = "创建审批流失败，该项目已经提交审批，无法再次提交" });
                    }
                    var ApprEntity = new Appr()
                    {
                        ApprFlowId = ApprFlow.ApprFlowId,
                        SourceId = SourceId,
                        Tile = Title,
                        Note = Note,
                        ApprNote = ApprNote,
                        SubmissionTime = DateTime.Now,
                        Submitter = HttpContext.Session.GetInt32("user_id"),
                        SubmitterCorp = HttpContext.Session.GetInt32("CorpId"),
                        SubmitterDept = HttpContext.Session.GetInt32("DeptId"),
                        SubmitterPost = HttpContext.Session.GetInt32("PostId"),
                        SubmitterPhone=HttpContext.Session.GetString("Phone"),
                        Status = "审批中",
                        CreationDate = DateTime.Now,
                        CreationUser = HttpContext.Session.GetInt32("user_id")
                    };
                    context.Entry(ApprEntity).State = EntityState.Added;
                    context.SaveChanges();
                    context.Entry(ApprEntity);
                    context.ApprTrans.Add(new ApprTran()
                    {
                        ApprId = ApprEntity.ApprId,
                        TranNumber = 1,
                        SubmissionTime = DateTime.Now,
                        SubmitterNote = ApprNote,
                        Submitter = HttpContext.Session.GetInt32("user_id"),
                        SubmitterCorp = HttpContext.Session.GetInt32("CorpId"),
                        SubmitterDept = HttpContext.Session.GetInt32("DeptId"),
                        SubmitterPost = HttpContext.Session.GetInt32("PostId"),
                        SubmitNodeId = nodeStart.NodeId,
                        ApprNote = ApprNote,
                        NextSubmitNodeId = NextNode.NodeId,
                        NextSubmitNodeSubmitter = NextApprSubmitter,
                        Status = "发起",
                        CreationDate = DateTime.Now,
                        CreationUser = HttpContext.Session.GetInt32("user_id")
                    });
                    context.SaveChanges();
                    var sql = @"update " + ApprType.TableName + " set " + ApprType.TableStatusName + " = '" + ApprType.ApprStartStatus + "', "+ApprType.TableApprIdName+" = '" + ApprEntity.ApprId + "'  where " + ApprType.TablePkName + " =@SourceId ";
                    int n = await context.Database.ExecuteSqlRawAsync(sql, new[] { new SqlParameter("@SourceId", SourceId) });
                    if (n>0)
                    {
                        await tran.CommitAsync();
                        return Json(new { code = 200, msg = "提交成功" });
                    }
                    else
                    {
                        await tran.RollbackAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "：提交失败。[" + SourceId + "]" + sql);
                        return Json(new { code = 300, msg = "提交失败" });
                    }                    
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "：创建审批流失败。" + ex.Message);
                    return Json(new { code = 300, msg = "创建审批流失败，请联系管理员" });
                }
            }
        }
        #endregion

        #region 获取退回节点
        [HttpGet]
        public async Task<IActionResult> GetBackNodes(int ApprId)
        {
            try
            {
                var Nodes = await ((from t in context.ApprTrans
                                    join u in context.AppUsers on new { Submitter = (int)t.Submitter } equals new { Submitter = u.UserId }
                                    join n in context.FlowNodes
                                          on new { SubmitNodeId = (int)t.SubmitNodeId, ApprId = (int)t.ApprId, t.Status }
                                      equals new { SubmitNodeId = n.NodeId, ApprId = ApprId, Status = "同意" }
                                    select new
                                    {
                                        t.TranId,
                                        SubmitNodeId = (int?)t.SubmitNodeId,
                                        u.UserId,
                                        NodeName = (n.NodeName + "：" + u.UserName)
                                    }).Distinct()).ToListAsync();
                if (Nodes.Count > 0)
                {
                    return Json(new { code = 0, msg = "查询成功", count = Nodes.Count, data = Nodes });
                }
                else
                {
                    return Json(new { code = 0, msg = "查询成功", count = 0, data = Nodes });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：查询退回节点出错。" + ex.Message);
                return Json(new { code = 1, msg = "查询可退回的节点时出错", count = 0, data = new { } });
            }
        } 
        #endregion
    }

    #region 审批流设计json类
    public class WorkFlow
    {
        /// <summary>
        /// 节点
        /// </summary>
        public class Node
        {
            public string id { get; set; }
            public string name { get; set; }
            public string left { get; set; }
            public string top { get; set; }
            public string type { get; set; }
            public string width { get; set; }
            public string height { get; set; }
            public string alt { get; set; }
        }
        /// <summary>
        /// 线
        /// </summary>
        public class Line
        {
            public string id { get; set; }
            public string type { get; set; }
            public string from { get; set; }
            public string to { get; set; }
            public string name { get; set; }
            public string alt { get; set; }
        }
        /// <summary>
        /// 区域
        /// </summary>
        public class Area
        {
            public string name { get; set; }
            public string left { get; set; }
            public string top { get; set; }
            public string color { get; set; }
            public string width { get; set; }
            public string height { get; set; }
            public string alt { get; set; }
        }
        /// <summary>
        /// 根
        /// </summary>
        public class RootObject
        {
            public string title { get; set; }
            public string initNum { get; set; }
        }
    } 
    #endregion
}
