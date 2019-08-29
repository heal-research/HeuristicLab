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

using System;
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("347CA25D-FB37-4C4F-9B61-9D79288B2B28")]
  [Item("LinearLeaf", "A leaf type that uses linear models as leaf models. This is the standard for decision tree regression")]
  public class LinearLeaf : LeafBase {
    #region Constructors & Cloning
    [StorableConstructor]
    protected LinearLeaf(StorableConstructorFlag _) : base(_) { }
    protected LinearLeaf(LinearLeaf original, Cloner cloner) : base(original, cloner) { }
    public LinearLeaf() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearLeaf(this, cloner);
    }
    #endregion

    #region IModelType
    public override bool ProvidesConfidence {
      get { return true; }
    }
    public override IRegressionModel Build(IRegressionProblemData pd, IRandom random, CancellationToken cancellationToken, out int numberOfParameters) {
      if (pd.Dataset.Rows < MinLeafSize(pd)) throw new ArgumentException("The number of training instances is too small to create a linear model");
      double rmse, cvRmse;
      numberOfParameters = pd.AllowedInputVariables.Count() + 1;
      var res = LinearRegression.CreateSolution(pd, out rmse, out cvRmse);
      return res.Model;
    }

    public override int MinLeafSize(IRegressionProblemData pd) {
      return pd.AllowedInputVariables.Count() == 1 ? 2 : pd.AllowedInputVariables.Count() + 2;
    }
    #endregion
  }
}