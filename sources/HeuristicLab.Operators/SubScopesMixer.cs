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

namespace HeuristicLab.Operators {
  public class SubScopesMixer : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public SubScopesMixer()
      : base() {
      AddVariableInfo(new VariableInfo("Partitions", "Number of partitions to mix", typeof(IntData), VariableKind.In));
      GetVariableInfo("Partitions").Local = true;
      AddVariable(new Variable("Partitions", new IntData(2)));
    }

    public override IOperation Apply(IScope scope) {
      int partitions = GetVariableValue<IntData>("Partitions", scope, true).Data;
      int[] sequence = new int[scope.SubScopes.Count];
      int partitionSize = sequence.Length / partitions;

      // mix sub-scopes -> alternately take one sub-scope from each partition
      for (int i = 0; i < partitionSize; i++) {
        for (int j = 0; j < partitions; j++)
          sequence[i * partitions + j] = j * partitionSize + i;
      }
      scope.ReorderSubScopes(sequence);

      return null;
    }
  }
}
