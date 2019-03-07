#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("32F7D410-066A-4ABF-81BB-9CD5A49C7B17")]
  [Item("HyperbolicSineIntegral", "Symbol that represents the hyperbolic sine integral.")]
  public sealed class HyperbolicTangent : Symbol {
    private const int minimumArity = 1;
    private const int maximumArity = 1;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [StorableConstructor]
    private HyperbolicTangent(StorableConstructorFlag _) : base(_) { }
    private HyperbolicTangent(HyperbolicTangent original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HyperbolicTangent(this, cloner);
    }
    public HyperbolicTangent() : base("HyperbolicTangent", "Symbol that represents the hyperbolic tanh().") { }
  }
}


