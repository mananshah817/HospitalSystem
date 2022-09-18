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
                        "~/Lib/angular/angular.js",
                        "~/Lib/angular/angular-resource.js",
                        "~/Lib/angular/angular-sanitize.js",
                        "~/Lib/angular/angular-animate.js",
                        "~/Lib/angular/angular-route.js",
                        "~/Lib/angular/angular-cookies.js",
                        "~/Lib/angular-input-masks/releases/angular-input-masks-standalone.js",
                        "~/Lib/angularjs-datepicker/dist/angular-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap.min.css"));
            #endregion

            #region MRD Froms
            bundles.Add(new ScriptBundle("~/bundles/mrdController").Include(
                     "~/App/MedicalRecord/MedicalRecordController.js",
                     "~/App/MedicalRecord/MedicalRecordService.js"));
            #endregion

            BundleTable.EnableOptimizations = true;
        }
    }
}