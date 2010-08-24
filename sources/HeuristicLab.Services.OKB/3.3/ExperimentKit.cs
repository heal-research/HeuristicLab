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

using System.Runtime.Serialization;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB {
  /// <summary>
  /// Contains an <see cref="Algorithm"/> and a <see cref="Problem"/> populated with all
  /// required data to execute experiments.
  /// </summary>
  [DataContract]
  public class ExperimentKit {
    /// <summary>
    /// Gets an <see cref="Algorithm"/> populated with all required data to execute
    /// experiments.
    /// </summary>
    /// <value>An <see cref="Algorithm"/>.</value>
    [DataMember]
    public Algorithm Algorithm { get; set; }

    /// <summary>
    /// Gets a <see cref="Problem"/> populated with all required data to execute
    /// experiments.
    /// </summary>
    /// <value>A <see cref="Problem"/>.</value>
    [DataMember]
    public Problem Problem { get; set; }
  }

}
