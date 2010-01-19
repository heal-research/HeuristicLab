#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using HeuristicLab.Tracing.Properties;
using log4net;
using System.Diagnostics;
using log4net.Config;
using System.IO;

namespace HeuristicLab.Tracing {

  public class Logger {

    protected static bool IsConfigured = false;

    protected static void Configure() {
      if (IsConfigured) return;
      IsConfigured = true;
      if (string.IsNullOrEmpty(Settings.Default.TracingLog4netConfigFile)) {
        Settings.Default.TracingLog4netConfigFile =
            "HeuristicLab.log4net.xml";
      }
      XmlConfigurator.ConfigureAndWatch(
        new FileInfo(Settings.Default.TracingLog4netConfigFile));
      Info("logging initialized " + DateTime.Now);
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
