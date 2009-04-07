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
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Data;

namespace HeuristicLab.Assignment.QAP {
  public class QAPEvaluator : SingleObjectiveEvaluatorBase {
    public QAPEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Distances", "Distance matrix for given locations", typeof(DoubleMatrixData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Weights", "Weight matrix denoting the flow strength between different facilities", typeof(DoubleMatrixData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Permutation", "Permutation representing a QAP assignment of facilities to locations", typeof(Permutation.Permutation), VariableKind.In));
    }

    protected override double Evaluate(IScope scope) {
      double[,] distances = GetVariableValue<DoubleMatrixData>("Distances", scope, true).Data;
      double[,] weights = GetVariableValue<DoubleMatrixData>("Weights", scope, true).Data;
      int[] perm = GetVariableValue<Permutation.Permutation>("Permutation", scope, false).Data;
      double costs = 0;

      for (int i = 0; i < perm.Length; i++) {
        for (int j = 0; j < perm.Length; j++) {
          costs += distances[i, j] * weights[perm[i], perm[j]];
        }
      }
      return costs;
    }
  }
}
