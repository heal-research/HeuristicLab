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
  public sealed class Frog : Symbol {
    public override int MinimumArity {
      get { return 1; }
    }
    public override int MaximumArity {
      get { return 1; }
    }
    [StorableConstructor]
    private Frog(bool deserializing) : base(deserializing) { }
    private Frog(Frog original, Cloner cloner)
      : base(original, cloner) {
    }

    public Frog()
      : base("Frog", "Jumps to a relative position (determined by the argument).") {
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Frog(this, cloner);
    }
  }
}
