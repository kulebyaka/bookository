using System.Configuration;
using System.Web.Mvc;

namespace BookProgress.Web.Controllers
{
    public class BaseController : Controller
    {

        public static bool Debug
        {
            get
            {
                return ConfigurationManager.AppSettings["Debug"] == "true";
            }
        }

        protected bool IsFormSubmit()
        {
            return Request.Params["modalFormSubmit"] != null;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

    }
}