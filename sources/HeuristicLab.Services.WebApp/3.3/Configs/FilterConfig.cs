using System.Web.Mvc;

namespace HeuristicLab.Services.WebApp.Configs {
  public class FilterConfig {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
      filters.Add(new HandleErrorAttribute());
    }
  }
}
