using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore
{
    public class CheckCustomerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session.GetInt32("user_id") == null)//判断session是否为null
            {
                filterContext.HttpContext.Response.Redirect("/Login/Index");//跳转到登陆界面
            }
        }
    }
}
