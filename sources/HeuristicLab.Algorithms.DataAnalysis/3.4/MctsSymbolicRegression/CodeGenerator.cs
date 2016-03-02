#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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

using System.Diagnostics.Contracts;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {
  internal class CodeGenerator {

    public const int MaxCodeLength = 10000;
    public const int MaxConstants = 100;

    private readonly byte[] codeArray = new byte[MaxCodeLength];
    private int pc;
    private int nParams;

    public void Reset() {
      pc = 0;
      nParams = 0;
    }

    public void Emit1(OpCodes code) {
      codeArray[pc++] = (byte)code;
      if (code == OpCodes.LoadParamN) nParams++;
    }

    public void Emit2(OpCodes code, short arg) {
      Contract.Assert(code == OpCodes.LoadVar); // only loadVar opcode has params
      Emit1(code);
      codeArray[pc++] = (byte)(arg >> 8);
      codeArray[pc++] = (byte)(arg & 0xFF);
    }

    public void GetCode(out byte[] code, out int nParams) {
      code = codeArray;
      nParams = this.nParams;
    }
  }
}
