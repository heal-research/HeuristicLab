#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Multinomial logit regression data analysis algorithm.
  /// </summary>
  [Item("Multinomial Logit Classification", "Multinomial logit classification data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class MultiNomialLogitClassification : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string LogitClassificationModelResultName = "Logit classification solution";

    [StorableConstructor]
    private MultiNomialLogitClassification(bool deserializing) : base(deserializing) { }
    private MultiNomialLogitClassification(MultiNomialLogitClassification original, Cloner cloner)
      : base(original, cloner) {
    }
    public MultiNomialLogitClassification()
      : base() {
      Problem = new ClassificationProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiNomialLogitClassification(this, cloner);
    }

    #region logit classification
    protected override void Run() {
      double rmsError, relClassError;
      var solution = CreateLogitClassificationSolution(Problem.ProblemData, out rmsError, out relClassError);
      Results.Add(new Result(LogitClassificationModelResultName, "The logit classification solution.", solution));
      Results.Add(new Result("Root mean squared error", "The root of the mean of squared errors of the logit regression solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Relative classification error", "Relative classification error on the training set (percentage of misclassified cases).", new PercentValue(relClassError)));
    }

    public static IClassificationSolution CreateLogitClassificationSolution(IClassificationProblemData problemData, out double rmsError, out double relClassError) {
      Dataset dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndices;
      double[,] inputMatrix = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables.Concat(new string[] { targetVariable }), rows);
      if (inputMatrix.Cast<double>().Any(x => double.IsNaN(x) || double.IsInfinity(x)))
        throw new NotSupportedException("Multinomial logit classification does not support NaN or infinity values in the input dataset.");

      alglib.logitmodel lm = new alglib.logitmodel();
      alglib.mnlreport rep = new alglib.mnlreport();
      int nRows = inputMatrix.GetLength(0);
      int nFeatures = inputMatrix.GetLength(1) - 1;
      double[] classValues = dataset.GetDoubleValues(targetVariable).Distinct().OrderBy(x => x).ToArray();
      int nClasses = classValues.Count();
      // map original class values to values [0..nClasses-1]
      Dictionary<double, double> classIndices = new Dictionary<double, double>();
      for (int i = 0; i < nClasses; i++) {
        classIndices[classValues[i]] = i;
      }
      for (int row = 0; row < nRows; row++) {
        inputMatrix[row, nFeatures] = classIndices[inputMatrix[row, nFeatures]];
      }
      int info;
      alglib.mnltrainh(inputMatrix, nRows, nFeatures, nClasses, out info, out lm, out rep);
      if (info != 1) throw new ArgumentException("Error in calculation of logit classification solution");

      rmsError = alglib.mnlrmserror(lm, inputMatrix, nRows);
      relClassError = alglib.mnlrelclserror(lm, inputMatrix, nRows);

      MultinomialLogitClassificationSolution solution = new MultinomialLogitClassificationSolution((IClassificationProblemData)problemData.Clone(), new MultinomialLogitModel(lm, targetVariable, allowedInputVariables, classValues));
      return solution;
    }
    #endregion
  }
}
