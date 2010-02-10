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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which applies a specified operator on all sub-scopes of the current scope in parallel.
  /// </summary>
  [Item("UniformParallelSubScopesProcessor", "An operator which applies a specified operator on all sub-scopes of the current scope in parallel.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public sealed class UniformParallelSubScopesProcessor : SingleCallOperator {
    public UniformParallelSubScopesProcessor()
      : base() {
      Parameters.Remove("Operator");
      Parameters.Add(new OperatorParameter("Operator", "The operator which should be applied on all sub-scopes of the current scope in parallel."));
    }

    public override IExecutionSequence Apply() {
      ExecutionContextCollection next = new ExecutionContextCollection(base.Apply());
      if (Operator != null) {
        ExecutionContextCollection inner = new ExecutionContextCollection();
        inner.Parallel = true;
        for (int i = 0; i < ExecutionContext.Scope.SubScopes.Count; i++)
          inner.Add(new ExecutionContext(ExecutionContext.Parent, Operator, ExecutionContext.Scope.SubScopes[i]));
        next.Insert(0, inner);
      }
      return next;
    }
  }
}
