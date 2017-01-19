using System.Web;
using System.Web.Optimization;
using System.Web.Optimization.React;

namespace TornadoSDK {
    public class BundleConfig {

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/react").Include(
                      "~/Scripts/react.js",
                      "~/Scripts/react-dom.js",
                      "~/Scripts/reflux.min.js"
                      //"~/Scripts/babel.min.js"
                      ));

            bundles.Add(new BabelBundle("~/bundles/home/index").Include(
                      "~/Scripts/React-components/Home/Index/index-components.jsx",
                      "~/Scripts/React-jsx/Home/Index/index.jsx"));
        }
    }
}
