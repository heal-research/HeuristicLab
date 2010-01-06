using System;
using HeuristicLab.Tracing.Properties;
using log4net;
using System.Diagnostics;
using log4net.Config;
using System.IO;

namespace HeuristicLab.Tracing {
  public class HiveLogger {
    protected static bool IsConfigured = false;
    protected static void Configure() {
      if (IsConfigured) return;
      IsConfigured = true;
      if (string.IsNullOrEmpty(Settings.Default.TracingLog4netConfigFile)) {
        Settings.Default.TracingLog4netConfigFile =
            "HeuristicLab.Hive.log4net.xml";
      }
      XmlConfigurator.ConfigureAndWatch(
        new FileInfo(Settings.Default.TracingLog4netConfigFile));
      Info("Hive Logging initialized " + DateTime.Now);
    }

    public static ILog GetDefaultLogger(int nParents) {
      Configure();
      StackFrame frame = new StackFrame(nParents + 1);
      return LogManager.GetLogger(frame.GetMethod().DeclaringType);
    }

    public static ILog GetDefaultLogger() {
      Configure();
      StackFrame frame = new StackFrame(1);
      return LogManager.GetLogger(frame.GetMethod().DeclaringType);
    }

    public static void Debug(object message) {
      GetDefaultLogger(1).Debug(DateTime.Now + ":" + DateTime.Now.Millisecond + " - " + message);
    }

    public static void Info(object message) {
      GetDefaultLogger(1).Info(DateTime.Now + ":" + DateTime.Now.Millisecond + " - " + message);
    }

    public static void Warn(object message) {
      GetDefaultLogger(1).Warn(DateTime.Now + ":" + DateTime.Now.Millisecond + " - " + message);
    }

    public static void Error(object message) {
      GetDefaultLogger(1).Error(DateTime.Now + ":" + DateTime.Now.Millisecond + " - " + message);
    }

    public static void Fatal(object message) {
      GetDefaultLogger(1).Fatal(DateTime.Now + ":" + DateTime.Now.Millisecond + " - " + message);
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

    public static void Debug(object message, Exception exception) {
      GetDefaultLogger(1).Debug(message, exception);
    }

    public static void Info(object message, Exception exception) {
      GetDefaultLogger(1).Info(message, exception);
    }

    public static void Warn(object message, Exception exception) {
      GetDefaultLogger(1).Warn(message, exception);
    }

    public static void Error(object message, Exception exception) {
      GetDefaultLogger(1).Error(message, exception);
    }

    public static void Fatal(object message, Exception exception) {
      GetDefaultLogger(1).Fatal(message, exception);
    }

    public static void Debug(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Debug(message, exception);
    }

    public static void Info(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Info(message, exception);
    }

    public static void Warn(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Warn(message, exception);
    }

    public static void Error(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Error(message, exception);
    }

    public static void Fatal(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Fatal(message, exception);
    }

  }
}
