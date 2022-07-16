using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Auth
{
    public class Authorization : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public Authorization(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if(httpContext.Session["role"]!=null)
            {
                if (httpContext.Session["role"].Equals(allowedroles[0]))
                {
                    return true;
                }
            }
            return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Result = new HttpUnauthorizedResult();
            filterContext.Controller.TempData["Link"] = "/" + allowedroles[0] + "/login";
            filterContext.Result = new RedirectResult("/Error/Unauthorized/");
        }
    }
}