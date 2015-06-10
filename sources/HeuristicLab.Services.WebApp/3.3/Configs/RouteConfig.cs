using System.Web.Mvc;
using System.Web.Routing;

namespace HeuristicLab.Services.WebApp.Configs {
  public class RouteConfig {
    public static void RegisterRoutes(RouteCollection routes) {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "App", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
