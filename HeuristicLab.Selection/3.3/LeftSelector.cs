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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// An operator which selects sub-scopes from left to right.
  /// </summary>
  [Item("LeftSelector", "An operator which selects sub-scopes from left to right.")]
  [StorableClass]
  public sealed class LeftSelector : Selector {
    private IValueParameter<BoolValue> CopySelectedParameter {
      get { return (IValueParameter<BoolValue>)Parameters["CopySelected"]; }
    }
    public IValueLookupParameter<IntValue> NumberOfSelectedSubScopesParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["NumberOfSelectedSubScopes"]; }
    }

    public BoolValue CopySelected {
      get { return CopySelectedParameter.Value; }
      set { CopySelectedParameter.Value = value; }
    }

    [StorableConstructor]
    private LeftSelector(bool deserializing) : base(deserializing) { }
    private LeftSelector(LeftSelector original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LeftSelector(this, cloner);
    }

    public LeftSelector()
      : base() {
      Parameters.Add(new ValueParameter<BoolValue>("CopySelected", "True if the selected sub-scopes should be copied, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfSelectedSubScopes", "The number of sub-scopes which should be selected."));
      CopySelectedParameter.Hidden = true;
    }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      IScope[] selected = new IScope[count];

      int j = 0;
      for (int i = 0; i < count; i++) {
        if (copy) {
          selected[i] = (IScope)scopes[j].Clone();
          j++;
          if (j >= scopes.Count) j = 0;
        } else {
          selected[i] = scopes[0];
          scopes.RemoveAt(0);
        }
      }
      return selected;
    }
  }
}
