using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using Helper;

namespace netcore.Controllers.AppPost
{
    public class AppPostController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<AppPostController> logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        public AppPostController(netcore_databaseContext _context, ILogger<AppPostController> _logger, IWebHostEnvironment _hostingEnvironment)
        {
            context = _context;
            logger = _logger;
            hostingEnvironment = _hostingEnvironment;
        }

        [CheckCustomer]
        public IActionResult AppPost()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult AppPostEdit()
        {
            ViewBag.status = HttpContext.Request.Query["status"];
            ViewBag.PostId = ViewBag.status == "add" ? "" : HttpContext.Request.Query["Rowid"].ToString();
            return View();
        }

        #region 获取列表
        [HttpGet]
        public async Task<IActionResult> GetList(int CorpId, int DeptId, string Status, string PostCode, string PostName,
                                                 int page, int limit)
        {
            try
            {
                var list = await (from p in context.AppPosts
                                             join ps in context.AppPosts on new { parent_post_id = (int)p.ParentPostId } equals new { parent_post_id = ps.PostId } into ps_join
                                             from ps in ps_join.DefaultIfEmpty()
                                             join d in context.AppDepts on new { dept_id = (int)p.DeptId } equals new { dept_id = d.DeptId }
                                             join c in context.AppCorps on new { corp_id = (int)d.CorpId } equals new { corp_id = c.CorpId }
                                             select new
                                             {
                                                 p.PostId,
                                                 ParentPostId = (int?)p.ParentPostId,
                                                 ParentPostName = ps.PostName,
                                                 ParentPostCode = ps.PostCode,
                                                 p.PostCode,
                                                 p.PostName,
                                                 p.Note,
                                                 p.Status,
                                                 DeptId = (int?)p.DeptId,
                                                 d.DeptName,
                                                 CorpId = (int?)d.CorpId,
                                                 c.CorpName
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
                        if (CorpId!=-99)
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
                    if (!string.IsNullOrEmpty(PostCode))
                    {
                        list = list.Where(u => u.PostCode.Contains(PostCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(PostName))
                    {
                        list = list.Where(u => u.PostName.Contains(PostName)).ToList();
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
                logger.LogInformation(HttpContext.Session.GetString("who") + "查询岗位失败。" + ex.Message);
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
        public async Task<IActionResult> GetById(int PostId)
        {
            var single = await context.AppPosts.SingleOrDefaultAsync(u => u.PostId == PostId);
            return Json(new { code = single == null ? 1 : 0, msg = single == null ? "查询不到该岗位信息" : "查询成功", count = single == null ? 0 : 1, data = single });
        }
        #endregion

        #region 根据部门Id获取岗位
        [HttpGet]
        public async Task<IActionResult> GetPostTree(int DeptId)
        {
            try
            {
                var list = await context.AppPosts.Where(u=>u.DeptId==DeptId).ToListAsync();
                if (list.Count>0)
                {
                    var data = ToPostJson(list,0);
                    data = data.Replace("PostId","value").Replace("PostName","name");
                    return Json(new { code = 0, msg = "查询成功", data = data.ToJson() });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "查询岗位树数据为0行。");
                    return Json(new { code = 0, msg = "未查询到岗位", data = "[]".ToJson() });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "查询岗位树失败。" + ex.Message);
                return Json(new { code = 1, msg = "查询失败，请联系管理员", data = new { } });
            }
        }

        #region 转换成json树结构
        private string ToPostJson(List<Models.AppPost> data, int parentId)
        {
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("[");
            List<Models.AppPost> entitys = data.FindAll(t => t.ParentPostId == parentId);
            if (entitys.Count > 0)
            {
                foreach (var item in entitys)
                {
                    string strJson = item.ToJson();
                    strJson = strJson.Insert(strJson.Length - 1, ",\"children\":" + ToPostJson(data, item.PostId) + "");
                    sbJson.Append(strJson + ",");
                }
                sbJson = sbJson.Remove(sbJson.Length - 1, 1);
            }
            sbJson.Append("]");
            return sbJson.ToString();
        }
        #endregion
        #endregion

        #region 新增
        [HttpPost]
        public async Task<IActionResult> Insert(int DeptId,int ParentPostId, string PostCode, string PostName, string Note)
        {
            try
            {
                var single = await context.AppPosts.SingleOrDefaultAsync(u => u.PostCode == PostCode && u.DeptId == DeptId);
                if (single == null)
                {
                    var single1 = await context.AppPosts.SingleOrDefaultAsync(u => u.PostName == PostName && u.DeptId == DeptId);
                    if (single1 == null)
                    {
                        await context.AppPosts.AddAsync(new Models.AppPost()
                        {
                            DeptId = DeptId,
                            ParentPostId= ParentPostId,
                            PostCode = PostCode ?? "",
                            PostName = PostName ?? "",
                            Note = Note,
                            Status = "编辑",
                            CreationDate = DateTime.Now,
                            CreationUser = HttpContext.Session.GetInt32("user_id")
                        });
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "新增岗位成功。");
                        return Json(new { code = 200, msg = "新增成功" });
                    }
                    else
                    {
                        return Json(new { code = 300, msg = "该岗位名称在该部门中已经存在" });
                    }
                }
                else
                {
                    return Json(new { code = 300, msg = "该岗位代码在该部门中已经存在" });
                }

            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增岗位失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑
        [HttpPost]
        public async Task<IActionResult> Modify(
            int PostId, int DeptId, int ParentPostId, string PostCode, string PostName, string Note)
        {
            try
            {
                var single = await context.AppPosts.SingleOrDefaultAsync(u => u.PostCode == PostCode && u.DeptId == DeptId);
                if (single == null)
                {
                    var single1 = await context.AppPosts.SingleOrDefaultAsync(u => u.PostName == PostName && u.DeptId == DeptId);
                    if (single1 == null)
                    {
                        var modify = await context.AppPosts.SingleOrDefaultAsync(u => u.PostId == PostId);
                        if (modify != null)
                        {
                            modify.DeptId = DeptId;
                            modify.ParentPostId = ParentPostId;
                            modify.PostCode = PostCode ?? "";
                            modify.PostName = PostName ?? "";
                            modify.Note = Note ?? "";
                            modify.LastModifiedDate = DateTime.Now;
                            modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                            context.AppPosts.Update(modify);
                            await context.SaveChangesAsync();
                            logger.LogInformation(HttpContext.Session.GetString("who") + "新增岗位成功。");
                            return Json(new { code = 200, msg = "新增成功" });
                        }
                        else
                        {
                            return Json(new { code = 300, msg = "该岗位不存在与任何部门" });
                        }
                    }
                    else
                    {
                        return Json(new { code = 300, msg = "该岗位名称在该部门中已经存在" });
                    }
                }
                else
                {
                    if (single.PostId != PostId)
                    {
                        return Json(new { code = 300, msg = "该岗位代码在该部门中已经存在" });
                    }
                    else
                    {
                        var single1 = await context.AppPosts.SingleOrDefaultAsync(u => u.PostName == PostName && u.DeptId == DeptId);
                        if (single1 == null)
                        {
                            var modify = await context.AppPosts.SingleOrDefaultAsync(u => u.PostId == PostId);
                            if (modify != null)
                            {
                                modify.DeptId = DeptId;
                                modify.ParentPostId = ParentPostId;
                                modify.PostCode = PostCode ?? "";
                                modify.PostName = PostName ?? "";
                                modify.Note = Note ?? "";
                                modify.LastModifiedDate = DateTime.Now;
                                modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                                context.AppPosts.Update(modify);
                                await context.SaveChangesAsync();
                                logger.LogInformation(HttpContext.Session.GetString("who") + "新增岗位成功。");
                                return Json(new { code = 200, msg = "新增成功" });
                            }
                            else
                            {
                                return Json(new { code = 300, msg = "该岗位不存在与任何部门" });
                            }
                        }
                        else
                        {
                            if (single1.PostId != PostId)
                            {
                                return Json(new { code = 300, msg = "该岗位名称在该部门中已经存在" });
                            }
                            else
                            {
                                var modify = await context.AppPosts.SingleOrDefaultAsync(u => u.PostId == PostId);
                                if (modify != null)
                                {
                                    modify.DeptId = DeptId;
                                    modify.ParentPostId = ParentPostId;
                                    modify.PostCode = PostCode ?? "";
                                    modify.PostName = PostName ?? "";
                                    modify.Note = Note ?? "";
                                    modify.LastModifiedDate = DateTime.Now;
                                    modify.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                                    context.AppPosts.Update(modify);
                                    await context.SaveChangesAsync();
                                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增岗位成功。");
                                    return Json(new { code = 200, msg = "新增成功" });
                                }
                                else
                                {
                                    return Json(new { code = 300, msg = "该岗位不存在与任何部门" });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增岗位失败。" + ex.Message);
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
                        var single = await context.AppPosts.SingleOrDefaultAsync(u => u.PostId == id[i]);
                        if (single != null)
                        {
                            var list = await context.AppPosts.Where(u => u.ParentPostId == id[i]).ToListAsync();
                            if (list.Count > 0)
                            {
                                context.AppPosts.RemoveRange(list);
                                context.SaveChanges();
                            }
                            var list4 = await context.AppUsers.Where(u => u.PostId == id[i]).ToListAsync();
                            if (list4.Count > 0)
                            {
                                for (int k = 0; k < list4.Count; k++)
                                {
                                    list4[i].PostId = -999;
                                    list4[i].Status = "失效";
                                    list4[4].LastModifiedDate = DateTime.Now;
                                    list4[i].LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                                    context.AppUsers.Update(list4[i]);
                                    context.SaveChanges();
                                }
                            }
                            context.AppPosts.Remove(single);
                            context.SaveChanges();
                        }
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除岗位成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除岗位失败。" + ex.Message);
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
                var list = new List<Models.AppPost>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppPosts.SingleOrDefaultAsync(u => u.PostId == id[i]);
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
                    context.AppPosts.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效岗位成功。");
                    return Json(new { code = 200, msg = "生效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "生效岗位失败，岗位不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "岗位不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "生效岗位失败。" + ex.Message);
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
                var list = new List<Models.AppPost>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppPosts.SingleOrDefaultAsync(u => u.PostId == id[i]);
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
                    context.AppPosts.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效岗位成功。");
                    return Json(new { code = 200, msg = "失效成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "失效岗位失败，岗位不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "岗位不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "失效岗位失败。" + ex.Message);
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
                var list = new List<Models.AppPost>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppPosts.SingleOrDefaultAsync(u => u.PostId == id[i]);
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
                    context.AppPosts.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "岗位恢复编辑成功。");
                    return Json(new { code = 200, msg = "恢复编辑成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "岗位恢复编辑失败，岗位不存在或未勾选数据。");
                    return Json(new { code = 300, msg = "岗位不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "岗位恢复编辑失败。" + ex.Message);
                return Json(new { code = 300, msg = "恢复编辑失败" });
            }
        }
        #endregion

        #region 导出
        [HttpPost]
        public async Task<IActionResult> Export(int CorpId, int DeptId, string Status, string PostCode, string PostName)
        {
            try
            {
                var list = await (from p in context.AppPosts
                                  join ps in context.AppPosts on new { parent_post_id = (int)p.ParentPostId } equals new { parent_post_id = ps.PostId } into ps_join
                                  from ps in ps_join.DefaultIfEmpty()
                                  join d in context.AppDepts on new { dept_id = (int)p.DeptId } equals new { dept_id = d.DeptId }
                                  join c in context.AppCorps on new { corp_id = (int)d.CorpId } equals new { corp_id = c.CorpId }
                                  select new
                                  {
                                      p.PostId,
                                      ParentPostId = (int?)p.ParentPostId,
                                      ParentPostName = ps.PostName,
                                      ParentPostCode=ps.PostCode,
                                      p.PostCode,
                                      p.PostName,
                                      p.Note,
                                      p.Status,
                                      DeptId = (int?)p.DeptId,
                                      d.DeptName,
                                      CorpId = (int?)d.CorpId,
                                      c.CorpName
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
                    if (!string.IsNullOrEmpty(PostCode))
                    {
                        list = list.Where(u => u.PostCode.Contains(PostCode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(PostName))
                    {
                        list = list.Where(u => u.PostName.Contains(PostName)).ToList();
                    }
                }
                var list1 = list.Select(u => new
                {
                    CorpName = u.CorpName,
                    DeptName = u.DeptName,
                    PostCode = u.PostCode,
                    PostName = u.PostName,
                    ParentPostCode = u.PostCode,
                    ParentPostName = u.ParentPostName,
                    Note = u.Note,
                    Status = u.Status
                }).ToList();
                byte[] buffer = ExcelHelper.Export(list1, "岗位信息", ExcelTitle.AppPost).GetBuffer();
                return File(buffer, "application/ms-excel", "岗位信息.xls");
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：导出岗位信息失败。" + ex.Message);
                return File(new byte[] { }, "application/ms-excel", "岗位信息.xls");
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
                    List<ExcelAppPost> excel = excelHelper.GetList<ExcelAppPost>(path).ToList();
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
                                            if (string.IsNullOrEmpty(excel[i].PostCode))
                                            {
                                                returnMsg += "第" + (i + 1) + "行：岗位代码不能为空。\r\n";
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(excel[i].PostName))
                                                {
                                                    returnMsg += "第" + (i + 1) + "行：岗位名称不能为空。\r\n";
                                                }
                                                else
                                                {
                                                    var corp = await context.AppCorps.SingleOrDefaultAsync(u => u.CorpName == excel[i].CorpName);
                                                    if (corp != null)
                                                    {
                                                        var dept = await context.AppDepts.SingleOrDefaultAsync(u => u.DeptName == excel[i].DeptName&&u.CorpId==corp.CorpId);
                                                        if (dept == null)
                                                        {
                                                            returnMsg += "第" + (i + 1) + "行：该部门不存在或该部门不在所属公司下。\r\n";
                                                        }
                                                        else
                                                        {
                                                            if (!string.IsNullOrEmpty(excel[i].ParentPostCode))
                                                            {
                                                                var post = await context.AppPosts.SingleOrDefaultAsync(u => u.PostCode == excel[i].ParentPostCode&&u.DeptId==dept.DeptId);
                                                                if (post == null)
                                                                {
                                                                    if (!string.IsNullOrEmpty(excel[i].ParentPostName))
                                                                    {
                                                                        post = await context.AppPosts.SingleOrDefaultAsync(u => u.PostName == excel[i].ParentPostName && u.DeptId == dept.DeptId);
                                                                        if (post == null)
                                                                        {
                                                                            returnMsg += "第" + (i + 1) + "行：上级岗位不存在或上级岗位不在所属部门下。\r\n";
                                                                        }
                                                                        else
                                                                        {
                                                                            var postAdd = await context.AppPosts.SingleOrDefaultAsync(u => u.PostCode == excel[i].PostCode && u.DeptId == post.DeptId);
                                                                            if (postAdd == null)
                                                                            {
                                                                                postAdd = await context.AppPosts.SingleOrDefaultAsync(u => u.PostName == excel[i].PostName && u.DeptId == post.DeptId);
                                                                                if (postAdd == null)
                                                                                {
                                                                                    context.AppPosts.Add(new Models.AppPost()
                                                                                    {
                                                                                        ParentPostId = post.PostId,
                                                                                        PostCode = excel[i].PostCode ?? "",
                                                                                        PostName = excel[i].PostName ?? "",
                                                                                        DeptId = post.DeptId,
                                                                                        Note = excel[i].Note ?? "",
                                                                                        Status = excel[i].Status ?? "",
                                                                                        CreationDate = DateTime.Now,
                                                                                        CreationUser = HttpContext.Session.GetInt32("user_id")
                                                                                    });
                                                                                    await context.SaveChangesAsync();
                                                                                    returnMsg += "第" + (i + 1) + "行【" + excel[i].PostCode + "】" + excel[i].PostName + "：执行成功。\r\n";
                                                                                }
                                                                                else
                                                                                {
                                                                                    returnMsg += "第" + (i + 1) + "行：该岗位名称在所属部门下已经存在。\r\n";
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                returnMsg += "第" + (i + 1) + "行：该岗位代码在所属部门下已经存在。\r\n";
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        returnMsg += "第" + (i + 1) + "行：上级岗位不存在。\r\n";
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (!string.IsNullOrEmpty(excel[i].ParentPostName))
                                                                {
                                                                    var post = await context.AppPosts.SingleOrDefaultAsync(u => u.PostName == excel[i].ParentPostName&&u.DeptId==dept.DeptId);
                                                                    if (post == null)
                                                                    {
                                                                        returnMsg += "第" + (i + 1) + "行：上级岗位不存在或上级岗位不在所属部门下。\r\n";
                                                                    }
                                                                    else
                                                                    {
                                                                        var postAdd = await context.AppPosts.SingleOrDefaultAsync(u => u.PostCode == excel[i].PostCode && u.DeptId == post.DeptId);
                                                                        if (postAdd == null)
                                                                        {
                                                                            postAdd = await context.AppPosts.SingleOrDefaultAsync(u => u.PostName == excel[i].PostName && u.DeptId == post.DeptId);
                                                                            if (postAdd == null)
                                                                            {
                                                                                context.AppPosts.Add(new Models.AppPost()
                                                                                {
                                                                                    ParentPostId = post.PostId,
                                                                                    PostCode = excel[i].PostCode ?? "",
                                                                                    PostName = excel[i].PostName ?? "",
                                                                                    DeptId = post.DeptId,
                                                                                    Note = excel[i].Note ?? "",
                                                                                    Status = excel[i].Status ?? "",
                                                                                    CreationDate = DateTime.Now,
                                                                                    CreationUser = HttpContext.Session.GetInt32("user_id")
                                                                                });
                                                                                await context.SaveChangesAsync();
                                                                                returnMsg += "第" + (i + 1) + "行【" + excel[i].PostCode + "】" + excel[i].PostName + "：执行成功。\r\n";
                                                                            }
                                                                            else
                                                                            {
                                                                                returnMsg += "第" + (i + 1) + "行：该岗位名称在所属部门下已经存在。\r\n";
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            returnMsg += "第" + (i + 1) + "行：该岗位代码在所属部门下已经存在。\r\n";
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var postAdd = await context.AppPosts.SingleOrDefaultAsync(u => u.PostCode == excel[i].PostCode && u.DeptId == dept.DeptId);
                                                                    if (postAdd == null)
                                                                    {
                                                                        postAdd = await context.AppPosts.SingleOrDefaultAsync(u => u.PostName == excel[i].PostName && u.DeptId == dept.DeptId);
                                                                        if (postAdd == null)
                                                                        {
                                                                            context.AppPosts.Add(new Models.AppPost()
                                                                            {
                                                                                ParentPostId = 0,
                                                                                PostCode = excel[i].PostCode ?? "",
                                                                                PostName = excel[i].PostName ?? "",
                                                                                DeptId = dept.DeptId,
                                                                                Note = excel[i].Note ?? "",
                                                                                Status = excel[i].Status ?? "",
                                                                                CreationDate = DateTime.Now,
                                                                                CreationUser = HttpContext.Session.GetInt32("user_id")
                                                                            });
                                                                            await context.SaveChangesAsync();
                                                                            returnMsg += "第" + (i + 1) + "行【" + excel[i].PostCode + "】" + excel[i].PostName + "：执行成功。\r\n";
                                                                        }
                                                                        else
                                                                        {
                                                                            returnMsg += "第" + (i + 1) + "行：该岗位名称在所属部门下已经存在。\r\n";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        returnMsg += "第" + (i + 1) + "行：该岗位代码在所属部门下已经存在。\r\n";
                                                                    }
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
