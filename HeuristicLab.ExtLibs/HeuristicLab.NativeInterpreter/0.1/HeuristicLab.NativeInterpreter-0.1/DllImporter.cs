using System;
using System.Runtime.InteropServices;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  public struct NativeInstruction {
    public byte opcode;
    public ushort narg;
    public int childIndex;
    public double value;
    public double weight;

    public IntPtr buf;
    public IntPtr data;
  }

  public static class NativeWrapper {
    private const string x86dll = "hl-native-interpreter-msvc-x86.dll";
    private const string x64dll = "hl-native-interpreter-msvc-x64.dll";

    private readonly static bool is64;

    static NativeWrapper() {
      is64 = Environment.Is64BitProcess;
    }

    public static double GetValue(NativeInstruction[] code, int len, int row) {
      return is64 ? GetValue64(code, len, row) : GetValue32(code, len, row);
    }

    public static void GetValues(NativeInstruction[] code, int len, int[] rows, int nRows, double[] result) {
      if (is64)
        GetValues64(code, len, rows, nRows, result);
      else
        GetValues32(code, len, rows, nRows, result);
    }

    public static void GetValuesVectorized(NativeInstruction[] code, int len, int[] rows, int nRows, double[] result) {
      if (is64)
        GetValuesVectorized64(code, len, rows, nRows, result);
      else
        GetValuesVectorized32(code, len, rows, nRows, result);
    }

    // x86
    [DllImport(x86dll, EntryPoint = "GetValue", CallingConvention = CallingConvention.Cdecl)]
    internal static extern double GetValue32(NativeInstruction[] code, int len, int row);

    [DllImport(x86dll, EntryPoint = "GetValues", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void GetValues32(NativeInstruction[] code, int len, int[] rows, int nRows, double[] result);

    [DllImport(x86dll, EntryPoint = "GetValuesVectorized", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void GetValuesVectorized32(NativeInstruction[] code, int len, int[] rows, int nRows, double[] result);

    // x64
    [DllImport(x64dll, EntryPoint = "GetValue", CallingConvention = CallingConvention.Cdecl)]
    internal static extern double GetValue64(NativeInstruction[] code, int len, int row);

    [DllImport(x64dll, EntryPoint = "GetValues", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void GetValues64(NativeInstruction[] code, int len, int[] rows, int nRows, double[] result);

    [DllImport(x64dll, EntryPoint = "GetValuesVectorized", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void GetValuesVectorized64(NativeInstruction[] code, int len, int[] rows, int nRows, double[] result);
  }
}
