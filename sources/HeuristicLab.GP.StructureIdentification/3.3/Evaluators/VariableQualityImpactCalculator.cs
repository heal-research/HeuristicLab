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

namespace HeuristicLab.GP.StructureIdentification {
  public class VariableQualityImpactCalculator : HeuristicLab.Modeling.VariableQualityImpactCalculator {

    public VariableQualityImpactCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("TreeEvaluator", "The evaluator that should be used to evaluate the expression tree", typeof(ITreeEvaluator), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be evaluated", typeof(IFunctionTree), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeSize", "Size (number of nodes) of the tree to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("PunishmentFactor", "Punishment factor for invalid estimations", typeof(DoubleData), VariableKind.In));
    }

    protected override double CalculateQuality(IScope scope, Dataset dataset, int targetVariable, int start, int end) {
      ITreeEvaluator evaluator = GetVariableValue<ITreeEvaluator>("TreeEvaluator", scope, true);
      IFunctionTree tree = GetVariableValue<IFunctionTree>("FunctionTree", scope, true);
      double punishmentFactor = GetVariableValue<DoubleData>("PunishmentFactor", scope, true).Data;
      evaluator.PrepareForEvaluation(dataset, targetVariable, start, end, punishmentFactor, tree);

      double[,] result = new double[end - start,2];
      for (int i = start; i < end; i++) {
        result[i - start, 0] = dataset.GetValue(i, targetVariable);
        result[i - start,1] = evaluator.Evaluate(i);
      }

      return HeuristicLab.Modeling.SimpleMSEEvaluator.Calculate(result);
    }
  }
}
