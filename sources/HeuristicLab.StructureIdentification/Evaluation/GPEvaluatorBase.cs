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
    private int trainingStart;
    private int trainingEnd;
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
      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "Start index of training samples in dataset", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "End index of training samples in dataset", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("UseEstimatedTargetValue", "Wether to use the original (measured) or the estimated (calculated) value for the targat variable when doing autoregressive modelling", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "The evaluated quality of the model", typeof(DoubleData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      // get all variable values
      targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      IFunctionTree functionTree = GetVariableValue<IFunctionTree>("FunctionTree", scope, true);
      double maximumPunishment = GetVariableValue<DoubleData>("PunishmentFactor", scope, true).Data * dataset.GetRange(targetVariable);
      treeSize = scope.GetVariableValue<IntData>("TreeSize", false).Data;
      totalEvaluatedNodes = scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data;
      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      useEstimatedValues = GetVariableValue<BoolData>("UseEstimatedTargetValue", scope, true).Data;
      // prepare for autoregressive modelling by saving the original values of the target-variable to a backup array
      if(useEstimatedValues && 
        (backupValues == null || trainingStart!=this.trainingStart || trainingEnd!=this.trainingEnd)) {
        this.trainingStart = trainingStart;
        this.trainingEnd = trainingEnd;
        backupValues = new double[trainingEnd - trainingStart];
        for(int i = trainingStart; i < trainingEnd; i++) {
          backupValues[i - trainingStart] = dataset.GetValue(i, targetVariable);
        }
      }
      // get the mean of the values of the target variable to determin the max and min bounds of the estimated value
      targetMean = dataset.GetMean(targetVariable, trainingStart, trainingEnd);
      estimatedValueMin = targetMean - maximumPunishment;
      estimatedValueMax = targetMean + maximumPunishment;

      // initialize and reset the evaluator
      if(evaluator == null) evaluator = functionTree.CreateEvaluator(dataset);
      evaluator.ResetEvaluator(functionTree);
      evaluatedSamples = 0;

      // calculate the quality measure
      double result = Evaluate(trainingStart, trainingEnd);

      // restore the values of the target variable from the backup array if necessary
      if(useEstimatedValues) RestoreDataset(dataset, targetVariable, trainingStart, trainingEnd);
      // update the value of total evaluated nodes
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + treeSize * evaluatedSamples;
      // write the calculate quality value
      DoubleData quality = GetVariableValue<DoubleData>("Quality", scope, false, false);
      if(quality == null) {
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Quality"), new DoubleData(result)));
      } else {
        quality.Data = result;
      }
      return null;
    }

    private void RestoreDataset(Dataset dataset, int targetVariable, int from, int to) {
      for(int i = from; i < to; i++) {
        dataset.SetValue(i, targetVariable, backupValues[i - from]);
      }
    }

    public abstract double Evaluate(int start, int end);

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
