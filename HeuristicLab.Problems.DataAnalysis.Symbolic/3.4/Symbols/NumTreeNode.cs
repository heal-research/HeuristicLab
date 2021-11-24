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
using HeuristicLab.Random;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("247DBD04-18F2-4184-B6F5-6E283BF06FD0")]
  public sealed class NumTreeNode : SymbolicExpressionTreeTerminalNode {
    public new Num Symbol {
      get { return (Num)base.Symbol; }
    }

    private double constantValue;
    [Storable]
    public double Value {
      get { return constantValue; }
      set { constantValue = value; }
    }

    [StorableConstructor]
    private NumTreeNode(StorableConstructorFlag _) : base(_) { }

    private NumTreeNode(NumTreeNode original, Cloner cloner)
      : base(original, cloner) {
      constantValue = original.constantValue;
    }

    private NumTreeNode() : base() { }
    public NumTreeNode(Num numSymbol) : base(numSymbol) { }

    public override bool HasLocalParameters {
      get {
        return true;
      }
    }
    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      var range = Symbol.MaxValue - Symbol.MinValue;
      Value = random.NextDouble() * range + Symbol.MinValue;
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      base.ShakeLocalParameters(random, shakingFactor);
      // 50% additive & 50% multiplicative
      if (random.NextDouble() < 0.5) {
        double x = NormalDistributedRandom.NextDouble(random, Symbol.ManipulatorMu, Symbol.ManipulatorSigma);
        Value = Value + x * shakingFactor;
      } else {
        double x = NormalDistributedRandom.NextDouble(random, 1.0, Symbol.MultiplicativeManipulatorSigma);
        Value = Value * x;
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NumTreeNode(this, cloner);
    }

    public override string ToString() {
      return $"<{constantValue:E4}>";
      // return constantValue.ToString("E4");
    }
  }
}
