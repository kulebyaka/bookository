using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookProgress.Web
{
    public class HandleAjaxErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var viewResult = new ViewResult() { ViewName = "~/Views/Shared/ErrorAjax.cshtml" };
                viewResult.ViewBag.Exception = filterContext.Exception;
                filterContext.Result = viewResult;
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.StatusCode = 500;
            }
            else
            {
                base.OnException(filterContext);
            }
        }
    }
}