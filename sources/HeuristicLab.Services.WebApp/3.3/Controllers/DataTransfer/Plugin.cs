using System;

namespace HeuristicLab.Services.WebApp.Controllers.DataTransfer {
  public class Plugin {
    public string Name { get; set; }
    public string AssemblyName { get; set; }
    public DateTime? LastReload { get; set; }
    public int Reloads { get; set; }
  }
}