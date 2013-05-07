#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item(ProgramRootSymbol.ProgramRootSymbolName, ProgramRootSymbol.ProgramRootSymbolDescription)]
  public sealed class ProgramRootSymbol : Symbol, IReadOnlySymbol {
    public const string ProgramRootSymbolName = "ProgramRootSymbol";
    public const string ProgramRootSymbolDescription = "Special symbol that represents the program root node of a symbolic expression tree.";
    private const int minimumArity = 1;
    private const int maximumArity = byte.MaxValue;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [StorableConstructor]
    private ProgramRootSymbol(bool deserializing) : base(deserializing) { }
    private ProgramRootSymbol(ProgramRootSymbol original, Cloner cloner) : base(original, cloner) { }
    public ProgramRootSymbol() : base(ProgramRootSymbol.ProgramRootSymbolName, ProgramRootSymbol.ProgramRootSymbolDescription) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProgramRootSymbol(this, cloner);
    }
    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new SymbolicExpressionTreeTopLevelNode(this);
    }
  }
}
