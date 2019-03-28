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

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {

  /// <summary>
  /// Describes instances of Multi Objective Test Functions (MOTF).
  /// </summary>
  public class MOTFData {
    /// <summary>
    /// The name of the instance
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Optional! The description of the instance
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// The operator used for evaluations
    /// </summary>
    public IMultiObjectiveTestFunction TestFunction { get; private set; }

    public MOTFData(string name, string description, IMultiObjectiveTestFunction testFunction) {
      Name = name;
      Description = description;
      TestFunction = testFunction;
    }

    internal MOTFData(MOTFDataDescriptor descriptor) {
      Name = descriptor.Name;
      Description = descriptor.Description;
      TestFunction = descriptor.TestFunction;
    }
  }

}