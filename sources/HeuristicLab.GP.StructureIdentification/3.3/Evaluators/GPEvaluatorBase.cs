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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.StructureIdentification {
  public abstract class GPEvaluatorBase : OperatorBase {
    public GPEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("TreeEvaluator", "The evaluator that should be used to evaluate the expression tree", typeof(ITreeEvaluator), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be evaluated", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the column of the dataset that holds the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TotalEvaluatedNodes", "Number of evaluated nodes", typeof(DoubleData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("UseEstimatedTargetValue", "Wether to use the original (measured) or the estimated (calculated) value for the target variable for autoregressive modelling", typeof(BoolData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      // get all variable values
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, true);
      double totalEvaluatedNodes = scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data;
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;
      bool useEstimatedValues = GetVariableValue<BoolData>("UseEstimatedTargetValue", scope, true).Data;
      ITreeEvaluator evaluator = GetVariableValue<ITreeEvaluator>("TreeEvaluator", scope, true);
      evaluator.PrepareForEvaluation(dataset, gpModel.FunctionTree);

      double[] backupValues = null;
      // prepare for autoregressive modelling by saving the original values of the target-variable to a backup array
      if (useEstimatedValues &&
        (backupValues == null || backupValues.Length != end - start)) {
        backupValues = new double[end - start];
        for (int i = start; i < end; i++) {
          backupValues[i - start] = dataset.GetValue(i, targetVariable);
        }
      }
      dataset.FireChangeEvents = false;

      Evaluate(scope, evaluator, dataset, targetVariable, start, end, useEstimatedValues);

      // restore the values of the target variable from the backup array if necessary
      if (useEstimatedValues) {
        for (int i = start; i < end; i++) {
          dataset.SetValue(i, targetVariable, backupValues[i - start]);
        }
      }
      dataset.FireChangeEvents = true;
      dataset.FireChanged();

      // update the value of total evaluated nodes
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + gpModel.Size * (end - start);
      return null;
    }

    public abstract void Evaluate(IScope scope, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end, bool updateTargetValues);
  }
}
