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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("LaggedSymbol", "Represents a symblol whose evaluation is shifted.")]
  public abstract class LaggedSymbol : Symbol {
    [Storable]
    private int minLag;
    public virtual int MinLag {
      get { return minLag; }
      set { minLag = value; }
    }
    [Storable]
    private int maxLag;
    public virtual int MaxLag {
      get { return maxLag; }
      set { maxLag = value; }
    }

    [StorableConstructor]
    protected LaggedSymbol(bool deserializing) : base(deserializing) { }
    protected LaggedSymbol(LaggedSymbol original, Cloner cloner)
      : base(original, cloner) {
      minLag = original.minLag;
      maxLag = original.maxLag;
    }
    protected LaggedSymbol()
      : base("LaggedSymbol", "Represents a symbol whose evaluation is shifted.") {
      minLag = -1; maxLag = -1;
    }
    protected LaggedSymbol(string name, string description)
      : base(name, description) {
      minLag = -1; maxLag = -1;
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new LaggedTreeNode(this);
    }
  }
}
