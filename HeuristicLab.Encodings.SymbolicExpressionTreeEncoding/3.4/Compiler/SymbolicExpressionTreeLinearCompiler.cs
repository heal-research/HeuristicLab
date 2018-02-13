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

using System;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public static class SymbolicExpressionTreeLinearCompiler {
    public static LinearInstruction[] Compile(ISymbolicExpressionTree tree, Func<ISymbolicExpressionTreeNode, byte> opCodeMapper) {
      var root = tree.Root.GetSubtree(0).GetSubtree(0);
      var code = new LinearInstruction[root.GetLength()];
      if (root.SubtreeCount > ushort.MaxValue) throw new ArgumentException("Number of subtrees is too big (>65.535)");
      code[0] = new LinearInstruction { dynamicNode = root, nArguments = (ushort)root.SubtreeCount, opCode = opCodeMapper(root) };
      int c = 1, i = 0;
      foreach (var node in root.IterateNodesBreadth()) {
        for (int j = 0; j < node.SubtreeCount; ++j) {
          var s = node.GetSubtree(j);
          if (s.SubtreeCount > ushort.MaxValue) throw new ArgumentException("Number of subtrees is too big (>65.535)");
          code[c + j] = new LinearInstruction { dynamicNode = s, nArguments = (ushort)s.SubtreeCount, opCode = opCodeMapper(s) };
        }
        code[i].childIndex = c;
        c += node.SubtreeCount;
        ++i;
      }
      return code;
    }
  }
}
