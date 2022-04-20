﻿#region License Information
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("Autoregressive Modeling (AR)", "Timeseries modeling algorithm that creates AR-N models.")]
  [Creatable(CreatableAttribute.Categories.DataAnalysis, Priority = 130)]
  [StorableType("3BB9D723-944A-420F-A44A-A86F6FB4DABE")]
  public class AutoregressiveModeling : FixedDataAnalysisAlgorithm<ITimeSeriesPrognosisProblem> {
    private const string TimeOffesetParameterName = "Maximum Time Offset";

    public IFixedValueParameter<IntValue> TimeOffsetParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[TimeOffesetParameterName]; }
    }


    public int TimeOffset {
      get { return TimeOffsetParameter.Value.Value; }
      set { TimeOffsetParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected AutoregressiveModeling(StorableConstructorFlag _) : base(_) { }
    protected AutoregressiveModeling(AutoregressiveModeling original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AutoregressiveModeling(this, cloner);
    }

    public AutoregressiveModeling()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(TimeOffesetParameterName, "The maximum time offset for the model ranging from 1 to infinity.", new IntValue(1)));
      Problem = new TimeSeriesPrognosisProblem();
    }

    protected override void Run(CancellationToken cancellationToken) {
      double rmsError, cvRmsError;
      var solution = CreateAutoRegressiveSolution(Problem.ProblemData, TimeOffset, out rmsError, out cvRmsError);
      Results.Add(new Result("Autoregressive solution", "The autoregressive time series prognosis solution.", solution));
      Results.Add(new Result("Root mean square error", "The root of the mean of squared errors of the autoregressive time series prognosis solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Estimated root mean square error (cross-validation)", "The estimated root of the mean of squared errors of the autoregressive time series prognosis solution via cross validation.", new DoubleValue(cvRmsError)));
    }

    /// <summary>
    /// Calculates an AR(p) model. For further information see http://en.wikipedia.org/wiki/Autoregressive_model
    /// </summary>
    /// <param name="problemData">The problem data which should be used for training</param>
    /// <param name="timeOffset">The parameter p of the AR(p) specifying the maximum time offset [1,infinity] </param>
    /// <returns>The times series autoregressive solution </returns>
    public static ITimeSeriesPrognosisSolution CreateAutoRegressiveSolution(ITimeSeriesPrognosisProblemData problemData, int timeOffset) {
      double rmsError, cvRmsError;
      return CreateAutoRegressiveSolution(problemData, timeOffset, out rmsError, out cvRmsError);
    }

    private static ITimeSeriesPrognosisSolution CreateAutoRegressiveSolution(ITimeSeriesPrognosisProblemData problemData, int timeOffset, out double rmsError, out double cvRmsError) {
      string targetVariable = problemData.TargetVariable;

      double[,] inputMatrix = new double[problemData.TrainingPartition.Size, timeOffset + 1];
      var targetValues = problemData.Dataset.GetDoubleValues(targetVariable).ToList();
      for (int i = 0, row = problemData.TrainingPartition.Start; i < problemData.TrainingPartition.Size; i++, row++) {
        for (int col = 0; col < timeOffset; col++) {
          inputMatrix[i, col] = targetValues[row - col - 1];
        }
      }
      // set target values in last column
      for (int i = 0; i < inputMatrix.GetLength(0); i++)
        inputMatrix[i, timeOffset] = targetValues[i + problemData.TrainingPartition.Start];

      if (inputMatrix.ContainsNanOrInfinity())
        throw new NotSupportedException("Linear regression does not support NaN or infinity values in the input dataset.");


      alglib.linearmodel lm = new alglib.linearmodel();
      alglib.lrreport ar = new alglib.lrreport();
      int nRows = inputMatrix.GetLength(0);
      int nFeatures = inputMatrix.GetLength(1) - 1;
      double[] coefficients = new double[nFeatures + 1]; // last coefficient is for the offset

      int retVal = 1;
      alglib.lrbuild(inputMatrix, nRows, nFeatures, out retVal, out lm, out ar);
      if (retVal != 1) throw new ArgumentException("Error in calculation of linear regression solution");
      rmsError = ar.rmserror;
      cvRmsError = ar.cvrmserror;

      alglib.lrunpack(lm, out coefficients, out nFeatures);

      var tree = LinearModelToTreeConverter.CreateTree(
        variableNames: Enumerable.Repeat(problemData.TargetVariable, nFeatures).ToArray(),
        lags: Enumerable.Range(0, timeOffset).Select(i => (i + 1) * -1).ToArray(),
        coefficients: coefficients.Take(nFeatures).ToArray(),
        @const: coefficients[nFeatures]
        );

      var interpreter = new SymbolicTimeSeriesPrognosisExpressionTreeInterpreter(problemData.TargetVariable);
      var model = new SymbolicTimeSeriesPrognosisModel(problemData.TargetVariable, tree, interpreter);
      var solution = model.CreateTimeSeriesPrognosisSolution((ITimeSeriesPrognosisProblemData)problemData.Clone());
      return solution;
    }
  }
}
