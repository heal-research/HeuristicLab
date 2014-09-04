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
using System.Linq.Expressions;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;
using LibSVM;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public class SupportVectorMachineUtil {
    /// <summary>
    /// Transforms <paramref name="problemData"/> into a data structure as needed by libSVM.
    /// </summary>
    /// <param name="problemData">The problem data to transform</param>
    /// <param name="rowIndices">The rows of the dataset that should be contained in the resulting SVM-problem</param>
    /// <returns>A problem data type that can be used to train a support vector machine.</returns>
    public static svm_problem CreateSvmProblem(Dataset dataset, string targetVariable, IEnumerable<string> inputVariables, IEnumerable<int> rowIndices) {
      double[] targetVector = dataset.GetDoubleValues(targetVariable, rowIndices).ToArray();
      svm_node[][] nodes = new svm_node[targetVector.Length][];
      int maxNodeIndex = 0;
      int svmProblemRowIndex = 0;
      List<string> inputVariablesList = inputVariables.ToList();
      foreach (int row in rowIndices) {
        List<svm_node> tempRow = new List<svm_node>();
        int colIndex = 1; // make sure the smallest node index for SVM = 1
        foreach (var inputVariable in inputVariablesList) {
          double value = dataset.GetDoubleValue(inputVariable, row);
          // SVM also works with missing values
          // => don't add NaN values in the dataset to the sparse SVM matrix representation
          if (!double.IsNaN(value)) {
            tempRow.Add(new svm_node() { index = colIndex, value = value }); // nodes must be sorted in ascending ordered by column index
            if (colIndex > maxNodeIndex) maxNodeIndex = colIndex;
          }
          colIndex++;
        }
        nodes[svmProblemRowIndex++] = tempRow.ToArray();
      }
      return new svm_problem { l = targetVector.Length, y = targetVector, x = nodes };
    }

    /// <summary>
    /// Instantiate and return a svm_parameter object with default values.
    /// </summary>
    /// <returns>A svm_parameter object with default values</returns>
    public static svm_parameter DefaultParameters() {
      svm_parameter parameter = new svm_parameter();
      parameter.svm_type = svm_parameter.NU_SVR;
      parameter.kernel_type = svm_parameter.RBF;
      parameter.C = 1;
      parameter.nu = 0.5;
      parameter.gamma = 1;
      parameter.p = 1;
      parameter.cache_size = 500;
      parameter.probability = 0;
      parameter.eps = 0.001;
      parameter.degree = 3;
      parameter.shrinking = 1;
      parameter.coef0 = 0;

      return parameter;
    }

    public static void CrossValidate(IDataAnalysisProblemData problemData, svm_parameter parameters, int numberOfFolds, out double avgTestMse) {
      var partitions = GenerateSvmPartitions(problemData, numberOfFolds);
      CalculateCrossValidationPartitions(partitions, parameters, out avgTestMse);
    }

    public static svm_parameter GridSearch(IDataAnalysisProblemData problemData, int numberOfFolds, Dictionary<string, IEnumerable<double>> parameterRanges, int maxDegreeOfParallelism = 1) {
      DoubleValue mse = new DoubleValue(Double.MaxValue);
      var bestParam = DefaultParameters();
      var crossProduct = parameterRanges.Values.CartesianProduct();
      var setters = parameterRanges.Keys.Select(GenerateSetter).ToList();
      var partitions = GenerateSvmPartitions(problemData, numberOfFolds);
      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, parameterCombination => {
        var parameters = DefaultParameters();
        var parameterValues = parameterCombination.ToList();
        for (int i = 0; i < parameterValues.Count; ++i) {
          setters[i](parameters, parameterValues[i]);
        }
        double testMse;
        CalculateCrossValidationPartitions(partitions, parameters, out testMse);
        if (testMse < mse.Value) {
          lock (mse) {
            mse.Value = testMse;
            bestParam = (svm_parameter)parameters.Clone();
          }
        }
      });
      return bestParam;
    }

    private static void CalculateCrossValidationPartitions(Tuple<svm_problem, svm_problem>[] partitions, svm_parameter parameters, out double avgTestMse) {
      avgTestMse = 0;
      var calc = new OnlineMeanSquaredErrorCalculator();
      foreach (Tuple<svm_problem, svm_problem> tuple in partitions) {
        var trainingSvmProblem = tuple.Item1;
        var testSvmProblem = tuple.Item2;
        var model = svm.svm_train(trainingSvmProblem, parameters);
        calc.Reset();
        for (int i = 0; i < testSvmProblem.l; ++i)
          calc.Add(testSvmProblem.y[i], svm.svm_predict(model, testSvmProblem.x[i]));
        avgTestMse += calc.MeanSquaredError;
      }
      avgTestMse /= partitions.Length;
    }


    private static Tuple<svm_problem, svm_problem>[] GenerateSvmPartitions(IDataAnalysisProblemData problemData, int numberOfFolds) {
      var folds = GenerateFolds(problemData, numberOfFolds).ToList();
      var targetVariable = GetTargetVariableName(problemData);
      var partitions = new Tuple<svm_problem, svm_problem>[numberOfFolds];
      for (int i = 0; i < numberOfFolds; ++i) {
        int p = i; // avoid "access to modified closure" warning below
        var trainingRows = folds.SelectMany((par, j) => j != p ? par : Enumerable.Empty<int>());
        var testRows = folds[i];
        var trainingSvmProblem = CreateSvmProblem(problemData.Dataset, targetVariable, problemData.AllowedInputVariables, trainingRows);
        var testSvmProblem = CreateSvmProblem(problemData.Dataset, targetVariable, problemData.AllowedInputVariables, testRows);
        partitions[i] = new Tuple<svm_problem, svm_problem>(trainingSvmProblem, testSvmProblem);
      }
      return partitions;
    }

    /// <summary>
    /// Generate a collection of row sequences corresponding to folds in the data (used for crossvalidation)
    /// </summary>
    /// <remarks>This method is aimed to be lightweight and as such does not clone the dataset.</remarks>
    /// <param name="problemData">The problem data</param>
    /// <param name="numberOfFolds">The number of folds to generate</param>
    /// <returns>A sequence of folds representing each a sequence of row numbers</returns>
    private static IEnumerable<IEnumerable<int>> GenerateFolds(IDataAnalysisProblemData problemData, int numberOfFolds) {
      int size = problemData.TrainingPartition.Size;
      int f = size / numberOfFolds, r = size % numberOfFolds; // number of folds rounded to integer and remainder
      int start = 0, end = f;
      for (int i = 0; i < numberOfFolds; ++i) {
        if (r > 0) { ++end; --r; }
        yield return problemData.TrainingIndices.Skip(start).Take(end - start);
        start = end;
        end += f;
      }
    }

    private static Action<svm_parameter, double> GenerateSetter(string fieldName) {
      var targetExp = Expression.Parameter(typeof(svm_parameter));
      var valueExp = Expression.Parameter(typeof(double));
      var fieldExp = Expression.Field(targetExp, fieldName);
      var assignExp = Expression.Assign(fieldExp, Expression.Convert(valueExp, fieldExp.Type));
      var setter = Expression.Lambda<Action<svm_parameter, double>>(assignExp, targetExp, valueExp).Compile();
      return setter;
    }

    private static string GetTargetVariableName(IDataAnalysisProblemData problemData) {
      var regressionProblemData = problemData as IRegressionProblemData;
      var classificationProblemData = problemData as IClassificationProblemData;

      if (regressionProblemData != null)
        return regressionProblemData.TargetVariable;
      if (classificationProblemData != null)
        return classificationProblemData.TargetVariable;

      throw new ArgumentException("Problem data is neither regression or classification problem data.");
    }

  }
}
