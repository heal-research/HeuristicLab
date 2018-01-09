#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class VRPData : IVRPData {
    /// <summary>
    /// The name of the instance
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Optional! The description of the instance
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The number of customers and the depot
    /// </summary>
    public int Dimension { get; set; }
    /// <summary>
    /// The distance measure that is used to calculate the distance between
    ///the coordinates if no <see cref="Distances"/> is given.
    /// </summary>
    public DistanceMeasure DistanceMeasure { get; set; }
    /// <summary>
    /// Optional! The maximum number of vehicles that can be used.
    /// </summary>
    /// <remarks>
    /// If no number is given, but a maximum is required, it can be assumed that
    /// the maximum number of vehicles is equal to the number of customers as
    /// there cannot be more than one vehicle per customer.
    /// </remarks>
    public double? MaximumVehicles { get; set; }
    /// <remarks>
    /// Either Distances or the <see cref="Coordinates"/> need to be specified along
    /// with a distance measure.
    /// </remarks>
    public double[,] Distances { get; set; }
    /// <summary>
    /// Optional! A a matrix of dimension [N, 2] where each row is either the customer
    /// or the depot and the columns represent x and y coordinates respectively.
    /// </summary>
    /// <remarks>
    /// Either <see cref="Distances"/> or the Coordinates need to be specified along
    /// with a distance measure.
    /// </remarks>
    public double[,] Coordinates { get; set; }
    /// <summary>
    /// The demand vector that specifies how many goods need to be delivered.
    /// The vector has to include the depot, but with a demand of 0.
    /// </summary>
    public double[] Demands { get; set; }

    /// <summary>
    /// Optional! The best-known solution as a list of tours in path-encoding.
    /// </summary>
    public int[][] BestKnownTour { get; set; }
    /// <summary>
    /// Optional! Specifies the used vehicle for a given tour.
    /// </summary>
    public int[] BestKnownTourVehicleAssignment { get; set; }
    /// <summary>
    /// Optional! The quality of the best-known solution.
    /// </summary>
    public double? BestKnownQuality { get; set; }

    /// <summary>
    /// If only the coordinates are given, can calculate the distance matrix.
    /// </summary>
    /// <returns>A full distance matrix between all cities.</returns>
    public double[,] GetDistanceMatrix() {
      return DistanceHelper.GetDistanceMatrix(DistanceMeasure, Coordinates, Distances, Dimension);
    }
  }
}
