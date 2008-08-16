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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Functions;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.StructureIdentification {
  public abstract class GPEvaluatorBase : OperatorBase {
    private IEvaluator evaluator;
    private int targetVariable;
    private int start;
    private int end;
    private bool useEstimatedValues;
    private double[] backupValues;
    private int evaluatedSamples;
    private double estimatedValueMax;
    private double estimatedValueMin;
    private int treeSize;
    private double totalEvaluatedNodes;
    private Dataset dataset;
    private double targetMean;
    protected double TargetMean { get { return targetMean; } }

    public GPEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be evaluated", typeof(IFunctionTree), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeSize", "Size (number of nodes) of the tree to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the column of the dataset that holds the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("PunishmentFactor", "Punishment factor for invalid estimations", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TotalEvaluatedNodes", "Number of evaluated nodes", typeof(DoubleData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("UseEstimatedTargetValue", "Wether to use the original (measured) or the estimated (calculated) value for the targat variable when doing autoregressive modelling", typeof(BoolData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      // get all variable values
      targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      IFunctionTree functionTree = GetVariableValue<IFunctionTree>("FunctionTree", scope, true);
      double maximumPunishment = GetVariableValue<DoubleData>("PunishmentFactor", scope, true).Data * dataset.GetRange(targetVariable);
      treeSize = scope.GetVariableValue<IntData>("TreeSize", false).Data;
      totalEvaluatedNodes = scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data;
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;
      useEstimatedValues = GetVariableValue<BoolData>("UseEstimatedTargetValue", scope, true).Data;
      // prepare for autoregressive modelling by saving the original values of the target-variable to a backup array
      if(useEstimatedValues &&
        (backupValues == null || start != this.start || end != this.end)) {
        this.start = start;
        this.end = end;
        backupValues = new double[end - start];
        for(int i = start; i < end; i++) {
          backupValues[i - start] = dataset.GetValue(i, targetVariable);
        }
      }
      // get the mean of the values of the target variable to determin the max and min bounds of the estimated value
      targetMean = dataset.GetMean(targetVariable, start, end - 1);
      estimatedValueMin = targetMean - maximumPunishment;
      estimatedValueMax = targetMean + maximumPunishment;

      // initialize and reset the evaluator
      if(evaluator == null) evaluator = functionTree.CreateEvaluator();
      evaluator.ResetEvaluator(functionTree, dataset);
      evaluatedSamples = 0;

      Evaluate(start, end);

      // restore the values of the target variable from the backup array if necessary
      if(useEstimatedValues) RestoreDataset(dataset, targetVariable, start, end);
      // update the value of total evaluated nodes
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + treeSize * evaluatedSamples;
      return null;
    }

    private void RestoreDataset(Dataset dataset, int targetVariable, int from, int to) {
      for(int i = from; i < to; i++) {
        dataset.SetValue(i, targetVariable, backupValues[i - from]);
      }
    }

    public abstract void Evaluate(int start, int end);

    public void SetOriginalValue(int sample, double value) {
      if(useEstimatedValues) {
        dataset.SetValue(sample, targetVariable, value);
      }
    }

    public double GetOriginalValue(int sample) {
      return dataset.GetValue(sample, targetVariable);
    }

    public double GetEstimatedValue(int sample) {
      evaluatedSamples++;
      double estimated = evaluator.Evaluate(sample);
      if(double.IsNaN(estimated) || double.IsInfinity(estimated)) {
        estimated = estimatedValueMax;
      } else if(estimated > estimatedValueMax) {
        estimated = estimatedValueMax;
      } else if(estimated < estimatedValueMin) {
        estimated = estimatedValueMin;
      }
      return estimated;
    }
  }
}
