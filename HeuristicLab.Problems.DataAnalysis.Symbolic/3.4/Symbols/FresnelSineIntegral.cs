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
using HEAL.Fossil;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("CEB592FC-E402-4A7C-A207-E4048F87DB82")]
  [Item("FresnelSineIntegral", "Symbol that represents the Fresnel sine integral.")]
  public sealed class FresnelSineIntegral : Symbol {
    private const int minimumArity = 1;
    private const int maximumArity = 1;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [StorableConstructor]
    private FresnelSineIntegral(StorableConstructorFlag _) : base(_) { }
    private FresnelSineIntegral(FresnelSineIntegral original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new FresnelSineIntegral(this, cloner);
    }
    public FresnelSineIntegral() : base("FresnelSineIntegral", "Symbol that represents the Fresnel sine integral.") { }
  }
}
