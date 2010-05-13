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

using System;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Core;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Compiler {
  // total size of this struct should be small to improve cache access while executing the code
  // should be aligned to 8/16/32 byte
  // size = 4(8) + 1 + 1 + 2 = 8 (12)
  public struct Instruction {
    // the tree node can hold additional data that is necessary for the execution of this instruction
    public SymbolicExpressionTreeNode dynamicNode;
    // op code of the function that determines what operation should be executed
    public byte opCode;
    // number of arguments of the current instruction
    public byte nArguments;
    // an optional short value (addresses for calls, argument index for arguments)
    public ushort iArg0;
  }
}
