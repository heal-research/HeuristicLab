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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [StorableClass]
  public sealed class ConstantTreeNode : SymbolicExpressionTreeTerminalNode {
    public new Constant Symbol {
      get { return (Constant)base.Symbol; }
    }

    private double constantValue;
    [Storable]
    public double Value {
      get { return constantValue; }
      set { constantValue = value; }
    }

    [StorableConstructor]
    private ConstantTreeNode(bool deserializing) : base(deserializing) { }
    private ConstantTreeNode(ConstantTreeNode original, Cloner cloner)
      : base(original, cloner) {
        this.constantValue = original.constantValue;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ConstantTreeNode(this, cloner);
    }

    public ConstantTreeNode(Constant constantSymbol) : base(constantSymbol) { }

    public override bool HasLocalParameters {
      get {
        return true;
      }
    }
    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      var range = Symbol.MaxValue - Symbol.MinValue;
      Value = random.NextDouble() * range - Symbol.MinValue;
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      base.ShakeLocalParameters(random, shakingFactor);
      var normalDistributedRNG = new NormalDistributedRandom(random, Symbol.ManipulatorNu, Symbol.ManipulatorSigma);
      double x = normalDistributedRNG.NextDouble();
      Value = Value + x * shakingFactor;
    }

    public override string ToString() {
      return ";" + constantValue.ToString("E4");
    }
  }
}
