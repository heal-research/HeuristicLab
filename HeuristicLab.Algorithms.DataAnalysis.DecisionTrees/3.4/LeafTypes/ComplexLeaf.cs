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
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("F34A0ED9-2CF6-4DEE-850D-08790663B66D")]
  [Item("ComplexLeaf", "A leaf type that uses an arbitrary RegressionAlgorithm to create leaf models")]
  public sealed class ComplexLeaf : LeafBase {
    public const string RegressionParameterName = "Regression";
    public IValueParameter<IDataAnalysisAlgorithm<IRegressionProblem>> RegressionParameter {
      get { return (IValueParameter<IDataAnalysisAlgorithm<IRegressionProblem>>)Parameters[RegressionParameterName]; }
    }
    public IDataAnalysisAlgorithm<IRegressionProblem> Regression {
      get { return RegressionParameter.Value; }
      set { RegressionParameter.Value = value; }
    }

    #region Constructors & Cloning
    [StorableConstructor]
    private ComplexLeaf(StorableConstructorFlag _) : base(_) { }
    private ComplexLeaf(ComplexLeaf original, Cloner cloner) : base(original, cloner) { }
    public ComplexLeaf() {
      var regression = new KernelRidgeRegression();
      Parameters.Add(new ValueParameter<IDataAnalysisAlgorithm<IRegressionProblem>>(RegressionParameterName, "The algorithm creating RegressionModels", regression));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ComplexLeaf(this, cloner);
    }
    #endregion

    #region IModelType
    public override bool ProvidesConfidence {
      get { return false; }
    }
    public override IRegressionModel Build(IRegressionProblemData pd, IRandom random, CancellationToken cancellationToken, out int noParameters) {
      if (pd.Dataset.Rows < MinLeafSize(pd)) throw new ArgumentException("The number of training instances is too small to create a linear model");
      noParameters = pd.Dataset.Rows + 1;
      Regression.Problem = new RegressionProblem { ProblemData = pd };
      var res = RegressionTreeUtilities.RunSubAlgorithm(Regression, random.Next(), cancellationToken);
      var t = res.Select(x => x.Value).OfType<IRegressionSolution>().FirstOrDefault();
      if (t == null) throw new ArgumentException("No RegressionSolution was provided by the algorithm");
      return t.Model;
    }
    public override int MinLeafSize(IRegressionProblemData pd) {
      return 3;
    }
    #endregion
  }
}