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
  /// Describes an instance of the Generalized Quadratic Assignment Problem (GQAP).
  /// </summary>
  public class GQAPData {
    /// <summary>
    /// The name of the instance.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// A description of the instance.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// |E| = The number of equipments are to be assigned in this instance.
    /// </summary>
    public int Equipments { get; set; }
    /// <summary>
    /// |L| = The number of locations that are available for the equipments.
    /// </summary>
    public int Locations { get; set; }
    /// <summary>
    /// Vector of length |E| that describes the space demand for the equipments.
    /// </summary>
    public double[] Demands { get; set; }
    /// <summary>
    /// Vector of length |L| that describes the space capacity for the locations.
    /// </summary>
    public double[] Capacities { get; set; }
    /// <summary>
    /// |E|x|E| matrix with the weights (flows) between the equipments. These describe the strength of the respective bonding.
    /// </summary>
    public double[,] Weights { get; set; }
    /// <summary>
    /// |L|x|L| matrix with the distances between the locations.
    /// </summary>
    public double[,] Distances { get; set; }
    /// <summary>
    /// |E|x|L| matrix that describes the costs of installing equipment x at location y.
    /// </summary>
    public double[,] InstallationCosts { get; set; }
    /// <summary>
    /// A factor that scales the weights.
    /// </summary>
    public double TransportationCosts { get; set; }

    /// <summary>
    /// Optional! The best-known assignment is a vector of length |E| with numbers ranging from 0 to |L| - 1
    /// </summary>
    public int[] BestKnownAssignment { get; set; }
    /// <summary>
    /// Optional! The quality of the best-known assignment.
    /// </summary>
    public double? BestKnownQuality { get; set; }
  }
}
