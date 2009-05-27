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
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.StructureIdentification.ConditionalEvaluation {
  public class ConditionalSimpleEvaluator : GPEvaluatorBase {
    public ConditionalSimpleEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("MaxTimeOffset", "Maximal time offset for all feature", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinTimeOffset", "Minimal time offset for all feature", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("ConditionVariable", "Variable index which indicates if the row should be evaluated (0 means do not evaluate, != 0 evaluate)", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Values", "The values of the target variable as predicted by the model and the original value of the target variable", typeof(ItemList), VariableKind.New | VariableKind.Out));
    }

    public override void Evaluate(IScope scope, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end, bool updateTargetValues) {
      ItemList values = GetVariableValue<ItemList>("Values", scope, false, false);
      if (values == null) {
        values = new ItemList();
        IVariableInfo info = GetVariableInfo("Values");
        if (info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, values));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), values));
      }
      values.Clear();

      int maxTimeOffset = GetVariableValue<IntData>("MaxTimeOffset", scope, true).Data;
      int minTimeOffset = GetVariableValue<IntData>("MinTimeOffset", scope, true).Data;
      int conditionVariable = GetVariableValue<IntData>("ConditionVariable", scope, true).Data;

      for (int sample = start; sample < end; sample++) {
        // check if condition variable is true between sample - minTimeOffset and sample - maxTimeOffset
        bool skip = false;
        for (int checkIndex = sample - minTimeOffset; checkIndex <= sample - maxTimeOffset && !skip ; checkIndex++) {
          if (dataset.GetValue(checkIndex, conditionVariable) == 0)
            skip = true;
        }
        if (!skip) {
          ItemList row = new ItemList();
          double estimated = evaluator.Evaluate(sample);
          double original = dataset.GetValue(sample, targetVariable);
          if (updateTargetValues) {
            dataset.SetValue(sample, targetVariable, estimated);
          }
          row.Add(new DoubleData(estimated));
          row.Add(new DoubleData(original));
          values.Add(row);
        }
      }
    }
  }
}
