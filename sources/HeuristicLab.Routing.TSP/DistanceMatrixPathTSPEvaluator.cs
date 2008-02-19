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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Permutation;

namespace HeuristicLab.Routing.TSP {
  public class DistanceMatrixPathTSPEvaluator : SingleObjectiveEvaluatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public DistanceMatrixPathTSPEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Permutation", "Permutation representing a TSP solution in path encoding", typeof(Permutation.Permutation), VariableKind.In));
      AddVariableInfo(new VariableInfo("DistanceMatrix", "Distance matrix containing all distances between cities", typeof(DoubleMatrixData), VariableKind.In));
    }

    protected override double Evaluate(IScope scope) {
      double[,] distanceMatrix = GetVariableValue<DoubleMatrixData>("DistanceMatrix", scope, true).Data;
      int[] perm = GetVariableValue<Permutation.Permutation>("Permutation", scope, false).Data;
      double length = 0;

      for (int i = 0; i < perm.Length - 1; i++)
        length += distanceMatrix[perm[i], perm[i + 1]];
      length += distanceMatrix[perm[perm.Length - 1], perm[0]];
      return length;
    }
  }
}
