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

      // Expression.Property can be used here as well
      var fieldExp = Expression.Field(targetExp, field);
      var assignExp = Expression.Assign(fieldExp, Expression.Convert(valueExp, fieldExp.Type));
      var setter = Expression.Lambda<Action<RFParameter, double>>(assignExp, targetExp, valueExp).Compile();
      return setter;
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

    public static void CrossValidate(IRegressionProblemData problemData, IEnumerable<IEnumerable<int>> folds, RFParameter parameter, int seed, out double avgTestMSE) {
      CrossValidate(problemData, folds, (int)Math.Round(parameter.n), parameter.m, parameter.r, seed, out avgTestMSE);
    }

    public static void CrossValidate(IRegressionProblemData problemData, IEnumerable<IEnumerable<int>> folds, int nTrees, double m, double r, int seed, out double avgTestMSE) {
      avgTestMSE = 0;

      var ds = problemData.Dataset;
      var targetVariable = problemData.TargetVariable;

      var partitions = folds.ToList();

      for (int i = 0; i < partitions.Count; ++i) {
        double rmsError, avgRelError, outOfBagAvgRelError, outOfBagRmsError;
        var test = partitions[i];
        var training = new List<int>();
        for (int j = 0; j < i; ++j)
          training.AddRange(partitions[j]);

        for (int j = i + 1; j < partitions.Count; ++j)
          training.AddRange(partitions[j]);

        var model = RandomForestModel.CreateRegressionModel(problemData, nTrees, m, r, seed, out rmsError, out avgRelError, out outOfBagAvgRelError, out outOfBagRmsError, training);
        var estimatedValues = model.GetEstimatedValues(ds, test);
        var outputValues = ds.GetDoubleValues(targetVariable, test);

        OnlineCalculatorError calculatorError;
        double mse = OnlineMeanSquaredErrorCalculator.Calculate(estimatedValues, outputValues, out calculatorError);
        if (calculatorError != OnlineCalculatorError.None)
          mse = double.NaN;
        avgTestMSE += mse;
      }

      avgTestMSE /= partitions.Count;
    }

    public static RFParameter GridSearch(IRegressionProblemData problemData, IEnumerable<IEnumerable<int>> folds, Dictionary<string, IEnumerable<double>> parameterRanges, int seed = 12345, int maxDegreeOfParallelism = 1) {
      DoubleValue mse = new DoubleValue(Double.MaxValue);
      RFParameter bestParameter = new RFParameter { n = 1, m = 0.1, r = 0.1 }; // some random defaults

      var pNames = parameterRanges.Keys.ToList();
      var pRanges = pNames.Select(x => parameterRanges[x]);
      var setters = pNames.Select(GenerateSetter).ToList();

      var crossProduct = pRanges.CartesianProduct();

      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, nuple => {
        var list = nuple.ToList();
        double testMSE;
        var parameters = new RFParameter();
        for (int i = 0; i < pNames.Count; ++i) {
          var s = setters[i];
          s(parameters, list[i]);
        }
        CrossValidate(problemData, folds, parameters, seed, out testMSE);
        if (testMSE < mse.Value) {
          lock (mse) {
            mse.Value = testMSE;
          }
          lock (bestParameter) {
            bestParameter = (RFParameter)parameters.Clone();
          }
        }
      });
      return bestParameter;
    }
  }
}
