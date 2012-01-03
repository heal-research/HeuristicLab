#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  internal class CutPoint {
    public ISymbolicExpressionTreeNode Parent { get; set; }
    public ISymbolicExpressionTreeNode Child { get; set; }
    private int childIndex;
    public int ChildIndex {
      get { return childIndex; }
    }
    public CutPoint(ISymbolicExpressionTreeNode parent, ISymbolicExpressionTreeNode child) {
      this.Parent = parent;
      this.Child = child;
      this.childIndex = parent.IndexOfSubtree(child);
    }
    public CutPoint(ISymbolicExpressionTreeNode parent, int childIndex) {
      this.Parent = parent;
      this.childIndex = childIndex;
      this.Child = null;
    }
  }
}
