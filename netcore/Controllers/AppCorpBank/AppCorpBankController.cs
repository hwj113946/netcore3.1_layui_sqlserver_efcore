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

namespace netcore.Controllers.AppCorpBank
{
    public class AppCorpBankController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<AppCorpBankController> logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        public AppCorpBankController(netcore_databaseContext _context, ILogger<AppCorpBankController> _logger, IWebHostEnvironment _hostingEnvironment)
        {
            context = _context;
            logger = _logger;
            hostingEnvironment = _hostingEnvironment;
        }

        [CheckCustomer]
        public IActionResult AppCorpBank()
        {
            return View();
        }

        #region 获取公司银行账号：列表
        [HttpGet]
        public async Task<IActionResult> GetList(int CorpId, string Status, int page, int limit)
        {
            try
            {
                var list = await (from b in context.AppCorpBanks
                                  join c in context.AppCorps
                                  on b.CorpId equals c.CorpId
                                  select new
                                  {
                                      b.BankAccount,
                                      b.BankCity,
                                      b.BankName,
                                      b.BankNo,
                                      b.BankProvince,
                                      b.CorpBankId,
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
                logger.LogInformation(HttpContext.Session.GetString("who") + "：查询新闻失败。" + ex.Message);
                return Json(new { code = 0, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 根据ID获取公司
        [HttpGet]
        public async Task<IActionResult> GetById(int CorpBankId)
        {
            var single = await context.AppCorpBanks.SingleOrDefaultAsync(u => u.CorpBankId == CorpBankId);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该公司银行账号信息" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增公司银行账号信息
        [HttpPost]
        public async Task<IActionResult> Insert(int CorpId, string BankProvince, string BankCity, string BankName,
                                                string BankAccount, string BankNo, string Note)
        {
            try
            {
                await context.AppCorpBanks.AddAsync(new Models.AppCorpBank()
                {
                    CorpId = CorpId,
                    BankProvince = BankProvince ?? "",
                    BankCity = BankCity ?? "",
                    BankName = BankName ?? "",
                    BankAccount = BankAccount ?? "",
                    BankNo = BankNo ?? "",
                    Note = Note,
                    Status = "编辑",
                    CreationDate = DateTime.Now,
                    CreationUser = HttpContext.Session.GetInt32("user_id")
                });
                await context.SaveChangesAsync();
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增公司银行账号信息成功。");
                return Json(new { code = 200, msg = "新增成功" });

            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增公司银行账号信息失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑公司银行账号信息
        [HttpPost]
        public async Task<IActionResult> Modify(int CorpBankId, int CorpId, string BankProvince, string BankCity, string BankName,
                                                string BankAccount, string BankNo, string Note)
        {
            try
            {
                var modify = await context.AppCorpBanks.SingleOrDefaultAsync(u => u.CorpBankId == CorpBankId);
                if (modify != null)
                {
                    modify.CorpId = CorpId;
                    modify.BankProvince = BankProvince ?? "";
                    modify.BankCity = BankCity ?? "";
                    modify.BankName = BankName ?? "";
                    modify.BankAccount = BankAccount ?? "";
                    modify.BankNo = BankNo ?? "";
                    modify.Note = Note;
                    context.AppCorpBanks.Update(modify);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公司银行账号信息成功。");
                    return Json(new { code = 200, msg = "编辑成功" });
                }
                else
                {
                    return Json(new { code = 300, msg = "公司银行账号信息不存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑公司银行账号信息失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除公司银行账号信息
        [HttpPost]
        public async Task<IActionResult> Delete(int[] id)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < id.Length; i++)
                    {
                        var single = await context.AppCorpBanks.SingleOrDefaultAsync(u => u.CorpBankId == id[i]);
                        if (single != null)
                        {
                            context.AppCorpBanks.Remove(single);
                            context.SaveChanges();
                        }
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除公司银行账号信息成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除公司银行账号信息失败。" + ex.Message);
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
                var list = new List<Models.AppCorpBank>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppCorpBanks.SingleOrDefaultAsync(u => u.CorpBankId == id[i]);
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
                    context.AppCorpBanks.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效公司银行账号信息成功。");
                    return Json(new { code = 200, msg = "生效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效公司银行账号信息失败，公司银行账号信息不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "公司银行账号信息不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "生效公司银行账号信息失败。" + ex.Message);
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
                var list = new List<Models.AppCorpBank>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppCorpBanks.SingleOrDefaultAsync(u => u.CorpBankId == id[i]);
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
                    context.AppCorpBanks.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效公司银行账号信息成功。");
                    return Json(new { code = 200, msg = "失效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效公司银行账号信息失败，公司银行账号信息不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "公司银行账号信息不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "失效公司银行账号信息失败。" + ex.Message);
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
                var list = new List<Models.AppCorpBank>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppCorpBanks.SingleOrDefaultAsync(u => u.CorpBankId == id[i]);
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
                    context.AppCorpBanks.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "公司银行账号信息恢复编辑成功。");
                    return Json(new { code = 200, msg = "恢复编辑成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "公司银行账号信息恢复编辑失败，公司银行账号信息不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "公司银行账号信息不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "失效公司银行账号信息恢复编辑失败。" + ex.Message);
                return Json(new { code = 300, msg = "恢复编辑失败" });
            }
        }
        #endregion

        #region 导出公司银行账号信息
        [HttpPost]
        public async Task<IActionResult> Export(int CorpId, string Status)
        {
            try
            {
                var list = await (from b in context.AppCorpBanks
                                  join c in context.AppCorps
                                  on b.CorpId equals c.CorpId
                                  select new
                                  {
                                      CorpName = c.CorpName,
                                      b.BankProvince,
                                      b.BankCity,
                                      b.BankName,
                                      b.BankAccount,
                                      b.BankNo,
                                      b.Note,
                                      b.Status
                                  }).ToListAsync();
                byte[] buffer = ExcelHelper.Export(list, "公司银行账号信息", ExcelTitle.Corp).GetBuffer();
                return File(buffer, "application/ms-excel", "公司银行账号信息.xls");
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：导出公司银行账号信息失败。" + ex.Message);
                return File(new byte[] { }, "application/ms-excel", "公司银行账号信息.xls");
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
                    List<ExcelAppCorpBank> excel = excelHelper.GetList<ExcelAppCorpBank>(path).ToList();
                    var returnMsg = "";
                    if (excel.Count > 0)
                    {
                        using (var tran = context.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 0; i < excel.Count; i++)
                                {
                                    var corp = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpCode == excel[i].CorpCode && u.CorpName == excel[i].CorpName);
                                    if (corp!=null)
                                    {
                                        context.AppCorpBanks.Add(new Models.AppCorpBank()
                                        {
                                            BankAccount = excel[i].BankAccount ?? "",
                                            BankCity = excel[i].BankCity ?? "",
                                            BankName = excel[i].BankName ?? "",
                                            BankNo = excel[i].BankNo ?? "",
                                            BankProvince = excel[i].BankProvince ?? "",
                                            CorpId=corp.CorpId,
                                            Note = excel[i].Note ?? "",
                                            Status = excel[i].Status ?? "",
                                            CreationDate = DateTime.Now,
                                            CreationUser = HttpContext.Session.GetInt32("user_id")
                                        });
                                        await context.SaveChangesAsync();
                                        returnMsg += "第" + (i + 1) + "行【" + excel[i].BankName + "】" + excel[i].BankAccount + "：执行成功。\r\n";
                                    }
                                    else
                                    {
                                        returnMsg += "第" + (i + 1) + "行【" + excel[i].BankName + "】" + excel[i].BankAccount + "：公司不存在。\r\n";
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
