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
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace netcore.Controllers.News
{
    public class NewsController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<NewsController> logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        public NewsController(netcore_databaseContext _context, ILogger<NewsController> _logger, IWebHostEnvironment _hostingEnvironment)
        {
            context = _context;
            logger = _logger;
            hostingEnvironment = _hostingEnvironment;
        }

        [CheckCustomer]
        public IActionResult News()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult NewsCard()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult NewsEdit()
        {
            ViewBag.status = HttpContext.Request.Query["status"].ToString();
            ViewBag.NewsId = ViewBag.status == "add" ? "" : HttpContext.Request.Query["NewsId"].ToString();
            return View();
        }

        public IActionResult NewsView()
        {
            ViewBag.status = HttpContext.Request.Query["status"].ToString() ?? "";
            ViewBag.news_id = HttpContext.Request.Query["Rowid"].ToString() ?? "";
            return View();
        }

        #region 获取新闻列表:Table
        [HttpGet]
        public async Task<IActionResult> GetNewsList(int NewsTypeId,string NewsTitle,string Status,int limit,int page)
        {
            try
            {
                var list = await (from n in context.AppNews
                                  join t in context.AppNewsTypes
                                  on n.NewsTypeId equals t.NewsTypeId
                                  select new
                                  {
                                      n.BrowseNumber,
                                      n.CreationDate,
                                      n.CreationUser,
                                      n.LastModifiedDate,
                                      n.LastModifiedUser,
                                      n.NewsAuthor,
                                      n.NewsContent,
                                      n.NewsCoverImageUrl,
                                      n.NewsId,
                                      n.NewsReleaseTime,
                                      n.NewsTitle,
                                      n.NewsTypeId,
                                      n.Status,
                                      t.NewsTypeName
                                  }).OrderByDescending(u => u.NewsReleaseTime).ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    if (NewsTypeId==0)
                    {

                    }
                    else
                    {
                        list = list.Where(u => u.NewsTypeId == NewsTypeId).ToList();
                    }
                    if (!string.IsNullOrEmpty(Status))
                    {
                        if (Status=="全部")
                        {

                        }
                        else
                        {
                            list = list.Where(u => u.Status == Status).ToList();
                        }                        
                    }
                    if (!string.IsNullOrEmpty(NewsTitle))
                    {
                        list = list.Where(u => u.NewsTitle == NewsTitle).ToList();
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

        #region 获取新闻列表:Flow
        [HttpGet]
        public async Task<IActionResult> GetNewsFlow(int page)
        {
            try
            {
                var list = await (from n in context.AppNews
                                  join t in context.AppNewsTypes
                                  on n.NewsTypeId equals t.NewsTypeId
                                  where n.Status.Equals("已发布")
                                  select new
                                  {
                                      n.BrowseNumber,
                                      n.CreationDate,
                                      n.CreationUser,
                                      n.LastModifiedDate,
                                      n.LastModifiedUser,
                                      n.NewsAuthor,
                                      n.NewsContent,
                                      n.NewsCoverImageUrl,
                                      n.NewsId,
                                      n.NewsReleaseTime,
                                      n.NewsTitle,
                                      n.NewsTypeId,
                                      n.Status,
                                      t.NewsTypeName
                                  }).OrderByDescending(u=>u.NewsReleaseTime).ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    count = Math.Ceiling(Convert.ToDecimal(list.Count) / Convert.ToDecimal(6));
                    if (page > count)
                    {
                        return Json(new { code = 1, msg = "没有更多数据了。", count = Convert.ToInt32(count), data = new { } });
                    }
                    list = list.Skip((page - 1) * 6).Take(6).ToList();
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

        #region 根据ID获取新闻数据信息
        [HttpGet]
        public async Task<IActionResult> GetNewsById(int news_id)
        {
            try
            {
                var single = await context.AppNews.SingleOrDefaultAsync(u => u.NewsId == news_id);
                if (single != null)
                {
                    single.BrowseNumber = single.BrowseNumber + 1;
                    context.AppNews.Update(single);
                    await context.SaveChangesAsync();
                    var news = await (from n in context.AppNews
                                      join t in context.AppNewsTypes
                                      on n.NewsTypeId equals t.NewsTypeId
                                      where n.NewsId.Equals(news_id)
                                      select new
                                      {
                                          n.BrowseNumber,
                                          n.CreationDate,
                                          n.CreationUser,
                                          n.LastModifiedDate,
                                          n.LastModifiedUser,
                                          n.NewsAuthor,
                                          n.NewsContent,
                                          n.NewsCoverImageUrl,
                                          n.NewsId,
                                          n.NewsReleaseTime,
                                          n.NewsTitle,
                                          n.NewsTypeId,
                                          n.Status,
                                          t.NewsTypeName
                                      }).SingleOrDefaultAsync();
                    return Json(new { code = 0, msg = "查询成功", count = 1, data = news });
                }
                else
                {
                    return Json(new { code = 1, msg = "查询不到该新闻", count = 0, data = new { } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 1, msg = ex.Message, count = 0, data = new { } });
            }
        }
        #endregion

        #region 根据ID获取新闻数据信息:不增加浏览次数
        [HttpGet]
        public async Task<IActionResult> GetNewsByIdNotAdd(int news_id)
        {
            try
            {
                var news = await (from n in context.AppNews
                                  join t in context.AppNewsTypes
                                  on n.NewsTypeId equals t.NewsTypeId
                                  where n.NewsId.Equals(news_id)
                                  select new
                                  {
                                      n.BrowseNumber,
                                      n.CreationDate,
                                      n.CreationUser,
                                      n.LastModifiedDate,
                                      n.LastModifiedUser,
                                      n.NewsAuthor,
                                      n.NewsContent,
                                      n.NewsCoverImageUrl,
                                      n.NewsId,
                                      n.NewsReleaseTime,
                                      n.NewsTitle,
                                      n.NewsTypeId,
                                      n.Status,
                                      t.NewsTypeName
                                  }).SingleOrDefaultAsync();
                if (news != null)
                {
                    return Json(new { code = 0, msg = "查询成功", count = 1, data = news });
                }
                else
                {
                    return Json(new { code = 1, msg = "查询不到该新闻", count = 0, data = new { } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 1, msg = ex.Message, count = 0, data = new { } });
            }
        }
        #endregion

        #region 获取新闻类型
        [HttpGet]
        public async Task<IActionResult> GetNewsType()
        {
            try
            {
                var list = await context.AppNewsTypes.ToListAsync();
                if (list.Count>0)
                {
                    return Json(new { code =0, msg = "查询成功", count = list.Count, data = list });
                }
                else
                {
                    return Json(new { code = 0, msg = "查询成功，与查询条件相符的数据为0行", count = 0, data = new { } });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "：查询新闻类型失败。" + ex.Message);
                return Json(new { code = 1, msg = ex.Message, count = 0, data = new { } });
            }
        }
        #endregion

        #region 封面图片上传
        [HttpPost]
        public async Task<IActionResult> UploadCoverImage()
        {
            try
            {
                var file = Request.Form.Files[0];
                //var file = files[0];
                var path = hostingEnvironment.WebRootPath + "\\News\\CoverImages";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var filename = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);//file.FileName;
                using (var fs = new FileStream(path + "\\" + filename, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fs);
                    fs.Close();
                }
                return Json(new { code = 200, msg = "上传完成", url = "/News/CoverImages/" + filename });
            }
            catch (Exception)
            {
                return Json(new { code = 300, msg = "上传失败", url = "" });
            }
        }
        #endregion

        #region 图片上传
        [HttpPost]
        public async Task<IActionResult> ImageFileUpload()
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Count == 0)
                {
                    return Json(new { code = 300, msg = "没有文件", location = "" });
                }
                var file = files[0];
                var path = hostingEnvironment.WebRootPath + "\\News\\images";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var filename = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);//file.FileName;
                using (var fs = new FileStream(path + "\\" + filename, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fs);
                    fs.Close();
                }
                return Json(new { code = 200, msg = "上传完成", location = "/News/images/" + filename });
            }
            catch (Exception)
            {
                return Json(new { code = 300, msg = "执行出错", location = "" });
            }
        }
        #endregion

        #region 文件上传
        [HttpPost]
        public async Task<IActionResult> FileUpload()
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Count == 0)
                {
                    return Json(new { code = 300, msg = "没有文件", location = "" });
                }
                var file = files[0];
                var path = hostingEnvironment.WebRootPath + "\\News\\Files";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var filename = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);//file.FileName;
                using (var fs = new FileStream(path + "\\" + filename, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fs);
                    fs.Close();
                }
                return Json(new { code = 200, msg = "上传完成", location = "/News/Files/" + filename });
            }
            catch (Exception)
            {
                return Json(new { code = 300, msg = "执行出错", location = "" });
            }
        }
        #endregion

        #region 新增新闻
        [HttpPost]
        public async Task<IActionResult> Insert(int NewsId,string NewsTitle,int NewsTypeId,string NewsAuthor,string NewsCoverImageUrl,string NewsContent)
        {
            try
            {
                AppNews single = new AppNews()
                {
                    NewsTitle = NewsTitle ?? "",
                    NewsTypeId = NewsTypeId,
                    NewsAuthor = NewsAuthor ?? "",
                    NewsCoverImageUrl = NewsCoverImageUrl ?? "",
                    NewsContent = NewsContent == null ? "" : NewsContent.Replace("\"", "&quot;"),
                    BrowseNumber = 0,
                    CreationDate = DateTime.Now,
                    CreationUser = HttpContext.Session.GetInt32("user_id"),
                    Status = "编辑"
                };
                context.AppNews.Add(single);
                await context.SaveChangesAsync();
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增新闻成功。");
                return Json(new { code = 200, msg = "新增成功" });
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增新闻失败。" + ex.Message);
                return Json(new { code = 300, msg = "新增失败" });
            }
        }
        #endregion

        #region 编辑新闻
        [HttpPost]
        public async Task<IActionResult> Modify(int NewsId, string NewsTitle, int NewsTypeId, string NewsAuthor, string NewsCoverImageUrl, string NewsContent)
        {
            try
            {
                var single = await context.AppNews.SingleOrDefaultAsync(u=>u.NewsId==NewsId);
                if (single!=null)
                {
                    single.NewsTitle = NewsTitle ?? "";
                    single.NewsTypeId = NewsTypeId;
                    single.NewsAuthor = NewsAuthor ?? "";
                    single.NewsCoverImageUrl = NewsCoverImageUrl ?? "";
                    single.NewsContent = NewsContent == null ? "" : NewsContent.Replace("\"", "&quot;");
                    single.LastModifiedDate = DateTime.Now;
                    single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                    context.AppNews.Update(single);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "编辑新闻成功。");
                    return Json(new { code = 200, msg = "编辑成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "编辑：新闻不存在。");
                    return Json(new { code = 300, msg = "新闻不存在，无法编辑" });
                }                
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑新闻失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除新闻
        [HttpPost]
        public async Task<IActionResult> Delete(int[] id)
        {
            try
            {
                List<AppNews> list = new List<AppNews>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppNews.SingleOrDefaultAsync(u => u.NewsId == id[i]);
                    if (single != null)
                    {
                        list.Add(single);
                    }
                }
                if (list.Count > 0)
                {
                    context.AppNews.RemoveRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除新闻成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除新闻失败，勾选的数据已不存在或未勾选数据.");
                    return Json(new { code = 300, msg = "勾选的数据已不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "删除新闻失败。" + ex.Message);
                return Json(new { code = 300, msg = "删除失败，请联系管理员" });
            }
        }
        #endregion

        #region 发布新闻
        [HttpPost]
        public async Task<IActionResult> ReleaseNews(int[] id)
        {
            try
            {
                List<AppNews> list = new List<AppNews>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppNews.SingleOrDefaultAsync(u => u.NewsId == id[i]);
                    if (single != null)
                    {
                        single.Status = "已发布";
                        single.LastModifiedDate = DateTime.Now;
                        single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        list.Add(single);
                    }
                }
                if (list.Count > 0)
                {
                    context.AppNews.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "发布新闻成功。");
                    return Json(new { code = 200, msg = "发布成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "发布新闻失败，勾选的数据已不存在或未勾选数据.");
                    return Json(new { code = 300, msg = "勾选的数据已不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "发布新闻失败。" + ex.Message);
                return Json(new { code = 300, msg = "发布失败，请联系管理员" });
            }
        }
        #endregion

        #region 取消发布新闻
        [HttpPost]
        public async Task<IActionResult> CancelNews(int[] id)
        {
            try
            {
                List<AppNews> list = new List<AppNews>();
                for (int i = 0; i < id.Length; i++)
                {
                    var single = await context.AppNews.SingleOrDefaultAsync(u => u.NewsId == id[i]);
                    if (single != null)
                    {
                        single.Status = "取消发布";
                        single.LastModifiedDate = DateTime.Now;
                        single.LastModifiedUser = HttpContext.Session.GetInt32("user_id");
                        list.Add(single);
                    }
                }
                if (list.Count > 0)
                {
                    context.AppNews.UpdateRange(list);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "取消发布新闻成功。");
                    return Json(new { code = 200, msg = "取消发布成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "取消发布新闻失败，勾选的数据已不存在或未勾选数据.");
                    return Json(new { code = 300, msg = "勾选的数据已不存在或未勾选数据" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "取消发布新闻失败。" + ex.Message);
                return Json(new { code = 300, msg = "取消发布失败，请联系管理员" });
            }
        }
        #endregion
    }
}
