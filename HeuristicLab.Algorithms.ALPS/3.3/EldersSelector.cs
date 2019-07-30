#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.ALPS {

  [Item("EldersSelector", "Select all individuals which are too old for their current layer.")]
  [StorableType("5C81CE34-F8D4-4B92-A2E4-A62332D68B1C")]
  public sealed class EldersSelector : Selector {
    public IScopeTreeLookupParameter<DoubleValue> AgeParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Age"]; }
    }
    public ILookupParameter<IntArray> AgeLimitsParameter {
      get { return (ILookupParameter<IntArray>)Parameters["AgeLimits"]; }
    }
    public ILookupParameter<IntValue> NumberOfLayersParameter {
      get { return (ILookupParameter<IntValue>)Parameters["NumberOfLayers"]; }
    }
    public ILookupParameter<IntValue> LayerParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Layer"]; }
    }

    [StorableConstructor]
    private EldersSelector(StorableConstructorFlag _) : base(_) { }
    private EldersSelector(EldersSelector original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EldersSelector(this, cloner);
    }
    public EldersSelector()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Age", "The age of individuals."));
      Parameters.Add(new LookupParameter<IntArray>("AgeLimits", "The maximum age an individual is allowed to reach in a certain layer."));
      Parameters.Add(new LookupParameter<IntValue>("NumberOfLayers", "The number of layers."));
      Parameters.Add(new LookupParameter<IntValue>("Layer", "The number of the current layer."));
    }

    protected override IScope[] Select(List<IScope> scopes) {
      var ages = AgeParameter.ActualValue;
      var ageLimits = AgeLimitsParameter.ActualValue;
      int numberOfLayers = NumberOfLayersParameter.ActualValue.Value;
      int layer = LayerParameter.ActualValue.Value;

      if (layer >= numberOfLayers - 1) // is max layer?
        return new IScope[0];

      int limit = ageLimits[layer];
      var elders = ages
        .Select((x, index) => new { index, age = x.Value })
        .Where(x => x.age > limit)
        .ToList();

      IScope[] selected = new IScope[elders.Count];
      for (int i = 0; i < elders.Count; i++) {
        selected[i] = scopes[elders[i].index];
        scopes[elders[i].index] = null;
      }
      scopes.RemoveAll(x => x == null);
      return selected;
    }
  }
}