using System.Web.Optimization;

namespace CanoHealth.WebPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //TO IMPROVE SEE THE LAYAOUT.cshtml merge the jquery and bootstrap bundles into one called lib Third Party Libraries
            bundles.Add(new ScriptBundle("~/bundles/lib").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/underscore-min.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/bootbox.min.js",
                        "~/Scripts/toastr.js",
                        "~/Scripts/app/app.js",
                        "~/Scripts/kendo/2018.1.221/jszip.min.js",
                        "~/Scripts/kendo/2018.1.221/kendo.all.min.js",
                        "~/Scripts/kendo/2018.1.221/kendo.aspnetmvc.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/contracts").Include(
                        "~/Scripts/services/contractService.js",
                        "~/Scripts/controllers/contractsController.js",
                        "~/Scripts/kendoEventHandlers/contractEventHandlers.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/doctors").Include(
                "~/Scripts/services/doctorService.js",
                "~/Scripts/controllers/doctorsController.js",
                "~/Scripts/services/personalFileService.js",
                "~/Scripts/controllers/personalFileController.js",
                "~/Scripts/controllers/individualProvidersController.js",
                "~/Scripts/kendoEventHandlers/doctorEventsHandler.js",
                "~/Scripts/controllers/linkedContractController.js",
                "~/Scripts/controllers/linkedContractDetailController.js",
                "~/Scripts/kendoEventHandlers/linkedContractEventHandler.js"

            ));

            bundles.Add(new ScriptBundle("~/bundles/insurances").Include(
                "~/Scripts/services/insuranceService.js",
                "~/Scripts/controllers/insuranceController.js",
                "~/Scripts/kendoEventHandlers/insuranceEventsHandler.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/placeofservices").Include(
                "~/Scripts/services/placeOfService.js",
                "~/Scripts/controllers/placeOfServiceController.js",
                "~/Scripts/services/doctorService.js",
                "~/Scripts/controllers/doctorsController.js",
                "~/Scripts/kendoEventHandlers/placeOfServiceEventHandlers.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/razontheme").Include(
                        "~/Scripts/jquery-2.2.4.js", //"~/Scripts/jquery-{version}.js",
                        "~/Scripts/underscore-min.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/bootbox.min.js",
                        "~/Scripts/toastr.js",
                        "~/Scripts/app/app.js",
                        "~/Scripts/kendo/2018.1.221/jszip.min.js",
                        "~/Scripts/kendo/2018.1.221/kendo.all.min.js",
                        "~/Scripts/kendo/2018.1.221/kendo.aspnetmvc.min.js",

                        "~/Scripts/js/vendors.js",
                        "~/Scripts/js/syntaxhighlighter/shCore.js",
                        "~/Scripts/js/syntaxhighlighter/shBrushXml.js",
                        "~/Scripts/js/syntaxhighlighter/shBrushJScript.js",
                        "~/Scripts/js/DropdownHover.js",
                        "~/Scripts/js/razonartificialapp.js", //original app.js
                        "~/Scripts/js/holder.js"
            ));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/kendo/jszip.min.js",
                "~/Scripts/kendo/kendo.all.min.js",
                // "~/Scripts/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler
                "~/Scripts/kendo/kendo.aspnetmvc.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/Site.css",
                     "~/Content/toastr.css",

                     "~/Content/kendo/2018.1.221/kendo.common-bootstrap.min.css",
                     "~/Content/kendo/2018.1.221/kendo.bootstrap.min.css",
                     "~/Content/afterBootstrapAndKendoStyles.css",
                     "~/Content/font-awesome.css"
                     ));

            bundles.Add(new StyleBundle("~/Content/razontheme").Include(
                     "~/Content/bootstrap.css",

                     "~/Content/toastr.css",
                     "~/Content/font-awesome.css",
                     "~/Content/kendo/2018.1.221/kendo.common-bootstrap.min.css",
                     "~/Content/kendo/2018.1.221/kendo.bootstrap.min.css",
                     "~/Content/afterBootstrapAndKendoStyles.css",

                    "~/Content/preload.css",
                    "~/Content/vendors.css",
                    "~/Content/syntaxhighlighter/shCore.css",
                    "~/Content/style-green.css",
                    "~/Content/width-full.css",
                    "~/Content/Site_razonartificial_theme.css"
                ));

            bundles.Add(new StyleBundle("~/Content/kendo").Include(
                "~/Content/kendo/2018.1.221/kendo.common-bootstrap.min.css",
                "~/Content/kendo/2018.1.221/kendo.bootstrap.min.css"));
        }
    }
}

