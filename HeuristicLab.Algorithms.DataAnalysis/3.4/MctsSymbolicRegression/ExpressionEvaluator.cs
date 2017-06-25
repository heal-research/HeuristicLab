#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics.Contracts;
using System.Linq;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {
  // evalutes expressions (on vectors)
  internal class ExpressionEvaluator {
    // manages it's own vector buffers
    private readonly double[][] vectorBuffers;
    private readonly double[][] scalarBuffers; // scalars are vectors of length 1 (to allow mixing scalars and vectors on the same stack)
    private int lastVecBufIdx;
    private int lastScalarBufIdx;


    private double[] GetVectorBuffer() {
      return vectorBuffers[--lastVecBufIdx];
    }
    private double[] GetScalarBuffer() {
      return scalarBuffers[--lastScalarBufIdx];
    }

    private void ReleaseBuffer(double[] buf) {
      if (buf.Length == 1) {
        scalarBuffers[lastScalarBufIdx++] = buf;
      } else {
        vectorBuffers[lastVecBufIdx++] = buf;
      }
    }

    public const int MaxStackSize = 100;
    public const int MaxParams = 50;
    private readonly int vLen;
    private readonly double lowerEstimationLimit;
    private readonly double upperEstimationLimit;
    private readonly double nanReplacementValue;

    private readonly double[][] stack;
    private readonly double[][][] gradientStack;

    // preallocate stack and gradient stack 
    public ExpressionEvaluator(int vLen, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue) {
      if (vLen <= 1) throw new ArgumentException("number of elements in a variable must be > 1", "vlen");
      this.vLen = vLen;
      this.lowerEstimationLimit = lowerEstimationLimit;
      this.upperEstimationLimit = upperEstimationLimit;
      this.nanReplacementValue = (upperEstimationLimit - lowerEstimationLimit) / 2.0 + lowerEstimationLimit;

      stack = new double[MaxStackSize][];
      gradientStack = new double[MaxParams][][];

      for (int k = 0; k < MaxParams; k++) {
        gradientStack[k] = new double[MaxStackSize][];
      }

      // preallocate buffers 
      vectorBuffers = new double[MaxStackSize * (1 + MaxParams)][];
      scalarBuffers = new double[MaxStackSize * (1 + MaxParams)][];
      for (int i = 0; i < MaxStackSize; i++) {
        ReleaseBuffer(new double[vLen]);
        ReleaseBuffer(new double[1]);

        for (int k = 0; k < MaxParams; k++) {
          ReleaseBuffer(new double[vLen]);
          ReleaseBuffer(new double[1]);
        }
      }
    }

    // pred must be allocated by the caller
    // if adjustOffsetForLogAndExp is set to true we determine c in log(c + f(x)) to make sure that c + f(x) is positive
    public void Exec(byte[] code, double[][] vars, double[] consts, double[] pred, bool adjustOffsetForLogAndExp = false) {
      Contract.Assert(pred != null && pred.Length >= vLen);
      int topOfStack = -1;
      int pc = 0;
      int curParamIdx = -1;
      byte op;
      short arg;
      // checked at the end to make sure we do not leak buffers
      int initialScalarCount = lastScalarBufIdx;
      int initialVectorCount = lastVecBufIdx;

      while (true) {
        ReadNext(code, ref pc, out op, out arg);
        switch (op) {
          case (byte)OpCodes.Nop: throw new InvalidProgramException(); // not allowed
          case (byte)OpCodes.LoadConst0: {
              ++topOfStack;
              var z = GetScalarBuffer();
              z[0] = 0;
              stack[topOfStack] = z;
              break;
            }
          case (byte)OpCodes.LoadConst1: {
              ++topOfStack;
              var z = GetScalarBuffer();
              z[0] = 1.0;
              stack[topOfStack] = z;
              break;
            }
          case (byte)OpCodes.LoadParamN: {
              ++topOfStack;
              var c = consts[++curParamIdx];
              var z = GetScalarBuffer();
              z[0] = c;
              stack[topOfStack] = z;
              break;
            }
          case (byte)OpCodes.LoadVar: {
              ++topOfStack;
              var z = GetVectorBuffer();
              Array.Copy(vars[arg], z, vars[arg].Length);
              stack[topOfStack] = z;
              break;
            }
          case (byte)OpCodes.Add: {
              topOfStack--;
              var a = stack[topOfStack + 1];
              var b = stack[topOfStack];
              stack[topOfStack] = Add(a, b);
              ReleaseBuffer(a);
              ReleaseBuffer(b);
              break;
            }
          case (byte)OpCodes.Mul: {
              topOfStack--;
              var a = stack[topOfStack + 1];
              var b = stack[topOfStack];
              stack[topOfStack] = Mul(a, b);
              ReleaseBuffer(a);
              ReleaseBuffer(b);
              break;
            }
          case (byte)OpCodes.Log: {
              if (adjustOffsetForLogAndExp) {
                // here we assume that the last used parameter is c in log(f(x) + c)
                // this must match actions for producing code in the automaton!

                // we can easily adjust c to make sure that f(x) + c is positive because at this point we all values for f(x)
                var fxc = stack[topOfStack];
                var minFx = fxc.Min() - consts[curParamIdx]; // stack[topOfStack] is f(x) + c

                var delta = 1.0 - minFx - consts[curParamIdx];
                // adjust c so that minFx + c = 1 ... log(minFx + c) = 0
                consts[curParamIdx] += delta;

                // also adjust values on stack
                for (int i = 0; i < fxc.Length; i++) fxc[i] += delta;
              }
              var x = stack[topOfStack];
              for (int i = 0; i < x.Length; i++)
                x[i] = Math.Log(x[i]);
              break;
            }
          case (byte)OpCodes.Exp: {
              if (adjustOffsetForLogAndExp) {
                // here we assume that the last used parameter is c in exp(f(x) * c)
                // this must match actions for producing code in the automaton!

                // adjust c to make sure that exp(f(x) * c) is not too large
                var fxc = stack[topOfStack];
                var maxFx = fxc.Max() / consts[curParamIdx]; // stack[topOfStack] is f(x) * c

                var f = 1.0 / (maxFx * consts[curParamIdx]);
                // adjust c so that maxFx*c = 1 TODO: this is not ideal as it enforces positive arguments to exp()
                consts[curParamIdx] *= f;

                // also adjust values on stack
                for (int i = 0; i < fxc.Length; i++) fxc[i] *= f;
              }

              var x = stack[topOfStack];
              for (int i = 0; i < x.Length; i++)
                x[i] = Math.Exp(x[i]);
              break;
            }
          case (byte)OpCodes.Inv: {
              var x = stack[topOfStack];
              for (int i = 0; i < x.Length; i++)
                x[i] = 1.0 / (x[i]);
              break;
            }
          case (byte)OpCodes.Exit:
            Contract.Assert(topOfStack == 0);
            var r = stack[topOfStack];
            if (r.Length == 1) {
              var v = double.IsNaN(r[0]) ? nanReplacementValue : Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, r[0]));
              for (int i = 0; i < vLen; i++)
                pred[i] = v;
            } else {
              for (int i = 0; i < vLen; i++) {
                var v = double.IsNaN(r[i]) ? nanReplacementValue : Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, r[i]));
                pred[i] = v;
              }
            }
            ReleaseBuffer(r);
            Contract.Assert(lastVecBufIdx == initialVectorCount);
            Contract.Assert(lastScalarBufIdx == initialScalarCount);
            return;
        }
      }
    }


    // evaluation with forward autodiff
    // pred and gradients must be allocated by the caller
    public void ExecGradient(byte[] code, double[][] vars, double[] consts, double[] pred, double[][] gradients) {
      Contract.Assert(pred != null && pred.Length >= vLen);
      int topOfStack = -1;
      int pc = 0;
      int curParamIdx = -1;
      byte op;
      short arg;
      int nParams = consts.Length;
      Contract.Assert(gradients != null && gradients.Length >= nParams && gradients.All(g => g.Length >= vLen));

      // checked at the end to make sure we do not leak buffers
      int initialScalarCount = lastScalarBufIdx;
      int initialVectorCount = lastVecBufIdx;

      while (true) {
        ReadNext(code, ref pc, out op, out arg);
        switch (op) {
          case (byte)OpCodes.Nop: throw new InvalidProgramException(); // not allowed
          case (byte)OpCodes.LoadConst0: {
              ++topOfStack;
              var z = GetScalarBuffer();
              z[0] = 0;
              stack[topOfStack] = z;
              for (int k = 0; k < nParams; ++k) {
                var b = GetScalarBuffer();
                b[0] = 0.0;
                gradientStack[k][topOfStack] = b;
              }
              break;
            }
          case (byte)OpCodes.LoadConst1: {
              ++topOfStack;
              var z = GetScalarBuffer();
              z[0] = 1.0;
              stack[topOfStack] = z;
              for (int k = 0; k < nParams; ++k) {
                var b = GetScalarBuffer();
                b[0] = 0.0;
                gradientStack[k][topOfStack] = b;
              }
              break;
            }
          case (byte)OpCodes.LoadParamN: {
              var c = consts[++curParamIdx];
              ++topOfStack;
              var z = GetScalarBuffer();
              z[0] = c;
              stack[topOfStack] = z;
              for (int k = 0; k < nParams; ++k) {
                var b = GetScalarBuffer();
                b[0] = k == curParamIdx ? 1.0 : 0.0;
                gradientStack[k][topOfStack] = b;
              }
              break;
            }
          case (byte)OpCodes.LoadVar: {
              ++topOfStack;
              var z = GetVectorBuffer();
              Array.Copy(vars[arg], z, vars[arg].Length);
              stack[topOfStack] = z;
              for (int k = 0; k < nParams; ++k) {
                var b = GetScalarBuffer();
                b[0] = 0.0;
                gradientStack[k][topOfStack] = b;
              }
            }
            break;
          case (byte)OpCodes.Add: {
              topOfStack--;
              var a = stack[topOfStack + 1];
              var b = stack[topOfStack];
              stack[topOfStack] = Add(a, b);
              ReleaseBuffer(a);
              ReleaseBuffer(b);

              // same for gradient
              for (int k = 0; k < nParams; ++k) {
                var ag = gradientStack[k][topOfStack + 1];
                var bg = gradientStack[k][topOfStack];
                gradientStack[k][topOfStack] = Add(ag, bg);
                ReleaseBuffer(ag);
                ReleaseBuffer(bg);
              }
              break;
            }
          case (byte)OpCodes.Mul: {
              topOfStack--;
              var a = stack[topOfStack + 1];
              var b = stack[topOfStack];
              stack[topOfStack] = Mul(a, b);

              // same for gradient
              // f(x) g(x)	f '(x) g(x) + f(x) g'(x)
              for (int k = 0; k < nParams; ++k) {
                var ag = gradientStack[k][topOfStack + 1];
                var bg = gradientStack[k][topOfStack];
                var t1 = Mul(ag, b);
                var t2 = Mul(a, bg);
                gradientStack[k][topOfStack] = Add(t1, t2);
                ReleaseBuffer(ag);
                ReleaseBuffer(bg);
                ReleaseBuffer(t1);
                ReleaseBuffer(t2);
              }

              ReleaseBuffer(a);
              ReleaseBuffer(b);

              break;
            }
          case (byte)OpCodes.Log: {
              var x = stack[topOfStack];
              // calc gradients first before destroying x
              // log(f(x))' = f(x)'/f(x)
              for (int k = 0; k < nParams; k++) {
                var xg = gradientStack[k][topOfStack];
                gradientStack[k][topOfStack] = Frac(xg, x);
                ReleaseBuffer(xg);
              }

              for (int i = 0; i < x.Length; i++)
                x[i] = Math.Log(x[i]);

              break;
            }
          case (byte)OpCodes.Exp: {
              var x = stack[topOfStack];
              for (int i = 0; i < x.Length; i++)
                x[i] = Math.Exp(x[i]);

              for (int k = 0; k < nParams; k++) {
                var xg = gradientStack[k][topOfStack];
                gradientStack[k][topOfStack] = Mul(x, xg);  // e(f(x))' = e(f(x)) * f(x)'
                ReleaseBuffer(xg);
              }
              break;
            }
          case (byte)OpCodes.Inv: {
              var x = stack[topOfStack];
              for (int i = 0; i < x.Length; i++)
                x[i] = 1.0 / x[i];

              for (int k = 0; k < nParams; k++) {
                var xg = gradientStack[k][topOfStack];
                // x has already been inverted above
                // (1/f)' = -f' / f²
                var invF = Mul(xg, x);
                gradientStack[k][topOfStack] = Mul(invF, x, factor: -1.0);
                ReleaseBuffer(xg);
                ReleaseBuffer(invF);
              }
              break;
            }
          case (byte)OpCodes.Exit:
            Contract.Assert(topOfStack == 0);
            var r = stack[topOfStack];
            if (r.Length == 1) {
              var v = double.IsNaN(r[0]) ? nanReplacementValue : Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, r[0]));
              for (int i = 0; i < vLen; i++)
                pred[i] = v;
            } else {
              for (int i = 0; i < vLen; i++) {
                var v = double.IsNaN(r[i]) ? nanReplacementValue : Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, r[i]));
                pred[i] = v;
              }
            }
            ReleaseBuffer(r);

            // same for gradients
            for (int k = 0; k < nParams; k++) {
              var g = gradientStack[k][topOfStack];
              if (g.Length == 1) {
                for (int i = 0; i < vLen; i++)
                  gradients[k][i] = g[0];
              } else
                Array.Copy(g, gradients[k], vLen);
              ReleaseBuffer(g);
            }

            Contract.Assert(lastVecBufIdx == initialVectorCount);
            Contract.Assert(lastScalarBufIdx == initialScalarCount);
            return; // break loop
        }
      }
    }

    private double[] Add(double[] a, double[] b) {
      double[] target = null;
      if (a.Length > 1) {
        target = GetVectorBuffer();
        if (b.Length > 1) {
          for (int i = 0; i < vLen; i++)
            target[i] = a[i] + b[i];
        } else {
          // b == scalar
          for (int i = 0; i < vLen; i++)
            target[i] = a[i] + b[0];
        }
      } else {
        // a == scalar
        if (b.Length > 1) {
          target = GetVectorBuffer();
          for (int i = 0; i < vLen; i++)
            target[i] = a[0] + b[i];
        } else {
          // b == scalar
          target = GetScalarBuffer();
          target[0] = a[0] + b[0];
        }
      }
      return target;
    }

    private double[] Mul(double[] a, double[] b, double factor = 1.0) {
      double[] target = null;
      if (a.Length > 1) {
        if (b.Length > 1) {
          target = GetVectorBuffer();
          for (int i = 0; i < vLen; i++)
            target[i] = factor * a[i] * b[i];
        } else {
          // b == scalar
          if (Math.Abs(b[0]) < 1E-12 /* == 0 */) {
            target = GetScalarBuffer();
            target[0] = 0.0;
          } else {
            target = GetVectorBuffer();
            for (int i = 0; i < vLen; i++)
              target[i] = factor * a[i] * b[0];
          }
        }
      } else {
        // a == scalar
        if (b.Length > 1) {
          if (Math.Abs(a[0]) < 1E-12 /* == 0 */) {
            target = GetScalarBuffer();
            target[0] = 0.0;
          } else {
            target = GetVectorBuffer();
            for (int i = 0; i < vLen; i++)
              target[i] = factor * a[0] * b[i];
          }
        } else {
          // b == scalar
          target = GetScalarBuffer();
          target[0] = factor * a[0] * b[0];
        }
      }
      return target;
    }

    private double[] Frac(double[] a, double[] b) {
      double[] target = null;
      if (a.Length > 1) {
        target = GetVectorBuffer();
        if (b.Length > 1) {
          for (int i = 0; i < vLen; i++)
            target[i] = a[i] / b[i];
        } else {
          // b == scalar
          for (int i = 0; i < vLen; i++)
            target[i] = a[i] / b[0];
        }
      } else {
        // a == scalar
        if (b.Length > 1) {
          if (Math.Abs(a[0]) < 1E-12 /* == 0 */) {
            target = GetScalarBuffer();
            target[0] = 0.0;
          } else {
            target = GetVectorBuffer();
            for (int i = 0; i < vLen; i++)
              target[i] = a[0] / b[i];
          }
        } else {
          // b == scalar
          target = GetScalarBuffer();
          target[0] = a[0] / b[0];
        }
      }
      return target;
    }

    private void ReadNext(byte[] code, ref int pc, out byte op, out short s) {
      op = code[pc++];
      s = 0;
      if (op == (byte)OpCodes.LoadVar) {
        s = (short)((code[pc] << 8) | code[pc + 1]);
        pc += 2;
      }
    }
  }
}
