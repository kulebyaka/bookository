using System.Web;
using System.Web.Optimization;

namespace BookProgress.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Static/js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Static/js/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Static/js/jquery.unobtrusive*",
                        "~/Static/js/jquery.validate*"));
            
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Static/js/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Static/js/bootstrap.js",
                      "~/Static/js/respond.js",
                      "~/Static/js/bootstrap-datetimepicker.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                      "~/Static/js/common.js"));

            bundles.Add(new StyleBundle("~/Static/css").Include("~/Static/site.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Static/css/font-awesome.min.css",
                        "~/Static/css/bootstrap.css",
                        "~/Static/css/bootstrap.min.css",
                        "~/Static/css/bootstrap-theme.min.css",
                        "~/Static/css/site.css",
                        "~/Static/css/Debugger.css",
                        "~/Static/css/bootstrap-datetimepicker.min.css",
                        "~/Static/css/styles.less",

                        "~/Static/css/jquery/jquery.ui.core.css",
                        "~/Static/css/jquery/jquery.ui.resizable.css",
                        "~/Static/css/jquery/jquery.ui.selectable.css",
                        "~/Static/css/jquery/jquery.ui.accordion.css",
                        "~/Static/css/jquery/jquery.ui.autocomplete.css",
                        "~/Static/css/jquery/jquery.ui.button.css",
                        "~/Static/css/jquery/jquery.ui.dialog.css",
                        "~/Static/css/jquery/jquery.ui.slider.css",
                        "~/Static/css/jquery/jquery.ui.tabs.css",
                        "~/Static/css/jquery/jquery.ui.datepicker.css",
                        "~/Static/css/jquery/jquery.ui.progressbar.css",
                        "~/Static/css/jquery/jquery.ui.theme.css"));
            
        }

    }
}