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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  [StorableType("BF268663-45F4-4E94-8605-33373BE2D612")]
  public class ShotPowerTreeNode : SymbolicExpressionTreeTerminalNode {
    private double value;
    [Storable]
    public double Value {
      get { return value; }
      private set { this.value = value; }
    }

    [StorableConstructor]
    protected ShotPowerTreeNode(StorableConstructorFlag _) : base(_) { }
    protected ShotPowerTreeNode(ShotPowerTreeNode original, Cloner cloner)
      : base(original, cloner) {
      this.value = original.value;
    }

    public ShotPowerTreeNode(ShotPower powerSy) : base(powerSy) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShotPowerTreeNode(this, cloner);
    }

    public override bool HasLocalParameters {
      get { return true; }
    }

    public override void ResetLocalParameters(IRandom random) {
      // random initialization
      value = Math.Max(0.1, random.NextDouble() * 3);
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      // mutation
      var d = random.NextDouble() * 2.0 - 1.0;
      value += shakingFactor * d;
      if (value < 0.1) value = 0.1;
      if (value > 3) value = 3;
    }
  }
}