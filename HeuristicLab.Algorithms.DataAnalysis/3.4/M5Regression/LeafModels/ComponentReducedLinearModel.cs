#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("4E5B8317-648D-4A5A-A3F7-A1A5BEB9AA69")]
  public class ComponentReducedLinearModel : RegressionModel {
    [Storable]
    private IRegressionModel Model;
    [Storable]
    private PrincipleComponentTransformation Pca;

    [StorableConstructor]
    private ComponentReducedLinearModel(StorableConstructorFlag _) : base(_) { }
    private ComponentReducedLinearModel(ComponentReducedLinearModel original, Cloner cloner) : base(original, cloner) {
      Model = cloner.Clone(original.Model);
      Pca = cloner.Clone(original.Pca);
    }

    public ComponentReducedLinearModel(string targetVariable, IRegressionModel model, PrincipleComponentTransformation pca) : base(targetVariable) {
      Model = model;
      Pca = pca;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ComponentReducedLinearModel(this, cloner);
    }

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return Model.VariablesUsedForPrediction; }
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      var data = ReduceDataset(dataset, rows.ToArray());
      return Model.GetEstimatedValues(Pca.TransformDataset(data), Enumerable.Range(0, data.Rows));
    }

    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, problemData);
    }

    private IDataset ReduceDataset(IDataset data, IReadOnlyList<int> rows) {
      return new Dataset(data.DoubleVariables, data.DoubleVariables.Select(v => data.GetDoubleValues(v, rows).ToList()));
    }
  }
}