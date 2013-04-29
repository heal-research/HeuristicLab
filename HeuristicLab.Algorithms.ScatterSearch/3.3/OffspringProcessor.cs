#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ScatterSearch {
  /// <summary>
  /// An operator that creates a subscope with subscopes for every variable in the current scope.
  /// </summary>
  [Item("OffspringProcessor", "An operator that creates a subscope with subscopes for every variable in the current scope.")]
  [StorableClass]
  public sealed class OffspringProcessor : SingleSuccessorOperator {
    #region Parameter properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    #endregion

    #region Properties
    private IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private OffspringProcessor(bool deserializing) : base(deserializing) { }
    private OffspringProcessor(OffspringProcessor original, Cloner cloner) : base(original, cloner) { }
    public OffspringProcessor()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the offspring as variables."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OffspringProcessor(this, cloner);
    }

    public override IOperation Apply() {
      var child = new Scope();
      child.Variables.AddRange(CurrentScope.Variables);
      var offspringScope = new Scope();
      offspringScope.SubScopes.Add(child);
      var parents = CurrentScope.SubScopes.ToArray();
      CurrentScope.Variables.Clear();
      CurrentScope.SubScopes.Clear();
      CurrentScope.SubScopes.AddRange(parents);
      CurrentScope.SubScopes.Add(offspringScope);
      return base.Apply();
    }
  }
}
