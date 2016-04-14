#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("RegressionSolution Impacts Calculator", "Calculation of the impacts of input variables for a concrete ")]
  public sealed class RegressionSolutionVariableImpactsCalculator : ParameterizedNamedItem {
    public enum ReplacementMethodEnum {
      Median,
      Average
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
    private RegressionSolutionVariableImpactsCalculator(bool deserializing) : base(deserializing) { }
    private RegressionSolutionVariableImpactsCalculator(RegressionSolutionVariableImpactsCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionSolutionVariableImpactsCalculator(this, cloner);
    }

    public RegressionSolutionVariableImpactsCalculator()
      : base() {
      Parameters.Add(new FixedValueParameter<EnumValue<ReplacementMethodEnum>>(ReplacementParameterName, "The replacement method for variables during impact calculation.", new EnumValue<ReplacementMethodEnum>(ReplacementMethodEnum.Median)));
      Parameters.Add(new FixedValueParameter<EnumValue<DataPartitionEnum>>(ReplacementParameterName, "The data partition on which the impacts are calculated.", new EnumValue<DataPartitionEnum>(DataPartitionEnum.Training)));
    }

    //mkommend: annoying name clash with static method, open to better naming suggestions
    public IEnumerable<Tuple<string, double>> Calculate(IRegressionSolution solution) {
      return CalculateImpacts(solution, DataPartition, ReplacementMethod);
    }

    public static IEnumerable<Tuple<string, double>> CalculateImpacts(IRegressionSolution solution,
      DataPartitionEnum data = DataPartitionEnum.Training,
      ReplacementMethodEnum replacement = ReplacementMethodEnum.Median) {

      var problemData = solution.ProblemData;
      var dataset = problemData.Dataset;

      IEnumerable<int> rows;
      IEnumerable<double> targetValues;
      double originalR2 = -1;

      OnlineCalculatorError error;

      switch (data) {
        case DataPartitionEnum.All:
          rows = solution.ProblemData.AllIndices;
          targetValues = problemData.TargetVariableValues.ToList();
          originalR2 = OnlinePearsonsRCalculator.Calculate(problemData.TargetVariableValues, solution.EstimatedValues, out error);
          if (error != OnlineCalculatorError.None) throw new InvalidOperationException("Error during R² calculation.");
          originalR2 = originalR2 * originalR2;
          break;
        case DataPartitionEnum.Training:
          rows = problemData.TrainingIndices;
          targetValues = problemData.TargetVariableTrainingValues.ToList();
          originalR2 = solution.TrainingRSquared;
          break;
        case DataPartitionEnum.Test:
          rows = problemData.TestIndices;
          targetValues = problemData.TargetVariableTestValues.ToList();
          originalR2 = solution.TestRSquared;
          break;
        default: throw new ArgumentException(string.Format("DataPartition {0} cannot be handled.", data));
      }


      var impacts = new Dictionary<string, double>();
      var modifiableDataset = ((Dataset)dataset).ToModifiable();

      foreach (var inputVariable in problemData.AllowedInputVariables) {
        var newEstimates = EvaluateModelWithReplacedVariable(solution.Model, inputVariable, modifiableDataset, rows, replacement);
        var newR2 = OnlinePearsonsRCalculator.Calculate(targetValues, newEstimates, out error);
        if (error != OnlineCalculatorError.None) throw new InvalidOperationException("Error during R² calculation with replaced inputs.");

        newR2 = newR2 * newR2;
        var impact = originalR2 - newR2;
        impacts[inputVariable] = impact;
      }
      return impacts.OrderByDescending(i => i.Value).Select(i => Tuple.Create(i.Key, i.Value));
    }

    private static IEnumerable<double> EvaluateModelWithReplacedVariable(IRegressionModel model, string variable, ModifiableDataset dataset, IEnumerable<int> rows, ReplacementMethodEnum replacement = ReplacementMethodEnum.Median) {
      var originalValues = dataset.GetReadOnlyDoubleValues(variable).ToList();
      double replacementValue;

      switch (replacement) {
        case ReplacementMethodEnum.Median:
          replacementValue = rows.Select(r => originalValues[r]).Median();
          break;
        case ReplacementMethodEnum.Average:
          replacementValue = rows.Select(r => originalValues[r]).Average();
          break;
        default:
          throw new ArgumentException(string.Format("ReplacementMethod {0} cannot be handled.", replacement));
      }

      dataset.ReplaceVariable(variable, Enumerable.Repeat(replacementValue, dataset.Rows).ToList());
      //mkommend: ToList is used on purpose to avoid lazy evaluation that could result in wrong estimates due to variable replacements
      var estimates = model.GetEstimatedValues(dataset, rows).ToList();
      dataset.ReplaceVariable(variable, originalValues);

      return estimates;
    }
  }
}
