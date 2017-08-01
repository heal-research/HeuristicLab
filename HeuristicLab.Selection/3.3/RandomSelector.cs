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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A random selection operator.
  /// </summary>
  [Item("RandomSelector", "A random selection operator.")]
  [StorableClass]
  public sealed class RandomSelector : StochasticSelector, ISelector {
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
    private RandomSelector(bool deserializing) : base(deserializing) { }
    private RandomSelector(RandomSelector original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomSelector(this, cloner);
    }
    public RandomSelector()
      : base() {
      Parameters.Add(new ValueParameter<BoolValue>("CopySelected", "True if the selected sub-scopes should be copied, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfSelectedSubScopes", "The number of sub-scopes which should be selected."));
      CopySelectedParameter.Hidden = true;
    }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      IRandom random = RandomParameter.ActualValue;
      IScope[] selected = new IScope[count];

      for (int i = 0; i < count; i++) {
        if (copy)
          selected[i] = (IScope)scopes[random.Next(scopes.Count)].Clone();
        else {
          int index = random.Next(scopes.Count);
          selected[i] = scopes[index];
          scopes.RemoveAt(index);
        }
      }
      return selected;
    }
  }
}
