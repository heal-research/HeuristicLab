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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("B53C4C71-3715-4BB1-9A95-4E969D8135C6")]
  [Item("Shape-constrained Regression Problem", "A regression problem with shape constraints.")]
  public class ShapeConstrainedRegressionProblem : DataAnalysisProblem<IRegressionProblemData>, IShapeConstrainedRegressionProblem {
    [StorableConstructor]
    protected ShapeConstrainedRegressionProblem(StorableConstructorFlag _) : base(_) { }
    protected ShapeConstrainedRegressionProblem(ShapeConstrainedRegressionProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new ShapeConstrainedRegressionProblem(this, cloner); }

    public ShapeConstrainedRegressionProblemData ShapeConstrainedRegressionProblemData {
      get => (ShapeConstrainedRegressionProblemData)ProblemData;
      set => ProblemData = value;
    }
    public ShapeConstrainedRegressionProblem() : base(new ShapeConstrainedRegressionProblemData()) { }


    public override void Load(IRegressionProblemData data) {
      if (data is ShapeConstrainedRegressionProblemData scProblemData) {
        // use directly
      } else {
        scProblemData = new ShapeConstrainedRegressionProblemData(data.Dataset, data.AllowedInputVariables, data.TargetVariable,
                                                                      data.TrainingPartition, data.TestPartition) {
          Name = data.Name,
          Description = data.Description
        };
      }
      base.Load(scProblemData);
    }
  }
}
