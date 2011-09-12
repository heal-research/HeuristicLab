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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Random forest classification data analysis algorithm.
  /// </summary>
  [Item("Random Forest Classification", "Random forest classification data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class RandomForestClassification : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string RandomForestClassificationModelResultName = "Random forest classification solution";
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
    private RandomForestClassification(bool deserializing) : base(deserializing) { }
    private RandomForestClassification(RandomForestClassification original, Cloner cloner)
      : base(original, cloner) {
    }
    public RandomForestClassification()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(NumberOfTreesParameterName, "The number of trees in the forest. Should be between 50 and 100", new IntValue(50)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(RParameterName, "The ratio of the training set that will be used in the construction of individual trees (0<r<=1). Should be adjusted depending on the noise level in the dataset in the range from 0.66 (low noise) to 0.05 (high noise). This parameter should be adjusted to achieve good generalization error.", new DoubleValue(0.3)));
      Problem = new ClassificationProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomForestClassification(this, cloner);
    }

    #region random forest
    protected override void Run() {
      double rmsError, relClassificationError, outOfBagRmsError, outOfBagRelClassificationError;
      var solution = CreateRandomForestClassificationSolution(Problem.ProblemData, NumberOfTrees, R, out rmsError, out relClassificationError, out outOfBagRmsError, out outOfBagRelClassificationError);
      Results.Add(new Result(RandomForestClassificationModelResultName, "The random forest classification solution.", solution));
      Results.Add(new Result("Root mean square error", "The root of the mean of squared errors of the random forest regression solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Relative classification error", "Relative classification error of the random forest regression solution on the training set.", new PercentValue(relClassificationError)));
      Results.Add(new Result("Root mean square error (out-of-bag)", "The out-of-bag root of the mean of squared errors of the random forest regression solution.", new DoubleValue(outOfBagRmsError)));
      Results.Add(new Result("Relative classification error (out-of-bag)", "The out-of-bag relative classification error  of the random forest regression solution.", new PercentValue(outOfBagRelClassificationError)));
    }

    public static IClassificationSolution CreateRandomForestClassificationSolution(IClassificationProblemData problemData, int nTrees, double r,
      out double rmsError, out double relClassificationError, out double outOfBagRmsError, out double outOfBagRelClassificationError) {
      Dataset dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndizes;
      double[,] inputMatrix = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables.Concat(new string[] { targetVariable }), rows);
      if (inputMatrix.Cast<double>().Any(x => double.IsNaN(x) || double.IsInfinity(x)))
        throw new NotSupportedException("Random forest classification does not support NaN or infinity values in the input dataset.");


      alglib.decisionforest dforest;
      alglib.dfreport rep;
      int nRows = inputMatrix.GetLength(0);
      int nCols = inputMatrix.GetLength(1);
      int info;
      double[] classValues = dataset.GetDoubleValues(targetVariable).Distinct().OrderBy(x => x).ToArray();
      int nClasses = classValues.Count();
      // map original class values to values [0..nClasses-1]
      Dictionary<double, double> classIndizes = new Dictionary<double, double>();
      for (int i = 0; i < nClasses; i++) {
        classIndizes[classValues[i]] = i;
      }
      for (int row = 0; row < nRows; row++) {
        inputMatrix[row, nCols - 1] = classIndizes[inputMatrix[row, nCols - 1]];
      }
      // execute random forest algorithm
      alglib.dfbuildrandomdecisionforest(inputMatrix, nRows, nCols - 1, nClasses, nTrees, r, out info, out dforest, out rep);
      if (info != 1) throw new ArgumentException("Error in calculation of random forest classification solution");

      rmsError = rep.rmserror;
      outOfBagRmsError = rep.oobrmserror;
      relClassificationError = rep.relclserror;
      outOfBagRelClassificationError = rep.oobrelclserror;
      return new RandomForestClassificationSolution((IClassificationProblemData)problemData.Clone(), new RandomForestModel(dforest, targetVariable, allowedInputVariables, classValues));
    }
    #endregion
  }
}
