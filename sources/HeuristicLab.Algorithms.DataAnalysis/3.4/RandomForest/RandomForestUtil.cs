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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("RFParameter", "A random forest parameter collection")]
  [StorableClass]
  public class RFParameter : ParameterCollection {
    public RFParameter() {
      base.Add(new FixedValueParameter<IntValue>("N", "The number of random forest trees", new IntValue(50)));
      base.Add(new FixedValueParameter<DoubleValue>("M", "The ratio of features that will be used in the construction of individual trees (0<m<=1)", new DoubleValue(0.1)));
      base.Add(new FixedValueParameter<DoubleValue>("R", "The ratio of the training set that will be used in the construction of individual trees (0<r<=1)", new DoubleValue(0.1)));
    }

    [StorableConstructor]
    protected RFParameter(bool deserializing)
      : base(deserializing) {
    }

    protected RFParameter(RFParameter original, Cloner cloner)
      : base(original, cloner) {
      this.N = original.N;
      this.R = original.R;
      this.M = original.M;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RFParameter(this, cloner);
    }

    private IFixedValueParameter<IntValue> NParameter {
      get { return (IFixedValueParameter<IntValue>)base["N"]; }
    }

    private IFixedValueParameter<DoubleValue> RParameter {
      get { return (IFixedValueParameter<DoubleValue>)base["R"]; }
    }

    private IFixedValueParameter<DoubleValue> MParameter {
      get { return (IFixedValueParameter<DoubleValue>)base["M"]; }
    }

    public int N {
      get { return NParameter.Value.Value; }
      set { NParameter.Value.Value = value; }
    }

    public double R {
      get { return RParameter.Value.Value; }
      set { RParameter.Value.Value = value; }
    }

    public double M {
      get { return MParameter.Value.Value; }
      set { MParameter.Value.Value = value; }
    }
  }

  public static class RandomForestUtil {
    private static readonly object locker = new object();

    private static void CrossValidate(IRegressionProblemData problemData, Tuple<IEnumerable<int>, IEnumerable<int>>[] partitions, int nTrees, double r, double m, int seed, out double avgTestMse) {
      avgTestMse = 0;
      var ds = problemData.Dataset;
      var targetVariable = GetTargetVariableName(problemData);
      foreach (var tuple in partitions) {
        double rmsError, avgRelError, outOfBagAvgRelError, outOfBagRmsError;
        var trainingRandomForestPartition = tuple.Item1;
        var testRandomForestPartition = tuple.Item2;
        var model = RandomForestModel.CreateRegressionModel(problemData, trainingRandomForestPartition, nTrees, r, m, seed, out rmsError, out avgRelError, out outOfBagRmsError, out outOfBagAvgRelError);
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
        var model = RandomForestModel.CreateClassificationModel(problemData, trainingRandomForestPartition, nTrees, r, m, seed, out rmsError, out avgRelError, out outOfBagRmsError, out outOfBagAvgRelError);
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

    // grid search without cross-validation since in the case of random forests, the out-of-bag estimate is unbiased
    public static RFParameter GridSearch(IRegressionProblemData problemData, Dictionary<string, IEnumerable<double>> parameterRanges, int seed = 12345, int maxDegreeOfParallelism = 1) {
      var setters = parameterRanges.Keys.Select(GenerateSetter).ToList();
      var crossProduct = parameterRanges.Values.CartesianProduct();
      double bestOutOfBagRmsError = double.MaxValue;
      RFParameter bestParameters = new RFParameter();

      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, parameterCombination => {
        var parameterValues = parameterCombination.ToList();
        var parameters = new RFParameter();
        for (int i = 0; i < setters.Count; ++i) { setters[i](parameters, parameterValues[i]); }
        double rmsError, outOfBagRmsError, avgRelError, outOfBagAvgRelError;
        RandomForestModel.CreateRegressionModel(problemData, problemData.TrainingIndices, parameters.N, parameters.R, parameters.M, seed, out rmsError, out outOfBagRmsError, out avgRelError, out outOfBagAvgRelError);

        lock (locker) {
          if (bestOutOfBagRmsError > outOfBagRmsError) {
            bestOutOfBagRmsError = outOfBagRmsError;
            bestParameters = (RFParameter)parameters.Clone();
          }
        }
      });
      return bestParameters;
    }

    public static RFParameter GridSearch(IClassificationProblemData problemData, Dictionary<string, IEnumerable<double>> parameterRanges, int seed = 12345, int maxDegreeOfParallelism = 1) {
      var setters = parameterRanges.Keys.Select(GenerateSetter).ToList();
      var crossProduct = parameterRanges.Values.CartesianProduct();

      double bestOutOfBagRmsError = double.MaxValue;
      RFParameter bestParameters = new RFParameter();

      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, parameterCombination => {
        var parameterValues = parameterCombination.ToList();
        var parameters = new RFParameter();
        for (int i = 0; i < setters.Count; ++i) { setters[i](parameters, parameterValues[i]); }
        double rmsError, outOfBagRmsError, avgRelError, outOfBagAvgRelError;
        RandomForestModel.CreateClassificationModel(problemData, problemData.TrainingIndices, parameters.N, parameters.R, parameters.M, seed,
                                                                out rmsError, out outOfBagRmsError, out avgRelError, out outOfBagAvgRelError);

        lock (locker) {
          if (bestOutOfBagRmsError > outOfBagRmsError) {
            bestOutOfBagRmsError = outOfBagRmsError;
            bestParameters = (RFParameter)parameters.Clone();
          }
        }
      });
      return bestParameters;
    }

    public static RFParameter GridSearch(IRegressionProblemData problemData, int numberOfFolds, bool shuffleFolds, Dictionary<string, IEnumerable<double>> parameterRanges, int seed = 12345, int maxDegreeOfParallelism = 1) {
      DoubleValue mse = new DoubleValue(Double.MaxValue);
      RFParameter bestParameter = new RFParameter();

      var setters = parameterRanges.Keys.Select(GenerateSetter).ToList();
      var partitions = GenerateRandomForestPartitions(problemData, numberOfFolds);
      var crossProduct = parameterRanges.Values.CartesianProduct();

      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, parameterCombination => {
        var parameterValues = parameterCombination.ToList();
        double testMSE;
        var parameters = new RFParameter();
        for (int i = 0; i < setters.Count; ++i) {
          setters[i](parameters, parameterValues[i]);
        }
        CrossValidate(problemData, partitions, parameters.N, parameters.R, parameters.M, seed, out testMSE);

        lock (locker) {
          if (testMSE < mse.Value) {
            mse.Value = testMSE;
            bestParameter = (RFParameter)parameters.Clone();
          }
        }
      });
      return bestParameter;
    }

    public static RFParameter GridSearch(IClassificationProblemData problemData, int numberOfFolds, bool shuffleFolds, Dictionary<string, IEnumerable<double>> parameterRanges, int seed = 12345, int maxDegreeOfParallelism = 1) {
      DoubleValue accuracy = new DoubleValue(0);
      RFParameter bestParameter = new RFParameter();

      var setters = parameterRanges.Keys.Select(GenerateSetter).ToList();
      var crossProduct = parameterRanges.Values.CartesianProduct();
      var partitions = GenerateRandomForestPartitions(problemData, numberOfFolds, shuffleFolds);

      Parallel.ForEach(crossProduct, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, parameterCombination => {
        var parameterValues = parameterCombination.ToList();
        double testAccuracy;
        var parameters = new RFParameter();
        for (int i = 0; i < setters.Count; ++i) {
          setters[i](parameters, parameterValues[i]);
        }
        CrossValidate(problemData, partitions, parameters.N, parameters.R, parameters.M, seed, out testAccuracy);

        lock (locker) {
          if (testAccuracy > accuracy.Value) {
            accuracy.Value = testAccuracy;
            bestParameter = (RFParameter)parameters.Clone();
          }
        }
      });
      return bestParameter;
    }

    private static Tuple<IEnumerable<int>, IEnumerable<int>>[] GenerateRandomForestPartitions(IDataAnalysisProblemData problemData, int numberOfFolds, bool shuffleFolds = false) {
      var folds = GenerateFolds(problemData, numberOfFolds, shuffleFolds).ToList();
      var partitions = new Tuple<IEnumerable<int>, IEnumerable<int>>[numberOfFolds];

      for (int i = 0; i < numberOfFolds; ++i) {
        int p = i; // avoid "access to modified closure" warning
        var trainingRows = folds.SelectMany((par, j) => j != p ? par : Enumerable.Empty<int>());
        var testRows = folds[i];
        partitions[i] = new Tuple<IEnumerable<int>, IEnumerable<int>>(trainingRows, testRows);
      }
      return partitions;
    }

    public static IEnumerable<IEnumerable<int>> GenerateFolds(IDataAnalysisProblemData problemData, int numberOfFolds, bool shuffleFolds = false) {
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

    private static Action<RFParameter, double> GenerateSetter(string field) {
      var targetExp = Expression.Parameter(typeof(RFParameter));
      var valueExp = Expression.Parameter(typeof(double));
      var fieldExp = Expression.Property(targetExp, field);
      var assignExp = Expression.Assign(fieldExp, Expression.Convert(valueExp, fieldExp.Type));
      var setter = Expression.Lambda<Action<RFParameter, double>>(assignExp, targetExp, valueExp).Compile();
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
