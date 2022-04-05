using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HeuristicLab.Build {
  public class UpdateVersion : Task {

    [Required]
    public string ProjectDir { get; set; }

    public override bool Execute() {

      string commitCount = ThisAssembly.Git.Commits;
      Log.LogMessage(MessageImportance.High, $"Updating {Path.Combine(ProjectDir, "Properties", "AssemblyInfo.cs")}");
      var assemblyInfoFrameContent = File.ReadAllText(Path.Combine(ProjectDir, "Properties", "AssemblyInfo.cs.frame"));
      File.WriteAllText(Path.Combine(ProjectDir, "Properties", "AssemblyInfo.cs"), assemblyInfoFrameContent.Replace("$WCREV$", commitCount));

      Log.LogMessage(MessageImportance.High, $"Updating {Path.Combine(ProjectDir, "Plugin.cs")}");
      var pluginFrameContent = File.ReadAllText(Path.Combine(ProjectDir, "Plugin.cs.frame"));
      File.WriteAllText(Path.Combine(ProjectDir, "Plugin.cs"), pluginFrameContent.Replace("$WCREV$", commitCount));

      return true;
    }
  }
}
