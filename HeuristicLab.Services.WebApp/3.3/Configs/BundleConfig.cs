#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System.Collections.Generic;
using System.IO;
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
        "~/WebApp/libs/angularjs/loading-bar/loading-bar.css",
        "~/WebApp/libs/angularjs/angular-tablesort/tablesort.css"
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
        // cryptojs
        "~/WebApp/libs/cryptojs/aes.js",
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
        "~/WebApp/libs/angularjs/angular-tablesort/angular-tablesort.js",
        "~/WebApp/libs/angularjs/angular-ui/ui-bootstrap-tpls-0.13.0.min.js",
        "~/WebApp/libs/angularjs/loading-bar/loading-bar.js",
        "~/WebApp/libs/angularjs/ocLazyLoad/ocLazyLoad.min.js",
        "~/WebApp/libs/angularjs/fittext/ng-FitText.js",
        // smoothScroll
        "~/WebApp/libs/smoothScroll/smoothScroll.js"
      ));

      // Application
      bundles.Add(new StyleBundle("~/Bundles/WebApp/css").Include(
        "~/WebApp/app.css"
      ));

      bundles.Add(new ScriptBundle("~/Bundles/WebApp/Main").Include(
        "~/WebApp/main.js"
      ));

      bundles.Add(new ScriptBundle("~/Bundles/WebApp/Shared")
        .IncludeDirectory("~/WebApp/shared/services", "*.js", true)
        .IncludeDirectory("~/WebApp/shared/directives", "*.js", true)
        .IncludeDirectory("~/WebApp/shared/filter", "*.js", true)
        .IncludeDirectory("~/WebApp/shared/menu", "*.js", true)
      );
    }

    public static IEnumerable<string> GetWebAppScripts() {
      var jsFiles = new List<string> {
        "WebApp/helper.js",
        "WebApp/app.js"
      };
      PluginManager pluginManager = PluginManager.Instance;
      foreach (var plugin in pluginManager.Plugins) {
        var path = Path.Combine(PluginManager.PluginsDirectory, plugin.Name, string.Concat(plugin.Name, ".js"));
        if (File.Exists(path)) {
          jsFiles.Add(string.Format("WebApp/plugins/{0}/{0}.js", plugin.Name));
        }
      }
      jsFiles.Add("WebApp/main.js");
      return jsFiles;
    }
  }
}
