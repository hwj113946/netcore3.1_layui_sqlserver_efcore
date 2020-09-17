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

namespace netcore.Controllers.Base.Corp
{
    public class CorpController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<CorpController> logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        public CorpController(netcore_databaseContext _context, ILogger<CorpController> _logger, IWebHostEnvironment _hostingEnvironment)
        {
            context = _context;
            logger = _logger;
            hostingEnvironment = _hostingEnvironment;
        }

        [CheckCustomer]
        public IActionResult Corp()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult EditCorp()
        {
            ViewBag.status = HttpContext.Request.Query["status"];
            ViewBag.corp_id = ViewBag.status == "add" ? "" : HttpContext.Request.Query["Rowid"].ToString();
            return View();
        }

        #region 获取公司：列表
        [HttpGet]
        public async Task<IActionResult> GetList(string CorpCode, string CorpName, string Status, int page, int limit)
        {
            try
            {
                var list = await context.AppCorps.ToListAsync();
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
                    if (!string.IsNullOrEmpty(CorpCode))
                    {
                        list = list.Where(u => u.CorpCode.Contains(CorpCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(CorpName))
                    {
                        list = list.Where(u => u.CorpName.Contains(CorpName)).ToList();
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
                logger.LogInformation(HttpContext.Session.GetString("who") + "：查询公司失败。" + ex.Message);
                return Json(new { code = 1, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 根据ID获取公司
        [HttpGet]
        public async Task<IActionResult> GetCorpById(int corp_id)
        {
            var single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpId == corp_id);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该公司" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增公司
        [HttpPost]
        public async Task<IActionResult> Insert(string CorpCode,string CorpName,string Fax,string Zip,string Email,string ContractPersonName, string ContractPersonPhone, string ContractPersonIdentity, string LawPersonName,string LawPersonPhone,string LawPersonIdentity,string Address,string TaxRqNumber,string Note)
        {
            try
            {
                var single= await context.AppCorps.SingleOrDefaultAsync(u => u.CorpCode == CorpCode);
                if (single==null)
                {
                    single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpName == CorpName);
                    if (single == null)
                    {
                        await context.AppCorps.AddAsync(new AppCorp()
                        {
                            CorpCode = CorpCode ?? "",
                            CorpName = CorpName ?? "",
                            Fax = Fax ?? "",
                            Zip = Zip ?? "",
                            Email = Email ?? "",
                            ContractPersonName = ContractPersonName ?? "",
                            ContractPersonPhone = ContractPersonPhone ?? "",
                            ContractPersonIdentity = ContractPersonIdentity ?? "",
                            LawPersonName = LawPersonName ?? "",
                            LawPersonPhone = LawPersonPhone ?? "",
                            LawPersonIdentity = LawPersonIdentity ?? "",
                            Address = Address ?? "",
                            TaxRqNumber = TaxRqNumber ?? "",
                            Note = Note,
                            Status = "编辑",
                            CreationDate = DateTime.Now,
                            CreationUser = HttpContext.Session.GetInt32("user_id")
                        });
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "新增公司成功。");
                        return Json(new { code = 200, msg = "新增成功" });
                    }
                    else
                    {
                        return Json(new { code = 300, msg = "公司名称已存在" });
                    }
                }
                else
                {
                    return Json(new { code = 300, msg = "公司代码已存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增公司失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑公司
        [HttpPost]
        public async Task<IActionResult> Modify(int CorpId, string CorpCode, string CorpName, string Fax, string Zip, string Email, string ContractPersonName, string ContractPersonPhone, string ContractPersonIdentity, string LawPersonName, string LawPersonPhone, string LawPersonIdentity, string Address, string TaxRqNumber, string Note)
        {
            try
            {
                var single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpCode == CorpCode);
                if (single==null)
                {
                    single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpName == CorpName);
                    if (single == null)
                    {
                        var modify = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpId == CorpId);
                        if (modify != null)
                        {
                            modify.CorpCode = CorpCode ?? "";
                            modify.CorpName = CorpName ?? "";
                            modify.Fax = Fax ?? "";
                            modify.Zip = Zip ?? "";
                            modify.Email = Email ?? "";
                            modify.ContractPersonName = ContractPersonName ?? "";
                            modify.ContractPersonPhone = ContractPersonPhone ?? "";
                            modify.ContractPersonIdentity = ContractPersonIdentity ?? "";
                            modify.LawPersonName = LawPersonName ?? "";
                            modify.LawPersonPhone = LawPersonPhone ?? "";
                            modify.LawPersonIdentity = LawPersonIdentity ?? "";
                            modify.Address = Address ?? "";
                            modify.TaxRqNumber = TaxRqNumber ?? "";
                            modify.Note = Note;
                            context.AppCorps.Update(modify);
                            await context.SaveChangesAsync();
                            logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公司成功。");
                            return Json(new { code = 200, msg = "编辑成功" });
                        }
                        else
                        {
                            return Json(new { code = 300, msg = "公司不存在" });
                        }
                    }
                    else
                    {
                        if (single.CorpId != CorpId)
                        {
                            return Json(new { code = 300, msg = "公司名称已存在" });
                        }
                        else
                        {
                            var modify = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpId == CorpId);
                            if (modify != null)
                            {
                                modify.CorpCode = CorpCode ?? "";
                                modify.CorpName = CorpName ?? "";
                                modify.Fax = Fax ?? "";
                                modify.Zip = Zip ?? "";
                                modify.Email = Email ?? "";
                                modify.ContractPersonName = ContractPersonName ?? "";
                                modify.ContractPersonPhone = ContractPersonPhone ?? "";
                                modify.ContractPersonIdentity = ContractPersonIdentity ?? "";
                                modify.LawPersonName = LawPersonName ?? "";
                                modify.LawPersonPhone = LawPersonPhone ?? "";
                                modify.LawPersonIdentity = LawPersonIdentity ?? "";
                                modify.Address = Address ?? "";
                                modify.TaxRqNumber = TaxRqNumber ?? "";
                                modify.Note = Note;
                                context.AppCorps.Update(modify);
                                await context.SaveChangesAsync();
                                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公司成功。");
                                return Json(new { code = 200, msg = "编辑成功" });
                            }
                            else
                            {
                                return Json(new { code = 300, msg = "公司不存在" });
                            }
                        }
                    }
                }
                else
                {
                    if (single.CorpId != CorpId)
                    {
                        return Json(new { code = 300, msg = "公司代码已存在" });
                    }
                    else
                    {
                        single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpName == CorpName);
                        if (single == null)
                        {
                            var modify = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpId == CorpId);
                            if (modify != null)
                            {
                                modify.CorpCode = CorpCode ?? "";
                                modify.CorpName = CorpName ?? "";
                                modify.Fax = Fax ?? "";
                                modify.Zip = Zip ?? "";
                                modify.Email = Email ?? "";
                                modify.ContractPersonName = ContractPersonName ?? "";
                                modify.ContractPersonPhone = ContractPersonPhone ?? "";
                                modify.ContractPersonIdentity = ContractPersonIdentity ?? "";
                                modify.LawPersonName = LawPersonName ?? "";
                                modify.LawPersonPhone = LawPersonPhone ?? "";
                                modify.LawPersonIdentity = LawPersonIdentity ?? "";
                                modify.Address = Address ?? "";
                                modify.TaxRqNumber = TaxRqNumber ?? "";
                                modify.Note = Note;
                                context.AppCorps.Update(modify);
                                await context.SaveChangesAsync();
                                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公司成功。");
                                return Json(new { code = 200, msg = "编辑成功" });
                            }
                            else
                            {
                                return Json(new { code = 300, msg = "公司不存在" });
                            }
                        }
                        else
                        {
                            if (single.CorpId != CorpId)
                            {
                                return Json(new { code = 300, msg = "公司名称已存在" });
                            }
                            else
                            {
                                var modify = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpId == CorpId);
                                if (modify != null)
                                {
                                    modify.CorpCode = CorpCode ?? "";
                                    modify.CorpName = CorpName ?? "";
                                    modify.Fax = Fax ?? "";
                                    modify.Zip = Zip ?? "";
                                    modify.Email = Email ?? "";
                                    modify.ContractPersonName = ContractPersonName ?? "";
                                    modify.ContractPersonPhone = ContractPersonPhone ?? "";
                                    modify.ContractPersonIdentity = ContractPersonIdentity ?? "";
                                    modify.LawPersonName = LawPersonName ?? "";
                                    modify.LawPersonPhone = LawPersonPhone ?? "";
                                    modify.LawPersonIdentity = LawPersonIdentity ?? "";
                                    modify.Address = Address ?? "";
                                    modify.TaxRqNumber = TaxRqNumber ?? "";
                                    modify.Note = Note;
                                    context.AppCorps.Update(modify);
                                    await context.SaveChangesAsync();
                                    logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公司成功。");
                                    return Json(new { code = 200, msg = "编辑成功" });
                                }
                                else
                                {
                                    return Json(new { code = 300, msg = "公司不存在" });
                                }
                            }
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公司失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除公司及其相关的银行账号、部门、岗位
        [HttpPost]
        public async Task<IActionResult> Delete(int[] id)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < id.Length; i++)
                    {
                        var single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpId == id[i]);
                        if (single != null)
                        {
                            context.AppCorps.Remove(single);
                            context.SaveChanges();
                            //银行账号
                            var list1 = await context.AppCorpBanks.Where(u => u.CorpId == id[i]).ToListAsync();
                            if (list1.Count > 0)
                            {
                                context.AppCorpBanks.RemoveRange(list1);
                                context.SaveChanges();
                            }
                            //部门
                            var list2 = await context.AppDepts.Where(u => u.CorpId == id[i]).ToListAsync();
                            if (list2.Count > 0)
                            {
                                for (int j = 0; j < list2.Count; j++)
                                {
                                    //岗位
                                    var list3 = await context.AppPosts.Where(u => u.DeptId == list2[j].DeptId).ToListAsync();
                                    if (list3.Count > 0)
                                    {
                                        context.AppPosts.RemoveRange(list3);
                                        context.SaveChanges();
                                    }
                                }
                                context.AppDepts.RemoveRange(list2);
                                context.SaveChanges();
                            }
                            var list4 = await context.AppUsers.Where(u=>u.CorpId==id[i]).ToListAsync();
                            if (list4.Count>0)
                            {
                                for (int k = 0; k < list4.Count; k++)
                                {
                                    list4[i].CorpId = -999;
                                    list4[i].DeptId = -999;
                                    list4[i].PostId = -999;
                                    list4[i].Status = "失效";
                                    list4[4].LastModifiedDate = DateTime.Now;
                                    list4[i].LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                                    context.AppUsers.Update(list4[i]);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除公司成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除公司失败。" + ex.Message);
                    return Json(new { code = 300, msg = "删除失败" });
                }
            }
        }
        #endregion

        #region 生效
        [HttpPost]
        public async Task<IActionResult> EnableStatusForCorp(int[] id)
        {
            try
            {
                var list = new List<AppCorp>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpId == id[i]);
                    if (single != null)
                    {
                        single.Status = "有效";
                        single.LastModifiedDate = DateTime.Now;
                        single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        list.Add(single);
                    }
                }
                if (list.Count>0)
                {
                    context.AppCorps.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效公司成功。");
                    return Json(new { code = 200, msg = "生效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效公司失败，公司不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "公司不存在或未勾选数据" });
                }                
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "生效公司失败。" + ex.Message);
                return Json(new { code = 300, msg = "生效失败" });
            }
        }
        #endregion

        #region 失效
        [HttpPost]
        public async Task<IActionResult> FailureStatusForCorp(int[] id)
        {
            try
            {
                var list = new List<AppCorp>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpId == id[i]);
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
                    context.AppCorps.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效公司成功。");
                    return Json(new { code = 200, msg = "失效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效公司失败，公司不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "公司不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "失效公司失败。" + ex.Message);
                return Json(new { code = 300, msg = "失效失败" });
            }
        }
        #endregion

        #region 恢复编辑
        [HttpPost]
        public async Task<IActionResult> ResetStatusForCorp(int[] id)
        {
            try
            {
                var list = new List<AppCorp>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpId == id[i]);
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
                    context.AppCorps.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "恢复编辑状态成功。");
                    return Json(new { code = 200, msg = "恢复编辑状态成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "恢复编辑状态失败，公司不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "公司不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "恢复编辑状态失败。" + ex.Message);
                return Json(new { code = 300, msg = "恢复编辑状态失败" });
            }
        }
        #endregion

        #region 导出公司
        [HttpPost]
        public async Task<IActionResult> Export(string CorpCode, string CorpName, string Status)
        {
            try
            {
                var list = await context.AppCorps.ToListAsync();
                if (list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Status))
                    {
                        if (Status != "全部")
                        {
                            list = list.Where(u => u.Status == Status).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(CorpCode))
                    {
                        list = list.Where(u => u.CorpCode.Contains(CorpCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(CorpName))
                    {
                        list = list.Where(u => u.CorpName.Contains(CorpName)).ToList();
                    }
                }
                var list1 = list.Select(u => new
                {
                    u.CorpCode,
                    u.CorpName,
                    u.Fax,
                    u.Zip,
                    u.Email,
                    u.ContractPersonName,
                    u.ContractPersonPhone,
                    u.ContractPersonIdentity,
                    u.LawPersonName,
                    u.LawPersonPhone,
                    u.LawPersonIdentity,
                    u.Address,
                    u.TaxRqNumber,
                    u.Note,
                    u.Status
                }).ToList();
                byte[] buffer = ExcelHelper.Export(list1, "公司", ExcelTitle.Corp).GetBuffer() ;
                return File(buffer, "application/ms-excel", "公司数据导出.xls");
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：导出公司数据失败。" + ex.Message);
                return File(new byte[] { }, "application/ms-excel", "公司数据导出.xls");
            }
        }
        #endregion

        #region 导入
        [HttpPost]
        public async Task<IActionResult> Import([FromForm] IFormFile file)
        {
            if (file.Length>0)
            {
                long fileSize = file.Length / 5242880;
                if (fileSize>15)
                {
                    return Json(new { code = 300, msg = "文件不能大于15M", returnMsg = "文件不能大于15M" });
                }
                else
                {
                    var path=hostingEnvironment.WebRootPath+ "\\" + Guid.NewGuid() + file.FileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    ExcelHelper excelHelper = new ExcelHelper();
                    List<ExcelAppCorp> excel = excelHelper.GetList<ExcelAppCorp>(path).ToList();
                    var returnMsg = "";
                    if (excel.Count>0)
                    {
                        using (var tran=context.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 0; i < excel.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(excel[i].CorpCode))
                                    {
                                        returnMsg += "公司代码不能为空。\r\n";
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(excel[i].CorpName))
                                        {
                                            returnMsg += "公司名称不能为空。\r\n";
                                        }
                                        else
                                        {
                                            var single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpCode == excel[i].CorpCode);
                                            if (single == null)
                                            {
                                                single = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpName == excel[i].CorpName);
                                                if (single == null)
                                                {
                                                    context.AppCorps.Add(new AppCorp()
                                                    {
                                                        CorpCode = excel[i].CorpCode ?? "",
                                                        CorpName = excel[i].CorpName ?? "",
                                                        Address = excel[i].Address ?? "",
                                                        ContractPersonIdentity = excel[i].ContractPersonIdentity ?? "",
                                                        ContractPersonName = excel[i].ContractPersonName ?? "",
                                                        ContractPersonPhone = excel[i].ContractPersonPhone ?? "",
                                                        Email = excel[i].Email ?? "",
                                                        Fax = excel[i].Fax ?? "",
                                                        Zip = excel[i].Zip ?? "",
                                                        TaxRqNumber = excel[i].TaxRqNumber ?? "",
                                                        Note = excel[i].Note ?? "",
                                                        Status = excel[i].Status ?? "",
                                                        CreationDate = DateTime.Now,
                                                        CreationUser = HttpContext.Session.GetInt32("user_id"),
                                                        LawPersonIdentity = excel[i].LawPersonIdentity ?? "",
                                                        LawPersonName = excel[i].LawPersonName ?? "",
                                                        LawPersonPhone = excel[i].LawPersonPhone ?? ""
                                                    });
                                                    await context.SaveChangesAsync();
                                                    returnMsg += "第" + (i + 1) + "行【" + excel[i].CorpCode + "】" + excel[i].CorpName + "：执行成功。\r\n";
                                                }
                                                else
                                                {
                                                    returnMsg += "第" + (i + 1) + "行【" + excel[i].CorpCode + "】" + excel[i].CorpName + "：公司名称已存在，不执行导入本行数据。\r\n";
                                                }
                                            }
                                            else
                                            {
                                                returnMsg += "第" + (i + 1) + "行【" + excel[i].CorpCode + "】" + excel[i].CorpName + "：公司代码已存在，不执行导入本行数据。\r\n";
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
