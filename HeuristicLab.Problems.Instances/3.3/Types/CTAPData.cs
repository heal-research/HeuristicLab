#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Describes instances of the Capacitated Task Assignment Problem (CTAP).
  /// </summary>
  public class CTAPData {
    /// <summary>
    /// The name of the instance
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Optional! The description of the instance
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The number M of processors
    /// </summary>
    public int Processors { get; set; }
    /// <summary>
    /// The number N of tasks
    /// </summary>
    public int Tasks { get; set; }
    /// <summary>
    /// An MxN Matrix with M = |Processors| and N = |Tasks|
    /// </summary>
    public double[,] ExecutionCosts { get; set; }
    /// <summary>
    /// An NxN Matrix with N = |Tasks|
    /// </summary>
    public double[,] CommunicationCosts { get; set; }
    /// <summary>
    /// An array of length N with N = |Tasks|
    /// </summary>
    public double[] MemoryRequirements { get; set; }
    /// <summary>
    /// An array of length M with M = |Processors|
    /// </summary>
    public double[] MemoryCapacities { get; set; }

    /// <summary>
    /// Optional! An array of length N with N = |Tasks|
    /// </summary>
    public int[] BestKnownAssignment { get; set; }
    /// <summary>
    /// Optional! The quality value of the <see cref="BestKnownAssignment"/>
    /// </summary>
    public double? BestKnownQuality { get; set; }
  }
}
