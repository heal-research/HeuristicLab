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

using System;
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("852B9F7D-9C2B-4574-BB71-EE70106EA809")]
  [Item("GaussianProcessLeaf", "A leaf type that uses Gaussian process models as leaf models.")]
  public class GaussianProcessLeaf : LeafBase {
    #region ParameterNames
    public const string TriesParameterName = "Tries";
    public const string RegressionParameterName = "Regression";
    #endregion

    #region ParameterProperties
    public IFixedValueParameter<IntValue> TriesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[TriesParameterName]; }
    }
    public IValueParameter<GaussianProcessRegression> RegressionParameter {
      get { return (IValueParameter<GaussianProcessRegression>)Parameters[RegressionParameterName]; }
    }
    #endregion

    #region Properties
    public int Tries {
      get { return TriesParameter.Value.Value; }
      set { TriesParameter.Value.Value = value; }
    }
    public GaussianProcessRegression Regression {
      get { return RegressionParameter.Value; }
      set { RegressionParameter.Value = value; }
    }
    #endregion

    #region Constructors & Cloning
    [StorableConstructor]
    protected GaussianProcessLeaf(StorableConstructorFlag _) : base(_) { }
    protected GaussianProcessLeaf(GaussianProcessLeaf original, Cloner cloner) : base(original, cloner) { }
    public GaussianProcessLeaf() {
      var gp = new GaussianProcessRegression();
      gp.CovarianceFunctionParameter.Value = new CovarianceRationalQuadraticIso();
      gp.MeanFunctionParameter.Value = new MeanLinear();

      Parameters.Add(new FixedValueParameter<IntValue>(TriesParameterName, "Number of restarts (default = 10)", new IntValue(10)));
      Parameters.Add(new ValueParameter<GaussianProcessRegression>(RegressionParameterName, "The algorithm creating Gaussian process models", gp));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessLeaf(this, cloner);
    }
    #endregion

    #region IModelType
    public override bool ProvidesConfidence {
      get { return true; }
    }

    public override IRegressionModel Build(IRegressionProblemData pd, IRandom random, CancellationToken cancellationToken, out int numberOfParameters) {
      if (pd.Dataset.Rows < MinLeafSize(pd)) throw new ArgumentException("The number of training instances is too small to create a Gaussian process model");
      Regression.Problem = new RegressionProblem { ProblemData = pd };
      var cvscore = double.MaxValue;
      GaussianProcessRegressionSolution sol = null;

      for (var i = 0; i < Tries; i++) {
        var res = RegressionTreeUtilities.RunSubAlgorithm(Regression, random.Next(), cancellationToken);
        var t = res.Select(x => x.Value).OfType<GaussianProcessRegressionSolution>().FirstOrDefault();
        var score = ((DoubleValue)res["Negative log pseudo-likelihood (LOO-CV)"].Value).Value;
        if (score >= cvscore || t == null || double.IsNaN(t.TrainingRSquared)) continue;
        cvscore = score;
        sol = t;
      }
      Regression.Runs.Clear();
      if (sol == null) throw new ArgumentException("Could not create Gaussian process model");

      numberOfParameters = pd.Dataset.Rows + 1
                           + Regression.CovarianceFunction.GetNumberOfParameters(pd.AllowedInputVariables.Count())
                           + Regression.MeanFunction.GetNumberOfParameters(pd.AllowedInputVariables.Count());
      return sol.Model;
    }

    public override int MinLeafSize(IRegressionProblemData pd) {
      return 3;
    }
    #endregion
  }
}