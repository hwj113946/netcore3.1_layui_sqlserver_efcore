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

namespace netcore.Controllers.Menu
{
    public class MenuController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<MenuController> logger;
        public MenuController(netcore_databaseContext _context, ILogger<MenuController> _logger)
        {
            context = _context;
            logger = _logger;
        }

        [CheckCustomer]
        public IActionResult Menu()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult MenuEdit()
        {
            ViewBag.zt = HttpContext.Request.Query["zt"];
            ViewBag.type = HttpContext.Request.Query["type"];
            ViewBag.menuid = ViewBag.zt == "add" ? "" : HttpContext.Request.Query["menuid"].ToString();
            ViewBag.isone = HttpContext.Request.Query["isone"];
            ViewBag.parentid = ViewBag.isone == "0" ? HttpContext.Request.Query["menuid"] : HttpContext.Request.Query["parentid"];
            return View();
        }

        #region 获取所有菜单
        /// <summary>
        /// 菜单转json
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllMenu()
        {
            var list = await (from m in context.AppMenus
                              orderby m.MenuSort
                              select new
                              {
                                  m.MenuId,
                                  m.MenuName,
                                  m.MenuSort,
                                  m.MenuType,
                                  m.MenuUrl,
                                  m.MenuIcon,
                                  pid = m.ParentMenuId
                              }).ToListAsync();
            if (list.Count>0)
            {
                return Json(new { code = 200, msg = "已查询到数据", count = list.Count, data = list });
            }
            else
            {
                return Json(new { code = 300, msg = "暂无数据", count = list.Count, data = new { } });
            }
        }
        #endregion

        #region 根据菜单ID获取菜单信息
        [HttpGet]
        public async Task<IActionResult> GetMenuInfoById(int menu_id)
        {
            var menu = await context.AppMenus.SingleOrDefaultAsync(u => u.MenuId == menu_id);
            if (menu!=null)
            {
                return Json(new { code = 0, msg = "已查询到数据", data = menu });
            }
            else
            {
                return Json(new { code = 1, msg = "查无数据", data = new { } });
            }
        }
        #endregion

        #region 新增
        [HttpPost]
        public async Task<IActionResult> Insert(string menu_name, string menu_icon, string menu_url, string menu_type, double menu_sort, int parent_menu_id)
        {
            try
            {
                AppMenu appMenu = new AppMenu()
                {
                    MenuIcon = menu_icon,
                    MenuName = menu_name,
                    MenuSort = menu_sort,
                    MenuType = menu_type,
                    MenuUrl = menu_url,
                    ParentMenuId = parent_menu_id
                };
                await context.AppMenus.AddAsync(appMenu);
                await context.SaveChangesAsync();
                logger.LogError(HttpContext.Session.GetString("who") + "新增成功" );
                return Json(new { code = 200, msg = "新增成功" });
            }
            catch (Exception ex)
            {
                logger.LogError(HttpContext.Session.GetString("who") + "新增菜单出错:" +ex.Message);
                return Json(new { code = 300, msg = "新增失败，请联系管理员" });
            }
        }
        #endregion

        #region 编辑
        [HttpPost]
        public async Task<IActionResult> Modify( string menu_name, string menu_icon, string menu_url, string menu_type, double menu_sort, int parent_menu_id, int menu_id)
        {
            try
            {
                var menu = await context.AppMenus.SingleOrDefaultAsync(u => u.MenuId == menu_id);
                if (menu != null)
                {
                    menu.MenuIcon = menu_icon;
                    menu.MenuName = menu_name;
                    menu.MenuSort = menu_sort;
                    menu.MenuType = menu_type;
                    menu.MenuUrl = menu_url;
                    menu.ParentMenuId = parent_menu_id;
                    context.AppMenus.Update(menu);
                    await context.SaveChangesAsync();
                    logger.LogError(HttpContext.Session.GetString("who") + "编辑菜单成功");
                    return Json(new { code = 200, msg = "保存成功" });
                }
                else
                {
                    return Json(new { code = 300, msg = "查无数据", data = new { } });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(HttpContext.Session.GetString("who") + "编辑菜单出错:" + ex.Message);
                return Json(new { code = 300, msg = "保存失败，请联系管理员" });
            }            
        }
        #endregion

        #region 删除
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            using (var tran=context.Database.BeginTransaction())
            {
                try
                {
                    var m_p = await context.AppRoleMenus.Where(u => u.MenuId == id).ToListAsync();
                    if (m_p.Count > 0)
                    {
                        context.AppRoleMenus.RemoveRange(m_p);
                        context.SaveChanges();
                    }
                    var list = await context.AppMenus.Where(u => u.MenuId == id || u.ParentMenuId == id).ToListAsync();
                    if (list.Count > 0)
                    {
                        context.AppMenus.RemoveRange(list);
                        context.SaveChanges();
                        await tran.CommitAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "删除菜单成功");
                        return Json(new { code = 200, msg = "删除成功" });
                    }
                    return Json(new { code = 300, msg = "" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogError(HttpContext.Session.GetString("who") + "删除菜单出错:" + ex.Message);
                    return Json(new { code = 300, msg = "删除失败，请联系管理员" });
                }
            }
        }
        #endregion

        #region 获取菜单树
        [HttpGet]
        public async Task<IActionResult> GetMenuTree()
        {
            try
            {
                var list = await context.AppMenus.Where(u => u.MenuType == "菜单").ToListAsync();
                var json = ToMenuJson(list, 0);
                return Json(new { code = 0, msg = "success", data = json.ToJson() });
            }
            catch (Exception)
            {
                return Json(new { code = 1, msg = "false", data = new { } });
            }
        }

        private string ToMenuJson(List<AppMenu> data, int parentId)
        {
            if (data == null)
            {
                return "";
            }
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("[");
            List<AppMenu> entitys = data.FindAll(t => t.ParentMenuId == parentId);
            if (entitys.Count > 0)
            {
                foreach (var item in entitys)
                {
                    string strJson = item.ToJson();
                    strJson = strJson.Insert(strJson.Length - 1, ",\"children\":" + ToMenuJson(data, item.MenuId) + "");
                    sbJson.Append(strJson + ",");
                }
                sbJson = sbJson.Remove(sbJson.Length - 1, 1);
            }
            sbJson.Append("]");
            return sbJson.ToString().Replace("ParentMenuId","pid").Replace("MenuId", "value").Replace("MenuName","name");
        }
        #endregion
    }
}
