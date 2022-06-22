using System;
using System.Runtime.InteropServices;

namespace HeuristicLab.NativeInterpreter {
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  public struct NativeInstruction {
    public int OpCode;
    public int Arity;
    public int Length;
    public int Optimize; // used for parameters only
    public double Coeff; // node coefficient (e.g. variable weight, constant value)
    public IntPtr Data; // used for variables only
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  public struct SolverOptions {
    public int Iterations;
    public int Algorithm; // var pro algorithm
  }

  // proxy structure to pass information from ceres back to the caller
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  public class SolverSummary {
    public double InitialCost;      // value of the objective function before the optimization
    public double FinalCost;        // value of the objective function after the optimization
    public int Iterations;          // number of iterations actually performed by the solver
    public int ResidualEvaluations; // number of residual evaluations
    public int JacobianEvaluations; // number of jacobian evaluations
    public int Success;             // whether the optimization was successful
  }

  public static class NativeWrapper {
    private const string x64dll = "hl-native-interpreter.dll";
    private readonly static bool is64;

    static NativeWrapper() {
      is64 = Environment.Is64BitProcess;
    }

    public static void GetValues(NativeInstruction[] code, int[] rows, double[] result) {
      if (is64) {
        GetValues(code, code.Length, rows, rows.Length, result);
      } else {
        throw new NotSupportedException("Native interpreter is only available on x64 builds");
      }
    }

    public static void Optimize(NativeInstruction[] code, int[] rows, double[] target, double[] weights, SolverOptions options, double[] result, out SolverSummary summary) {
      summary = new SolverSummary();
      if (is64) {
        Optimize(code, code.Length, rows, rows.Length, target, weights, options, result, summary);
      } else {
        throw new NotSupportedException("Native interpreter is only available on x64 builds");
      }
    }
    public static void OptimizeVarPro(NativeInstruction[] code, int[] termIndices, int[] rows, double[] target, double[] weights, double[] coefficients, SolverOptions options, double[] result, out SolverSummary summary) {
      summary = new SolverSummary();
      if (is64) {
        OptimizeVarPro(code, code.Length, termIndices, termIndices.Length, rows, rows.Length, target, weights, coefficients, options, result, summary);
      } else {
        throw new NotSupportedException("Native interpreter is only available on x64 builds");
      }
    }

    [DllImport(x64dll, EntryPoint = "evaluate", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void GetValues(NativeInstruction[] code, int ncode, int[] rows, int nrows, double[] result);


    [DllImport(x64dll, EntryPoint = "optimize", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void Optimize(
      [In, Out] NativeInstruction[] code, // parameters are optimized by callee
      int len,
      int[] rows,
      int nRows,
      double[] target,
      double[] weights,
      SolverOptions options,
      double[] result,
      SolverSummary optSummary);

    [DllImport(x64dll, EntryPoint = "optimize_varpro", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void OptimizeVarPro(
      [In, Out] NativeInstruction[] code,  // the values fields for non-linear parameters are changed by the callee
      int len,
      int[] termIndices,
      int nTerms,
      int[] rows,
      int nRows,
      [In] double[] target,
      [In] double[] weights,
      [In, Out] double[] coefficients,
      [In] SolverOptions options,
      [In, Out] double[] result,
      [In, Out] SolverSummary optSummary);
  }
}
