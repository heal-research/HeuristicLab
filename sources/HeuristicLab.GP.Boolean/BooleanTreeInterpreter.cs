#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Core;
using System.Xml;
using System.Diagnostics;
using HeuristicLab.Data;

namespace HeuristicLab.GP.Boolean {
  internal class BooleanTreeInterpreter {
    private const double EPSILON = 0.00001;
    private Dataset dataset;
    private IFunctionTree tree;
    private int targetVariable;
    private int currentRow;

    public void Reset(Dataset dataset, IFunctionTree tree, int targetVariable) {
      this.dataset = dataset;
      this.tree = tree;
      this.targetVariable = targetVariable;
    }

    internal int GetNumberMatchingInstances(int start, int end) {
      int matchingInstances = 0;
      for(int i = start; i < end; i++) {
        currentRow = i;
        int result = Step(tree) ? 1 : 0;
        if(result - dataset.GetValue(i, targetVariable) < EPSILON) matchingInstances++;
      }
      return matchingInstances;
    }

    internal bool Step(IFunctionTree tree) {
      int symbol = SymbolTable.MapFunction(tree.Function);
      switch(symbol) {
        case SymbolTable.AND: return Step(tree.SubTrees[0]) & Step(tree.SubTrees[0]);
        case SymbolTable.OR: return Step(tree.SubTrees[0]) | Step(tree.SubTrees[0]);
        case SymbolTable.NOT: return !Step(tree.SubTrees[0]);
        case SymbolTable.XOR: return Step(tree.SubTrees[0]) ^ Step(tree.SubTrees[1]);
        case SymbolTable.NAND: return !(Step(tree.SubTrees[0]) & Step(tree.SubTrees[0]));
        case SymbolTable.NOR: return !(Step(tree.SubTrees[0]) | Step(tree.SubTrees[0]));
        case SymbolTable.VARIABLE:
          int index = ((ConstrainedIntData)tree.LocalVariables.ToArray()[0].Value).Data;
          if(dataset.GetValue(currentRow, targetVariable) == 0.0) return false;
          else return true;
        case SymbolTable.UNKNOWN:
        default:
          throw new InvalidOperationException(tree.Function.ToString());

      }
    }
  }
}
