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
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which contains multiple operators of which each is applied sequentially on one sub-scope of the current scope. The first operator is applied on the first sub-scope, the second on the second, and so on.
  /// </summary>
  [Item("SequentialSubScopesProcessor", "An operator which contains multiple operators of which each is applied sequentially on one sub-scope of the current scope. The first operator is applied on the first sub-scope, the second on the second, and so on.")]
  [Creatable("Test")]
  public sealed class SequentialSubScopesProcessor : MultipleCallsOperator {
    public SequentialSubScopesProcessor()
      : base() {
    }

    public override IExecutionSequence Apply() {
      ExecutionContextCollection next = new ExecutionContextCollection(base.Apply());
      if (Operators.Count > 0) {
        ExecutionContextCollection inner = new ExecutionContextCollection();
        for (int i = 0; (i < ExecutionContext.Scope.SubScopes.Count) && (i < Operators.Count); i++)
          inner.Add(new ExecutionContext(ExecutionContext.Parent, Operators[i], ExecutionContext.Scope.SubScopes[i]));
        next.Insert(0, inner);
      }
      return next;
    }
  }
}
