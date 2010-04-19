#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which contains multiple operators of which each is applied on one sub-scope of the current scope. The first operator is applied on the first sub-scope, the second on the second, and so on.
  /// </summary>
  [Item("SubScopesProcessor", "An operator which contains multiple operators of which each is applied on one sub-scope of the current scope. The first operator is applied on the first sub-scope, the second on the second, and so on.")]
  [StorableClass]
  public sealed class SubScopesProcessor : MultiOperator<IOperator> {
    public ValueLookupParameter<BoolValue> ParallelParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Parallel"]; }
    }

    public BoolValue Parallel {
      get { return ParallelParameter.Value; }
      set { ParallelParameter.Value = value; }
    }

    public SubScopesProcessor()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Parallel", "True if the operators should be applied in parallel on the sub-scopes, otherwise false.", new BoolValue(false)));
    }

    public override IOperation Apply() {
      BoolValue parallel = ParallelParameter.ActualValue;
      OperationCollection next = new OperationCollection(base.Apply());
      if (Operators.Count > 0) {
        OperationCollection inner = new OperationCollection();
        inner.Parallel = parallel == null ? false : parallel.Value;
        for (int i = 0; (i < ExecutionContext.Scope.SubScopes.Count) && (i < Operators.Count); i++)
          if (Operators[i] != null) inner.Add(ExecutionContext.CreateOperation(Operators[i], ExecutionContext.Scope.SubScopes[i]));
        next.Insert(0, inner);
      }
      return next;
    }
  }
}
