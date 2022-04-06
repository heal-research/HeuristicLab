using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HeuristicLab.Build {
  public class UpdateVersion : Task {

    [Required]
    public string ProjectDir { get; set; }

    public override bool Execute() {
      string version = GetVersion();

      string assemblyInfoFramePath = Path.Combine(ProjectDir, "Properties", "AssemblyInfo.cs.frame");
      string assemblyInfoPath = Path.Combine(ProjectDir, "Properties", "AssemblyInfo.cs");

      if (File.Exists(assemblyInfoFramePath)) {
        Log.LogMessage($"Updating {assemblyInfoPath}");
        var assemblyInfoFrameContent = File.ReadAllText(assemblyInfoFramePath);
        File.WriteAllText(assemblyInfoPath, assemblyInfoFrameContent.Replace("$WCREV$", version));
      }


      string pluginFramePath = Path.Combine(ProjectDir, "Plugin.cs.frame");
      string pluginPath = Path.Combine(ProjectDir, "Plugin.cs");

      if (File.Exists(pluginFramePath)) {
        Log.LogMessage($"Updating {pluginPath}");
        var pluginFrameContent = File.ReadAllText(pluginFramePath);
        File.WriteAllText(pluginPath, pluginFrameContent.Replace("$WCREV$", version));
      }

      return true;
    }

    public string GetVersion() {
      return ThisAssembly.Git.Commits;
    }
  }
}
