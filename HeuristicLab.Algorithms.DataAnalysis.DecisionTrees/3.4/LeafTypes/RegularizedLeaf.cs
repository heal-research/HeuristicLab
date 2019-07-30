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
using HeuristicLab.Algorithms.DataAnalysis.Glmnet;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("0AED959D-78C3-4927-BDCF-473D0AEE32AA")]
  [Item("RegularizedLeaf", "A leaf type that uses regularized linear models as leaf models.")]
  public sealed class RegularizedLeaf : LeafBase {
    #region Constructors & Cloning
    [StorableConstructor]
    private RegularizedLeaf(StorableConstructorFlag _) : base(_) { }
    private RegularizedLeaf(RegularizedLeaf original, Cloner cloner) : base(original, cloner) { }
    public RegularizedLeaf() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegularizedLeaf(this, cloner);
    }
    #endregion

    #region IModelType
    public override bool ProvidesConfidence {
      get { return false; }
    }

    public override IRegressionModel Build(IRegressionProblemData pd, IRandom random, CancellationToken cancellationToken, out int numberOfParameters) {
      if (pd.Dataset.Rows < MinLeafSize(pd)) throw new ArgumentException("The number of training instances is too small to create a linear model");
      numberOfParameters = pd.AllowedInputVariables.Count() + 1;

      double x1, x2;
      var coeffs = ElasticNetLinearRegression.CalculateModelCoefficients(pd, 1, 0.2, out x1, out x2);
      numberOfParameters = coeffs.Length;
      return ElasticNetLinearRegression.CreateSymbolicSolution(coeffs, pd).Model;
    }
    public override int MinLeafSize(IRegressionProblemData pd) {
      return pd.AllowedInputVariables.Count() + 2;
    }
    #endregion
  }
}