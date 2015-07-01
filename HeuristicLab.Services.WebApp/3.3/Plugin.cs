#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace HeuristicLab.Services.WebApp {
  public class Plugin {
    public string Name { get; set; }
    public string Directory { get; set; }
    public string AssemblyName { get; set; }
    public string Exception { get; set; }
    public DateTime? LastReload { get; set; }

    private HttpConfiguration configuration;
    public HttpConfiguration Configuration {
      get { return configuration; }
      set {
        if (configuration != value) {
          configuration = value;
          ReloadControllers();
        }
      }
    }

    private IDictionary<string, HttpControllerDescriptor> controllers;
    public IDictionary<string, HttpControllerDescriptor> Controllers {
      get { return controllers ?? (controllers = new ConcurrentDictionary<string, HttpControllerDescriptor>()); }
    }

    public Plugin(string name, string directory, HttpConfiguration configuration) {
      Name = name;
      Directory = directory;
      Configuration = configuration;
    }

    public HttpControllerDescriptor GetController(string name) {
      HttpControllerDescriptor controller;
      Controllers.TryGetValue(name, out controller);
      return controller;
    }

    public void ReloadControllers() {
      AssemblyName = null;
      Exception = null;
      Controllers.Clear();
      LastReload = DateTime.Now;
      if (configuration == null)
        return;
      try {
        string searchPattern = string.Format("HeuristicLab.Services.WebApp.{0}*.dll", Name);
        var assemblies = System.IO.Directory.GetFiles(Directory, searchPattern);
        if (!assemblies.Any())
          return;
        var assemblyPath = assemblies.First();
        AssemblyName = Path.GetFileName(assemblyPath);
        var assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
        var assemblyTypes = assembly.GetTypes();
        var apiControllers = assemblyTypes.Where(c => typeof(ApiController).IsAssignableFrom(c)).ToList();
        foreach (var apiController in apiControllers) {
          var controllerName = apiController.Name.Remove(apiController.Name.Length - 10).ToLower();
          Controllers.Add(controllerName, new HttpControllerDescriptor(configuration, controllerName, apiController));
        }
      }
      catch (Exception e) {
        Exception = e.ToString();
        Controllers.Clear();
      }
    }
  }
}