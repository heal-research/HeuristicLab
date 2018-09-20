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

using System;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class SARCOS : ResourceRegressionDataDescriptor {
    public SARCOS() : base("SARCOS - Inverse Dynamics.txt") { }
    public override string Name { get { return "SARCOS - Inverse Dynamics"; } }
    public override string Description {
      get {
        return "Publication: Rasmussen and Williams: Gaussian Processes for Machine Learing" + Environment.NewLine +
               "Link: http://www.gaussianprocess.org/gpml/data/";
      }
    }
    protected override string TargetVariable { get { return "y1"; } }
    protected override string[] VariableNames {
      get { return new string[] { "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x10", "x11", "x12", "x13", "x14", "x15", "x16", "x17", "x18", "x19", "x20", "x21", "y1", "y2", "y3", "y4", "y5", "y6", "y7" }; }
    }
    protected override string[] AllowedInputVariables {
      get { return new string[] { "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x10", "x11", "x12", "x13", "x14", "x15", "x16", "x17", "x18", "x19", "x20", "x21" }; }
    }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 44484; } }
    protected override int TestPartitionStart { get { return 44484; } }
    protected override int TestPartitionEnd { get { return 48933; } }
  }
}
