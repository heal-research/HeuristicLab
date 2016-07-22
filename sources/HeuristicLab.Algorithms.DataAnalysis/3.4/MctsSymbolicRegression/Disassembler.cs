#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Text;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {
  internal class Disassembler {
    public static string CodeToString(byte[] code, double[] consts) {
      var sb = new StringBuilder();
      int pc = 0;
      int nextParamIdx = -1;
      while (code[pc] != (byte)OpCodes.Exit) {
        var op = code[pc++];
        switch (op) {
          case (byte)OpCodes.Add: sb.Append(" + "); break;
          case (byte)OpCodes.Mul: sb.Append(" * "); break;
          case (byte)OpCodes.LoadConst1: sb.Append(" 1 "); break;
          case (byte)OpCodes.LoadConst0: sb.Append(" 0 "); break;
          case (byte)OpCodes.LoadParamN: sb.AppendFormat(" {0:N3} ", consts[++nextParamIdx]); break;
          case (byte)OpCodes.LoadVar:
          {
              short arg = (short)((code[pc] << 8) | code[pc + 1]);
              pc += 2;
            sb.AppendFormat(" var{0} ", arg); break;
          }
          case (byte)OpCodes.Exp: sb.Append(" exp "); break;
          case (byte)OpCodes.Log: sb.Append(" log "); break;
          case (byte)OpCodes.Inv: sb.Append(" inv "); break;          
        }
      }
      return sb.ToString();
    }
  }
}
