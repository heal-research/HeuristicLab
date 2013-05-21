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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.LawnMower {
  [StorableClass]
  public sealed class ConstantTreeNode : SymbolicExpressionTreeTerminalNode {
    private Tuple<int, int> value;

    public override bool HasLocalParameters {
      get { return true; }
    }

    [Storable]
    public Tuple<int, int> Value { get { return value; } private set { this.value = value; } }

    [StorableConstructor]
    private ConstantTreeNode(bool deserializing) : base(deserializing) { }
    private ConstantTreeNode(ConstantTreeNode original, Cloner cloner)
      : base(original, cloner) {
      this.value = new Tuple<int, int>(original.value.Item1, original.value.Item2);
    }

    public ConstantTreeNode()
      : base(new Constant()) {
      value = Tuple.Create(0, 0);
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ConstantTreeNode(this, cloner);
    }
    public override void ResetLocalParameters(IRandom random) {
      value = new Tuple<int, int>(random.Next(-20, 20), random.Next(-20, 20));
    }
    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      int dX = (int)Math.Round(random.Next(-10, +10) * shakingFactor);
      int dY = (int)Math.Round(random.Next(-10, +10) * shakingFactor);
      value = new Tuple<int, int>(value.Item1 + dX, value.Item2 + dY);
    }
  }
}
