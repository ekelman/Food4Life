using System.Web;
using System.Web.Optimization;

namespace Food4Life
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap_united.css",
                      "~/Content/Site.css",
                      "~/Content/Custom.css"));

            bundles.Add(new ScriptBundle("~/Scripts/wysiwyg").Include(
                      "~/Scripts/wysihtml5.js",
                      "~/Scripts/bootstrap-wysihtml5.js"));

            bundles.Add(new StyleBundle("~/Content/wysiwyg").Include(
                      "~/Content/bootstrap-wysihtml5.css",
                      "~/Content/bootstrap-wysiwyg5-color.css"));
        }
    }
}
