
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using HeuristicLab.Services.WebApp.Controllers;

namespace HeuristicLab.Services.WebApp.Configs {
  public static class WebApiConfig {
    public static void Register(HttpConfiguration config) {
      // Dynamic API Controllers
      config.Services.Replace(typeof(IHttpControllerSelector), new WebAppHttpControllerSelector(config));
      // Web API routes
      config.MapHttpAttributeRoutes();
      config.Routes.MapHttpRoute(
          name: "WebAppApi",
          routeTemplate: "api/{module}/{controller}/{action}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
      config.Formatters.JsonFormatter.MediaTypeMappings.Add(
        new QueryStringMapping("json", "true", "application/json")
      );
    }
  }
}