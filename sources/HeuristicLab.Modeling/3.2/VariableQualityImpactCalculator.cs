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
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Linq;

namespace HeuristicLab.Modeling {
  public class VariableQualityImpactCalculator : OperatorBase {
    public override string Description {
      get { return @"Calculates the impact of all allowed input variables on the quality of the model using evaluator supplied as suboperator."; }
    }

    public VariableQualityImpactCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("Dataset", "Dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "TargetVariable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("AllowedFeatures", "Indexes of allowed input variables", typeof(ItemList<IntData>), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "TrainingSamplesStart", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "TrainingSamplesEnd", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("VariableQualityImpacts", "Effect on quality of model (percentage of original quality) if variable is replaced by its mean.", typeof(ItemList), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      ItemList<IntData> allowedFeatures = GetVariableValue<ItemList<IntData>>("AllowedFeatures", scope, true);
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      Dataset dirtyDataset = (Dataset)dataset.Clone();
      int start = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;

      if (SubOperators.Count < 1) throw new InvalidOperationException("VariableQualityImpactCalculator needs a suboperator to evaluate the model");
      IOperator evaluationOperator = this.SubOperators[0];
      ItemList variableQualityImpacts = new ItemList();

      // calculateReferenceQuality
      double referenceQuality = CalculateQuality(scope, dataset, evaluationOperator);

      for (int i = 0; i < allowedFeatures.Count; i++) {
        int currentVariable = allowedFeatures[i].Data;
        var oldValues = ReplaceVariableValues(dirtyDataset, currentVariable , CalculateNewValues(dirtyDataset, currentVariable, start, end), start, end);
        double newQuality = CalculateQuality(scope, dirtyDataset, evaluationOperator);
        double ratio = newQuality / referenceQuality;
        ItemList row = new ItemList();
        row.Add(new StringData(dataset.GetVariableName(currentVariable)));
        row.Add(new DoubleData(ratio));
        variableQualityImpacts.Add(row);
        ReplaceVariableValues(dirtyDataset, currentVariable, oldValues, start, end);
      }
      scope.AddVariable(new Variable(scope.TranslateName("VariableQualityImpacts"), variableQualityImpacts));
      return null;
    }

    private double CalculateQuality(IScope scope, Dataset dataset, IOperator evaluationOperator) {
      Scope s = new Scope();
      s.AddVariable(new Variable("Dataset", dataset));
      scope.AddSubScope(s);
      evaluationOperator.Execute(s);
      double quality = s.GetVariableValue<DoubleData>("Quality", false).Data;
      scope.RemoveSubScope(s);
      return quality;
    }

    private IEnumerable<double> ReplaceVariableValues(Dataset ds, int variableIndex, IEnumerable<double> newValues, int start, int end) {
      double[] oldValues = new double[end - start];
      for (int i = 0; i < end - start; i++) oldValues[i] = ds.GetValue(i + start, variableIndex);
      if (newValues.Count() != end - start) throw new ArgumentException("The length of the new values sequence doesn't match the required length (number of replaced values)");

      int index = start;
      ds.FireChangeEvents = false;
      foreach(double v in newValues) {
        ds.SetValue(index++, variableIndex, v);
      }
      ds.FireChangeEvents = true;
      ds.FireChanged();
      return oldValues;
    }

    private IEnumerable<double> CalculateNewValues(Dataset ds, int variableIndex, int start, int end) {
      double mean = ds.GetMean(variableIndex, start, end);
      return Enumerable.Repeat(mean, end - start);
    }
  }
}
