#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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