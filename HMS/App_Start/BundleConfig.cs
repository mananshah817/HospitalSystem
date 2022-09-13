using System.Web.Optimization;

namespace HMS.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Common Files
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.6.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                        "~/lib/angular/angular.js",
                        "~/lib/angular/angular-resource.js",
                        "~/lib/angular/angular-sanitize.js",
                        "~/lib/angular/angular-animate.js",
                        "~/lib/angular/angular-route.js",
                        "~/lib/angular/angular-cookies.js",
                        "~/lib/angular-input-masks/releases/angular-input-masks-standalone.js",
                        "~/lib/angularjs-datepicker/dist/angular-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/lib/bootstrap/dist/js/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/lib/bootstrap/dist/css/bootstrap.css",
                       "~/Lib/angular-ui-tree-master/dist/angular-ui-tree.min.css"));
            #endregion

            #region MRD Froms
            bundles.Add(new ScriptBundle("~/bundles/documentController").Include(
                    "~/App/MRD/MRD/DocumentController.js",
                    "~/App/Directive/fileModel.js",
                    "~/App/MRD/MRD/DocumentService.js"));

            bundles.Add(new ScriptBundle("~/bundles/mrdController").Include(
                     "~/App/MRD/MRD/MRDController.js",
                     "~/App/Directive/fileModel.js",
                     "~/App/MRD/MRD/MRDService.js",
                     "~/App/MRD/MRD/DocumentService.js"));
            #endregion

            BundleTable.EnableOptimizations = true;
        }
    }
}