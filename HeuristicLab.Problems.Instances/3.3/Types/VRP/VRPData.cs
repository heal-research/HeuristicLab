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


namespace HeuristicLab.Problems.Instances {
  /// <summary>
  /// Describes instances of the Vehicle Routing Problem (VRP).
  /// </summary>
  public class VRPData : TSPData, IVRPData {
    /// <summary>
    /// Optional! The maximum number of vehicles that can be used.
    /// </summary>
    /// <remarks>
    /// If no number is given, but a maximum is required, it can be assumed that
    /// the maximum number of vehicles is equal to the number of customers as
    /// there cannot be more than one vehicle per customer.
    /// </remarks>
    public double? MaximumVehicles { get; set; }
    /// <summary>
    /// The demand vector that specifies how many goods need to be delivered.
    /// The vector has to include the depot, but with a demand of 0.
    /// </summary>
    public double[] Demands { get; set; }

    /// <summary>
    /// Optional! The best-known solution as a list of tours in path-encoding.
    /// </summary>
    public new int[][] BestKnownTour { get; set; }
    /// <summary>
    /// Optional! Specifies the used vehicle for a given tour.
    /// </summary>
    public int[] BestKnownTourVehicleAssignment { get; set; }
  }
}
