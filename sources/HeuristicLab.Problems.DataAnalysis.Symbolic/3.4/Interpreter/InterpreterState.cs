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

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public class InterpreterState {
    private readonly double[] argumentStack;
    private int argumentStackPointer;
    private readonly Instruction[] code;

    public int ProgramCounter { get; set; }
    public bool InLaggedContext { get; set; }

    public InterpreterState(Instruction[] code, int argumentStackSize) {
      this.code = code;
      this.ProgramCounter = 0;
      this.InLaggedContext = false;
      if (argumentStackSize > 0) {
        this.argumentStack = new double[argumentStackSize];
      }
      this.argumentStackPointer = 0;
    }

    public void Reset() {
      this.ProgramCounter = 0;
      this.argumentStackPointer = 0;
      this.InLaggedContext = false;
    }

    public Instruction NextInstruction() {
      return code[ProgramCounter++];
    }
    // skips a whole branch
    public void SkipInstructions() {
      int i = 1;
      while (i > 0) {
        i += NextInstruction().nArguments;
        i--;
      }
    }

    private void Push(double val) {
      argumentStack[argumentStackPointer++] = val;
    }
    private double Pop() {
      return argumentStack[--argumentStackPointer];
    }

    public void CreateStackFrame(double[] argValues) {
      // push in reverse order to make indexing easier
      for (int i = argValues.Length - 1; i >= 0; i--) {
        argumentStack[argumentStackPointer++] = argValues[i];
      }
      Push(argValues.Length);
    }

    public void RemoveStackFrame() {
      int size = (int)Pop();
      argumentStackPointer -= size;
    }

    public double GetStackFrameValue(ushort index) {
      // layout of stack:
      // [0]   <- argumentStackPointer
      // [StackFrameSize = N + 1]
      // [Arg0] <- argumentStackPointer - 2 - 0
      // [Arg1] <- argumentStackPointer - 2 - 1
      // [...]
      // [ArgN] <- argumentStackPointer - 2 - N
      // <Begin of stack frame>
      return argumentStack[argumentStackPointer - index - 2];
    }
  }
}
