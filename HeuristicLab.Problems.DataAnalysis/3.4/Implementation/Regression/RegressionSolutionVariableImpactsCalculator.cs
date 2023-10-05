﻿#region License Information

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("414B25CD-6643-4E42-9EB2-B9A24F5E1528")]
  [Item("RegressionSolution Impacts Calculator", "Calculation of the impacts of input variables for any regression solution")]
  public sealed class RegressionSolutionVariableImpactsCalculator : ParameterizedNamedItem {
    #region Parameters/Properties
    [StorableType("45a48ef7-e1e6-44b7-95b1-ae9d01aa5de4")]
    public enum ReplacementMethodEnum {
      Median,
      Average,
      Shuffle,
      Noise
    }

    [StorableType("78df33f8-4715-4d25-a69a-f2bc1277fa3b")]
    public enum FactorReplacementMethodEnum {
      Best,
      Mode,
      Shuffle
    }

    [StorableType("946646da-1c0b-435e-88f9-38d649fc5194")]
    public enum DataPartitionEnum {
      Training,
      Test,
      All
    }

    private const string ReplacementParameterName = "Replacement Method";
    private const string FactorReplacementParameterName = "Factor Replacement Method";
    private const string DataPartitionParameterName = "DataPartition";
    private const string RepetitionsParameterName = "Repetitions";
    
    public IFixedValueParameter<EnumValue<ReplacementMethodEnum>> ReplacementParameter {
      get { return (IFixedValueParameter<EnumValue<ReplacementMethodEnum>>)Parameters[ReplacementParameterName]; }
    }
    public IFixedValueParameter<EnumValue<FactorReplacementMethodEnum>> FactorReplacementParameter {
      get { return (IFixedValueParameter<EnumValue<FactorReplacementMethodEnum>>)Parameters[FactorReplacementParameterName]; }
    }
    public IFixedValueParameter<EnumValue<DataPartitionEnum>> DataPartitionParameter {
      get { return (IFixedValueParameter<EnumValue<DataPartitionEnum>>)Parameters[DataPartitionParameterName]; }
    }
    public IFixedValueParameter<IntValue> RepetitionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[RepetitionsParameterName]; }
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
    public int Repetitions {
      get { return RepetitionsParameter.Value.Value; }
      set { RepetitionsParameter.Value.Value = value; }
    }
    #endregion

    #region Ctor/Cloner
    [StorableConstructor]
    private RegressionSolutionVariableImpactsCalculator(StorableConstructorFlag _) : base(_) { }
    private RegressionSolutionVariableImpactsCalculator(RegressionSolutionVariableImpactsCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public RegressionSolutionVariableImpactsCalculator()
      : base() {
      Parameters.Add(new FixedValueParameter<EnumValue<ReplacementMethodEnum>>(ReplacementParameterName, "The replacement method for variables during impact calculation.", new EnumValue<ReplacementMethodEnum>(ReplacementMethodEnum.Shuffle)));
      Parameters.Add(new FixedValueParameter<EnumValue<FactorReplacementMethodEnum>>(FactorReplacementParameterName, "The replacement method for factor variables during impact calculation.", new EnumValue<FactorReplacementMethodEnum>(FactorReplacementMethodEnum.Best)));
      Parameters.Add(new FixedValueParameter<EnumValue<DataPartitionEnum>>(DataPartitionParameterName, "The data partition on which the impacts are calculated.", new EnumValue<DataPartitionEnum>(DataPartitionEnum.Training)));
      Parameters.Add(new FixedValueParameter<IntValue>(RepetitionsParameterName, "The number of repetitions for the impact calculation.", new IntValue(1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionSolutionVariableImpactsCalculator(this, cloner);
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(RepetitionsParameterName))
        Parameters.Add(new FixedValueParameter<IntValue>(RepetitionsParameterName, "The number of repetitions for the impact calculation.", new IntValue(1)));
    }
    #endregion

    //mkommend: annoying name clash with static method, open to better naming suggestions
    public IEnumerable<Tuple<string, double>> Calculate(IRegressionSolution solution) {
      return CalculateImpacts(solution, ReplacementMethod, FactorReplacementMethod, DataPartition, Repetitions);
    }

    public static IEnumerable<Tuple<string, double>> CalculateImpacts(
      IRegressionSolution solution,
      ReplacementMethodEnum replacementMethod = ReplacementMethodEnum.Shuffle,
      FactorReplacementMethodEnum factorReplacementMethod = FactorReplacementMethodEnum.Best,
      DataPartitionEnum dataPartition = DataPartitionEnum.Training, 
      int repetitions = 1) {

      IEnumerable<int> rows = GetPartitionRows(dataPartition, solution.ProblemData);
      IEnumerable<double> estimatedValues = solution.GetEstimatedValues(rows);
      return CalculateImpacts(solution.Model, solution.ProblemData, estimatedValues, rows, replacementMethod, factorReplacementMethod, repetitions);
    }

    public static IEnumerable<Tuple<string, double>> CalculateImpacts(
     IRegressionModel model,
     IRegressionProblemData problemData,
     IEnumerable<double> estimatedValues,
     IEnumerable<int> rows,
     ReplacementMethodEnum replacementMethod = ReplacementMethodEnum.Shuffle,
     FactorReplacementMethodEnum factorReplacementMethod = FactorReplacementMethodEnum.Best,
     int repetitions = 1) {

      //fholzing: try and catch in case a different dataset is loaded, otherwise statement is neglectable
      var missingVariables = model.VariablesUsedForPrediction.Except(problemData.Dataset.VariableNames);
      if (missingVariables.Any()) {
        throw new InvalidOperationException(string.Format("Can not calculate variable impacts, because the model uses inputs missing in the dataset ({0})", string.Join(", ", missingVariables)));
      }
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var originalQuality = CalculateQuality(targetValues, estimatedValues);

      var impacts = new Dictionary<string, double>();
      var inputvariables = new HashSet<string>(problemData.AllowedInputVariables.Union(model.VariablesUsedForPrediction));
      var modifiableDataset = ((Dataset)(problemData.Dataset).Clone()).ToModifiable();
      
      for (int repetition = 0; repetition < repetitions; repetition++) {
        foreach (var inputVariable in inputvariables) {
          var impact = CalculateImpact(inputVariable, model, problemData, modifiableDataset, rows, replacementMethod, factorReplacementMethod, targetValues, originalQuality);
          if (!impacts.ContainsKey(inputVariable)) impacts.Add(inputVariable, 0.0);
          impacts[inputVariable] += impact / repetitions;
        }
      }

      return impacts.Select(i => Tuple.Create(i.Key, i.Value));
    }

    public static double CalculateImpact(string variableName,
      IRegressionModel model,
      IRegressionProblemData problemData,
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
        quality = CalculateQuality(model.GetEstimatedValues(modifiableDataset, rows), targetValues);
      }

      IList originalValues = null;
      IList replacementValues = GetReplacementValues(modifiableDataset, variableName, model, rows, targetValues, out originalValues, replacementMethod, factorReplacementMethod);

      double newValue = CalculateQualityForReplacement(model, modifiableDataset, variableName, originalValues, rows, replacementValues, targetValues);
      double impact = quality - newValue;

      return impact;
    }

    private static IList GetReplacementValues(ModifiableDataset modifiableDataset,
      string variableName,
      IRegressionModel model,
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

    private static IList GetReplacementValuesForString(IRegressionModel model,
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
      IRegressionModel model,
      ModifiableDataset modifiableDataset,
      string variableName,
      IList originalValues,
      IEnumerable<int> rows,
      IList replacementValues,
      IEnumerable<double> targetValues) {

      modifiableDataset.ReplaceVariable(variableName, replacementValues);
      //mkommend: ToList is used on purpose to avoid lazy evaluation that could result in wrong estimates due to variable replacements
      var estimates = model.GetEstimatedValues(modifiableDataset, rows).ToList();
      var ret = CalculateQuality(targetValues, estimates);
      modifiableDataset.ReplaceVariable(variableName, originalValues);

      return ret;
    }

    public static double CalculateQuality(IEnumerable<double> targetValues, IEnumerable<double> estimatedValues) {
      OnlineCalculatorError errorState;
      var ret = OnlinePearsonsRCalculator.Calculate(targetValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) { throw new InvalidOperationException("Error during calculation with replaced inputs."); }
      return ret * ret;
    }

    public static IEnumerable<int> GetPartitionRows(DataPartitionEnum dataPartition, IRegressionProblemData problemData) {
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
