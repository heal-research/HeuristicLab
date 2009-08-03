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
  public class VariableEvaluationImpactCalculator : HeuristicLab.Modeling.VariableEvaluationImpactCalculator {

    public VariableEvaluationImpactCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("TreeEvaluator", "The evaluator that should be used to evaluate the expression tree", typeof(ITreeEvaluator), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be evaluated", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeSize", "Size (number of nodes) of the tree to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("PunishmentFactor", "Punishment factor for invalid estimations", typeof(DoubleData), VariableKind.In));
    }


    protected override double[] GetOutputs(IScope scope, Dataset dataset, int targetVariable, int start, int end) {
      ITreeEvaluator evaluator = GetVariableValue<ITreeEvaluator>("TreeEvaluator", scope, true);
      IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, true);
      double punishmentFactor = GetVariableValue<DoubleData>("PunishmentFactor", scope, true).Data;
      evaluator.PrepareForEvaluation(dataset, targetVariable, start, end, punishmentFactor, gpModel.FunctionTree);

      double[] result = new double[end - start];
      for (int i = start; i < end; i++) {
        result[i - start] = evaluator.Evaluate(i);
      }

      return result;
    }
  }
}
