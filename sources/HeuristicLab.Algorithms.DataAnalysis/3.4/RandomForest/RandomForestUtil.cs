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

namespace HeuristicLab.Algorithms.DataAnalysis {
  public class RFParameter : ICloneable {
    public double n; // number of trees
    public double m;
    public double r;

    public object Clone() { return new RFParameter { n = this.n, m = this.m, r = this.r }; }
  }

  public static class RandomForestUtil {
    private static Action<RFParameter, double> GenerateSetter(string field) {
      var targetExp = Expression.Parameter(typeof(RFParameter));
      var valueExp = Expression.Parameter(typeof(double));
      var fieldExp = Expression.Field(targetExp, field);
      var assignExp = Expression.Assign(fieldExp, Expression.Convert(valueExp, fieldExp.Type));
      var setter = Expression.Lambda<Action<RFParameter, double>>(assignExp, targetExp, valueExp).Compile();
      return setter;
    }

    /// <summary>
    /// Generate a collection of row sequences corresponding to folds in the data (used for crossvalidation)
    /// </summary>
    /// <remarks>This method is aimed to be lightweight and as such does not clone the dataset.</remarks>
    /// <param name="problemData">The problem data</param>
    /// <param name="numberOfFolds">The number of folds to generate</param>
    /// <returns>A sequence of folds representing each a sequence of row numbers</returns>
    public static IEnumerable<IEnumerable<int>> GenerateFolds(IDataAnalysisProblemData problemData, int numberOfFolds) {
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

    private static Tuple<IEnumerable<int>, IEnumerable<int>>[] GenerateRandomForestPartitions(IDataAnalysisProblemData problemData, int numberOfFolds) {
      var folds = GenerateFolds(problemData, numberOfFolds).ToList();
      var partitions = new Tuple<IEnumerable<int>, IEnumerable<int>>[numberOfFolds];

      for (int i = 0; i < numberOfFolds; ++i) {
        int p = i; // avoid "access to modified closure" warning
        var trainingRows = folds.SelectMany((par, j) => j != p ? par : Enumerable.Empty<int>());
        var testRows = folds[i];
        partitions[i] = new Tuple<IEnumerable<int>, IEnumerable<int>>(trainingRows, testRows);
      }
      return partitions;
    }

    public static void CrossValidate(IDataAnalysisProblemData problemData, int numberOfFolds, RFParameter parameters, int seed, out double error) {
      var partitions = GenerateRandomForestPartitions(problemData, numberOfFolds);
      CrossValidate(problemData, partitions, parameters, seed, out error);
    }

    // user should call the more specific CrossValidate methods
    public static void CrossValidate(IDataAnalysisProblemData problemData, Tuple<IEnumerable<int>, IEnumerable<int>>[] partitions, RFParameter parameters, int seed, out double error) {
      CrossValidate(problemData, partitions, (int)parameters.n, parameters.m, parameters.r, seed, out error);
    }

    public static void CrossValidate(IDataAnalysisProblemData problemData, Tuple<IEnumerable<int>, IEnumerable<int>>[] partitions, int nTrees, double r, double m, int seed, out double error) {
      var regressionProblemData = problemData as IRegressionProblemData;
      var classificationProblemData = problemData as IClassificationProblemData;
      if (regressionProblemData != null)
        CrossValidate(regressionProblemData, partitions, nTrees, m, r, seed, out error);
      else if (classificationProblemData != null)
        CrossValidate(classificationProblemData, partitions, nTrees, m, r, seed, out error);
      else throw new ArgumentException("Problem data is neither regression or classification problem data.");
    }

    private static void CrossValidate(IRegressionProblemData problemData, Tuple<IEnumerable<int>, IEnumerable<int>>[] partitions, RFParameter parameters, int seed, out double avgTestMse) {
      CrossValidate(problemData, partitions, (int)parameters.n, parameters.m, parameters.r, seed, out avgTestMse);
    }

    private static void CrossValidate(IClassificationProblemData problemData, Tuple<IEnumerable<int>, IEnumerable<int>>[] partitions, RFParameter parameters, int seed, out double avgTestMse) {
      CrossValidate(problemData, partitions, (int)parameters.n, parameters.m, parameters.r, seed, out avgTestMse);
    }

    private static void CrossValidate(IRegressionProblemData problemData, Tuple<IEnumerable<int>, IEnumerable<int>>[] partitions, int nTrees, double r, double m, int seed, out double avgTestMse) {
      avgTestMse = 0;
      var ds = problemData.Dataset;
      var targetVariable = GetTargetVariableName(problemData);
      foreach (var tuple in partitions) {
        double rmsError, avgRelError, outOfBagAvgRelError, outOfBagRmsError;
        var trainingRandomForestPartition = tuple.Item1;
        var testRandomForestPartition = tuple.Item2;
        var model = RandomForestModel.CreateRegressionModel(problemData, nTrees, r, m, seed, out rmsError, out avgRelError, out outOfBagRmsError, out outOfBagAvgRelError, trainingRandomForestPartition);
        var estimatedValues = model.GetEstimatedValues(ds, testRandomForestPartition);
        var targetValues = ds.GetDoubleValues(targetVariable, testRandomForestPartition);
        OnlineCalculatorError calculatorError;
        double mse = OnlineMeanSquaredErrorCalculator.Calculate(estimatedValues, targetValues, out calculatorError);
        if (calculatorError != OnlineCalculatorError.None)
          mse = double.NaN;
        avgTestMse += mse;
      }
      avgTestMse /= partitions.Length;
    }

    private static void CrossValidate(IClassificationProblemData problemData, Tuple<IEnumerable<int>, IEnumerable<int>>[] partitions, int nTrees, double r, double m, int seed, out double avgTestAccuracy) {
      avgTestAccuracy = 0;
      var ds = problemData.Dataset;
      var targetVariable = GetTargetVariableName(problemData);
      foreach (var tuple in partitions) {
        double rmsError, avgRelError, outOfBagAvgRelError, outOfBagRmsError;
        var trainingRandomForestPartition = tuple.Item1;
        var testRandomForestPartition = tuple.Item2;
        var model = RandomForestModel.CreateClassificationModel(problemData, nTrees, r, m, seed, out rmsError, out avgRelError, out outOfBagRmsError, out outOfBagAvgRelError, trainingRandomForestPartition);
        var estimatedValues = model.GetEstimatedClassValues(ds, testRandomForestPartition);
        var targetValues = ds.GetDoubleValues(targetVariable, testRandomForestPartition);
        OnlineCalculatorError calculatorError;
        double accuracy = OnlineAccuracyCalculator.Calculate(estimatedValues, targetValues, out calculatorError);
        if (calculatorError != OnlineCalculatorError.None)
          accuracy = double.NaN;
        avgTestAccuracy += accuracy;
      }
      avgTestAccuracy /= partitions.Length;
    }

    public static RFParameter GridSearch(IDataAnalysisProblemData problemData, int numberOfFolds, Dictionary<string, IEnumerable<double>> parameterRanges, int seed = 12345, int maxDegreeOfParallelism = 1) {
      var regressionProblemData = problemData as IRegressionProblemData;
      var classificationProblemData = problemData as IClassificationProblemData;

      if (regressionProblemData != null)
        return GridSearch(regressionProblemData, numberOfFolds, parameterRanges, seed, maxDegreeOfParallelism);
      if (classificationProblemData != null)
        return GridSearch(classificationProblemData, numberOfFolds, parameterRanges, seed, maxDegreeOfParallelism);

      throw new ArgumentException("Problem data is neither regression or classification problem data.");
    }

    private static RFParameter GridSearch(IRegressionProblemData problemData, int numberOfFolds, Dictionary<string, IEnumerable<double>> parameterRanges, int seed = 12345, int maxDegreeOfParallelism = 1) {
      DoubleValue mse = new DoubleValue(Double.MaxValue);
      RFParameter bestParameter = new RFParameter { n = 1, m = 0.1, r = 0.1 }; // some random defaults

      var pNames = parameterRanges.Keys.ToList();
      var pRanges = pNames.Select(x => parameterRanges[x]);
      var setters = pNames.Select(GenerateSetter).ToList();
      var partitions = GenerateRandomForestPartitions(problemData, numberOfFolds);
      var crossProduct = pRanges.CartesianProduct();

      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, nuple => {
        var list = nuple.ToList();
        double testMSE;
        var parameters = new RFParameter();
        for (int i = 0; i < pNames.Count; ++i) {
          var s = setters[i];
          s(parameters, list[i]);
        }
        CrossValidate(problemData, partitions, parameters, seed, out testMSE);
        if (testMSE < mse.Value) {
          lock (mse) { mse.Value = testMSE; }
          lock (bestParameter) { bestParameter = (RFParameter)parameters.Clone(); }
        }
      });
      return bestParameter;
    }

    private static RFParameter GridSearch(IClassificationProblemData problemData, int numberOfFolds, Dictionary<string, IEnumerable<double>> parameterRanges, int seed = 12345, int maxDegreeOfParallelism = 1) {
      DoubleValue accuracy = new DoubleValue(0);
      RFParameter bestParameter = new RFParameter { n = 1, m = 0.1, r = 0.1 }; // some random defaults

      var pNames = parameterRanges.Keys.ToList();
      var pRanges = pNames.Select(x => parameterRanges[x]);
      var setters = pNames.Select(GenerateSetter).ToList();
      var partitions = GenerateRandomForestPartitions(problemData, numberOfFolds);
      var crossProduct = pRanges.CartesianProduct();

      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, nuple => {
        var list = nuple.ToList();
        double testAccuracy;
        var parameters = new RFParameter();
        for (int i = 0; i < pNames.Count; ++i) {
          var s = setters[i];
          s(parameters, list[i]);
        }
        CrossValidate(problemData, partitions, parameters, seed, out testAccuracy);
        if (testAccuracy > accuracy.Value) {
          lock (accuracy) { accuracy.Value = testAccuracy; }
          lock (bestParameter) { bestParameter = (RFParameter)parameters.Clone(); }
        }
      });
      return bestParameter;
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
