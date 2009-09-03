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
using HeuristicLab.Core;
using System.Xml;
using System.Diagnostics;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.DataAnalysis;

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

    internal int GetNumberOfErrors(int start, int end) {
      int errors = 0;
      for (int i = start; i < end; i++) {
        currentRow = i;
        int result = Step(tree) ? 1 : 0;
        if (Math.Abs(result - dataset.GetValue(i, targetVariable)) > EPSILON) errors++;
      }
      return errors;
    }

    internal bool Step(IFunctionTree t) {
      int symbol = SymbolTable.MapFunction(t.Function);
      switch (symbol) {
        case SymbolTable.AND: return Step(t.SubTrees[0]) && Step(t.SubTrees[1]);
        case SymbolTable.OR: return Step(t.SubTrees[0]) || Step(t.SubTrees[1]);
        case SymbolTable.NOT: return !Step(t.SubTrees[0]);
        case SymbolTable.XOR: return Step(t.SubTrees[0]) ^ Step(t.SubTrees[1]);
        case SymbolTable.NAND: return !(Step(t.SubTrees[0]) && Step(t.SubTrees[1]));
        case SymbolTable.NOR: return !(Step(t.SubTrees[0]) || Step(t.SubTrees[1]));
        case SymbolTable.VARIABLE: {
            var varNode = (VariableFunctionTree)t;
            int index = dataset.GetVariableIndex(varNode.VariableName);
            return !dataset.GetValue(currentRow, index).IsAlmost(0.0);
          }
        case SymbolTable.UNKNOWN:
        default:
          throw new UnknownFunctionException(t.Function.Name);
      }
    }
  }
}
