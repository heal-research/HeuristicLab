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

using HEAL.Attic;

using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("F91000E6-B041-4648-A9E8-595228F957FA")]
  public sealed class ConstantTreeNode : SymbolicExpressionTreeTerminalNode, INumericTreeNode {

    public new Constant Symbol => (Constant)base.Symbol;

    public double Value => Symbol.Value;

    [StorableConstructor]
    private ConstantTreeNode(StorableConstructorFlag _) : base(_) { }

    private ConstantTreeNode(ConstantTreeNode original, Cloner cloner)
      : base(original, cloner) {
    }

    private ConstantTreeNode() : base() { }
    public ConstantTreeNode(Constant constantSymbol) : base(constantSymbol) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ConstantTreeNode(this, cloner);
    }

    public override string ToString() {
      return $"{Value:E4}";
    }
  }
}