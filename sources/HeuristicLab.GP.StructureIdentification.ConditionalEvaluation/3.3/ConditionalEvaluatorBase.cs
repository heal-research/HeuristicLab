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
  public abstract class ConditionalEvaluatorBase : GPEvaluatorBase {
    public virtual string OutputVariableName { get { return "Quality"; } }

    public ConditionalEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("MaxTimeOffset", "Maximal time offset for all feature", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinTimeOffset", "Minimal time offset for all feature", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("ConditionVariable", "Variable index which indicates if the row should be evaluated (0 means do not evaluate, != 0 evaluate)", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(OutputVariableName, OutputVariableName, typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override void Evaluate(IScope scope, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end, bool updateTargetValues) {
      int maxTimeOffset = GetVariableValue<IntData>("MaxTimeOffset", scope, true).Data;
      int minTimeOffset = GetVariableValue<IntData>("MinTimeOffset", scope, true).Data;
      int conditionVariable = GetVariableValue<IntData>("ConditionVariable", scope, true).Data;

      // store original and estimated values in a double array
      double[,] values = new double[end - start, 2];
      for (int sample = start; sample < end; sample++) {
        // check if condition variable is true between sample - minTimeOffset and sample - maxTimeOffset
        bool skip = false;
        for (int checkIndex = sample - minTimeOffset; checkIndex <= sample - maxTimeOffset && !skip ; checkIndex++) {
          if (dataset.GetValue(checkIndex, conditionVariable) == 0)
            skip = true;
        }
        if (!skip) {
          double original = dataset.GetValue(sample, targetVariable);
          double estimated = evaluator.Evaluate(sample);
          if (updateTargetValues) {
            dataset.SetValue(sample, targetVariable, estimated);
          }
          values[sample - start, 0] = estimated;
          values[sample - start, 1] = original;
        }
      }

      // calculate quality value
      double quality = Evaluate(values);

      DoubleData qualityData = GetVariableValue<DoubleData>(OutputVariableName, scope, false, false);
      if (qualityData == null) {
        qualityData = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(OutputVariableName), qualityData));
      }
      qualityData.Data = quality;
    }

    public abstract double Evaluate(double[,] values);
  }
}
