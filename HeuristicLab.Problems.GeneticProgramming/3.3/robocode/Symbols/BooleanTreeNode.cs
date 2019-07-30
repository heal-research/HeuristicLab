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
  [StorableType("93829075-7B5D-4D2D-B297-64AD120B6342")]
  public class BooleanTreeNode : SymbolicExpressionTreeTerminalNode {
    private bool value;
    [Storable]
    public bool Value {
      get { return value; }
      private set { this.value = value; }
    }

    [StorableConstructor]
    protected BooleanTreeNode(StorableConstructorFlag _) : base(_) { }
    protected BooleanTreeNode(BooleanTreeNode original, Cloner cloner)
      : base(original, cloner) {
      value = original.value;
    }

    public BooleanTreeNode(BooleanValue boolValueSy) : base(boolValueSy) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BooleanTreeNode(this, cloner);
    }

    public override bool HasLocalParameters {
      get { return true; }
    }

    public override void ResetLocalParameters(IRandom random) {
      // initialization
      value = random.NextDouble() > 0.5;
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      // mutation: flip value with 5% probability
      if (random.NextDouble() < 0.05) value = !value;
    }
  }
}