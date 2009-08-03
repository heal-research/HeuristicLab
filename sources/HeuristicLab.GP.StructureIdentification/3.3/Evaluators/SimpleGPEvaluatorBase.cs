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

namespace HeuristicLab.GP.StructureIdentification {
  public abstract class SimpleGPEvaluatorBase : GPEvaluatorBase {
    public virtual string OutputVariableName { get { return "Quality"; } }

    public SimpleGPEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo(OutputVariableName, OutputVariableName, typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override void Evaluate(IScope scope, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end, bool updateTargetValues) {
      // store original and estimated values in a double array
      double[,] values = new double[end - start, 2];
      for (int sample = start; sample < end; sample++) {
        double original = dataset.GetValue(sample, targetVariable);
        double estimated = evaluator.Evaluate(sample);
        if (updateTargetValues) {
          dataset.SetValue(sample, targetVariable, estimated);
        }
        values[sample - start, 0] = estimated;
        values[sample - start, 1] = original;
      }

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
