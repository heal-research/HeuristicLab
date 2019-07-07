using System;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class BatchOperations {
    public const int BATCHSIZE = 64;

    public static void Load(double[] a, double[] b) {
      Array.Copy(b, a, BATCHSIZE);
    }

    public static void Add(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i) {
        a[i] += b[i];
      }
    }

    public static void Sub(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i) {
        a[i] -= b[i];
      }
    }

    public static void Div(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i) {
        a[i] /= b[i];
      }
    }

    public static void Mul(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i) {
        a[i] *= b[i];
      }
    }

    public static void Neg(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i) {
        a[i] = -b[i];
      }
    }

    public static void Inv(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i) {
        a[i] = 1 / b[i];
      }
    }

    public static void Log(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Log(b[i]);
    }

    public static void Exp(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Exp(b[i]);
    }

    public static void Sin(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Sin(b[i]);
    }

    public static void Cos(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Cos(b[i]);
    }

    public static void Tan(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Tan(b[i]);
    }
    public static void Tanh(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Tanh(b[i]);
    }

    public static void Pow(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Pow(a[i], Math.Round(b[i]));
    }

    public static void Root(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Pow(a[i], 1 / Math.Round(b[i]));
    }

    public static void Square(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Pow(b[i], 2d);
    }

    public static void Sqrt(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Sqrt(b[i]);
    }

    public static void Cube(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Pow(b[i], 3d);
    }

    public static void CubeRoot(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = b[i] < 0 ? -Math.Pow(-b[i], 1d / 3d) : Math.Pow(b[i], 1d / 3d);
    }

    public static void Absolute(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = Math.Abs(b[i]);
    }

    public static void AnalyticQuotient(double[] a, double[] b) {
      for (int i = 0; i < BATCHSIZE; ++i)
        a[i] = a[i] / Math.Sqrt(1d + b[i] * b[i]);
    }
  }
}
