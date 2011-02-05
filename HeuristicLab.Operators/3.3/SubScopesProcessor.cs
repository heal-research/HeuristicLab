#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which contains multiple operators of which each is applied on one sub-scope at the given depth of the current scope. The first operator is applied on the first sub-scope, the second on the second, and so on.
  /// </summary>
  [Item("SubScopesProcessor", "An operator which contains multiple operators of which each is applied on one sub-scope at the given depth of the current scope. The first operator is applied on the first sub-scope, the second on the second, and so on.")]
  [StorableClass]
  public sealed class SubScopesProcessor : MultiOperator<IOperator> {
    public ValueLookupParameter<BoolValue> ParallelParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Parallel"]; }
    }
    public ValueParameter<IntValue> DepthParameter {
      get { return (ValueParameter<IntValue>)Parameters["Depth"]; }
    }

    public BoolValue Parallel {
      get { return ParallelParameter.Value; }
      set { ParallelParameter.Value = value; }
    }
    public IntValue Depth {
      get { return DepthParameter.Value; }
      set { DepthParameter.Value = value; }
    }

    [StorableConstructor]
    private SubScopesProcessor(bool deserializing) : base(deserializing) { }
    private SubScopesProcessor(SubScopesProcessor original, Cloner cloner)
      : base(original, cloner) {
    }
    public SubScopesProcessor()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Parallel", "True if the operators should be applied in parallel on the sub-scopes, otherwise false.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IntValue>("Depth", "The number of steps to descend in the scope tree before applying operator.", new IntValue(1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubScopesProcessor(this, cloner);
    }

    public override IOperation Apply() {
      List<IScope> scopes = GetScopesOnLevel(ExecutionContext.Scope, Depth.Value).ToList();
      OperationCollection next = new OperationCollection(base.Apply());
      if (scopes.Count != Operators.Count)
        throw new ArgumentException("The number of operators doesn't match the number of sub-scopes at depth " + Depth.Value);
      OperationCollection inner = new OperationCollection();
      inner.Parallel = Parallel == null ? false : Parallel.Value;
      for (int i = 0; i < scopes.Count(); i++) {
        inner.Add(ExecutionContext.CreateOperation(Operators[i], scopes[i]));
      }
      next.Insert(0, inner);
      return next;
    }

    private IEnumerable<IScope> GetScopesOnLevel(IScope scope, int d) {
      if (d == 0) yield return scope;
      else {
        foreach (IScope subScope in scope.SubScopes) {
          foreach (IScope scopesOfSubScope in GetScopesOnLevel(subScope, d - 1)) {
            yield return scopesOfSubScope;
          }
        }
      }
    }
  }
}
