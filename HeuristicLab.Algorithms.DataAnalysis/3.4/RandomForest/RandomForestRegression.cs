#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Random forest regression data analysis algorithm.
  /// </summary>
  [Item("Random Forest Regression", "Random forest regression data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class RandomForestRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string RandomForestRegressionModelResultName = "Random forest regression solution";
    private const string NumberOfTreesParameterName = "Number of trees";
    private const string RParameterName = "R";
    #region parameter properties
    public IValueParameter<IntValue> NumberOfTreesParameter {
      get { return (IValueParameter<IntValue>)Parameters[NumberOfTreesParameterName]; }
    }
    public IValueParameter<DoubleValue> RParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[RParameterName]; }
    }
    #endregion
    #region properties
    public int NumberOfTrees {
      get { return NumberOfTreesParameter.Value.Value; }
      set { NumberOfTreesParameter.Value.Value = value; }
    }
    public double R {
      get { return RParameter.Value.Value; }
      set { RParameter.Value.Value = value; }
    }
    #endregion
    [StorableConstructor]
    private RandomForestRegression(bool deserializing) : base(deserializing) { }
    private RandomForestRegression(RandomForestRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public RandomForestRegression()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(NumberOfTreesParameterName, "The number of trees in the forest. Should be between 50 and 100", new IntValue(50)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(RParameterName, "The ratio of the training set that will be used in the construction of individual trees (0<r<=1). Should be adjusted depending on the noise level in the dataset in the range from 0.66 (low noise) to 0.05 (high noise). This parameter should be adjusted to achieve good generalization error.", new DoubleValue(0.3)));
      Problem = new RegressionProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomForestRegression(this, cloner);
    }

    #region random forest
    protected override void Run() {
      double rmsError, avgRelError, outOfBagRmsError, outOfBagAvgRelError;
      var solution = CreateRandomForestRegressionSolution(Problem.ProblemData, NumberOfTrees, R, out rmsError, out avgRelError, out outOfBagRmsError, out outOfBagAvgRelError);
      Results.Add(new Result(RandomForestRegressionModelResultName, "The random forest regression solution.", solution));
      Results.Add(new Result("Root mean square error", "The root of the mean of squared errors of the random forest regression solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Average relative error", "The average of relative errors of the random forest regression solution on the training set.", new PercentValue(avgRelError)));
      Results.Add(new Result("Root mean square error (out-of-bag)", "The out-of-bag root of the mean of squared errors of the random forest regression solution.", new DoubleValue(outOfBagRmsError)));
      Results.Add(new Result("Average relative error (out-of-bag)", "The out-of-bag average of relative errors of the random forest regression solution.", new PercentValue(outOfBagAvgRelError)));
    }

    public static IRegressionSolution CreateRandomForestRegressionSolution(IRegressionProblemData problemData, int nTrees, double r,
      out double rmsError, out double avgRelError, out double outOfBagRmsError, out double outOfBagAvgRelError) {
      Dataset dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndices;
      double[,] inputMatrix = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables.Concat(new string[] { targetVariable }), rows);
      if (inputMatrix.Cast<double>().Any(x => double.IsNaN(x) || double.IsInfinity(x)))
        throw new NotSupportedException("Random forest regression does not support NaN or infinity values in the input dataset.");


      alglib.decisionforest dforest;
      alglib.dfreport rep;
      int nRows = inputMatrix.GetLength(0);

      int info;
      alglib.dfbuildrandomdecisionforest(inputMatrix, nRows, allowedInputVariables.Count(), 1, nTrees, r, out info, out dforest, out rep);
      if (info != 1) throw new ArgumentException("Error in calculation of random forest regression solution");

      rmsError = rep.rmserror;
      avgRelError = rep.avgrelerror;
      outOfBagAvgRelError = rep.oobavgrelerror;
      outOfBagRmsError = rep.oobrmserror;

      return new RandomForestRegressionSolution((IRegressionProblemData)problemData.Clone(), new RandomForestModel(dforest, targetVariable, allowedInputVariables));
    }
    #endregion
  }
}
