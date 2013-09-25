#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


namespace HeuristicLab.Problems.Instances {
  /// <summary>
  /// Describes instances of the Multi-Depot Capacitated Vehicle Routing Problem (MDCVRP).
  /// </summary>
  public class MDCVRPTWData: MDCVRPData {
    /// <summary>
    /// The ready times of the customers and the depot
    /// </summary>
    public double[] ReadyTimes { get; set; }
    /// <summary>
    /// The service times of the customers and the depot
    /// </summary>
    public double[] ServiceTimes { get; set; }
    /// <summary>
    /// The due times of the customers and the depot
    /// </summary>
    public double[] DueTimes { get; set; }
  }
}
