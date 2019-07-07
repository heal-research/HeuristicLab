#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class MDCVRPData: VRPData {
    /// <summary>
    /// The number of depots
    /// </summary>
    public int Depots { get; set; }
    /// <summary>
    /// The assignment of the vehicles to the depot.
    /// </summary>
    public int[] VehicleDepotAssignment { get; set; }
    /// <summary>
    /// The capacity of the vehicles, which is not the same for all (heterogeneous fleet).
    /// </summary>
    public double[] Capacity { get; set; }
  }
}
