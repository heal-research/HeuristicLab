
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab_33.Tests {
  internal static class PluginLoader {
    public const string ExecutableExtension = ".exe";
    public const string AssemblyExtension = ".dll";
    public const string TestAccessorAssemblyExtension = "_Accessor.dll";
    public const string TestAssemblyExtension = ".Tests.dll";
    public static List<Assembly> Assemblies;

    static PluginLoader() {
      foreach (string path in Directory.EnumerateFiles(Environment.CurrentDirectory).Where(s => IsRelevantAssemblyPath(s))) {
        try {
          Assembly.LoadFrom(path);
        }
        catch (BadImageFormatException) { }
      }
      Assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
    }

    private static bool IsRelevantAssemblyPath(string path) {
      bool valid = true;
      valid = valid && (path.EndsWith(ExecutableExtension, StringComparison.OrdinalIgnoreCase) || path.EndsWith(AssemblyExtension, StringComparison.OrdinalIgnoreCase));
      valid = valid && !path.EndsWith(TestAccessorAssemblyExtension, StringComparison.OrdinalIgnoreCase) && !path.EndsWith(TestAssemblyExtension, StringComparison.OrdinalIgnoreCase);
      return valid;
    }

    public static bool IsPluginAssembly(Assembly assembly) {
      return assembly.GetExportedTypes().Any(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
    }
  }
}
