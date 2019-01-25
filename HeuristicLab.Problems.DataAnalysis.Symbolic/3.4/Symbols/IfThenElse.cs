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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("8AD33C34-9F5C-4F58-8C4C-25EA7F0A8C97")]
  [Item("IfThenElse", "Symbol that represents a conditional operator.")]
  public sealed class IfThenElse : Symbol {
    private const int minimumArity = 3;
    private const int maximumArity = 3;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [StorableConstructor]
    private IfThenElse(StorableConstructorFlag _) : base(_) { }
    private IfThenElse(IfThenElse original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IfThenElse(this, cloner);
    }
    public IfThenElse() : base("IfThenElse", "Symbol that represents a conditional operator.") { }
  }
}
