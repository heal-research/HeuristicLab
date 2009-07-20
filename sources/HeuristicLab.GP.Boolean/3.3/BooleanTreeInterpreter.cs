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
    private List<LightWeightFunction> expression;
    private int targetVariable;
    private int currentRow;
    private int pc;

    public void Reset(Dataset dataset, BakedFunctionTree tree, int targetVariable) {
      this.dataset = dataset;
      this.expression = tree.LinearRepresentation;
      this.targetVariable = targetVariable;
    }

    internal int GetNumberOfErrors(int start, int end) {
      int errors = 0;
      for (int i = start; i < end; i++) {
        pc = 0;
        currentRow = i;
        int result = Step() ? 1 : 0;
        if (Math.Abs(result - dataset.GetValue(i, targetVariable)) > EPSILON) errors++;
      }
      return errors;
    }

    internal bool Step() {
      LightWeightFunction curFun = expression[pc++];
      int symbol = SymbolTable.MapFunction(curFun.functionType);
      switch (symbol) {
        case SymbolTable.AND: return Step() & Step();
        case SymbolTable.OR: return Step() | Step();
        case SymbolTable.NOT: return !Step();
        case SymbolTable.XOR: return Step() ^ Step();
        case SymbolTable.NAND: return !(Step() & Step());
        case SymbolTable.NOR: return !(Step() | Step());
        case SymbolTable.VARIABLE:
          return dataset.GetValue(currentRow, (int)curFun.localData[0]) != 0.0;
        case SymbolTable.UNKNOWN:
        default:
          throw new InvalidOperationException(curFun.functionType.ToString());

      }
    }
  }
}
