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
using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public interface ISymbolicExpressionTreeNode : IDeepCloneable {
    ISymbolicExpressionTreeGrammar Grammar { get; }
    ISymbolicExpressionTreeNode Parent { get; set; }
    ISymbol Symbol { get; }
    bool HasLocalParameters { get; }

    int GetDepth();
    int GetLength();
    int GetBranchLevel(ISymbolicExpressionTreeNode child);

    IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth();
    IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix();
    IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix();
    void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode> a);
    void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode> a);

    IEnumerable<ISymbolicExpressionTreeNode> Subtrees { get; }
    int SubtreeCount { get; }
    ISymbolicExpressionTreeNode GetSubtree(int index);
    int IndexOfSubtree(ISymbolicExpressionTreeNode tree);
    void AddSubtree(ISymbolicExpressionTreeNode tree);
    void InsertSubtree(int index, ISymbolicExpressionTreeNode tree);
    void RemoveSubtree(int index);

    void ResetLocalParameters(IRandom random);
    void ShakeLocalParameters(IRandom random, double shakingFactor);
  }
}
