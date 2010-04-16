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
  /// An operator which applies a specified operator on all sub-scopes of the current scope.
  /// </summary>
  [Item("UniformSubScopesProcessor", "An operator which applies a specified operator on all sub-scopes of the current scope.")]
  [StorableClass]
  public sealed class UniformSubScopesProcessor : SingleSuccessorOperator {
    private OperatorParameter OperatorParameter {
      get { return (OperatorParameter)Parameters["Operator"]; }
    }
    public ValueLookupParameter<BoolValue> ParallelParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Parallel"]; }
    }

    public IOperator Operator {
      get { return OperatorParameter.Value; }
      set { OperatorParameter.Value = value; }
    }
    public BoolValue Parallel {
      get { return ParallelParameter.Value; }
      set { ParallelParameter.Value = value; }
    }

    public UniformSubScopesProcessor()
      : base() {
      Parameters.Add(new OperatorParameter("Operator", "The operator which should be applied on all sub-scopes of the current scope."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Parallel", "True if the operator should be applied in parallel on all sub-scopes, otherwise false.", new BoolValue(false)));
    }

    public override IOperation Apply() {
      BoolValue parallel = ParallelParameter.ActualValue;
      OperationCollection next = new OperationCollection(base.Apply());
      if (Operator != null) {
        OperationCollection inner = new OperationCollection();
        inner.Parallel = parallel == null ? false : parallel.Value;
        for (int i = 0; i < ExecutionContext.Scope.SubScopes.Count; i++)
          inner.Add(ExecutionContext.CreateOperation(Operator, ExecutionContext.Scope.SubScopes[i]));
        next.Insert(0, inner);
      }
      return next;
    }
  }
}
