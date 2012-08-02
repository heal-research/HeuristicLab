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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  [Item("SubScopesCounter", "Counts the number of direct sub-scopes and increments or assigns it to the value given in the parameter.")]
  [StorableClass]
  public class SubScopesCounter : SingleSuccessorOperator {

    public ILookupParameter<IntValue> ValueParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Value"]; }
    }
    public IValueParameter<BoolValue> AccumulateParameter {
      get { return (IValueParameter<BoolValue>)Parameters["Accumulate"]; }
    }

    [StorableConstructor]
    protected SubScopesCounter(bool deserializing) : base(deserializing) { }
    protected SubScopesCounter(SubScopesCounter original, Cloner cloner)
      : base(original, cloner) {
    }
    public SubScopesCounter() {
      Parameters.Add(new LookupParameter<IntValue>("Value", "The value that should be incremented by the number of direct sub-scopes. It will be created in the current scope if the value is not found. If Accumulate is set to false, the number of direct sub-scopes is assigned and not accumulated."));
      Parameters.Add(new ValueParameter<BoolValue>("Accumulate", "True if the number of direct sub-scopes should be accumulated, false if the number should be assigned.", new BoolValue(true)));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("Accumulate"))
        Parameters.Add(new ValueParameter<BoolValue>("Accumulate", "True if the number of direct sub-scopes should be accumulated, false if the number should be assigned.", new BoolValue(true)));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubScopesCounter(this, cloner);
    }

    public override IOperation Apply() {
      int count = ExecutionContext.Scope.SubScopes.Count;
      if (ValueParameter.ActualValue == null) ValueParameter.ActualValue = new IntValue();
      if (AccumulateParameter.Value.Value) ValueParameter.ActualValue.Value += count;
      else ValueParameter.ActualValue.Value = count;
      return base.Apply();
    }
  }
}
