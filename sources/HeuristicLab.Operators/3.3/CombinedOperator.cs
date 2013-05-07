#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which contains an operator graph.
  /// </summary>
  [Item("CombinedOperator", "An operator which contains an operator graph.")]
  [StorableClass]
  public sealed class CombinedOperator : AlgorithmOperator, IParameterizedItem, IStorableContent {
    public string Filename { get; set; }

    public override bool CanChangeDescription {
      get { return true; }
    }
    public new ParameterCollection Parameters {
      get { return base.Parameters; }
    }
    IKeyedItemCollection<string, IParameter> IParameterizedItem.Parameters {
      get { return Parameters; }
    }

    [StorableConstructor]
    private CombinedOperator(bool deserializing) : base(deserializing) { }
    private CombinedOperator(CombinedOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public CombinedOperator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CombinedOperator(this, cloner);
    }
  }
}
