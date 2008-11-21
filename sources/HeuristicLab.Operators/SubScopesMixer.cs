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
  /// <summary>
  /// Mixes the sub scopes of a specified scope according to a specified number of partitions.
  /// </summary>
  public class SubScopesMixer : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }
    /// <summary>
    /// Initializes a new instance of <see cref="SubScopesMixer"/> with one variable infos 
    /// (<c>Partitions</c>) and the <c>Local</c> flag set to <c>true</c>.
    /// </summary>
    public SubScopesMixer()
      : base() {
      AddVariableInfo(new VariableInfo("Partitions", "Number of partitions to mix", typeof(IntData), VariableKind.In));
      GetVariableInfo("Partitions").Local = true;
      AddVariable(new Variable("Partitions", new IntData(2)));
    }

    /// <summary>
    /// Mixes the subscopes of the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="IScope.ReorderSubScopes"/>.<br/>
    /// Mixing of sub scopes is based on the number of partitions.
    /// <example>12 sub scopes and 3 partitions:<br/>
    /// Partition 1 contains sub scopes 1-4, partition 2 sub scopes 5-8 and partition 3 sub scopes 9-12. <br/>
    /// Mixing is realized by selecting at the beginning the first sub scope from partition one, then the 
    /// first sub scope from partition 2, afterwards first sub scope from partition 3, 
    /// then the second sub scope from the first partition and so on. <br/>
    /// In the end the new sorting of the sub scopes is 1-5-9-2-6-10-3-7-11-4-8-12. 
    /// </example>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the number of sub scopes cannot be divided by
    /// the number of partitions without remainder.</exception>
    /// <param name="scope">The scope whose sub scopes should be mixed.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      int partitions = GetVariableValue<IntData>("Partitions", scope, true).Data;
      int[] sequence = new int[scope.SubScopes.Count];
      if ((sequence.Length % partitions) != 0)
        throw new ArgumentException("The number of subScopes is not divisible by the number of partitions without remainder.");
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
