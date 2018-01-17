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


namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public class CutPoint {
    public ISymbolicExpressionTreeNode Parent { get; private set; }
    public ISymbolicExpressionTreeNode Child { get; private set; }
    private readonly ISymbolicExpressionTreeGrammar grammar;

    public int ChildIndex { get; private set; }

    public CutPoint(ISymbolicExpressionTreeNode parent, ISymbolicExpressionTreeNode child) {      
      this.Parent = parent;
      this.Child = child;
      this.ChildIndex = parent.IndexOfSubtree(child);
      this.grammar = parent.Grammar;
    }
    public CutPoint(ISymbolicExpressionTreeNode parent, int childIndex) {
      this.Parent = parent;
      this.ChildIndex = childIndex;
      this.Child = null;
      this.grammar = parent.Grammar;
    }

    public bool IsMatchingPointType(ISymbolicExpressionTreeNode newChild) {
      var parent = this.Parent;
      if (newChild == null) {
        // make sure that one subtree can be removed and that only the last subtree is removed 
        return grammar.GetMinimumSubtreeCount(parent.Symbol) < parent.SubtreeCount &&
          this.ChildIndex == parent.SubtreeCount - 1;
      } else {
        // check syntax constraints of direct parent - child relation
        if (!grammar.ContainsSymbol(newChild.Symbol) ||
            !grammar.IsAllowedChildSymbol(parent.Symbol, newChild.Symbol, this.ChildIndex))
          return false;

        bool result = true;
        // check point type for the whole branch
        newChild.ForEachNodePostfix((n) => {
          result =
            result &&
            grammar.ContainsSymbol(n.Symbol) &&
            n.SubtreeCount >= grammar.GetMinimumSubtreeCount(n.Symbol) &&
            n.SubtreeCount <= grammar.GetMaximumSubtreeCount(n.Symbol);
        });
        return result;
      }
    }
  }
}
