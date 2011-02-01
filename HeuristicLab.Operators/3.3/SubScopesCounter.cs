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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  [Item("SubScopesCounter", "Counts the number of direct sub-scopes and increments the value given in the parameter.")]
  [StorableClass]
  public class SubScopesCounter : SingleSuccessorOperator {

    public ILookupParameter<IntValue> ValueParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Value"]; }
    }

    [StorableConstructor]
    protected SubScopesCounter(bool deserializing) : base(deserializing) { }
    protected SubScopesCounter(SubScopesCounter original, Cloner cloner)
      : base(original, cloner) {
    }
    public SubScopesCounter() {
      Parameters.Add(new LookupParameter<IntValue>("Value", "The value that should be incremented by the number of direct sub-scopes. It will be created in the current scope if the value is not found."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubScopesCounter(this, cloner);
    }

    public override IOperation Apply() {
      int increment = ExecutionContext.Scope.SubScopes.Count;
      if (ValueParameter.ActualValue == null) ValueParameter.ActualValue = new IntValue();
      ValueParameter.ActualValue.Value += increment;
      return base.Apply();
    }
  }
}
