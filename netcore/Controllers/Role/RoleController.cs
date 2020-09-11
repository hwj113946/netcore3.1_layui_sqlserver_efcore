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

namespace netcore.Controllers.Role
{
    public class RoleController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<RoleController> logger;
        public RoleController(netcore_databaseContext _context, ILogger<RoleController> _logger)
        {
            context = _context;
            logger = _logger;
        }

        [CheckCustomer]
        public IActionResult Role()
        {
            return View();
        }

        [CheckCustomer]
        public IActionResult RoleEdit()
        {
            ViewBag.status = HttpContext.Request.Query["status"];
            ViewBag.role_id = ViewBag.status == "add" ? "" : HttpContext.Request.Query["Rowid"].ToString();
            return View();
        }

        #region 获取角色
        [HttpGet]
        public async Task<IActionResult> GetRole(string role_name, int page, int limit)
        {
            try
            {
                var list = await context.AppRoles.ToListAsync();
                decimal count = 0;
                if (list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(role_name))
                    {
                        list = list.Where(u => u.RoleName.Contains(role_name)).ToList();
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

        #region 根据角色ID获取数据信息
        [HttpGet]
        public async Task<IActionResult> GetRoleById(int role_id)
        {
            var role = await context.AppRoles.SingleOrDefaultAsync(u => u.RoleId == role_id);
            return Json(new { code = role == null ? 1 : 0, msg = role == null ? "查询不到该角色" : "查询成功", count = role == null ? 0 : 1, data = role });
        }
        #endregion

        #region 新增角色
        [HttpPost]
        public async Task<IActionResult> Insert(string role_name)
        {
            try
            {
                var role = await context.AppRoles.SingleOrDefaultAsync(u => u.RoleName == role_name);
                if (role==null)
                {
                    await context.AppRoles.AddAsync(new AppRole()
                    {
                        RoleName = role_name ?? "",
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "新增角色成功。");
                    return Json(new { code = 200, msg = "新增成功" });
                }
                else
                {
                    return Json(new { code = 300, msg = "角色名称已存在" });
                }                
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "新增角色失败。"+ex.Message);
                return Json(new { code = 300, msg ="新增失败" });
            }
        }
        #endregion

        #region 修改角色
        [HttpPost]
        public async Task<IActionResult> Modify(string role_name, int role_id)
        {
            try
            {
                var role = await context.AppRoles.SingleOrDefaultAsync(u => u.RoleName == role_name);
                if (role == null)
                {
                    var modify=await context.AppRoles.SingleOrDefaultAsync(u => u.RoleId == role_id);
                    if (modify!=null)
                    {
                        modify.RoleName = role_name ?? "";
                        context.AppRoles.Update(modify);
                        await context.SaveChangesAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "编辑角色成功。");
                        return Json(new { code = 200, msg = "编辑成功" });
                    }
                    else
                    {
                        return Json(new { code = 300, msg = "角色不存在" });
                    }                    
                }
                else
                {
                    if (role.RoleId!=role_id)
                    {
                        return Json(new { code = 300, msg = "角色名称已存在" });
                    }
                    else
                    {
                        var modify = await context.AppRoles.SingleOrDefaultAsync(u => u.RoleId == role_id);
                        if (modify != null)
                        {
                            modify.RoleName = role_name ?? "";
                            context.AppRoles.Update(modify);
                            await context.SaveChangesAsync();
                            logger.LogInformation(HttpContext.Session.GetString("who") + "编辑角色成功。");
                            return Json(new { code = 200, msg = "编辑成功" });
                        }
                        else
                        {
                            return Json(new { code = 300, msg = "角色不存在" });
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "编辑角色失败。" + ex.Message);
                return Json(new { code = 300, msg = "编辑失败" });
            }
        }
        #endregion

        #region 删除角色
        [HttpPost]
        public async Task<IActionResult> Delete(int[] id)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var list = new List<AppRole>();
                    for (int i = 0; i < id.Length; i++)
                    {
                        var roleMenu = await context.AppRoleMenus.Where(u => u.RoleId == id[i]).ToListAsync();
                        if (roleMenu.Count > 0)
                        {
                            context.AppRoleMenus.RemoveRange(roleMenu);
                            context.SaveChanges();
                        }
                        var userRole = await context.AppUserRoles.Where(u => u.RoleId == id[i]).ToListAsync();
                        if (userRole.Count > 0)
                        {
                            context.AppUserRoles.RemoveRange(userRole);
                            context.SaveChanges();
                        }
                        var single = await context.AppRoles.SingleOrDefaultAsync(u => u.RoleId == id[i]);
                        if (single != null)
                        {
                            list.Add(single);
                        }
                    }
                    if (list.Count > 0)
                    {
                        context.AppRoles.RemoveRange(list);
                        context.SaveChanges();
                    }
                    await tran.CommitAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除角色成功。");
                    return Json(new { code = 200, msg = "删除成功" });
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "删除角色失败。" + ex.Message);
                    return Json(new { code = 300, msg = "删除失败" });
                }
            }            
        }
        #endregion

        #region 获取角色权限树
        [HttpGet]
        public async Task<IActionResult> GetRoleMenuTree(int RoleId)
        {
            var rmt = await (from t in context.AppMenus
                             join rm in context.AppRoleMenus on new { t.MenuId, RoleId = (int)RoleId }
                               equals new { rm.MenuId, rm.RoleId } into rm_join
                             from rm in rm_join.DefaultIfEmpty()
                             orderby
                               t.MenuSort
                             select new RoleMenuTree
                             {
                                 id = t.MenuId,
                                 pid = t.ParentMenuId,
                                 menu_type = t.MenuType,
                                 title = t.MenuName,
                                 @checked =
                               rm.MenuId == null ? "false" : (
                               t.MenuType == "菜单" ? "false" : "true"),
                                 spread = "true"
                             }).ToListAsync();
            string json = ToMenuJson(rmt, 0);
            return Json(json.ToJson().ToString().Replace("\"false\"", "false").Replace("\"true\"", "true").ToJson());
        }

        private string ToMenuJson(List<RoleMenuTree> data, int parentId)
        {
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("[");
            List<RoleMenuTree> entitys = data.FindAll(t => t.pid == parentId);
            if (entitys.Count > 0)
            {
                foreach (var item in entitys)
                {
                    string strJson = item.ToJson();
                    strJson = strJson.Insert(strJson.Length - 1, ",\"children\":" + ToMenuJson(data, item.id) + "");
                    sbJson.Append(strJson + ",");
                }
                sbJson = sbJson.Remove(sbJson.Length - 1, 1);
            }
            sbJson.Append("]");
            return sbJson.ToString();
        }
        #endregion

        #region 保存权限
        [HttpPost]
        public async Task<IActionResult> SaveRole(int RoleId, int[] menu_id)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var rm = await context.AppRoleMenus.Where(u => u.RoleId == RoleId).ToListAsync();
                    if (rm.Count > 0)
                    {
                        context.AppRoleMenus.RemoveRange(rm);
                        context.SaveChanges();
                    }
                    var list = new List<AppRoleMenu>();
                    for (int i = 0; i < menu_id.Length; i++)
                    {
                        list.Add(new AppRoleMenu()
                        {
                            MenuId = menu_id[i],
                            RoleId = RoleId
                        });
                    }
                    if (list.Count > 0)
                    {
                        context.AppRoleMenus.AddRange(list);
                        context.SaveChanges();
                        await tran.CommitAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "保存角色权限成功。");
                        return Json(new { code = 200, msg = "保存成功" });
                    }
                    else
                    {
                        await tran.CommitAsync();
                        logger.LogInformation(HttpContext.Session.GetString("who") + "保存角色权限成功，但未分配任何菜单。");
                        return Json(new { code = 300, msg = "保存成功，但未分配任何菜单" });
                    }
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "保存角色权限失败。"+ex.Message);
                    return Json(new { code = 300, msg = "保存失败" });
                }
            }
        }
        #endregion

        #region 清空权限
        [HttpPost]
        public async Task<IActionResult> AcceptReset(int RoleId)
        {
            try
            {
                var rm = await context.AppRoleMenus.Where(u => u.RoleId == RoleId).ToListAsync();
                if (rm.Count > 0)
                {
                    context.AppRoleMenus.RemoveRange(rm);
                    await context.SaveChangesAsync();
                    logger.LogInformation(HttpContext.Session.GetString("who") + "重置角色权限成功。");
                    return Json(new { code = 200, msg = "重置成功" });
                }
                else
                {
                    logger.LogInformation(HttpContext.Session.GetString("who") + "重置失败，该角色未分配任何菜单。");
                    return Json(new { code = 300, msg = "重置失败，该角色未分配任何菜单" });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(HttpContext.Session.GetString("who") + "重置失败。"+ex.Message);
                return Json(new { code = 300, msg = "重置失败" });
            }
        }
        #endregion
    }
}
