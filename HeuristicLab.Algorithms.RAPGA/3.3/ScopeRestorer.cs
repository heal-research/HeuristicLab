#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.RAPGA {
  /// <summary>
  /// An operator that restores created offspring from a scope list.
  /// </summary>
  /// <remarks>
  /// It adds all scopes in the list as sub-scopes to the current scope.
  /// </remarks>
  [Item("ScopeRestorer", "An operator that restores created offspring from a scope list. It adds all scopes in the list as sub-scopes to the current scope.")]
  [StorableClass]
  public class ScopeRestorer : SingleSuccessorOperator {
    #region Parameter Properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public ILookupParameter<ScopeList> OffspringListParameter {
      get { return (ILookupParameter<ScopeList>)Parameters["OffspringList"]; }
    }
    #endregion

    #region Properties
    private IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    private ScopeList OffspringList {
      get { return OffspringListParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    protected ScopeRestorer(bool deserializing) : base(deserializing) { }
    protected ScopeRestorer(ScopeRestorer original, Cloner cloner) : base(original, cloner) { }
    public ScopeRestorer()
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the offspring."));
      Parameters.Add(new LookupParameter<ScopeList>("OffspringList", "The list that contains the offspring."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScopeRestorer(this, cloner);
    }

    public override IOperation Apply() {
      CurrentScope.SubScopes.AddRange(OffspringList);
      return base.Apply();
    }
  }
}