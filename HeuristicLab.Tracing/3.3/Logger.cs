#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
namespace HeuristicLab.Tracing {

  /// <summary>
  /// HeuristicLab.Tracing using System.Diagnostics.Trace.
  /// Note that tracing is only activated if the DEBUG or TRACE symbol is set. 
  /// Configuration (type of listener, file name, ...) is done in app.config in the <system.diagnostics> section. 
  /// </summary>
  public class Logger {

    /// <summary>
    /// Issues a debug message.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Debug(object message) {
      StackFrame frame = new StackFrame(1);
      Trace.TraceInformation("{0} - {1}", frame.GetMethod().DeclaringType, message);
    }

    /// <summary>
    /// Issues an informational message. 
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Info(object message) {
      StackFrame frame = new StackFrame(1);
      Trace.TraceInformation("{0} - {1}", frame.GetMethod().DeclaringType, message);
    }

    /// <summary>
    /// Issues a warning message.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Warn(object message) {
      StackFrame frame = new StackFrame(1);
      Trace.TraceWarning("{0} - {1}", frame.GetMethod().DeclaringType, message);
    }

    /// <summary>
    /// Issues an error message.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Error(object message) {
      StackFrame frame = new StackFrame(1);
      Trace.TraceError("{0} - {1}", frame.GetMethod().DeclaringType, message);
    }

    /// <summary>
    /// Issues a debug message of the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    public static void Debug(Type type, object message) {
      Trace.TraceInformation("{0}: {1}", type, message);
    }

    /// <summary>
    /// Issues an informational message of the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    public static void Info(Type type, object message) {
      Trace.TraceInformation("{0}: {1}", type, message);
    }

    /// <summary>
    /// Issues a warning message of the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    public static void Warn(Type type, object message) {
      Trace.TraceWarning("{0}: {1}", type, message);
    }

    /// <summary>
    /// Issues an error message of the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    public static void Error(Type type, object message) {
      Trace.TraceError("{0}: {1}", type, message);
    }

    /// <summary>
    /// Issues a debug message including an exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Debug(object message, Exception exception) {
      Trace.TraceInformation("{0}: {1}:", message, exception);
    }

    /// <summary>
    /// Issues an informational message including an exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>        
    public static void Info(object message, Exception exception) {
      Trace.TraceInformation("{0}: {1}:", message, exception);
    }

    /// <summary>
    /// Issues a warning message including an exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Warn(object message, Exception exception) {
      Trace.TraceWarning("{0}: {1}:", message, exception);
    }

    /// <summary>
    /// Issues an error message including an exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Error(object message, Exception exception) {
      Trace.TraceError("{0}: {1}:", message, exception);
    }

    /// <summary>
    /// Issues a debug message of the specified type including an exception.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Debug(Type type, object message, Exception exception) {
      Trace.TraceInformation("{0}: {1}: {2}", type, message, exception);
    }

    /// <summary>
    /// Issues an informational message of the specified type including an exception.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Info(Type type, object message, Exception exception) {
      Trace.TraceInformation("{0}: {1}: {2}", type, message, exception);
    }

    /// <summary>
    /// Issues a warning message of the specified type including an exception.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Warn(Type type, object message, Exception exception) {
      Trace.TraceWarning("{0}: {1}: {2}", type, message, exception);
    }

    /// <summary>
    /// Issues an error message of the specified type including an exception.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Error(Type type, object message, Exception exception) {
      Trace.TraceError("{0}: {1}: {2}", type, message, exception);
    }
  }
}
