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
  /// Describes instances of the Job Shop Scheduling Problem (JSSP).
  /// </summary>
  public class JSSPData {
    /// <summary>
    /// The name of the instance
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Optional! The description of the instance
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The number of jobs.
    /// </summary>
    public int Jobs { get; set; }
    /// <summary>
    /// The number of resources.
    /// </summary>
    public int Resources { get; set; }
    /// <summary>
    /// The first dimension lists the jobs, the second dimension the operations and the value denotes the processing time.
    /// </summary>
    public double[,] ProcessingTimes { get; set; }
    /// <summary>
    /// The first dimension lists the jobs, the second dimension the operations and the value denotes the resource that is demanded.
    /// </summary>
    public int[,] Demands { get; set; }
    /// <summary>
    /// Optional! Specifies the due dates for each job.
    /// </summary>
    public double[] DueDates { get; set; }

    /// <summary>
    /// Optional! The best-known schedule.
    /// The first dimension lists the resources, the second dimension is the order and the value denotes the job
    /// </summary>
    public int[,] BestKnownSchedule { get; set; }
    /// <summary>
    /// Optional! The quality of the best-known schedule.
    /// </summary>
    public double? BestKnownQuality { get; set; }
  }
}
