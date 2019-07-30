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
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("C4F429C2-B26A-4BD8-AB6A-3EBE45D3EE25")]
  [Item("HyperbolicCosineIntegral", "Symbol that represents the hyperbolic cosine integral.")]
  public sealed class HyperbolicCosineIntegral : Symbol {
    private const int minimumArity = 1;
    private const int maximumArity = 1;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [StorableConstructor]
    private HyperbolicCosineIntegral(StorableConstructorFlag _) : base(_) { }
    private HyperbolicCosineIntegral(HyperbolicCosineIntegral original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HyperbolicCosineIntegral(this, cloner);
    }
    public HyperbolicCosineIntegral() : base("HyperbolicCosineIntegral", "Symbol that represents the hyperbolic cosine integral.") { }
  }
}
