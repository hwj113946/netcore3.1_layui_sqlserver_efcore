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
using NPOI.SS.Formula.Atp;

namespace netcore.Controllers.AppUser
{
    public class AppUserController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<AppUserController> logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        public AppUserController(netcore_databaseContext _context, ILogger<AppUserController> _logger, IWebHostEnvironment _hostingEnvironment)
        {
            context = _context;
            logger = _logger;
            hostingEnvironment = _hostingEnvironment;
        }
        [CheckCustomer]
        public IActionResult AppUser()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult AppUserEdit()
        {
            ViewBag.status = HttpContext.Request.Query["status"];
            ViewBag.UserId = ViewBag.status == "add" ? "" : HttpContext.Request.Query["Rowid"].ToString();
            return View();
        }

        [CheckCustomer]
        public IActionResult AppUserAllotRole()
        {
            ViewBag.UserId = HttpContext.Request.Query["Rowid"].ToString() ?? "";
            return View();
        }

        [CheckCustomer]
        public IActionResult AppUserModifyPassword()
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("user_id");
            return View();
        }

        [CheckCustomer]
        public IActionResult AppUserInfoView()
        {
            return View();
        }

        #region 获取列表
        [HttpGet]
        public async Task<IActionResult> GetList(int CorpId, int DeptId, string Status, string UserCode, string UserName,
                                                 int page, int limit)
        {
            try
            {
                var list = await (from u in context.AppUsers
                                  join c in context.AppCorps on new { corp_id = (int)u.CorpId } equals new { corp_id = c.CorpId }
                                  join d in context.AppDepts on new { dept_id = (int)u.DeptId } equals new { dept_id = d.DeptId }
                                  join p in context.AppPosts on new { post_id = (int)u.PostId } equals new { post_id = p.PostId }
                                  select new
                                  {
                                      u.UserId,
                                      u.Address,
                                      u.Email,
                                      u.ModifyPasswordDate,
                                      u.Phone,
                                      u.UserCode,
                                      u.UserName,
                                      u.Status,
                                      u.CorpId,
                                      c.CorpName,
                                      u.DeptId,
                                      d.DeptName,
                                      p.PostName
                                  }
                                  ).ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    //优先使用部门查询
                    if (DeptId != -99)
                    {
                        list = list.Where(u => u.DeptId == DeptId).ToList();
                    }
                    else
                    {
                        //部门不选时，使用公司
                        if (CorpId != -99)
                        {
                            list = list.Where(u => u.CorpId == CorpId).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(Status))
                    {
                        if (Status != "全部")
                        {
                            list = list.Where(u => u.Status == Status).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(UserCode))
                    {
                        list = list.Where(u => u.UserCode.Contains(UserCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(UserName))
                    {
                        list = list.Where(u => u.UserName.Contains(UserName)).ToList();
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
                logger.LogInformation(HttpContext.Session.GetString("who") + "查询用户失败。" + ex.Message);
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

        #region 根据ID获取信息
        [HttpGet]
        public async Task<IActionResult> GetById(int UserId)
        {
            var single = await context.AppUsers.SingleOrDefaultAsync(u => u.UserId == UserId);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该用户信息" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 新增
        [HttpPost]
        public async Task<IActionResult> Insert(int CorpId, int DeptId, int PostId, string UserCode, string UserName,
                                                string Phone, string Email, string Address)
        {
            try
            {
                var single = await context.AppUsers.SingleOrDefaultAsync(u => u.UserCode == UserCode);
                if (single == null)
                {
                    await context.AppUsers.AddAsync(new Models.AppUser()
                    {
                        CorpId = CorpId,
                        DeptId = DeptId,
                        PostId = PostId,
                        UserCode = UserCode ?? "",
                        UserName = UserName ?? "",
                        Phone = Phone ?? "",
                        Email = Email ?? "",
                        Address = Address ?? "",
                        Status = "编辑",
                        Password = Helper.EncodeHelper.MD5Hash(UserCode + "_" + DateTime.Now.Year),
                        CreationDate = DateTime.Now,
                        CreationUser = HttpContext.Session.GetInt32("user_id")
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增用户成功。");
                    return Json(new { code = 200, msg = "新增成功" });
                }
                else
                {
                    return Json(new { code = 300, msg = "该账户已经存在" });
                }

            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增用户失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑
        [HttpPost]
        public async Task<IActionResult> Modify(int UserId,int CorpId, int DeptId, int PostId, string UserCode, string UserName,
                                                string Phone, string Email, string Address)
        {
            try
            {
                var single = await context.AppUsers.SingleOrDefaultAsync(u => u.UserCode == UserCode);
                if (single == null)
                {
                    var modify= await context.AppUsers.SingleOrDefaultAsync(u => u.UserId == UserId);
                    if (modify != null)
                    {
                        modify.CorpId = CorpId;
                        modify.DeptId = DeptId;
                        modify.PostId = PostId;
                        modify.UserCode = UserCode ?? "";
                        modify.UserName = UserName ?? "";
                        modify.Phone = Phone ?? "";
                        modify.Email = Email ?? "";
                        modify.Address = Address ?? "";
                        modify.LastModifiedDate = DateTime.Now;
                        modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        context.AppUsers.Update(modify);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "编辑用户成功。");
                        return Json(new { code = 200, msg = "编辑成功" });
                    }
                    else
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "编辑用户，用户不存在。");
                        return Json(new { code = 300, msg = "用户不存在" });
                    }                    
                }
                else
                {
                    if (single.UserId!=UserId)
                    {
                        return Json(new { code = 300, msg = "该账户已经存在" });
                    }
                    else
                    {
                        single.DeptId = DeptId;
                        single.PostId = PostId;
                        single.UserCode = UserCode ?? "";
                        single.UserName = UserName ?? "";
                        single.Phone = Phone ?? "";
                        single.Email = Email ?? "";
                        single.Address = Address ?? "";
                        single.LastModifiedDate = DateTime.Now;
                        single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        context.AppUsers.Update(single);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "编辑用户成功。");
                        return Json(new { code = 200, msg = "编辑成功" });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑用户失败。" + ex.Message);
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
                    var list = new List<Models.AppUser>();
                    for (int i = 0; i < id.Length; i++)
                    {
                        var single = await context.AppUsers.SingleOrDefaultAsync(u => u.UserId == id[i]);
                        if (single != null)
                        {
                            list.Add(single);
                        }
                    }
                    if (list.Count>0)
                    {
                        context.AppUsers.RemoveRange(list);
                        await context.SaveChangesAsync();
                        await tran.CommitAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "删除岗位成功。");
                        return Json(new { code = 200, msg = "删除成功" });
                    }
                    else
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "删除用户：勾选的用户不存在或未勾选数据。");
                        return Json(new { code = 300, msg = "用户不存在或未勾选数据" });
                    }
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除用户失败。" + ex.Message);
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
                var list = new List<Models.AppUser>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppUsers.SingleOrDefaultAsync(u => u.UserId == id[i]);
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
                    context.AppUsers.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效用户成功。");
                    return Json(new { code = 200, msg = "生效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效用户失败，用户不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "用户不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "生效用户失败。" + ex.Message);
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
                var list = new List<Models.AppUser>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppUsers.SingleOrDefaultAsync(u => u.UserId == id[i]);
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
                    context.AppUsers.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效用户成功。");
                    return Json(new { code = 200, msg = "失效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效用户失败，用户不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "用户不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "失效用户失败。" + ex.Message);
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
                var list = new List<Models.AppUser>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppUsers.SingleOrDefaultAsync(u => u.UserId == id[i]);
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
                    context.AppUsers.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "用户恢复编辑成功。");
                    return Json(new { code = 200, msg = "恢复编辑成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "用户恢复编辑失败，用户不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "用户不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "用户恢复编辑失败。" + ex.Message);
                return Json(new { code = 300, msg = "恢复编辑失败" });
            }
        }
        #endregion

        #region 导出
        [HttpPost]
        public async Task<IActionResult> Export(int CorpId, int DeptId, string Status, string UserCode, string UserName)
        {
            try
            {
                var list = await (from u in context.AppUsers
                                  join c in context.AppCorps on new { corp_id = (int)u.CorpId } equals new { corp_id = c.CorpId }
                                  join d in context.AppDepts on new { dept_id = (int)u.DeptId } equals new { dept_id = d.DeptId }
                                  join p in context.AppPosts on new { post_id = (int)u.PostId } equals new { post_id = p.PostId }
                                  select new
                                  {
                                      u.UserId,
                                      u.Address,
                                      u.Email,
                                      u.ModifyPasswordDate,
                                      u.Phone,
                                      u.UserCode,
                                      u.UserName,
                                      u.Status,
                                      u.CorpId,
                                      c.CorpName,
                                      u.DeptId,
                                      d.DeptName,
                                      p.PostName
                                  }
                                  ).ToListAsync();
                if (list.Count > 0)
                {
                    //优先使用部门查询
                    if (DeptId != -99)
                    {
                        list = list.Where(u => u.DeptId == DeptId).ToList();
                    }
                    else
                    {
                        //部门不选时，使用公司
                        if (CorpId != -99)
                        {
                            list = list.Where(u => u.CorpId == CorpId).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(Status))
                    {
                        if (Status != "全部")
                        {
                            list = list.Where(u => u.Status == Status).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(UserCode))
                    {
                        list = list.Where(u => u.UserCode.Contains(UserCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(UserName))
                    {
                        list = list.Where(u => u.PostName.Contains(UserName)).ToList();
                    }
                }
                var list1 = list.Select(u => new
                {
                    CorpName = u.CorpName,
                    DeptName = u.DeptName,
                    PostName = u.PostName,
                    UserCode = u.UserCode,
                    UserName = u.UserName,
                    Phone = u.Phone,
                    Email = u.Email,
                    Address = u.Address,
                    ModifyPasswordDate = u.ModifyPasswordDate,
                    Status = u.Status
                }).ToList();
                byte[] buffer = ExcelHelper.Export(list1, "用户信息", ExcelTitle.AppUser).GetBuffer();
                return File(buffer, "application/ms-excel", "用户信息.xls");
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：导出用户信息失败。" + ex.Message);
                return File(new byte[] { }, "application/ms-excel", "用户信息.xls");
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
                    List<ExcelAppUser> excel = excelHelper.GetList<ExcelAppUser>(path).ToList();
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
                                        if (string.IsNullOrEmpty(excel[i].DeptName))
                                        {
                                            returnMsg += "第" + (i + 1) + "行：所属部门不能为空。\r\n";
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(excel[i].PostName))
                                            {
                                                returnMsg += "第" + (i + 1) + "行：所属岗位名称不能为空。\r\n";
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(excel[i].UserCode))
                                                {
                                                    returnMsg += "第" + (i + 1) + "行：账户不能为空。\r\n";
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrEmpty(excel[i].UserName))
                                                    {
                                                        returnMsg += "第" + (i + 1) + "行：姓名不能为空。\r\n";
                                                    }
                                                    else
                                                    {
                                                        if (string.IsNullOrEmpty(excel[i].Phone))
                                                        {
                                                            returnMsg += "第" + (i + 1) + "行：联系电话不能为空。\r\n";
                                                        }
                                                        else
                                                        {
                                                            if (string.IsNullOrEmpty(excel[i].Status))
                                                            {
                                                                returnMsg += "第" + (i + 1) + "行：状态不能为空。\r\n";
                                                            }
                                                            else
                                                            {
                                                                var corp = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpName == excel[i].CorpName);
                                                                if (corp != null)
                                                                {
                                                                    var dept = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptName == excel[i].DeptName && u.CorpId == corp.CorpId);
                                                                    if (dept == null)
                                                                    {
                                                                        returnMsg += "第" + (i + 1) + "行：该部门不存在或该部门不在所属公司下。\r\n";
                                                                    }
                                                                    else
                                                                    {

                                                                        var post = await context.AppPosts.SingleOrDefaultAsync(u => u.PostName == excel[i].PostName && u.DeptId == dept.DeptId);
                                                                        if (post == null)
                                                                        {
                                                                            returnMsg += "第" + (i + 1) + "行：该岗位不存在或该岗位不在所属部门下。\r\n";
                                                                        }
                                                                        else
                                                                        {

                                                                            var single = await context.AppUsers.SingleOrDefaultAsync(u => u.UserCode == excel[i].UserCode);
                                                                            if (single != null)
                                                                            {
                                                                                returnMsg += "第" + (i + 1) + "行：账户已存在。\r\n";
                                                                            }
                                                                            else
                                                                            {
                                                                                context.AppUsers.Add(new Models.AppUser()
                                                                                {
                                                                                    CorpId = corp.CorpId,
                                                                                    DeptId = dept.DeptId,
                                                                                    PostId = post.PostId,
                                                                                    UserCode = excel[i].UserCode,
                                                                                    UserName = excel[i].UserName,
                                                                                    Phone = excel[i].Phone,
                                                                                    Email = excel[i].Email,
                                                                                    Address = excel[i].Address,
                                                                                    Status = excel[i].Status,
                                                                                    CreationDate = DateTime.Now,
                                                                                    CreationUser = HttpContext.Session.GetInt32("user_id"),
                                                                                    Password = Helper.EncodeHelper.MD5Hash(excel[i].UserCode + "_" + DateTime.Now.Year)
                                                                                });
                                                                                await context.SaveChangesAsync();
                                                                                returnMsg += "第" + (i + 1) + "行：执行成功。\r\n";
                                                                            }
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

        #region 重置密码
        [HttpPost]
        public async Task<IActionResult> ResetPassword(int[] id)
        {
            try
            {
                var list = new List<Models.AppUser>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppUsers.SingleOrDefaultAsync(u => u.UserId == id[i]);
                    if (single != null)
                    {
                        single.Password = Helper.EncodeHelper.MD5Hash(single.UserCode+"_"+DateTime.Now.Year);
                        single.ModifyPasswordDate = DateTime.Now;
                        single.LastModifiedDate = DateTime.Now;
                        single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        list.Add(single);
                    }
                }
                if (list.Count > 0)
                {
                    context.AppUsers.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "用户重置密码成功。");
                    return Json(new { code = 200, msg = "重置成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "用户重置密码失败，用户不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "用户不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "用户重置密码失败。" + ex.Message);
                return Json(new { code = 300, msg = "重置失败" });
            }
        }
        #endregion

        #region 修改密码
        [HttpPost]
        public async Task<IActionResult> ModifyPassword(int UserId,string Password)
        {
            try
            {
                var modify = await context.AppUsers.SingleOrDefaultAsync(u => u.UserId == UserId);
                if (modify != null)
                {
                    modify.Password = Password ?? "";
                    modify.ModifyPasswordDate = DateTime.Now;
                    modify.LastModifiedDate = DateTime.Now;
                    modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                    context.AppUsers.Update(modify);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "用户修改密码成功。");
                    return Json(new { code = 200, msg = "修改成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "用户修改密码，用户不存在。");
                    return Json(new { code = 300, msg = "用户不存在" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "用户修改密码失败。" + ex.Message);
                return Json(new { code = 300, msg = "修改失败" });
            }
        }
        #endregion

        #region 用户分配角色：获取列表
        [HttpGet]
        public async Task<IActionResult> GetUserRoleList(int UserId, string RoleName, string Status, int page, int limit)
        {
            try
            {
                if (string.IsNullOrEmpty(Status))
                {
                    return Json(new { code = 1, msg = "传入参数不全", count = 0, data = new { } });
                }
                else
                {
                    if (Status == "未选")
                    {
                        var list = await (from rs in (
                                    (from r in context.AppRoles
                                     join ur in context.AppUserRoles
                                     on r.RoleId equals ur.RoleId
                                     into ur_join 
                                     from ur in ur_join.DefaultIfEmpty()
                                     select new
                                     {
                                         r.RoleId,
                                         r.RoleName,
                                         Status =
                                       ur.RoleId == null ? "未选" : "已选"
                                     }))
                                          where
                                            rs.Status == "未选"
                                          select new
                                          {
                                              rs.RoleId,
                                              rs.RoleName,
                                              rs.Status
                                          }).ToListAsync();
                        decimal count = 0;
                        if (list.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(RoleName))
                            {
                                list = list.Where(u => u.RoleName.Contains(RoleName)).ToList();
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
                    else
                    {
                        var list = await (from r in context.AppRoles
                                          join ur in context.AppUserRoles
                                          on r.RoleId equals ur.RoleId
                                          select new
                                          {
                                              r.RoleId,
                                              r.RoleName
                                          }).ToListAsync();
                        decimal count = 0;
                        if (list.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(RoleName))
                            {
                                list = list.Where(u => u.RoleName.Contains(RoleName)).ToList();
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
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "查询角色失败(用户分配角色界面)。"+ex.Message);
                return Json(new { code = 1, msg = "查询失败，请联系管理员", count = 0, data = new { } });
            }
        }
        #endregion

        #region 分配角色
        [HttpPost]
        public async Task<IActionResult> AllotRole(int[] id,int UserId)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var list = new List<Models.AppUserRole>();
                    for (int i = 0; i < id.Length; i++)
                    {
                        var single = await context.AppUserRoles.AsNoTracking().SingleOrDefaultAsync(u => u.RoleId == id[i] && u.UserId == UserId);
                        if (single == null)
                        {
                            list.Add(new Models.AppUserRole()
                            {
                                RoleId = id[i],
                                UserId = UserId
                            });
                        }
                    }
                    if (list.Count > 0)
                    {
                        context.AppUserRoles.AddRange(list);
                        await context.SaveChangesAsync();
                        await tran.CommitAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "分配成功。");
                        return Json(new { code = 200, msg = "分配成功" });
                    }
                    else
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "分配角色：角色不存在或未勾选数据或该用户已拥有勾选的角色。");
                        return Json(new { code = 300, msg = "角色不存在或未勾选数据或该用户已拥有勾选的角色" });
                    }
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "分配角色失败。" + ex.Message);
                    return Json(new { code = 300, msg = "分配失败" });
                }
            }
        }
        #endregion

        #region 移除角色
        [HttpPost]
        public async Task<IActionResult> RemoveRole(int[] id, int UserId)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var list = new List<Models.AppUserRole>();
                    for (int i = 0; i < id.Length; i++)
                    {
                        var single = await context.AppUserRoles.SingleOrDefaultAsync(u => u.RoleId == id[i] && u.UserId == UserId);
                        if (single != null)
                        {
                            list.Add(single);
                        }
                    }
                    if (list.Count > 0)
                    {
                        context.AppUserRoles.RemoveRange(list);
                        await context.SaveChangesAsync();
                        await tran.CommitAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "移除角色成功。");
                        return Json(new { code = 200, msg = "移除成功" });
                    }
                    else
                    {
                        logger.LogInformation(HttpContext.Session.GetString("who") + "移除橘色：角色不存在或未勾选数据或该用户已移除勾选的角色。");
                        return Json(new { code = 300, msg = "角色不存在或未勾选数据或该用户已移除勾选的角色" });
                    }
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "移除角色失败。" + ex.Message);
                    return Json(new { code = 300, msg = "移除失败" });
                }
            }
        }
        #endregion

        #region 根据ID获取信息
        [HttpGet]
        public async Task<IActionResult> GetUserInfoByUserId()
        {
            try
            {
                var single = await (from u in context.AppUsers
                                    join c in context.AppCorps on new { CorpId = (int)u.CorpId } equals new { CorpId = c.CorpId }
                                    join d in context.AppDepts on new { DeptId = (int)u.DeptId } equals new { DeptId = d.DeptId }
                                    join p in context.AppPosts on new { PostId = (int)u.PostId, u.UserId }
                                    equals new { PostId = p.PostId, UserId = (int)HttpContext.Session.GetInt32("user_id") }
                                    select new
                                    {
                                        u.UserCode,
                                        u.UserName,
                                        u.Email,
                                        u.Phone,
                                        c.CorpName,
                                        d.DeptName,
                                        p.PostName
                                    }).SingleOrDefaultAsync();
                var roles = await (from ur in context.AppUserRoles
                                    join r in context.AppRoles
                                          on new { ur.RoleId, ur.UserId }
                                      equals new { r.RoleId, UserId = (int)HttpContext.Session.GetInt32("user_id") }
                                    select new
                                    {
                                        r.RoleName
                                    }).ToListAsync();
                return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该用户信息" : "查询成功", count = single == null ? 0 : 1, data = single,roles=roles });
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who")+"：根据ID获取用户信息失败。"+ex.Message);
                return Json(new { code = 1, msg = "查询失败，请联系管理员", count = 0, data = new { },roles=new { } });
            }
        }
        #endregion

    }
}
