using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.NativeInterpreter;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Tests;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {
  [TestClass]
  public class VarProTest {

    [TestMethod]
    public void Exponential() {
      var rand = new MersenneTwister(31415);
      int n = 30;
      var x1 = Enumerable.Range(0, n).Select(_ => rand.NextDouble() * 2 - 1).ToArray();
      var x2 = Enumerable.Range(0, n).Select(_ => rand.NextDouble() * 2 - 1).ToArray();

      var x1Handle = GCHandle.Alloc(x1, GCHandleType.Pinned);
      var x2Handle = GCHandle.Alloc(x2, GCHandleType.Pinned);

      {
        var y = x1.Select(xi => Math.Exp(-0.3 * xi)).ToArray();

        var code = new NativeInterpreter.NativeInstruction[4];
        code[0] = EmitConst(1.0);
        code[1] = EmitVar(x1Handle, 1.0);
        code[2] = Emit(OpCode.Mul, code[0], code[1]);
        code[3] = Emit(OpCode.Exp, code[2]);

        var termIndices = new int[1];
        termIndices[0] = code.Length - 1;
        var rows = Enumerable.Range(0, n).ToArray();
        var coeff = new double[termIndices.Length + 1];
        var options = new SolverOptions();
        var y_pred = new double[y.Length];
        NativeInterpreter.NativeWrapper.GetValuesVarPro(code, code.Length, termIndices, nTerms: 1, rows, nRows: n, coeff, options, y_pred, y, out var optSummary);
        Assert.AreEqual(code[0].value, -0.3, 1e-6);
        Assert.AreEqual(code[1].value, 1.0);

        Array.Clear(coeff, 0, coeff.Length);
        Array.Clear(y_pred, 0, y_pred.Length);
        NativeInterpreter.NativeWrapper.GetValuesVarPro(code, code.Length, termIndices, nTerms: 1, rows, nRows: n, coeff, options, y_pred, y, out optSummary);
        Assert.AreEqual(code[0].value, -0.3, 1e-6);
        Assert.AreEqual(code[1].value, 1.0);
      }

      {
        var y = new double[n];
        for (int i = 0; i < n; i++) {
          y[i] = 3.0 * Math.Exp(-0.3 * x1[i]) + 4.0 * Math.Exp(0.5 * x2[i]) + 5.0;
        }

        var code = new NativeInterpreter.NativeInstruction[8];

        // first term
        code[0] = EmitConst(1.0);
        code[1] = EmitVar(x1Handle, 1.0);
        code[2] = Emit(OpCode.Mul, code[0], code[1]);
        code[3] = Emit(OpCode.Exp, code[2]);

        // second term
        code[4] = EmitConst(1.0);
        code[5] = EmitVar(x2Handle, 1.0);
        code[6] = Emit(OpCode.Mul, code[4], code[5]);
        code[7] = Emit(OpCode.Exp, code[6]);

        var termIndices = new int[] { 3, 7 };
        var rows = Enumerable.Range(0, n).ToArray();
        var coeff = new double[termIndices.Length + 1];
        var options = new SolverOptions();
        var y_pred = new double[y.Length];
        NativeInterpreter.NativeWrapper.GetValuesVarPro(code, code.Length, termIndices, termIndices.Length, rows, nRows: n, coeff, options, y_pred, y, out var optSummary);
        Assert.AreEqual(code[0].value, -0.3, 1e-6);
        Assert.AreEqual(code[1].value, 1.0);
        Assert.AreEqual(code[4].value, 0.5, 1e-6);
        Assert.AreEqual(code[5].value, 1.0);
        Assert.AreEqual(coeff[0], 3.0, 1e-6);
        Assert.AreEqual(coeff[1], 4.0, 1e-6);
        Assert.AreEqual(coeff[2], 5.0, 1e-6);

        Array.Clear(coeff, 0, coeff.Length);
        Array.Clear(y_pred, 0, y_pred.Length);
        NativeInterpreter.NativeWrapper.GetValuesVarPro(code, code.Length, termIndices, termIndices.Length, rows, nRows: n, coeff, options, y_pred, y, out optSummary);
        Assert.AreEqual(code[0].value, -0.3, 1e-6);
        Assert.AreEqual(code[1].value, 1.0);
        Assert.AreEqual(code[4].value, 0.5, 1e-6);
        Assert.AreEqual(code[5].value, 1.0);
        Assert.AreEqual(coeff[0], 3.0, 1e-6);
        Assert.AreEqual(coeff[1], 4.0, 1e-6);
        Assert.AreEqual(coeff[2], 5.0, 1e-6);
      }


      x1Handle.Free();
      x2Handle.Free();
    }

    private NativeInstruction Emit(OpCode opcode, params NativeInstruction[] args) {
      var instr = new NativeInstruction();
      instr.narg = (ushort)args.Length;
      instr.length = args.Sum(argi => argi.length) + 1;
      instr.opcode = (byte)opcode;
      instr.value = 0.0;
      instr.optimize = false;
      return instr;
    }

    private NativeInstruction EmitConst(double v) {
      var instr = new NativeInstruction();
      instr.narg = 0;
      instr.length = 1;
      instr.opcode = (byte)OpCode.Constant;
      instr.value = v;
      instr.optimize = true;
      return instr;
    }

    private NativeInstruction EmitVar(GCHandle gch, double v) {
      var instr = new NativeInstruction();
      instr.data = gch.AddrOfPinnedObject();
      instr.narg = 0;
      instr.length = 1;
      instr.opcode = (byte)OpCode.Variable;
      instr.value = v;
      instr.optimize = false;
      return instr;
    }
  }
}
