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

using HeuristicLab.Common;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("B05C382A-A637-4CD6-8BCC-E1E92B60F330")]
  public sealed class GradientBoostedTreesSolution : RegressionSolution {
    public new IGradientBoostedTreesModel Model {
      get { return (IGradientBoostedTreesModel)base.Model; }
    }


    [StorableConstructor]
    private GradientBoostedTreesSolution(StorableConstructorFlag _) : base(_) {
    }
    private GradientBoostedTreesSolution(GradientBoostedTreesSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public GradientBoostedTreesSolution(IRegressionModel model, IRegressionProblemData problemData)
      : base(model, problemData) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GradientBoostedTreesSolution(this, cloner);
    }
  }
}
