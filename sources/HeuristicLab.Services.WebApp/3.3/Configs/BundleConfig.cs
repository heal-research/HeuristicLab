using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace HeuristicLab.Services.WebApp.Configs {
  public class BundleConfig {
    public static void RegisterBundles(BundleCollection bundles) {
      bundles.IgnoreList.Clear();

      // IESupport
      bundles.Add(new ScriptBundle("~/Bundles/IESupport").Include(
        "~/WebApp/libs/misc/html5shiv.min.js",
        "~/WebApp/libs/misc/respond.min.js"
      ));

      // Vendors
      bundles.Add(new StyleBundle("~/Bundles/Vendors/css").Include(
        "~/WebApp/libs/bootstrap/css/bootstrap.min.css",
        "~/WebApp/libs/bootstrap/css/bootstrap-theme.min.css",
        "~/WebApp/libs/font-aweseome/font-aweseome.min.css",
        "~/WebApp/libs/angularjs/loading-bar/loading-bar.css"
      ));

      bundles.Add(new ScriptBundle("~/Bundles/Vendors/js").Include(
        // jquery
        "~/WebApp/libs/jquery/jquery-2.1.4.min.js",
        "~/WebApp/libs/jquery/jquery-ui/jquery-ui-1.11.4.min.js",
        "~/WebApp/libs/jquery/jquery-knob/jquery.knob.min.js",
        "~/WebApp/libs/jquery/jquery-flot/excanvas.min.js",
        "~/WebApp/libs/jquery/jquery-flot/jquery.flot.min.js",
        "~/WebApp/libs/jquery/jquery-flot/jquery.flot.time.min.js",
        "~/WebApp/libs/jquery/jquery-flot/jquery.flot.selection.min.js",
        "~/WebApp/libs/jquery/jquery-flot/jquery.flot.navigate.min.js",
        "~/WebApp/libs/jquery/jquery-flot/jquery.flot.resize.min.js",
        "~/WebApp/libs/jquery/jquery-flot/jquery.flot.stack.min.js",
        // bootstrap
        "~/WebApp/libs/bootstrap/js/bootstrap.min.js",
        // angular js
        "~/WebApp/libs/angularjs/angular.min.js",
        "~/WebApp/libs/angularjs/angular-route.min.js",
        "~/WebApp/libs/angularjs/angular-aria.min.js",
        "~/WebApp/libs/angularjs/angular-cookies.min.js",
        "~/WebApp/libs/angularjs/angular-loader.min.js",
        "~/WebApp/libs/angularjs/angular-messages.min.js",
        "~/WebApp/libs/angularjs/angular-resource.min.js",
        "~/WebApp/libs/angularjs/angular-sanitize.min.js",
        "~/WebApp/libs/angularjs/angular-touch.min.js",
        "~/WebApp/libs/angularjs/angular-ui-router.min.js",
        "~/WebApp/libs/angularjs/angular-knob/angular-knob.js",
        "~/WebApp/libs/angularjs/angular-ui/ui-bootstrap-tpls-0.13.0.min.js",
        "~/WebApp/libs/angularjs/loading-bar/loading-bar.js",
        "~/WebApp/libs/angularjs/ocLazyLoad/ocLazyLoad.min.js",
        // smoothScroll
        "~/WebApp/libs/smoothScroll/smoothScroll.js"
      ));

      // Application
      bundles.Add(new StyleBundle("~/Bundles/WebApp/css").Include(
        "~/WebApp/app.css"
      ));
      AddOrUpdateWebAppBundle();
    }

    public static void AddOrUpdateWebAppBundle() {
      var jsBundle = BundleTable.Bundles.GetBundleFor("~/Bundles/WebApp/js");
      if (jsBundle != null) {
        BundleTable.Bundles.Remove(jsBundle);
      }
      var jsFiles = new List<string> {
        "~/WebApp/helper.js",
        "~/WebApp/app.js"
      };
      var directories = Directory.GetDirectories(string.Format(@"{0}WebApp\plugins", HttpRuntime.AppDomainAppPath));
      jsFiles.AddRange(directories.Select(Path.GetFileName).Select(directory => string.Format("~/WebApp/plugins/{0}/{0}.js", directory)));
      jsFiles.Add("~/WebApp/main.js");
      jsBundle = new ScriptBundle("~/Bundles/WebApp/js");
      jsBundle.Include(jsFiles.ToArray());
      jsBundle.IncludeDirectory("~/WebApp/shared/", "*.js", true);
      BundleTable.Bundles.Add(jsBundle);
    }
  }
}
