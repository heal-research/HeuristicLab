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
using HeuristicLab.Modeling;
using HeuristicLab.GP.Interfaces;
using System.Linq;

namespace HeuristicLab.GP.StructureIdentification {
  public abstract class SimpleGPEvaluatorBase : GPEvaluatorBase {
    public virtual string OutputVariableName { get { return "Quality"; } }

    public SimpleGPEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo(OutputVariableName, OutputVariableName, typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override void Evaluate(IScope scope, IFunctionTree tree, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end) {
      // store original and estimated values in a double array
      double[,] values = Matrix<double>.Create(
        dataset.GetVariableValues(targetVariable, start, end),
        evaluator.Evaluate(dataset, tree, Enumerable.Range(start, end - start)).ToArray());

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
