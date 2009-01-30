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
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Attribute of an assembly when it was built.
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
  public class AssemblyBuildDateAttribute : Attribute {
    private DateTime buildDate;
    /// <summary>
    /// Gets or sets the date when the assembly has been built.
    /// </summary>
    public DateTime BuildDate {
      get { return buildDate; }
      set { buildDate = value; }
    }
    /// <summary>
    /// Initializes a new instance of <see cref="AssemblyBuildDateAttribute"/> with the given 
    /// <paramref name="timeStamp"/> as build date.
    /// </summary>
    /// <exception cref="FormatException">Thrown when the time stamp could not be parsed as build date.</exception>
    /// <param name="timeStamp">The build date of the assembly.</param>
    public AssemblyBuildDateAttribute(string timeStamp)
      : base() {

      if(!DateTime.TryParse(timeStamp, out buildDate)) {
        throw new FormatException("Can't parse AssemblyBuildDate " + timeStamp);
      }
    }
  }
}
