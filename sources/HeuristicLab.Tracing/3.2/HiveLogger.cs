using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Tracing.Properties;
using System.IO;
using log4net.Config;

namespace HeuristicLab.Tracing {
  public class HiveLogger: Logger {
    private static void Configure() {
      if (IsConfigured) return;
      IsConfigured = true;
      if (string.IsNullOrEmpty(Settings.Default.TracingLog4netConfigFile)) {
        Settings.Default.TracingLog4netConfigFile =
          Path.Combine(
            PluginInfrastructure.Properties.Settings.Default.PluginDir,
            "HeuristicLab.Hive.log4net.xml");
      }
      XmlConfigurator.ConfigureAndWatch(
        new FileInfo(Settings.Default.TracingLog4netConfigFile));
      Info("logging initialized " + DateTime.Now);
    }
  }
}
