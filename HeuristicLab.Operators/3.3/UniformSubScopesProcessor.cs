#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which applies a specified operator on all sub-scopes at the given depth of the current scope.
  /// </summary>
  [Item("UniformSubScopesProcessor", "An operator which applies a specified operator on all sub-scopes at the given depth of the current scope.")]
  [StorableClass]
  public sealed class UniformSubScopesProcessor : SingleSuccessorOperator {
    private OperatorParameter OperatorParameter {
      get { return (OperatorParameter)Parameters["Operator"]; }
    }
    public ValueLookupParameter<BoolValue> ParallelParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Parallel"]; }
    }
    public ValueParameter<IntValue> DepthParameter {
      get { return (ValueParameter<IntValue>)Parameters["Depth"]; }
    }

    public IOperator Operator {
      get { return OperatorParameter.Value; }
      set { OperatorParameter.Value = value; }
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
    private UniformSubScopesProcessor(bool deserializing) : base(deserializing) { }
    private UniformSubScopesProcessor(UniformSubScopesProcessor original, Cloner cloner)
      : base(original, cloner) {
    }
    public UniformSubScopesProcessor()
      : base() {
      Parameters.Add(new OperatorParameter("Operator", "The operator which should be applied on all sub-scopes of the current scope."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Parallel", "True if the operator should be applied in parallel on all sub-scopes, otherwise false.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IntValue>("Depth", "The number of steps to descend in the scope tree before applying operator.", new IntValue(1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformSubScopesProcessor(this, cloner);
    }

    public override IOperation Apply() {
      OperationCollection next = new OperationCollection(base.Apply());
      if (Operator != null) {
        List<IScope> scopes = GetScopesOnLevel(ExecutionContext.Scope, Depth.Value).ToList();
        OperationCollection inner = new OperationCollection();
        inner.Parallel = Parallel == null ? false : Parallel.Value;
        for (int i = 0; i < scopes.Count; i++) {
          inner.Add(ExecutionContext.CreateOperation(Operator, scopes[i]));
        }
        next.Insert(0, inner);
      }
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
