using System;
using Microsoft.Win32;

namespace cs_example {
  class Util {
    private const string registryKey = @"SOFTWARE\Scilab";
    private const string scilab_Install = @"LASTINSTALL";
    private const string scilab_PATH = @"SCIPATH";

    public static void AddScilabInstalltionPathToEnvironment() {
      var currentPath = Environment.GetEnvironmentVariable("path");
      if (currentPath == null) return;

      var scilabPath = GetScilabInstallPath();
      if (string.IsNullOrEmpty(scilabPath)) return;

      Environment.SetEnvironmentVariable("path", scilabPath + ";" + currentPath);
    }

    private static string GetScilabInstallPath() {
      using (var registryLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default)) {
        using (var registryScilab = registryLocalMachine.OpenSubKey(registryKey)) {
          if (registryScilab == null) return string.Empty;
          var scilabVersion = registryScilab.GetValue(scilab_Install);
          if (scilabVersion == null) return string.Empty;
          using (var registryScilabVersion = registryScilab.OpenSubKey(scilabVersion.ToString())) {
            if (registryScilabVersion == null) return string.Empty;
            var scilabPath = registryScilabVersion.GetValue(scilab_PATH);
            if (scilabPath == null) return string.Empty;
            string path = scilabPath.ToString();
            path += @"\bin";
            return path;
          }
        }
      }
    }
  }
}
