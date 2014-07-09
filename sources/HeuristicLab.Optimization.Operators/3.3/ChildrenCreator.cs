#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// An operator which is used to prepare crossover.
  /// </summary>
  /// <remarks>
  /// The sub-scopes of the current scope the operator is applied on represent the parents. The operator creates
  /// new and empty scopes for each child, adds the scopes that represent the child's parents as sub-scopes to
  /// the child and adds the child as sub-scope to the current scope.
  /// </remarks>
  [Item("ChildrenCreator", "An operator which is used to prepare crossover. The sub-scopes of the current scope the operator is applied on represent the parents. The operator creates new and empty scopes for each child, adds the scopes that represent the child's parents as sub-scopes to the child and adds the child as sub-scope to the current scope.")]
  [StorableClass]
  public sealed class ChildrenCreator : SingleSuccessorOperator {
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public ValueLookupParameter<IntValue> ParentsPerChildParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["ParentsPerChild"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    public IntValue ParentsPerChild {
      get { return ParentsPerChildParameter.Value; }
      set { ParentsPerChildParameter.Value = value; }
    }

    [StorableConstructor]
    private ChildrenCreator(bool deserializing) : base(deserializing) { }
    private ChildrenCreator(ChildrenCreator original, Cloner cloner) : base(original, cloner) { }
    public ChildrenCreator()
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope whose sub-scopes represent the parents."));
      Parameters.Add(new ValueLookupParameter<IntValue>("ParentsPerChild", "The number of parents that should be crossed per child. Note that some of the typical crossover operators require exactly two parents.", new IntValue(2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ChildrenCreator(this, cloner);
    }

    public override IOperation Apply() {
      int parentsPerChild = ParentsPerChildParameter.ActualValue.Value;
      int parents = CurrentScope.SubScopes.Count;
      if (parents % parentsPerChild > 0) throw new InvalidOperationException("Number of parents is not an integral multiple of ParentsPerChild.");
      int children = parents / parentsPerChild;

      for (int i = 0; i < children; i++) {
        IScope child = new Scope(i.ToString());
        for (int j = 0; j < parentsPerChild; j++) {
          IScope parent = CurrentScope.SubScopes[0];
          CurrentScope.SubScopes.RemoveAt(0);
          child.SubScopes.Add(parent);
        }
        CurrentScope.SubScopes.Add(child);
      }
      return base.Apply();
    }
  }
}
