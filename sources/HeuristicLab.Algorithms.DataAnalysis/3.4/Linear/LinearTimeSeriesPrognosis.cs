#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Linear time series prognosis data analysis algorithm.
  /// </summary>
  [Item("Linear Time Series Prognosis", "Linear time series prognosis data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class LinearTimeSeriesPrognosis : FixedDataAnalysisAlgorithm<ITimeSeriesPrognosisProblem> {
    private const string LinearTimeSeriesPrognosisModelResultName = "Linear time-series prognosis solution";
    private const string MaximalLagParameterName = "MaximalLag";

    public IFixedValueParameter<IntValue> MaximalLagParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximalLagParameterName]; }
    }

    public int MaximalLag {
      get { return MaximalLagParameter.Value.Value; }
      set { MaximalLagParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private LinearTimeSeriesPrognosis(bool deserializing) : base(deserializing) { }
    private LinearTimeSeriesPrognosis(LinearTimeSeriesPrognosis original, Cloner cloner)
      : base(original, cloner) {
    }
    public LinearTimeSeriesPrognosis()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(MaximalLagParameterName,
                                                       "The maximal lag to use for auto-regressive terms.",
                                                       new IntValue(1)));
      Problem = new TimeSeriesPrognosisProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearTimeSeriesPrognosis(this, cloner);
    }

    #region linear regression
    protected override void Run() {
      double rmsError, cvRmsError;
      var solution = CreateLinearTimeSeriesPrognosisSolution(Problem.ProblemData, MaximalLag, out rmsError, out cvRmsError);
      Results.Add(new Result(LinearTimeSeriesPrognosisModelResultName, "The linear time-series prognosis solution.", solution));
      Results.Add(new Result("Root mean square error", "The root of the mean of squared errors of the linear time-series prognosis solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Estimated root mean square error (cross-validation)", "The estimated root of the mean of squared errors of the linear time-series prognosis solution via cross validation.", new DoubleValue(cvRmsError)));
    }

    public static ISymbolicTimeSeriesPrognosisSolution CreateLinearTimeSeriesPrognosisSolution(ITimeSeriesPrognosisProblemData problemData, int maximalLag, out double rmsError, out double cvRmsError) {
      Dataset dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndizes;
      IEnumerable<int> lags = Enumerable.Range(1, maximalLag);
      double[,] inputMatrix = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables.Concat(new string[] { targetVariable }), rows, lags);
      if (inputMatrix.Cast<double>().Any(x => double.IsNaN(x) || double.IsInfinity(x)))
        throw new NotSupportedException("Linear time-series prognosis does not support NaN or infinity values in the input dataset.");

      alglib.linearmodel lm = new alglib.linearmodel();
      alglib.lrreport ar = new alglib.lrreport();
      int nRows = inputMatrix.GetLength(0);
      int nFeatures = inputMatrix.GetLength(1) - 1;
      double[] coefficients = new double[nFeatures + 1]; // last coefficient is for the constant

      int retVal = 1;
      alglib.lrbuild(inputMatrix, nRows, nFeatures, out retVal, out lm, out ar);
      if (retVal != 1) throw new ArgumentException("Error in calculation of linear time series prognosis solution");
      rmsError = ar.rmserror;
      cvRmsError = ar.cvrmserror;

      alglib.lrunpack(lm, out coefficients, out nFeatures);

      ISymbolicExpressionTree tree = new SymbolicExpressionTree(new ProgramRootSymbol().CreateTreeNode());
      ISymbolicExpressionTreeNode startNode = new StartSymbol().CreateTreeNode();
      tree.Root.AddSubtree(startNode);
      ISymbolicExpressionTreeNode addition = new Addition().CreateTreeNode();
      startNode.AddSubtree(addition);

      int col = 0;
      foreach (string column in allowedInputVariables) {
        foreach (int lag in lags) {
          LaggedVariableTreeNode vNode =
            (LaggedVariableTreeNode)new HeuristicLab.Problems.DataAnalysis.Symbolic.LaggedVariable().CreateTreeNode();
          vNode.VariableName = column;
          vNode.Weight = coefficients[col];
          vNode.Lag = -lag;
          addition.AddSubtree(vNode);
          col++;
        }
      }

      ConstantTreeNode cNode = (ConstantTreeNode)new Constant().CreateTreeNode();
      cNode.Value = coefficients[coefficients.Length - 1];
      addition.AddSubtree(cNode);

      SymbolicTimeSeriesPrognosisSolution solution = new SymbolicTimeSeriesPrognosisSolution(new SymbolicTimeSeriesPrognosisModel(tree, new SymbolicDataAnalysisExpressionTreeInterpreter()), (ITimeSeriesPrognosisProblemData)problemData.Clone());
      solution.Model.Name = "Linear Time-Series Prognosis Model";
      return solution;
    }
    #endregion
  }
}
