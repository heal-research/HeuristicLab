#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

  /// <summary>
  /// HeuristicLab Tracing entry point. Default logger. Reads configured tracing
  /// file and provides automatic logging with reflection of the calling type.
  /// </summary>
  public class Logger {

    /// <summary>
    /// true if Configure has been called already.
    /// </summary>
    protected static bool IsConfigured = false;

    /// <summary>
    /// Configures this instance: Reads the log file specified in the settings.
    /// </summary>
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

    /// <summary>
    /// Gets the default logger for the calling class n levels up in the
    /// call hierarchy.
    /// </summary>
    /// <param name="nParents">The number of parent calls.</param>
    /// <returns>An <see cref="ILog"/> instance.</returns>
    public static ILog GetDefaultLogger(int nParents) {
      Configure();
      StackFrame frame = new StackFrame(nParents + 1);
      return LogManager.GetLogger(frame.GetMethod().DeclaringType);
    }

    /// <summary>
    /// Gets the default logger: The logger for the class of the
    /// calling method.
    /// </summary>
    /// <returns>An <see cref="ILog"/> instance.</returns>
    public static ILog GetDefaultLogger() {
      Configure();
      StackFrame frame = new StackFrame(1);
      return LogManager.GetLogger(frame.GetMethod().DeclaringType);
    }

    /// <summary>
    /// Issues a debug message to the default logger.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Debug(object message) {
      GetDefaultLogger(1).Debug(message);
    }

    /// <summary>
    /// Issues an informational message to the default logger.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Info(object message) {
      GetDefaultLogger(1).Info(message);
    }

    /// <summary>
    /// Issues a warning message to the default logger.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Warn(object message) {
      GetDefaultLogger(1).Warn(message);
    }

    /// <summary>
    /// Issues an error message to the default logger.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Error(object message) {
      GetDefaultLogger(1).Error(message);
    }

    /// <summary>
    /// Issues a fatal error message to the default logger.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Fatal(object message) {
      GetDefaultLogger(1).Fatal(message);
    }

    /// <summary>
    /// Issues a debug message to the logger of the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    public static void Debug(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Debug(message);
    }

    /// <summary>
    /// Issues an iformational message to the logger of the specified
    /// type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    public static void Info(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Info(message);
    }

    /// <summary>
    /// Issues a warning message to the logger of
    /// the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    public static void Warn(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Warn(message);
    }

    /// <summary>
    /// Issues an error message to the logger of the specified
    /// type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    public static void Error(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Error(message);
    }

    /// <summary>
    /// Issues a fatal error message to the logger of
    /// the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    public static void Fatal(Type type, object message) {
      Configure();
      LogManager.GetLogger(type).Fatal(message);
    }

    /// <summary>
    /// Issues a debug message to the default
    /// logger including an exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Debug(object message, Exception exception) {
      GetDefaultLogger(1).Debug(message, exception);
    }

    /// <summary>
    /// Issues an informational message to the default
    /// logger including an exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>

    public static void Info(object message, Exception exception) {
      GetDefaultLogger(1).Info(message, exception);
    }

    /// <summary>
    /// Issues a warning message to the default
    /// logger including an exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Warn(object message, Exception exception) {
      GetDefaultLogger(1).Warn(message, exception);
    }

    /// <summary>
    /// Issues an error message to the default
    /// logger including an exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Error(object message, Exception exception) {
      GetDefaultLogger(1).Error(message, exception);
    }

    /// <summary>
    /// Issues a fatal error message to the default
    /// logger including an exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Fatal(object message, Exception exception) {
      GetDefaultLogger(1).Fatal(message, exception);
    }

    /// <summary>
    /// Issues a debug message to the logger of the specified
    /// type including an exception.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Debug(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Debug(message, exception);
    }

    /// <summary>
    /// Issues an informational message to the logger of the specified
    /// type including an exception.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Info(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Info(message, exception);
    }

    /// <summary>
    /// Issues a warning message to the logger of the specified
    /// type including an exception.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Warn(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Warn(message, exception);
    }

    /// <summary>
    /// Issues an error message to the logger of the specified
    /// type including an exception.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Error(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Error(message, exception);
    }

    /// <summary>
    /// Issues a fatal error message to the logger of the specified
    /// type including an exception.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Fatal(Type type, object message, Exception exception) {
      Configure();
      LogManager.GetLogger(type).Fatal(message, exception);
    }
  }
}
