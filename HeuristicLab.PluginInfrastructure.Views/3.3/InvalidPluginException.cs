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
using System.Runtime.Serialization;

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Exception class for invalid plugins.
  /// </summary>
  [Serializable]
  public sealed class InvalidPluginException : Exception {
    /// <summary>
    /// Initializes a new InvalidPluginException
    /// </summary>
    public InvalidPluginException() : base() { }
    /// <summary>
    /// Initializes a new InvalidPluginException with an error message.
    /// </summary>
    /// <param name="message">The exception message</param>
    public InvalidPluginException(string message) : base(message) { }
    /// <summary>
    /// Initializes a new InvalidPluginException with an error message and an inner exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The original exception.</param>
    public InvalidPluginException(string message, Exception innerException) : base(message, innerException) { }
    /// <summary>
    /// Constructor for serialization.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="contex">The serialization context.</param>
    private InvalidPluginException(SerializationInfo info, StreamingContext contex) : base(info, contex) { }
  }
}
