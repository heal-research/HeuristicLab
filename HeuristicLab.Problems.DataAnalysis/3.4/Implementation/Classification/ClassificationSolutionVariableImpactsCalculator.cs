#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("ClassificationSolution Impacts Calculator", "Calculation of the impacts of input variables for any classification solution")]
  public sealed class ClassificationSolutionVariableImpactsCalculator : ParameterizedNamedItem {
    public enum ReplacementMethodEnum {
      Median,
      Average,
      Shuffle,
      Noise
    }
    public enum FactorReplacementMethodEnum {
      Best,
      Mode,
      Shuffle
    }
    public enum DataPartitionEnum {
      Training,
      Test,
      All
    }

    private const string ReplacementParameterName = "Replacement Method";
    private const string DataPartitionParameterName = "DataPartition";

    public IFixedValueParameter<EnumValue<ReplacementMethodEnum>> ReplacementParameter {
      get { return (IFixedValueParameter<EnumValue<ReplacementMethodEnum>>)Parameters[ReplacementParameterName]; }
    }
    public IFixedValueParameter<EnumValue<DataPartitionEnum>> DataPartitionParameter {
      get { return (IFixedValueParameter<EnumValue<DataPartitionEnum>>)Parameters[DataPartitionParameterName]; }
    }

    public ReplacementMethodEnum ReplacementMethod {
      get { return ReplacementParameter.Value.Value; }
      set { ReplacementParameter.Value.Value = value; }
    }
    public DataPartitionEnum DataPartition {
      get { return DataPartitionParameter.Value.Value; }
      set { DataPartitionParameter.Value.Value = value; }
    }


    [StorableConstructor]
    private ClassificationSolutionVariableImpactsCalculator(bool deserializing) : base(deserializing) { }
    private ClassificationSolutionVariableImpactsCalculator(ClassificationSolutionVariableImpactsCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ClassificationSolutionVariableImpactsCalculator(this, cloner);
    }

    public ClassificationSolutionVariableImpactsCalculator()
      : base() {
      Parameters.Add(new FixedValueParameter<EnumValue<ReplacementMethodEnum>>(ReplacementParameterName, "The replacement method for variables during impact calculation.", new EnumValue<ReplacementMethodEnum>(ReplacementMethodEnum.Median)));
      Parameters.Add(new FixedValueParameter<EnumValue<DataPartitionEnum>>(DataPartitionParameterName, "The data partition on which the impacts are calculated.", new EnumValue<DataPartitionEnum>(DataPartitionEnum.Training)));
    }

    //mkommend: annoying name clash with static method, open to better naming suggestions
    public IEnumerable<Tuple<string, double>> Calculate(IClassificationSolution solution) {
      return CalculateImpacts(solution, DataPartition, ReplacementMethod);
    }

    public static IEnumerable<Tuple<string, double>> CalculateImpacts(
      IClassificationSolution solution,
      DataPartitionEnum data = DataPartitionEnum.Training,
      ReplacementMethodEnum replacementMethod = ReplacementMethodEnum.Median,
      FactorReplacementMethodEnum factorReplacementMethod = FactorReplacementMethodEnum.Best) {

      var problemData = solution.ProblemData;
      var dataset = problemData.Dataset;
      var model = (IClassificationModel)solution.Model.Clone(); //mkommend: clone of model is necessary, because the thresholds for IDiscriminantClassificationModels are updated

      IEnumerable<int> rows;
      IEnumerable<double> targetValues;
      double originalAccuracy;

      OnlineCalculatorError error;

      switch (data) {
        case DataPartitionEnum.All:
          rows = problemData.AllIndices;
          targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.AllIndices).ToList();
          originalAccuracy = OnlineAccuracyCalculator.Calculate(targetValues, solution.EstimatedClassValues, out error);
          if (error != OnlineCalculatorError.None) throw new InvalidOperationException("Error during accuracy calculation.");
          break;
        case DataPartitionEnum.Training:
          rows = problemData.TrainingIndices;
          targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices).ToList();
          originalAccuracy = OnlineAccuracyCalculator.Calculate(targetValues, solution.EstimatedTrainingClassValues, out error);
          if (error != OnlineCalculatorError.None) throw new InvalidOperationException("Error during accuracy calculation.");
          break;
        case DataPartitionEnum.Test:
          rows = problemData.TestIndices;
          targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TestIndices).ToList();
          originalAccuracy = OnlineAccuracyCalculator.Calculate(targetValues, solution.EstimatedTestClassValues, out error);
          if (error != OnlineCalculatorError.None) throw new InvalidOperationException("Error during accuracy calculation.");
          break;
        default: throw new ArgumentException(string.Format("DataPartition {0} cannot be handled.", data));
      }

      var impacts = new Dictionary<string, double>();
      var modifiableDataset = ((Dataset)dataset).ToModifiable();

      var inputvariables = new HashSet<string>(problemData.AllowedInputVariables.Union(solution.Model.VariablesUsedForPrediction));
      var allowedInputVariables = dataset.VariableNames.Where(v => inputvariables.Contains(v)).ToList();

      // calculate impacts for double variables
      foreach (var inputVariable in allowedInputVariables.Where(problemData.Dataset.VariableHasType<double>)) {
        var newEstimates = EvaluateModelWithReplacedVariable(model, inputVariable, modifiableDataset, rows, replacementMethod);
        var newAccuracy = OnlineAccuracyCalculator.Calculate(targetValues, newEstimates, out error);
        if (error != OnlineCalculatorError.None) throw new InvalidOperationException("Error during R² calculation with replaced inputs.");

        impacts[inputVariable] = originalAccuracy - newAccuracy;
      }

      // calculate impacts for string variables
      foreach (var inputVariable in allowedInputVariables.Where(problemData.Dataset.VariableHasType<string>)) {
        if (factorReplacementMethod == FactorReplacementMethodEnum.Best) {
          // try replacing with all possible values and find the best replacement value
          var smallestImpact = double.PositiveInfinity;
          foreach (var repl in problemData.Dataset.GetStringValues(inputVariable, rows).Distinct()) {
            var newEstimates = EvaluateModelWithReplacedVariable(model, inputVariable, modifiableDataset, rows,
              Enumerable.Repeat(repl, dataset.Rows));
            var newAccuracy = OnlineAccuracyCalculator.Calculate(targetValues, newEstimates, out error);
            if (error != OnlineCalculatorError.None)
              throw new InvalidOperationException("Error during accuracy calculation with replaced inputs.");

            var impact = originalAccuracy - newAccuracy;
            if (impact < smallestImpact) smallestImpact = impact;
          }
          impacts[inputVariable] = smallestImpact;
        } else {
          // for replacement methods shuffle and mode
          // calculate impacts for factor variables

          var newEstimates = EvaluateModelWithReplacedVariable(model, inputVariable, modifiableDataset, rows,
            factorReplacementMethod);
          var newAccuracy = OnlineAccuracyCalculator.Calculate(targetValues, newEstimates, out error);
          if (error != OnlineCalculatorError.None)
            throw new InvalidOperationException("Error during accuracy calculation with replaced inputs.");

          impacts[inputVariable] = originalAccuracy - newAccuracy;
        }
      } // foreach
      return impacts.OrderByDescending(i => i.Value).Select(i => Tuple.Create(i.Key, i.Value));
    }

    private static IEnumerable<double> EvaluateModelWithReplacedVariable(IClassificationModel model, string variable, ModifiableDataset dataset, IEnumerable<int> rows, ReplacementMethodEnum replacement = ReplacementMethodEnum.Median) {
      var originalValues = dataset.GetReadOnlyDoubleValues(variable).ToList();
      double replacementValue;
      List<double> replacementValues;
      IRandom rand;

      switch (replacement) {
        case ReplacementMethodEnum.Median:
          replacementValue = rows.Select(r => originalValues[r]).Median();
          replacementValues = Enumerable.Repeat(replacementValue, dataset.Rows).ToList();
          break;
        case ReplacementMethodEnum.Average:
          replacementValue = rows.Select(r => originalValues[r]).Average();
          replacementValues = Enumerable.Repeat(replacementValue, dataset.Rows).ToList();
          break;
        case ReplacementMethodEnum.Shuffle:
          // new var has same empirical distribution but the relation to y is broken
          rand = new FastRandom(31415);
          // prepare a complete column for the dataset
          replacementValues = Enumerable.Repeat(double.NaN, dataset.Rows).ToList();
          // shuffle only the selected rows
          var shuffledValues = rows.Select(r => originalValues[r]).Shuffle(rand).ToList();
          int i = 0;
          // update column values 
          foreach (var r in rows) {
            replacementValues[r] = shuffledValues[i++];
          }
          break;
        case ReplacementMethodEnum.Noise:
          var avg = rows.Select(r => originalValues[r]).Average();
          var stdDev = rows.Select(r => originalValues[r]).StandardDeviation();
          rand = new FastRandom(31415);
          // prepare a complete column for the dataset
          replacementValues = Enumerable.Repeat(double.NaN, dataset.Rows).ToList();
          // update column values 
          foreach (var r in rows) {
            replacementValues[r] = NormalDistributedRandom.NextDouble(rand, avg, stdDev);
          }
          break;

        default:
          throw new ArgumentException(string.Format("ReplacementMethod {0} cannot be handled.", replacement));
      }

      return EvaluateModelWithReplacedVariable(model, variable, dataset, rows, replacementValues);
    }

    private static IEnumerable<double> EvaluateModelWithReplacedVariable(
      IClassificationModel model, string variable, ModifiableDataset dataset,
      IEnumerable<int> rows,
      FactorReplacementMethodEnum replacement = FactorReplacementMethodEnum.Shuffle) {
      var originalValues = dataset.GetReadOnlyStringValues(variable).ToList();
      List<string> replacementValues;
      IRandom rand;

      switch (replacement) {
        case FactorReplacementMethodEnum.Mode:
          var mostCommonValue = rows.Select(r => originalValues[r])
            .GroupBy(v => v)
            .OrderByDescending(g => g.Count())
            .First().Key;
          replacementValues = Enumerable.Repeat(mostCommonValue, dataset.Rows).ToList();
          break;
        case FactorReplacementMethodEnum.Shuffle:
          // new var has same empirical distribution but the relation to y is broken
          rand = new FastRandom(31415);
          // prepare a complete column for the dataset
          replacementValues = Enumerable.Repeat(string.Empty, dataset.Rows).ToList();
          // shuffle only the selected rows
          var shuffledValues = rows.Select(r => originalValues[r]).Shuffle(rand).ToList();
          int i = 0;
          // update column values 
          foreach (var r in rows) {
            replacementValues[r] = shuffledValues[i++];
          }
          break;
        default:
          throw new ArgumentException(string.Format("FactorReplacementMethod {0} cannot be handled.", replacement));
      }

      return EvaluateModelWithReplacedVariable(model, variable, dataset, rows, replacementValues);
    }

    private static IEnumerable<double> EvaluateModelWithReplacedVariable(IClassificationModel model, string variable,
      ModifiableDataset dataset, IEnumerable<int> rows, IEnumerable<double> replacementValues) {
      var originalValues = dataset.GetReadOnlyDoubleValues(variable).ToList();
      dataset.ReplaceVariable(variable, replacementValues.ToList());

      var discModel = model as IDiscriminantFunctionClassificationModel;
      if (discModel != null) {
        var problemData = new ClassificationProblemData(dataset, dataset.VariableNames, model.TargetVariable);
        discModel.RecalculateModelParameters(problemData, rows);
      }

      //mkommend: ToList is used on purpose to avoid lazy evaluation that could result in wrong estimates due to variable replacements
      var estimates = model.GetEstimatedClassValues(dataset, rows).ToList();
      dataset.ReplaceVariable(variable, originalValues);

      return estimates;
    }
    private static IEnumerable<double> EvaluateModelWithReplacedVariable(IClassificationModel model, string variable,
      ModifiableDataset dataset, IEnumerable<int> rows, IEnumerable<string> replacementValues) {
      var originalValues = dataset.GetReadOnlyStringValues(variable).ToList();
      dataset.ReplaceVariable(variable, replacementValues.ToList());


      var discModel = model as IDiscriminantFunctionClassificationModel;
      if (discModel != null) {
        var problemData = new ClassificationProblemData(dataset, dataset.VariableNames, model.TargetVariable);
        discModel.RecalculateModelParameters(problemData, rows);
      }

      //mkommend: ToList is used on purpose to avoid lazy evaluation that could result in wrong estimates due to variable replacements
      var estimates = model.GetEstimatedClassValues(dataset, rows).ToList();
      dataset.ReplaceVariable(variable, originalValues);

      return estimates;
    }
  }
}
