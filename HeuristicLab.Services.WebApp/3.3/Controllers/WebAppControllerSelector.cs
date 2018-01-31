#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace HeuristicLab.Services.WebApp.Controllers {
  public class WebAppHttpControllerSelector : DefaultHttpControllerSelector {
    private readonly HttpConfiguration configuration;
    private readonly IDictionary<string, HttpControllerDescriptor> controllers;
    private readonly PluginManager pluginManager = PluginManager.Instance;

    public WebAppHttpControllerSelector(HttpConfiguration configuration)
      : base(configuration) {
      this.configuration = configuration;
      controllers = new ConcurrentDictionary<string, HttpControllerDescriptor>();
      LoadAppControllers();
    }

    private void LoadAppControllers() {
      var assembly = Assembly.GetExecutingAssembly();
      var assemblyTypes = assembly.GetTypes();
      var apiControllers = assemblyTypes.Where(c => typeof(ApiController).IsAssignableFrom(c)).ToList();
      foreach (var apiController in apiControllers) {
        var apiControllerName = apiController.Name.Remove(apiController.Name.Length - 10).ToLower();
        controllers.Add(apiControllerName, new HttpControllerDescriptor(configuration, apiControllerName, apiController));
      }
    }

    public override HttpControllerDescriptor SelectController(HttpRequestMessage request) {
      if (request == null) {
        throw new ArgumentNullException("request");
      }
      var parts = request.RequestUri.AbsolutePath.Split('/');
      int startIndex = parts.TakeWhile(part => part.ToLower() != "api").Count();
      if (parts.Length < startIndex + 2) {
        throw new ArgumentException("invalid request path");
      }
      string pluginName = parts[startIndex + 1].ToLower();
      string controllerName = parts[startIndex + 2].ToLower();
      // load controller
      if (pluginName == "app") {
        // from main app
        HttpControllerDescriptor controller;
        controllers.TryGetValue(controllerName, out controller);
        return controller;
      }
      // from plugin
      var plugin = pluginManager.GetPlugin(pluginName);
      if (plugin == null) {
        throw new ArgumentException(string.Format("invalid plugin '{0}'", pluginName));
      }
      return plugin.GetController(controllerName);
    }
  }
}