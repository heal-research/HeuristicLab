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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.LawnMower {
  [StorableClass]
  public sealed class Forward : Symbol {
    public override int MinimumArity {
      get { return 0; }
    }
    public override int MaximumArity {
      get { return 0; }
    }
    [StorableConstructor]
    private Forward(bool deserializing) : base(deserializing) { }
    private Forward(Forward original, Cloner cloner)
      : base(original, cloner) {
    }

    public Forward()
      : base("Forward", "Moves the lawn mower one square forward.") {
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Forward(this, cloner);
    }
  }
}
