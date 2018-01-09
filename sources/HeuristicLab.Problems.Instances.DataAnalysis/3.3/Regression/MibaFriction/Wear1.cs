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

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Wear1 : ResourceRegressionDataDescriptor {
    public override string Name { get { return "Wear1"; } }
    public override string Description {
      get {
        return "Wear, Publication: E. Lughofer, G. K. Kronberger, M. Kommenda, S. Saminger-Platz, " +
               "A. Promberger, F. Nickel, S. M. Winkler, M. Affenzeller - Robust Fuzzy Modeling and Symbolic Regression " +
               "for Establishing Accurate and Interpretable Prediction Models in Supervising Tribological Systems - " +
               "Proceedings of the 8th International Joint Conference on Computational Intelligence, Porto, Portugal, 2016, pp. 51-63";
      }
    }
    protected override string TargetVariable { get { return "Wear1"; } }
    protected override string[] VariableNames {
      get { return new string[] { "Partition", "Source1", "Source2", "x1", "Material_Cat", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x10", "x11", "x12", "x13", "x14", "x15", "x16", "Material", "Grooving", "Oil", "x17", "x18", "x19", "x20", "x21", "x22", "Wear1"
 };
      }
    }

    protected override string[] AllowedInputVariables {
      get { return new string[] { "Material_Cat",
        "Source1", "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x10", "x11", "x12", "x13", "x14", "x15", "x16",
        "Material", "Grooving", "Oil",
        "x17", "x18", "x19", "x20", "x21", "x22" }; }
    }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 641; } }
    protected override int TestPartitionStart { get { return 641; } }
    protected override int TestPartitionEnd { get { return 904; } }
  }
}
