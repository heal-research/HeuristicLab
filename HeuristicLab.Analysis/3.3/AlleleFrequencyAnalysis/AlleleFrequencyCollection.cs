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

using System.Collections.Generic;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Represents a collection of allele frequencies.
  /// </summary>
  [Item("AlleleFrequencyCollection", "Represents a collection of allele frequencies.")]
  [StorableClass]
  public class AlleleFrequencyCollection : ReadOnlyItemCollection<AlleleFrequency> {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Statistics; }
    }

    [StorableConstructor]
    protected AlleleFrequencyCollection(bool deserializing) : base(deserializing) { }
    protected AlleleFrequencyCollection(AlleleFrequencyCollection original, Cloner cloner) : base(original, cloner) { }
    public AlleleFrequencyCollection() : base() { }
    public AlleleFrequencyCollection(IEnumerable<AlleleFrequency> frequencies) : base(new ItemCollection<AlleleFrequency>(frequencies)) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlleleFrequencyCollection(this, cloner);
    }
  }
}
