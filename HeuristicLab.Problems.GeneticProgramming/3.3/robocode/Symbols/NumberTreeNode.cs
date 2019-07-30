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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  [StorableType("C2A2B6C5-3320-4231-A1B3-F8B8CF8B03A5")]
  public class NumberTreeNode : SymbolicExpressionTreeTerminalNode {
    private int value;
    [Storable]
    public int Value {
      get { return value; }
      private set { this.value = value; }
    }

    [StorableConstructor]
    protected NumberTreeNode(StorableConstructorFlag _) : base(_) { }
    protected NumberTreeNode(NumberTreeNode original, Cloner cloner)
      : base(original, cloner) {
      this.value = original.value;
    }

    public NumberTreeNode(Number numberSy) : base(numberSy) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NumberTreeNode(this, cloner);
    }

    public override bool HasLocalParameters {
      get { return true; }
    }

    public override void ResetLocalParameters(IRandom random) {
      // random initialization
      value = random.Next(-360, 360);
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      // random mutation (cyclic)
      value = value + random.Next(-20, 20);
      if (value < -360) value += 2 * 360;
      else if (value > 359) value -= 2 * 360;
    }
  }
}