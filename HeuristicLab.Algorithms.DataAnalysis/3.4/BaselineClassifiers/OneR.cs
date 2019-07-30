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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// 1R classification algorithm.
  /// </summary>
  [Item("OneR Classification", "A simple classification algorithm the searches the best single-variable split (does not support categorical features correctly). See R.C. Holte (1993). Very simple classification rules perform well on most commonly used datasets. Machine Learning. 11:63-91.")]
  [StorableType("22D1C518-CEDA-413C-8997-D34BC06B6267")]
  public sealed class OneR : FixedDataAnalysisAlgorithm<IClassificationProblem> {

    public IValueParameter<IntValue> MinBucketSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["MinBucketSize"]; }
    }

    [StorableConstructor]
    private OneR(StorableConstructorFlag _) : base(_) { }

    private OneR(OneR original, Cloner cloner)
      : base(original, cloner) { }

    public OneR()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("MinBucketSize", "Minimum size of a bucket for numerical values. (Except for the rightmost bucket)", new IntValue(6)));
      Problem = new ClassificationProblem();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneR(this, cloner);
    }

    protected override void Run(CancellationToken cancellationToken) {
      var solution = CreateOneRSolution(Problem.ProblemData, MinBucketSizeParameter.Value.Value);
      Results.Add(new Result("OneR solution", "The 1R classifier.", solution));
    }

    public static IClassificationSolution CreateOneRSolution(IClassificationProblemData problemData, int minBucketSize = 6) {
      var classValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices);
      var model1 = FindBestDoubleVariableModel(problemData, minBucketSize);
      var model2 = FindBestFactorModel(problemData);

      if (model1 == null && model2 == null) throw new InvalidProgramException("Could not create OneR solution");
      else if (model1 == null) return new OneFactorClassificationSolution(model2, (IClassificationProblemData)problemData.Clone());
      else if (model2 == null) return new OneRClassificationSolution(model1, (IClassificationProblemData)problemData.Clone());
      else {
        var model1EstimatedValues = model1.GetEstimatedClassValues(problemData.Dataset, problemData.TrainingIndices);
        var model1NumCorrect = classValues.Zip(model1EstimatedValues, (a, b) => a.IsAlmost(b)).Count(e => e);

        var model2EstimatedValues = model2.GetEstimatedClassValues(problemData.Dataset, problemData.TrainingIndices);
        var model2NumCorrect = classValues.Zip(model2EstimatedValues, (a, b) => a.IsAlmost(b)).Count(e => e);

        if (model1NumCorrect > model2NumCorrect) {
          return new OneRClassificationSolution(model1, (IClassificationProblemData)problemData.Clone());
        } else {
          return new OneFactorClassificationSolution(model2, (IClassificationProblemData)problemData.Clone());
        }
      }
    }

    private static OneRClassificationModel FindBestDoubleVariableModel(IClassificationProblemData problemData, int minBucketSize = 6) {
      var bestClassified = 0;
      List<Split> bestSplits = null;
      string bestVariable = string.Empty;
      double bestMissingValuesClass = double.NaN;
      var classValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices);

      var allowedInputVariables = problemData.AllowedInputVariables.Where(problemData.Dataset.VariableHasType<double>);

      if (!allowedInputVariables.Any()) return null;

      foreach (var variable in allowedInputVariables) {
        var inputValues = problemData.Dataset.GetDoubleValues(variable, problemData.TrainingIndices);
        var samples = inputValues.Zip(classValues, (i, v) => new Sample(i, v)).OrderBy(s => s.inputValue);

        var missingValuesDistribution = samples
          .Where(s => double.IsNaN(s.inputValue)).GroupBy(s => s.classValue)
          .ToDictionary(s => s.Key, s => s.Count())
          .MaxItems(s => s.Value)
          .FirstOrDefault();

        //calculate class distributions for all distinct inputValues
        List<Dictionary<double, int>> classDistributions = new List<Dictionary<double, int>>();
        List<double> thresholds = new List<double>();
        double lastValue = double.NaN;
        foreach (var sample in samples.Where(s => !double.IsNaN(s.inputValue))) {
          if (sample.inputValue > lastValue || double.IsNaN(lastValue)) {
            if (!double.IsNaN(lastValue)) thresholds.Add((lastValue + sample.inputValue) / 2);
            lastValue = sample.inputValue;
            classDistributions.Add(new Dictionary<double, int>());
            foreach (var classValue in problemData.ClassValues)
              classDistributions[classDistributions.Count - 1][classValue] = 0;

          }
          classDistributions[classDistributions.Count - 1][sample.classValue]++;
        }
        thresholds.Add(double.PositiveInfinity);

        var distribution = classDistributions[0];
        var threshold = thresholds[0];
        var splits = new List<Split>();

        for (int i = 1; i < classDistributions.Count; i++) {
          var samplesInSplit = distribution.Max(d => d.Value);
          //join splits if there are too few samples in the split or the distributions has the same maximum class value as the current split
          if (samplesInSplit < minBucketSize ||
            classDistributions[i].MaxItems(d => d.Value).Select(d => d.Key).Contains(
              distribution.MaxItems(d => d.Value).Select(d => d.Key).First())) {
            foreach (var classValue in classDistributions[i])
              distribution[classValue.Key] += classValue.Value;
            threshold = thresholds[i];
          } else {
            splits.Add(new Split(threshold, distribution.MaxItems(d => d.Value).Select(d => d.Key).First()));
            distribution = classDistributions[i];
            threshold = thresholds[i];
          }
        }
        splits.Add(new Split(double.PositiveInfinity, distribution.MaxItems(d => d.Value).Select(d => d.Key).First()));

        int correctClassified = 0;
        int splitIndex = 0;
        foreach (var sample in samples.Where(s => !double.IsNaN(s.inputValue))) {
          while (sample.inputValue >= splits[splitIndex].thresholdValue)
            splitIndex++;
          correctClassified += sample.classValue.IsAlmost(splits[splitIndex].classValue) ? 1 : 0;
        }
        correctClassified += missingValuesDistribution.Value;

        if (correctClassified > bestClassified) {
          bestClassified = correctClassified;
          bestSplits = splits;
          bestVariable = variable;
          bestMissingValuesClass = missingValuesDistribution.Value == 0 ? double.NaN : missingValuesDistribution.Key;
        }
      }

      //remove neighboring splits with the same class value
      for (int i = 0; i < bestSplits.Count - 1; i++) {
        if (bestSplits[i].classValue.IsAlmost(bestSplits[i + 1].classValue)) {
          bestSplits.Remove(bestSplits[i]);
          i--;
        }
      }

      var model = new OneRClassificationModel(problemData.TargetVariable, bestVariable,
        bestSplits.Select(s => s.thresholdValue).ToArray(),
        bestSplits.Select(s => s.classValue).ToArray(), bestMissingValuesClass);

      return model;
    }
    private static OneFactorClassificationModel FindBestFactorModel(IClassificationProblemData problemData) {
      var classValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices);
      var defaultClass = FindMostFrequentClassValue(classValues);
      // only select string variables
      var allowedInputVariables = problemData.AllowedInputVariables.Where(problemData.Dataset.VariableHasType<string>);

      if (!allowedInputVariables.Any()) return null;

      OneFactorClassificationModel bestModel = null;
      var bestModelNumCorrect = 0;

      foreach (var variable in allowedInputVariables) {
        var variableValues = problemData.Dataset.GetStringValues(variable, problemData.TrainingIndices);
        var groupedClassValues = variableValues
          .Zip(classValues, (v, c) => new KeyValuePair<string, double>(v, c))
          .GroupBy(kvp => kvp.Key)
          .ToDictionary(g => g.Key, g => FindMostFrequentClassValue(g.Select(kvp => kvp.Value)));

        var model = new OneFactorClassificationModel(problemData.TargetVariable, variable,
          groupedClassValues.Select(kvp => kvp.Key).ToArray(), groupedClassValues.Select(kvp => kvp.Value).ToArray(), defaultClass);

        var modelEstimatedValues = model.GetEstimatedClassValues(problemData.Dataset, problemData.TrainingIndices);
        var modelNumCorrect = classValues.Zip(modelEstimatedValues, (a, b) => a.IsAlmost(b)).Count(e => e);
        if (modelNumCorrect > bestModelNumCorrect) {
          bestModelNumCorrect = modelNumCorrect;
          bestModel = model;
        }
      }

      return bestModel;
    }

    private static double FindMostFrequentClassValue(IEnumerable<double> classValues) {
      return classValues.GroupBy(c => c).OrderByDescending(g => g.Count()).Select(g => g.Key).First();
    }

    #region helper classes
    private class Split {
      public double thresholdValue;
      public double classValue;

      public Split(double thresholdValue, double classValue) {
        this.thresholdValue = thresholdValue;
        this.classValue = classValue;
      }
    }

    private class Sample {
      public double inputValue;
      public double classValue;

      public Sample(double inputValue, double classValue) {
        this.inputValue = inputValue;
        this.classValue = classValue;
      }
    }
    #endregion
  }
}
