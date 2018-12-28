#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("B5DF4207-7FDD-478D-B2FD-E52D8B7CD709")]
  public class LaggedTreeNode : SymbolicExpressionTreeNode, ILaggedTreeNode {
    public new LaggedSymbol Symbol {
      get { return (LaggedSymbol)base.Symbol; }
    }
    [Storable]
    private int lag;
    public int Lag {
      get { return lag; }
      set { lag = value; }
    }

    [StorableConstructor]
    protected LaggedTreeNode(StorableConstructorFlag _) : base(_) { }
    protected LaggedTreeNode(LaggedTreeNode original, Cloner cloner)
      : base(original, cloner) {
      lag = original.lag;
    }
    public LaggedTreeNode(LaggedSymbol timeLagSymbol) : base(timeLagSymbol) { }

    public override bool HasLocalParameters {
      get { return true; }
    }

    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      lag = random.Next(Symbol.MinLag, Symbol.MaxLag + 1);
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      base.ShakeLocalParameters(random, shakingFactor);
      lag = Math.Min(Symbol.MaxLag, Math.Max(Symbol.MinLag, lag + random.Next(-1, 2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LaggedTreeNode(this, cloner);
    }

    public override string ToString() {
      return base.ToString() + " " + lag.ToString();
    }
  }
}
