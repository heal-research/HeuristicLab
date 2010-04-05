#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;
namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols {
  [StorableClass]
  public sealed class ConstantTreeNode : SymbolicExpressionTreeTerminalNode {
    public new Constant Symbol {
      get { return (Constant)base.Symbol; }
    }
    public override bool HasLocalParameters {
      get {
        return true;
      }
    }

    private double constantValue;
    [Storable]
    public double Value {
      get { return constantValue; }
      set { constantValue = value; }
    }
    // copy constructor
    private ConstantTreeNode(ConstantTreeNode original)
      : base(original) {
      constantValue = original.constantValue;
    }

    public ConstantTreeNode(Constant constantSymbol) : base(constantSymbol) { }

    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      var range = Symbol.MaxValue.Value - Symbol.MaxValue.Value;
      Value = random.NextDouble() * range - Symbol.MinValue.Value;
    }

    public override object Clone() {
      return new ConstantTreeNode(this);
    }
  }
}
