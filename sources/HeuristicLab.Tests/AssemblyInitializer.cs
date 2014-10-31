using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public static class AssemblyInitializer {
    private static string SamplesDirectory = SamplesUtils.SamplesDirectory;
    private static string ScriptsDirectory = ScriptingUtils.ScriptsDirectory;

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext) {
      // load all assemblies
      PluginLoader.Assemblies.Any();

      // create output directories for samples and scripts
      if (!Directory.Exists(SamplesDirectory)) Directory.CreateDirectory(SamplesDirectory);
      if (!Directory.Exists(ScriptsDirectory)) Directory.CreateDirectory(ScriptsDirectory);

      // initialize the MainForm
      ScriptingUtils.InitializeMainForm();
    }
  }
}
