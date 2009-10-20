#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Linq;

namespace HeuristicLab.Modeling {
  public class VariableEvaluationImpactCalculator : OperatorBase {

    public VariableEvaluationImpactCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("Predictor", "The predictor used to evaluate the model", typeof(IPredictor), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "TargetVariable", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("InputVariableNames", "Names of used variables in the model (optional)", typeof(ItemList<StringData>), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "TrainingSamplesStart", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "TrainingSamplesEnd", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(ModelingResult.VariableEvaluationImpact.ToString(), "VariableEvaluationImpacts", typeof(ItemList), VariableKind.New));
    }

    public override string Description {
      get { return @"Calculates the impact of all allowed input variables on the model outputs using evaluator supplied as suboperator."; }
    }

    public override IOperation Apply(IScope scope) {
      IPredictor predictor = GetVariableValue<IPredictor>("Predictor", scope, true);
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      string targetVariableName = GetVariableValue<StringData>("TargetVariable", scope, true).Data;
      int targetVariable = dataset.GetVariableIndex(targetVariableName);
      ItemList<StringData> inputVariableNames = GetVariableValue<ItemList<StringData>>("InputVariableNames", scope, true, false);
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;

      Dictionary<string, double> evaluationImpacts;
      if (inputVariableNames == null)
        evaluationImpacts = Calculate(dataset, predictor, targetVariableName, start, end);
      else
        evaluationImpacts = Calculate(dataset, predictor, targetVariableName, inputVariableNames.Select(iv => iv.Data), start, end);

      ItemList variableImpacts = new ItemList();
      foreach (KeyValuePair<string, double> p in evaluationImpacts) {
        if (p.Key != targetVariableName) {
          ItemList row = new ItemList();
          row.Add(new StringData(p.Key));
          row.Add(new DoubleData(p.Value));
          variableImpacts.Add(row);
        }
      }

      scope.AddVariable(new Variable(scope.TranslateName(ModelingResult.VariableEvaluationImpact.ToString()), variableImpacts));
      return null;

    }
    public static Dictionary<string, double> Calculate(Dataset dataset, IPredictor predictor, string targetVariableName, int start, int end) {
      return Calculate(dataset, predictor, targetVariableName, null, start, end);
    }

    public static Dictionary<string, double> Calculate(Dataset dataset, IPredictor predictor, string targetVariableName, IEnumerable<string> inputVariableNames, int start, int end) {
      Dictionary<string, double> evaluationImpacts = new Dictionary<string, double>();
      Dataset dirtyDataset = (Dataset)dataset.Clone();
      double[] referenceValues = predictor.Predict(dataset, start, end);

      double mean;
      IEnumerable<double> oldValues;
      double[] newValues;
      IEnumerable<string> variables;
      if (inputVariableNames != null)
        variables = inputVariableNames;
      else
        variables = dataset.VariableNames;

      foreach (string variableName in variables) {
        if (variableName != targetVariableName) {
          if (dataset.CountMissingValues(variableName, start, end) < (end - start) && dataset.GetRange(variableName, start, end) > 0.0) {
            mean = dataset.GetMean(variableName, start, end);
            oldValues = dirtyDataset.ReplaceVariableValues(variableName, Enumerable.Repeat(mean, end - start), start, end);
            newValues = predictor.Predict(dirtyDataset, start, end);
            evaluationImpacts[variableName] = 1 - CalculateVAF(referenceValues, newValues);
            dirtyDataset.ReplaceVariableValues(variableName, oldValues, start, end);
          } else {
            evaluationImpacts[variableName] = 0.0;
          }
        }
      }

      return evaluationImpacts;
    }

    private static double CalculateVAF(double[] referenceValues, double[] newValues) {
      try {
        return SimpleVarianceAccountedForEvaluator.Calculate(Matrix<double>.Create(referenceValues, newValues));
      }
      catch (ArgumentException) {
        return double.PositiveInfinity;
      }
    }
  }
}
