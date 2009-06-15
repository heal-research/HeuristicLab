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
  public abstract class VariableImpactCalculatorBase<T> : OperatorBase {
    private bool abortRequested = false;

    public override string Description {
      get { return @"Calculates the impact of all allowed input variables on the model."; }
    }

    public abstract string OutputVariableName { get; }

    public override void Abort() {
      abortRequested = true;
    }

    public VariableImpactCalculatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Dataset", "Dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "TargetVariable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("AllowedFeatures", "Indexes of allowed input variables", typeof(ItemList<IntData>), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "TrainingSamplesStart", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "TrainingSamplesEnd", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(OutputVariableName, OutputVariableName, typeof(ItemList), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      ItemList<IntData> allowedFeatures = GetVariableValue<ItemList<IntData>>("AllowedFeatures", scope, true);
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      Dataset dirtyDataset = (Dataset)dataset.Clone();
      int start = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;

      T referenceValue = CalculateValue(scope, dataset, targetVariable, allowedFeatures, start, end);
      double[] impacts = new double[allowedFeatures.Count];

      for (int i = 0; i < allowedFeatures.Count && !abortRequested; i++) {
        int currentVariable = allowedFeatures[i].Data;
        var oldValues = ReplaceVariableValues(dirtyDataset, currentVariable, CalculateNewValues(dirtyDataset, currentVariable, start, end), start, end);
        T newValue = CalculateValue(scope, dirtyDataset, targetVariable, allowedFeatures, start, end);
        impacts[i] = CalculateImpact(referenceValue, newValue);
        ReplaceVariableValues(dirtyDataset, currentVariable, oldValues, start, end);
      }

      if (!abortRequested) {
        impacts = PostProcessImpacts(impacts);

        ItemList variableImpacts = new ItemList();
        for (int i = 0; i < allowedFeatures.Count; i++) {
          int currentVariable = allowedFeatures[i].Data;
          ItemList row = new ItemList();
          row.Add(new StringData(dataset.GetVariableName(currentVariable)));
          row.Add(new DoubleData(impacts[i]));
          variableImpacts.Add(row);
        }

        scope.AddVariable(new Variable(scope.TranslateName(OutputVariableName), variableImpacts));
        return null;
      } else {
        return new AtomicOperation(this, scope);
      }
    }

    protected abstract T CalculateValue(IScope scope, Dataset dataset, int targetVariable, ItemList<IntData> allowedFeatures, int start, int end);

    protected abstract double CalculateImpact(T referenceValue, T newValue);

    protected virtual double[] PostProcessImpacts(double[] impacts) {
      return impacts;
    }

    private IEnumerable<double> ReplaceVariableValues(Dataset ds, int variableIndex, IEnumerable<double> newValues, int start, int end) {
      double[] oldValues = new double[end - start];
      for (int i = 0; i < end - start; i++) oldValues[i] = ds.GetValue(i + start, variableIndex);
      if (newValues.Count() != end - start) throw new ArgumentException("The length of the new values sequence doesn't match the required length (number of replaced values)");

      int index = start;
      ds.FireChangeEvents = false;
      foreach (double v in newValues) {
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
