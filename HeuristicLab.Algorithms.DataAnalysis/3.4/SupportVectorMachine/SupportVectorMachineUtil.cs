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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;
using LibSVM;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public class SupportVectorMachineUtil {
    /// <summary>
    /// Transforms <paramref name="dataset"/> into a data structure as needed by libSVM.
    /// </summary>
    /// <param name="dataset">The source dataset</param>
    /// <param name="targetVariable">The target variable</param>
    /// <param name="inputVariables">The selected input variables to include in the svm_problem.</param>
    /// <param name="rowIndices">The rows of the dataset that should be contained in the resulting SVM-problem</param>
    /// <returns>A problem data type that can be used to train a support vector machine.</returns>
    public static svm_problem CreateSvmProblem(IDataset dataset, string targetVariable, IEnumerable<string> inputVariables, IEnumerable<int> rowIndices) {
      double[] targetVector ;
      var nRows = rowIndices.Count();
      if (string.IsNullOrEmpty(targetVariable)) {
        // if the target variable is not set (e.g. for prediction of a trained model) we just use a zero vector
        targetVector = new double[nRows];
      } else {
        targetVector = dataset.GetDoubleValues(targetVariable, rowIndices).ToArray();
      }
      svm_node[][] nodes = new svm_node[nRows][];
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
            tempRow.Add(new svm_node() { index = colIndex, value = value });
            // nodes must be sorted in ascending ordered by column index
            if (colIndex > maxNodeIndex) maxNodeIndex = colIndex;
          }
          colIndex++;
        }
        nodes[svmProblemRowIndex++] = tempRow.ToArray();
      }
      return new svm_problem { l = targetVector.Length, y = targetVector, x = nodes };
    }

    /// <summary>
    /// Transforms <paramref name="dataset"/> into a data structure as needed by libSVM for prediction.
    /// </summary>
    /// <param name="dataset">The problem data to transform</param>
    /// <param name="inputVariables">The selected input variables to include in the svm_problem.</param>
    /// <param name="rowIndices">The rows of the dataset that should be contained in the resulting SVM-problem</param>
    /// <returns>A problem data type that can be used for prediction with a trained support vector machine.</returns>
    public static svm_problem CreateSvmProblem(IDataset dataset, IEnumerable<string> inputVariables, IEnumerable<int> rowIndices) {
      // for prediction we don't need a target variable
      return CreateSvmProblem(dataset, string.Empty, inputVariables, rowIndices);
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

    public static double CrossValidate(IDataAnalysisProblemData problemData, svm_parameter parameters, int numberOfFolds, bool shuffleFolds = true) {
      var partitions = GenerateSvmPartitions(problemData, numberOfFolds, shuffleFolds);
      return CalculateCrossValidationPartitions(partitions, parameters);
    }

    public static svm_parameter GridSearch(out double cvMse, IDataAnalysisProblemData problemData, Dictionary<string, IEnumerable<double>> parameterRanges, int numberOfFolds, bool shuffleFolds = true, int maxDegreeOfParallelism = 1) {
      DoubleValue mse = new DoubleValue(Double.MaxValue);
      var bestParam = DefaultParameters();
      var crossProduct = parameterRanges.Values.CartesianProduct();
      var setters = parameterRanges.Keys.Select(GenerateSetter).ToList();
      var partitions = GenerateSvmPartitions(problemData, numberOfFolds, shuffleFolds);

      var locker = new object(); // for thread synchronization
      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
      parameterCombination => {
        var parameters = DefaultParameters();
        var parameterValues = parameterCombination.ToList();
        for (int i = 0; i < parameterValues.Count; ++i)
          setters[i](parameters, parameterValues[i]);

        double testMse = CalculateCrossValidationPartitions(partitions, parameters);
        if (!double.IsNaN(testMse)) {
          lock (locker) {
            if (testMse < mse.Value) {
              mse.Value = testMse;
              bestParam = (svm_parameter)parameters.Clone();
            }
          }
        }
      });
      cvMse = mse.Value;
      return bestParam;
    }

    private static double CalculateCrossValidationPartitions(Tuple<svm_problem, svm_problem>[] partitions, svm_parameter parameters) {
      double avgTestMse = 0;
      var calc = new OnlineMeanSquaredErrorCalculator();
      foreach (Tuple<svm_problem, svm_problem> tuple in partitions) {
        var trainingSvmProblem = tuple.Item1;
        var testSvmProblem = tuple.Item2;
        var model = svm.svm_train(trainingSvmProblem, parameters);
        calc.Reset();
        for (int i = 0; i < testSvmProblem.l; ++i)
          calc.Add(testSvmProblem.y[i], svm.svm_predict(model, testSvmProblem.x[i]));
        double mse = calc.ErrorState == OnlineCalculatorError.None ? calc.MeanSquaredError : double.NaN;
        avgTestMse += mse;
      }
      avgTestMse /= partitions.Length;
      return avgTestMse;
    }

    private static Tuple<svm_problem, svm_problem>[] GenerateSvmPartitions(IDataAnalysisProblemData problemData, int numberOfFolds, bool shuffleFolds = true) {
      var folds = GenerateFolds(problemData, numberOfFolds, shuffleFolds).ToList();
      var targetVariable = GetTargetVariableName(problemData);
      var partitions = new Tuple<svm_problem, svm_problem>[numberOfFolds];
      for (int i = 0; i < numberOfFolds; ++i) {
        int p = i; // avoid "access to modified closure" warning below
        var trainingRows = folds.SelectMany((par, j) => j != p ? par : Enumerable.Empty<int>());
        var testRows = folds[i];
        var trainingSvmProblem = CreateSvmProblem(problemData.Dataset, targetVariable, problemData.AllowedInputVariables, trainingRows);
        var rangeTransform = RangeTransform.Compute(trainingSvmProblem);
        var testSvmProblem = rangeTransform.Scale(CreateSvmProblem(problemData.Dataset, targetVariable, problemData.AllowedInputVariables, testRows));
        partitions[i] = new Tuple<svm_problem, svm_problem>(rangeTransform.Scale(trainingSvmProblem), testSvmProblem);
      }
      return partitions;
    }

    public static IEnumerable<IEnumerable<int>> GenerateFolds(IDataAnalysisProblemData problemData, int numberOfFolds, bool shuffleFolds = true) {
      var random = new MersenneTwister((uint)Environment.TickCount);
      if (problemData is IRegressionProblemData) {
        var trainingIndices = shuffleFolds ? problemData.TrainingIndices.OrderBy(x => random.Next()) : problemData.TrainingIndices;
        return GenerateFolds(trainingIndices, problemData.TrainingPartition.Size, numberOfFolds);
      }
      if (problemData is IClassificationProblemData) {
        // when shuffle is enabled do stratified folds generation, some folds may have zero elements 
        // otherwise, generate folds normally
        return shuffleFolds ? GenerateFoldsStratified(problemData as IClassificationProblemData, numberOfFolds, random) : GenerateFolds(problemData.TrainingIndices, problemData.TrainingPartition.Size, numberOfFolds);
      }
      throw new ArgumentException("Problem data is neither regression or classification problem data.");
    }

    /// <summary>
    /// Stratified fold generation from classification data. Stratification means that we ensure the same distribution of class labels for each fold.
    /// The samples are grouped by class label and each group is split into @numberOfFolds parts. The final folds are formed from the joining of 
    /// the corresponding parts from each class label.
    /// </summary>
    /// <param name="problemData">The classification problem data.</param>
    /// <param name="numberOfFolds">The number of folds in which to split the data.</param>
    /// <param name="random">The random generator used to shuffle the folds.</param>
    /// <returns>An enumerable sequece of folds, where a fold is represented by a sequence of row indices.</returns>
    private static IEnumerable<IEnumerable<int>> GenerateFoldsStratified(IClassificationProblemData problemData, int numberOfFolds, IRandom random) {
      var values = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices);
      var valuesIndices = problemData.TrainingIndices.Zip(values, (i, v) => new { Index = i, Value = v }).ToList();
      IEnumerable<IEnumerable<IEnumerable<int>>> foldsByClass = valuesIndices.GroupBy(x => x.Value, x => x.Index).Select(g => GenerateFolds(g, g.Count(), numberOfFolds));
      var enumerators = foldsByClass.Select(f => f.GetEnumerator()).ToList();
      while (enumerators.All(e => e.MoveNext())) {
        yield return enumerators.SelectMany(e => e.Current).OrderBy(x => random.Next()).ToList();
      }
    }

    private static IEnumerable<IEnumerable<T>> GenerateFolds<T>(IEnumerable<T> values, int valuesCount, int numberOfFolds) {
      // if number of folds is greater than the number of values, some empty folds will be returned
      if (valuesCount < numberOfFolds) {
        for (int i = 0; i < numberOfFolds; ++i)
          yield return i < valuesCount ? values.Skip(i).Take(1) : Enumerable.Empty<T>();
      } else {
        int f = valuesCount / numberOfFolds, r = valuesCount % numberOfFolds; // number of folds rounded to integer and remainder
        int start = 0, end = f;
        for (int i = 0; i < numberOfFolds; ++i) {
          if (r > 0) {
            ++end;
            --r;
          }
          yield return values.Skip(start).Take(end - start);
          start = end;
          end += f;
        }
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
