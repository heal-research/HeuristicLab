#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Mixes the sub scopes of a specified scope according to a specified number of partitions.
  /// </summary>
  [Item("SubScopesMixer", "Changes the order of the sub-scopes by repartitioning the sub-scopes such that each new partition contains one scope from each old partition.")]
  [StorableType("B69AE39A-B6E9-4ADB-94B4-77E172691D13")]
  public class SubScopesMixer : SingleSuccessorOperator {
    public ValueParameter<IntValue> PartitionsParameter {
      get { return (ValueParameter<IntValue>)Parameters["Partitions"]; }
    }

    public IntValue Partitions {
      get { return PartitionsParameter.Value; }
      set { PartitionsParameter.Value = value; }
    }

    [StorableConstructor]
    protected SubScopesMixer(StorableConstructorFlag _) : base(_) { }
    protected SubScopesMixer(SubScopesMixer original, Cloner cloner)
      : base(original, cloner) {
    }
    /// <summary>
    /// Initializes a new instance of <see cref="SubScopesMixer"/> with one variable infos 
    /// (<c>Partitions</c>) and the <c>Local</c> flag set to <c>true</c>.
    /// </summary>
    public SubScopesMixer()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Partitions", "The number of equal-sized partitions.", new IntValue(2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubScopesMixer(this, cloner);
    }

    /// <summary>
    /// Mixes the sub-scopes of the scope this operator is applied on.
    /// </summary>
    /// <remarks>Mixing of sub-scopes is based on the number of partitions.
    /// <example>12 sub-scopes and 3 partitions:<br/>
    /// Partition 1 contains scopes 1-4, partition 2 scopes 5-8 and partition 3 scopes 9-12. <br/>
    /// Mixing is realized by selecting at the beginning the first scope from partition one, then the 
    /// first scope from partition 2, afterwards first scope from partition 3, 
    /// then the second scope from the first partition and so on. <br/>
    /// In the end the new sorting of the sub scopes is 1-5-9-2-6-10-3-7-11-4-8-12. 
    /// </example>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the number of sub scopes cannot be divided by
    /// the number of partitions without remainder.</exception>
    /// <param name="scope">The scope whose sub scopes should be mixed.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply() {
      int partitions = Partitions.Value;
      IScope scope = ExecutionContext.Scope;
      int count = scope.SubScopes.Count;
      if ((count % partitions) != 0)
        throw new ArgumentException(Name + ": The number of sub-scopes is not divisible by the number of partitions without remainder.");
      int partitionSize = count / partitions;

      IScope[] reorderedSubScopes = new IScope[count];

      // mix sub-scopes -> alternately take one sub-scope from each partition
      for (int i = 0; i < partitionSize; i++) {
        for (int j = 0; j < partitions; j++) {
          reorderedSubScopes[i * partitions + j] = scope.SubScopes[j * partitionSize + i];
        }
      }
      scope.SubScopes.Clear();
      scope.SubScopes.AddRange(reorderedSubScopes);

      return base.Apply();
    }
  }
}
