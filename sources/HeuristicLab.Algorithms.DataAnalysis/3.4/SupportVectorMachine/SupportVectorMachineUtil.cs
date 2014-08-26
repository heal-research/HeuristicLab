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
      double[] targetVector =
        dataset.GetDoubleValues(targetVariable, rowIndices).ToArray();

      svm_node[][] nodes = new svm_node[targetVector.Length][];
      List<svm_node> tempRow;
      int maxNodeIndex = 0;
      int svmProblemRowIndex = 0;
      List<string> inputVariablesList = inputVariables.ToList();
      foreach (int row in rowIndices) {
        tempRow = new List<svm_node>();
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

      return new svm_problem() { l = targetVector.Length, y = targetVector, x = nodes };
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

    /// <summary>
    /// Generate a collection of training indices corresponding to folds in the data (used for crossvalidation)
    /// </summary>
    /// <remarks>This method is aimed to be lightweight and as such does not clone the dataset.</remarks>
    /// <param name="problemData">The problem data</param>
    /// <param name="nFolds">The number of folds to generate</param>
    /// <returns>A sequence of folds representing each a sequence of row numbers</returns>
    public static IEnumerable<IEnumerable<int>> GenerateFolds(IRegressionProblemData problemData, int nFolds) {
      int size = problemData.TrainingPartition.Size;

      int foldSize = size / nFolds; // rounding to integer
      var trainingIndices = problemData.TrainingIndices;

      for (int i = 0; i < nFolds; ++i) {
        int n = i * foldSize;
        int s = n + 2 * foldSize > size ? foldSize + size % foldSize : foldSize;
        yield return trainingIndices.Skip(n).Take(s);
      }
    }

    /// <summary>
    /// Performs crossvalidation
    /// </summary>
    /// <param name="problemData">The problem data</param>
    /// <param name="parameters">The svm parameters</param>
    /// <param name="folds">The svm_problem instances for each fold</param>
    /// <param name="avgTestMSE">The average test mean squared error (not used atm)</param>
    public static void CrossValidate(IRegressionProblemData problemData, svm_parameter parameters, IEnumerable<IEnumerable<int>> folds, out double avgTestMSE) {
      avgTestMSE = 0;

      var calc = new OnlineMeanSquaredErrorCalculator();
      var ds = problemData.Dataset;
      var targetVariable = problemData.TargetVariable;
      var inputVariables = problemData.AllowedInputVariables;

      var svmProblem = CreateSvmProblem(ds, targetVariable, inputVariables, problemData.TrainingIndices);
      var partitions = folds.ToList();

      for (int i = 0; i < partitions.Count; ++i) {
        var test = partitions[i];
        var training = new List<int>();
        for (int j = 0; j < i; ++j)
          training.AddRange(partitions[j]);

        for (int j = i + 1; j < partitions.Count; ++j)
          training.AddRange(partitions[j]);

        var p = CreateSvmProblem(ds, targetVariable, inputVariables, training);
        var model = svm.svm_train(p, parameters);
        calc.Reset();
        foreach (var row in test) {
          calc.Add(svmProblem.y[row], svm.svm_predict(model, svmProblem.x[row]));
        }
        double error = calc.MeanSquaredError;
        avgTestMSE += error;
      }

      avgTestMSE /= partitions.Count;
    }

    /// <summary>
    /// Dynamically generate a setter for svm_parameter fields
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    private static Action<svm_parameter, double> GenerateSetter(string fieldName) {
      var targetExp = Expression.Parameter(typeof(svm_parameter));
      var valueExp = Expression.Parameter(typeof(double));

      // Expression.Property can be used here as well
      var fieldExp = Expression.Field(targetExp, fieldName);
      var assignExp = Expression.Assign(fieldExp, Expression.Convert(valueExp, fieldExp.Type));
      var setter = Expression.Lambda<Action<svm_parameter, double>>(assignExp, targetExp, valueExp).Compile();
      return setter;
    }

    public static svm_parameter GridSearch(IRegressionProblemData problemData, IEnumerable<IEnumerable<int>> folds, Dictionary<string, IEnumerable<double>> parameterRanges, int maxDegreeOfParallelism = 1) {
      DoubleValue mse = new DoubleValue(Double.MaxValue);
      var bestParam = DefaultParameters();

      // search for C, gamma and epsilon parameter combinations

      var pNames = parameterRanges.Keys.ToList();
      var pRanges = pNames.Select(x => parameterRanges[x]);

      var crossProduct = pRanges.CartesianProduct();
      var setters = pNames.Select(GenerateSetter).ToList();
      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, nuple => {
        //  foreach (var nuple in crossProduct) {
        var list = nuple.ToList();
        var parameters = DefaultParameters();
        for (int i = 0; i < pNames.Count; ++i) {
          var s = setters[i];
          s(parameters, list[i]);
        }
        double testMSE;
        CrossValidate(problemData, parameters, folds, out testMSE);
        if (testMSE < mse.Value) {
          lock (mse) { mse.Value = testMSE; }
          lock (bestParam) { // set best parameter values to the best found so far 
            bestParam = (svm_parameter)parameters.Clone();
          }
        }
      });
      return bestParam;
    }
  }
}
