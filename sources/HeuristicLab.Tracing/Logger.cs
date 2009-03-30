using System;
using HeuristicLab.Tracing.Properties;
using log4net;
using System.Diagnostics;
using log4net.Config;
using System.IO;

namespace HeuristicLab.Tracing {

  public class Logger {

    private static bool IsConfigured = false;

    private static void Configure() {
      if ( IsConfigured ) return;
      IsConfigured = true;
      if (string.IsNullOrEmpty(Settings.Default.log4netConfigFile)) {
        Settings.Default.log4netConfigFile = 
          Path.Combine(
            PluginInfrastructure.Properties.Settings.Default.PluginDir,
            "HeuristicLab.log4net.xml");
      }
      XmlConfigurator.ConfigureAndWatch(
        new FileInfo(Settings.Default.log4netConfigFile));
      Info("Logging started and configured");
    }

    public static ILog GetDefaultLogger(int nParents) {
      Configure();
      StackFrame frame = new StackFrame(nParents + 1);
      return LogManager.GetLogger(frame.GetMethod().DeclaringType);      
    }

    public static void Debug(object message) {
      GetDefaultLogger(1).Debug(message);
    }

    public static void Info(object message) {
      GetDefaultLogger(1).Info(message);
    }

    public static void Warn(object message) {
      GetDefaultLogger(1).Warn(message);
    }

    public static void Error(object message) {      
      GetDefaultLogger(1).Error(message);
    }

    public static void Fatal(object message) {
      GetDefaultLogger(1).Fatal(message);
    }

    public static void Debug(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Debug(message);      
    }

    public static void Info(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Info(message);      
    }

    public static void Warn(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Warn(message);      
    }

    public static void Error(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Error(message);
    }

    public static void Fatal(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Fatal(message);
    }

  }
}
