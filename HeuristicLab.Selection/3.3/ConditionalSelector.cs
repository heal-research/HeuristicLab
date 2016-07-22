#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  [Item("ConditionalSelector", "Selects sub-scopes where a certain boolean variable is true.")]
  [StorableClass]
  public class ConditionalSelector : Selector {
    public ScopeTreeLookupParameter<BoolValue> ConditionParameter {
      get { return (ScopeTreeLookupParameter<BoolValue>)Parameters["Condition"]; }
    }
    protected IValueParameter<BoolValue> CopySelectedParameter {
      get { return (IValueParameter<BoolValue>)Parameters["CopySelected"]; }
    }

    public BoolValue CopySelected {
      get { return CopySelectedParameter.Value; }
      set { CopySelectedParameter.Value = value; }
    }

    [StorableConstructor]
    protected ConditionalSelector(bool deserializing) : base(deserializing) { }
    protected ConditionalSelector(ConditionalSelector original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ConditionalSelector(this, cloner);
    }
    public ConditionalSelector()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<BoolValue>("Condition", "The boolean variable based on which the scopes are selected into a true scope-branch and a false scope-branch."));
      Parameters.Add(new ValueParameter<BoolValue>("CopySelected", "The parameter that decides whether the selected scopes should be copied or moved.", new BoolValue(true)));
      CopySelectedParameter.Hidden = true;
    }

    protected override IScope[] Select(List<IScope> scopes) {
      ItemArray<BoolValue> conditions = ConditionParameter.ActualValue;
      List<IScope> selected = new List<IScope>();
      if (CopySelected.Value) {
        for (int i = 0; i < scopes.Count; i++) {
          if (conditions[i].Value) {
            selected.Add((IScope)scopes[i].Clone());
          }
        }
      } else {
        for (int i = 0; i < scopes.Count; i++) {
          if (conditions[i + selected.Count].Value) {
            selected.Add(scopes[i]);
            scopes.RemoveAt(i);
            i--;
          }
        }
      }
      return selected.ToArray();
    }
  }
}
