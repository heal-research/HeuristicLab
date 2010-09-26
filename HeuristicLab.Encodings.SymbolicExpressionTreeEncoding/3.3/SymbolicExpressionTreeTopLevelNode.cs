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

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public class SymbolicExpressionTreeTopLevelNode : SymbolicExpressionTreeNode {
    public SymbolicExpressionTreeTopLevelNode()
      : base() {
    }

    public SymbolicExpressionTreeTopLevelNode(Symbol symbol)
      : base(symbol) {
    }

    [Storable]
    private ISymbolicExpressionGrammar grammar;
    public override ISymbolicExpressionGrammar Grammar {
      get { return grammar; }
    }
    public void SetGrammar(ISymbolicExpressionGrammar grammar) {
      this.grammar = grammar;
    }

    // copy constructor
    protected SymbolicExpressionTreeTopLevelNode(SymbolicExpressionTreeTopLevelNode original)
      : base(original) {
      if (original.Grammar != null)
        grammar = (ISymbolicExpressionGrammar)original.Grammar.Clone();
      //grammar = original.grammar;
    }

    public override object Clone() {
      return new SymbolicExpressionTreeTopLevelNode(this);
    }
  }
}
