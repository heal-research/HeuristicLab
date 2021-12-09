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
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("44E03792-5E65-4C70-99B2-7849B8927E28")]
  [Item("Constant", "Represents a constant number.")]
  public sealed class Constant : Symbol, INumericSymbol{
    private const int minimumArity = 0;
    private const int maximumArity = 0;

    public override int MinimumArity => minimumArity;
    public override int MaximumArity => maximumArity;

    [StorableConstructor]
    private Constant(StorableConstructorFlag _) : base(_) { }

    private Constant(Constant original, Cloner cloner) : base(original, cloner) {}
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Constant(this, cloner);
    }

    public Constant() : base("Constant", "Represents a constant number.") {}

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new ConstantTreeNode(this);
    }
  }
}
