using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace HeuristicLab.Services.WebApp {
  public class Plugin {
    private HttpConfiguration configuration;
    public string Name { get; set; }
    public string Directory { get; set; }
    public string AssemblyName { get; set; }
    public DateTime? LastReload { get; set; }

    private IDictionary<string, HttpControllerDescriptor> controllers;
    public IDictionary<string, HttpControllerDescriptor> Controllers {
      get { return controllers ?? (controllers = new ConcurrentDictionary<string, HttpControllerDescriptor>()); }
    }

    public void Configure(HttpConfiguration configuration) {
      if (this.configuration != configuration) {
        this.configuration = configuration;
        ReloadControllers();
      }
    }

    public HttpControllerDescriptor GetController(string name) {
      HttpControllerDescriptor controller;
      Controllers.TryGetValue(name, out controller);
      return controller;
    }

    public void ReloadControllers() {
      AssemblyName = null;
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
        var assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
        var assemblyTypes = assembly.GetTypes();
        var apiControllers = assemblyTypes.Where(c => typeof(ApiController).IsAssignableFrom(c)).ToList();
        foreach (var apiController in apiControllers) {
          var controllerName = apiController.Name.Remove(apiController.Name.Length - 10).ToLower();
          Controllers.Add(controllerName, new HttpControllerDescriptor(configuration, controllerName, apiController));
        }
        AssemblyName = Path.GetFileName(assemblyPath);
      }
      catch (ReflectionTypeLoadException ex) {
        StringBuilder sb = new StringBuilder();
        foreach (Exception exSub in ex.LoaderExceptions) {
          sb.AppendLine(exSub.Message);
          FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
          if (exFileNotFound != null) {
            if (!string.IsNullOrEmpty(exFileNotFound.FusionLog)) {
              sb.AppendLine("Fusion Log:");
              sb.AppendLine(exFileNotFound.FusionLog);
            }
          }
          sb.AppendLine();
        }
        AssemblyName = "Error loading assembly: " + sb.ToString();
        Controllers.Clear();
      }
    }
  }
}