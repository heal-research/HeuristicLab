#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances {
  /// <summary>
  /// Describes instances of the Traveling Salesman Problem (TSP).
  /// </summary>
  public class TSPData {
    /// <summary>
    /// The name of the instance
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Optional! The description of the instance
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The number of cities.
    /// </summary>
    public int Dimension { get; set; }
    /// <summary>
    /// Specifies the distance measure that is to be used.
    /// </summary>
    public DistanceMeasure DistanceMeasure { get; set; }
    /// <summary>
    /// Optional! The distances are given in form of a distance matrix.
    /// </summary>
    /// <remarks>
    /// Either this property or <see cref="Coordinates"/> needs to be specified.
    /// </remarks>
    public double[,] Distances { get; set; }
    /// <summary>
    /// Optional! A a matrix of dimension [N, 2] matrix where each row is one of the cities
    /// and the colmns represent x and y coordinates respectively.
    /// </summary>
    /// <remarks>
    /// Either this property or <see cref="Distances"/> needs to be specified.
    /// 
    /// If no distance matrix is given, the distances have to be calculated by the
    /// specified distance measure. If a distance matrix is given in addtion to the
    /// coordinates, the distance matrix takes precedence and the coordinates are
    /// for display only.
    /// </remarks>
    public double[,] Coordinates { get; set; }

    /// <summary>
    /// Optional! The best-known tour in path-encoding.
    /// </summary>
    public int[] BestKnownTour { get; set; }
    /// <summary>
    /// Optional! The quality of the best-known tour.
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
