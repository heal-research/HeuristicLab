#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("768AFEDB-5641-429E-85A1-A0BE8DFDD56F")]
  [Item("ClassificationSolution Impacts Calculator", "Calculation of the impacts of input variables for any classification solution")]
  public sealed class ClassificationSolutionVariableImpactsCalculator : ParameterizedNamedItem {
    #region Parameters/Properties
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
    private const string FactorReplacementParameterName = "Factor Replacement Method";
    private const string DataPartitionParameterName = "DataPartition";

    public IFixedValueParameter<EnumValue<ReplacementMethodEnum>> ReplacementParameter {
      get { return (IFixedValueParameter<EnumValue<ReplacementMethodEnum>>)Parameters[ReplacementParameterName]; }
    }
    public IFixedValueParameter<EnumValue<FactorReplacementMethodEnum>> FactorReplacementParameter {
      get { return (IFixedValueParameter<EnumValue<FactorReplacementMethodEnum>>)Parameters[FactorReplacementParameterName]; }
    }
    public IFixedValueParameter<EnumValue<DataPartitionEnum>> DataPartitionParameter {
      get { return (IFixedValueParameter<EnumValue<DataPartitionEnum>>)Parameters[DataPartitionParameterName]; }
    }

    public ReplacementMethodEnum ReplacementMethod {
      get { return ReplacementParameter.Value.Value; }
      set { ReplacementParameter.Value.Value = value; }
    }
    public FactorReplacementMethodEnum FactorReplacementMethod {
      get { return FactorReplacementParameter.Value.Value; }
      set { FactorReplacementParameter.Value.Value = value; }
    }
    public DataPartitionEnum DataPartition {
      get { return DataPartitionParameter.Value.Value; }
      set { DataPartitionParameter.Value.Value = value; }
    }
    #endregion

    #region Ctor/Cloner
    [StorableConstructor]
    private ClassificationSolutionVariableImpactsCalculator(StorableConstructorFlag _) : base(_) { }
    private ClassificationSolutionVariableImpactsCalculator(ClassificationSolutionVariableImpactsCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public ClassificationSolutionVariableImpactsCalculator()
      : base() {
      Parameters.Add(new FixedValueParameter<EnumValue<ReplacementMethodEnum>>(ReplacementParameterName, "The replacement method for variables during impact calculation.", new EnumValue<ReplacementMethodEnum>(ReplacementMethodEnum.Shuffle)));
      Parameters.Add(new FixedValueParameter<EnumValue<FactorReplacementMethodEnum>>(FactorReplacementParameterName, "The replacement method for factor variables during impact calculation.", new EnumValue<FactorReplacementMethodEnum>(FactorReplacementMethodEnum.Best)));
      Parameters.Add(new FixedValueParameter<EnumValue<DataPartitionEnum>>(DataPartitionParameterName, "The data partition on which the impacts are calculated.", new EnumValue<DataPartitionEnum>(DataPartitionEnum.Training)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ClassificationSolutionVariableImpactsCalculator(this, cloner);
    }
    #endregion

    //mkommend: annoying name clash with static method, open to better naming suggestions
    public IEnumerable<Tuple<string, double>> Calculate(IClassificationSolution solution) {
      return CalculateImpacts(solution, ReplacementMethod, FactorReplacementMethod, DataPartition);
    }

    public static IEnumerable<Tuple<string, double>> CalculateImpacts(
      IClassificationSolution solution,
      ReplacementMethodEnum replacementMethod = ReplacementMethodEnum.Shuffle,
      FactorReplacementMethodEnum factorReplacementMethod = FactorReplacementMethodEnum.Best,
      DataPartitionEnum dataPartition = DataPartitionEnum.Training) {

      IEnumerable<int> rows = GetPartitionRows(dataPartition, solution.ProblemData);
      IEnumerable<double> estimatedClassValues = solution.GetEstimatedClassValues(rows);
      var model = (IClassificationModel)solution.Model.Clone(); //mkommend: clone of model is necessary, because the thresholds for IDiscriminantClassificationModels are updated

      return CalculateImpacts(model, solution.ProblemData, estimatedClassValues, rows, replacementMethod, factorReplacementMethod);
    }

    public static IEnumerable<Tuple<string, double>> CalculateImpacts(
     IClassificationModel model,
     IClassificationProblemData problemData,
     IEnumerable<double> estimatedClassValues,
     IEnumerable<int> rows,
     ReplacementMethodEnum replacementMethod = ReplacementMethodEnum.Shuffle,
     FactorReplacementMethodEnum factorReplacementMethod = FactorReplacementMethodEnum.Best) {

      //fholzing: try and catch in case a different dataset is loaded, otherwise statement is neglectable
      var missingVariables = model.VariablesUsedForPrediction.Except(problemData.Dataset.VariableNames);
      if (missingVariables.Any()) {
        throw new InvalidOperationException(string.Format("Can not calculate variable impacts, because the model uses inputs missing in the dataset ({0})", string.Join(", ", missingVariables)));
      }
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var originalQuality = CalculateQuality(targetValues, estimatedClassValues);

      var impacts = new Dictionary<string, double>();
      var inputvariables = new HashSet<string>(problemData.AllowedInputVariables.Union(model.VariablesUsedForPrediction));
      var modifiableDataset = ((Dataset)(problemData.Dataset).Clone()).ToModifiable();

      foreach (var inputVariable in inputvariables) {
        impacts[inputVariable] = CalculateImpact(inputVariable, model, problemData, modifiableDataset, rows, replacementMethod, factorReplacementMethod, targetValues, originalQuality);
      }

      return impacts.Select(i => Tuple.Create(i.Key, i.Value));
    }

    public static double CalculateImpact(string variableName,
      IClassificationModel model,
      IClassificationProblemData problemData,
      ModifiableDataset modifiableDataset,
      IEnumerable<int> rows,
      ReplacementMethodEnum replacementMethod = ReplacementMethodEnum.Shuffle,
      FactorReplacementMethodEnum factorReplacementMethod = FactorReplacementMethodEnum.Best,
      IEnumerable<double> targetValues = null,
      double quality = double.NaN) {

      if (!model.VariablesUsedForPrediction.Contains(variableName)) { return 0.0; }
      if (!problemData.Dataset.VariableNames.Contains(variableName)) {
        throw new InvalidOperationException(string.Format("Can not calculate variable impact, because the model uses inputs missing in the dataset ({0})", variableName));
      }

      if (targetValues == null) {
        targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      }
      if (quality == double.NaN) {
        quality = CalculateQuality(model.GetEstimatedClassValues(modifiableDataset, rows), targetValues);
      }

      IList originalValues = null;
      IList replacementValues = GetReplacementValues(modifiableDataset, variableName, model, rows, targetValues, out originalValues, replacementMethod, factorReplacementMethod);

      double newValue = CalculateQualityForReplacement(model, modifiableDataset, variableName, originalValues, rows, replacementValues, targetValues);
      double impact = quality - newValue;

      return impact;
    }

    private static IList GetReplacementValues(ModifiableDataset modifiableDataset,
      string variableName,
      IClassificationModel model,
      IEnumerable<int> rows,
      IEnumerable<double> targetValues,
      out IList originalValues,
      ReplacementMethodEnum replacementMethod = ReplacementMethodEnum.Shuffle,
      FactorReplacementMethodEnum factorReplacementMethod = FactorReplacementMethodEnum.Best) {

      IList replacementValues = null;
      if (modifiableDataset.VariableHasType<double>(variableName)) {
        originalValues = modifiableDataset.GetReadOnlyDoubleValues(variableName).ToList();
        replacementValues = GetReplacementValuesForDouble(modifiableDataset, rows, (List<double>)originalValues, replacementMethod);
      } else if (modifiableDataset.VariableHasType<string>(variableName)) {
        originalValues = modifiableDataset.GetReadOnlyStringValues(variableName).ToList();
        replacementValues = GetReplacementValuesForString(model, modifiableDataset, variableName, rows, (List<string>)originalValues, targetValues, factorReplacementMethod);
      } else {
        throw new NotSupportedException("Variable not supported");
      }

      return replacementValues;
    }

    private static IList GetReplacementValuesForDouble(ModifiableDataset modifiableDataset,
      IEnumerable<int> rows,
      List<double> originalValues,
      ReplacementMethodEnum replacementMethod = ReplacementMethodEnum.Shuffle) {

      IRandom random = new FastRandom(31415);
      List<double> replacementValues;
      double replacementValue;

      switch (replacementMethod) {
        case ReplacementMethodEnum.Median:
          replacementValue = rows.Select(r => originalValues[r]).Median();
          replacementValues = Enumerable.Repeat(replacementValue, modifiableDataset.Rows).ToList();
          break;
        case ReplacementMethodEnum.Average:
          replacementValue = rows.Select(r => originalValues[r]).Average();
          replacementValues = Enumerable.Repeat(replacementValue, modifiableDataset.Rows).ToList();
          break;
        case ReplacementMethodEnum.Shuffle:
          // new var has same empirical distribution but the relation to y is broken
          // prepare a complete column for the dataset
          replacementValues = Enumerable.Repeat(double.NaN, modifiableDataset.Rows).ToList();
          // shuffle only the selected rows
          var shuffledValues = rows.Select(r => originalValues[r]).Shuffle(random).ToList();
          int i = 0;
          // update column values 
          foreach (var r in rows) {
            replacementValues[r] = shuffledValues[i++];
          }
          break;
        case ReplacementMethodEnum.Noise:
          var avg = rows.Select(r => originalValues[r]).Average();
          var stdDev = rows.Select(r => originalValues[r]).StandardDeviation();
          // prepare a complete column for the dataset
          replacementValues = Enumerable.Repeat(double.NaN, modifiableDataset.Rows).ToList();
          // update column values 
          foreach (var r in rows) {
            replacementValues[r] = NormalDistributedRandom.NextDouble(random, avg, stdDev);
          }
          break;

        default:
          throw new ArgumentException(string.Format("ReplacementMethod {0} cannot be handled.", replacementMethod));
      }

      return replacementValues;
    }

    private static IList GetReplacementValuesForString(IClassificationModel model,
      ModifiableDataset modifiableDataset,
      string variableName,
      IEnumerable<int> rows,
      List<string> originalValues,
      IEnumerable<double> targetValues,
      FactorReplacementMethodEnum factorReplacementMethod = FactorReplacementMethodEnum.Shuffle) {

      List<string> replacementValues = null;
      IRandom random = new FastRandom(31415);

      switch (factorReplacementMethod) {
        case FactorReplacementMethodEnum.Best:
          // try replacing with all possible values and find the best replacement value
          var bestQuality = double.NegativeInfinity;
          foreach (var repl in modifiableDataset.GetStringValues(variableName, rows).Distinct()) {
            List<string> curReplacementValues = Enumerable.Repeat(repl, modifiableDataset.Rows).ToList();
            //fholzing: this result could be used later on (theoretically), but is neglected for better readability/method consistency 
            var newValue = CalculateQualityForReplacement(model, modifiableDataset, variableName, originalValues, rows, curReplacementValues, targetValues);
            var curQuality = newValue;

            if (curQuality > bestQuality) {
              bestQuality = curQuality;
              replacementValues = curReplacementValues;
            }
          }
          break;
        case FactorReplacementMethodEnum.Mode:
          var mostCommonValue = rows.Select(r => originalValues[r])
            .GroupBy(v => v)
            .OrderByDescending(g => g.Count())
            .First().Key;
          replacementValues = Enumerable.Repeat(mostCommonValue, modifiableDataset.Rows).ToList();
          break;
        case FactorReplacementMethodEnum.Shuffle:
          // new var has same empirical distribution but the relation to y is broken
          // prepare a complete column for the dataset
          replacementValues = Enumerable.Repeat(string.Empty, modifiableDataset.Rows).ToList();
          // shuffle only the selected rows
          var shuffledValues = rows.Select(r => originalValues[r]).Shuffle(random).ToList();
          int i = 0;
          // update column values 
          foreach (var r in rows) {
            replacementValues[r] = shuffledValues[i++];
          }
          break;
        default:
          throw new ArgumentException(string.Format("FactorReplacementMethod {0} cannot be handled.", factorReplacementMethod));
      }

      return replacementValues;
    }

    private static double CalculateQualityForReplacement(
      IClassificationModel model,
      ModifiableDataset modifiableDataset,
      string variableName,
      IList originalValues,
      IEnumerable<int> rows,
      IList replacementValues,
      IEnumerable<double> targetValues) {

      modifiableDataset.ReplaceVariable(variableName, replacementValues);
      var discModel = model as IDiscriminantFunctionClassificationModel;
      if (discModel != null) {
        var problemData = new ClassificationProblemData(modifiableDataset, modifiableDataset.VariableNames, model.TargetVariable);
        discModel.RecalculateModelParameters(problemData, rows);
      }

      //mkommend: ToList is used on purpose to avoid lazy evaluation that could result in wrong estimates due to variable replacements
      var estimates = model.GetEstimatedClassValues(modifiableDataset, rows).ToList();
      var ret = CalculateQuality(targetValues, estimates);
      modifiableDataset.ReplaceVariable(variableName, originalValues);

      return ret;
    }

    public static double CalculateQuality(IEnumerable<double> targetValues, IEnumerable<double> estimatedClassValues) {
      OnlineCalculatorError errorState;
      var ret = OnlineAccuracyCalculator.Calculate(targetValues, estimatedClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) { throw new InvalidOperationException("Error during calculation with replaced inputs."); }
      return ret;
    }

    public static IEnumerable<int> GetPartitionRows(DataPartitionEnum dataPartition, IClassificationProblemData problemData) {
      IEnumerable<int> rows;

      switch (dataPartition) {
        case DataPartitionEnum.All:
          rows = problemData.AllIndices;
          break;
        case DataPartitionEnum.Test:
          rows = problemData.TestIndices;
          break;
        case DataPartitionEnum.Training:
          rows = problemData.TrainingIndices;
          break;
        default:
          throw new NotSupportedException("DataPartition not supported");
      }

      return rows;
    }
  }
}