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

using System.Collections.Generic;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Represents history values of heat maps.
  /// </summary>
  [Item("HeatMapHistory", "Represents history values of heat maps.")]
  [StorableClass]
  public class HeatMapHistory : ItemCollection<HeatMap> {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Cab; }
    }

    [StorableConstructor]
    protected HeatMapHistory(bool deserializing) : base(deserializing) { }
    protected HeatMapHistory(HeatMapHistory original, Cloner cloner) : base(original, cloner) { }
    public HeatMapHistory() : base() { }
    public HeatMapHistory(IEnumerable<HeatMap> collection) : base(new ItemCollection<HeatMap>(collection)) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HeatMapHistory(this, cloner);
    }
  }
}
