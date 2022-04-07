using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
      string version = DateTime.Now.ToString("yyyyMMddHHmmss");

      if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        using (Process process = new Process()) {
          process.StartInfo.FileName = "cmd.exe";
          process.StartInfo.WorkingDirectory = new DirectoryInfo(ProjectDir).FullName;
          process.StartInfo.Arguments = $"/c git rev-list --count HEAD";
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.RedirectStandardOutput = true;
          process.Start();

          version = process.StandardOutput.ReadToEnd().Replace("\n", "");
        }

      } else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {

      } else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {

      } else {

      }

      return version;
    }
  }
}
