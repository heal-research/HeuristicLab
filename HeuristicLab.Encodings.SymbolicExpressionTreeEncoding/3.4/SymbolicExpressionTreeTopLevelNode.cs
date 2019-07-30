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
using HEAL.Attic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("3888AE52-0D58-4FC4-BDAF-6112BF6AFD8A")]
  public class SymbolicExpressionTreeTopLevelNode : SymbolicExpressionTreeNode {
    [Storable]
    private ISymbolicExpressionTreeGrammar grammar;
    public override ISymbolicExpressionTreeGrammar Grammar {
      get { return grammar; }
    }
    public void SetGrammar(ISymbolicExpressionTreeGrammar grammar) {
      this.grammar = grammar;
    }

    [StorableConstructor]
    protected SymbolicExpressionTreeTopLevelNode(StorableConstructorFlag _) : base(_) { }
    protected SymbolicExpressionTreeTopLevelNode(SymbolicExpressionTreeTopLevelNode original, Cloner cloner)
      : base(original, cloner) {
      grammar = cloner.Clone(original.Grammar);
    }
    public SymbolicExpressionTreeTopLevelNode() : base() { }
    public SymbolicExpressionTreeTopLevelNode(ISymbol symbol) : base(symbol) { }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeTopLevelNode(this, cloner);
    }
  }
}
