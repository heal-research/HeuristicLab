#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.Benchmarks {
  [Item("Linpack", "Linpack performance benchmark.")]
  [StorableClass]
  public sealed class Linpack : Benchmark {
    private const int DEFAULT_PSIZE = 1500;

    private double eps_result = 0.0;
    private double mflops_result = 0.0;
    private double residn_result = 0.0;
    private double time_result = 0.0;
    private double total = 0.0;

    private CancellationToken cancellationToken;
    private Stopwatch sw = new Stopwatch();

    [StorableConstructor]
    private Linpack(bool deserializing) : base(deserializing) { }
    private Linpack(Linpack original, Cloner cloner) : base(original, cloner) { }
    public Linpack() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Linpack(this, cloner);
    }

    // implementation based on Java version: http://www.netlib.org/benchmark/linpackjava/
    public override void Run(CancellationToken token, ResultCollection results) {
      cancellationToken = token;
      bool stopBenchmark = false;
      TimeSpan executionTime = new TimeSpan();
      bool resultAchieved = false;
      do {
        int n = DEFAULT_PSIZE;
        int ldaa = DEFAULT_PSIZE;
        int lda = DEFAULT_PSIZE + 1;

        double[][] a = new double[ldaa][];
        double[] b = new double[ldaa];
        double[] x = new double[ldaa];

        double ops;
        double norma;
        double normx;
        double resid;
        int i;
        int info;
        int[] ipvt = new int[ldaa];

        for (i = 0; i < ldaa; i++) {
          a[i] = new double[lda];
        }

        ops = (2.0e0 * (((double)n) * n * n)) / 3.0 + 2.0 * (n * n);

        norma = mathGen(a, lda, n, b);

        if (cancellationToken.IsCancellationRequested) {
          throw new OperationCanceledException(cancellationToken);
        }

        sw.Reset();
        sw.Start();

        info = dgefa(a, lda, n, ipvt);

        if (cancellationToken.IsCancellationRequested) {
          throw new OperationCanceledException(cancellationToken);
        }

        dgesl(a, lda, n, ipvt, b, 0);

        sw.Stop();
        total = sw.Elapsed.TotalMilliseconds / 1000;

        if (cancellationToken.IsCancellationRequested) {
          throw new OperationCanceledException(cancellationToken);
        }

        for (i = 0; i < n; i++) {
          x[i] = b[i];
        }

        norma = mathGen(a, lda, n, b);

        for (i = 0; i < n; i++) {
          b[i] = -b[i];
        }

        dmxpy(n, b, n, lda, x, a);

        resid = 0.0;
        normx = 0.0;

        for (i = 0; i < n; i++) {
          resid = (resid > abs(b[i])) ? resid : abs(b[i]);
          normx = (normx > abs(x[i])) ? normx : abs(x[i]);
        }

        eps_result = epslon((double)1.0);

        residn_result = resid / (n * norma * normx * eps_result);
        residn_result += 0.005; // for rounding
        residn_result = (int)(residn_result * 100);
        residn_result /= 100;

        time_result = total;
        time_result += 0.005; // for rounding
        time_result = (int)(time_result * 100);
        time_result /= 100;

        mflops_result = ops / (1.0e6 * total);
        mflops_result += 0.0005; // for rounding
        mflops_result = (int)(mflops_result * 1000);
        mflops_result /= 1000;

        if (!resultAchieved) {
          results.Add(new Result("Mflops/s", new DoubleValue(mflops_result)));
          results.Add(new Result("Total Mflops/s", new DoubleValue(mflops_result * Environment.ProcessorCount)));
          resultAchieved = true;
        }

        executionTime += sw.Elapsed;
        if ((TimeLimit == null) || (TimeLimit.TotalMilliseconds == 0))
          stopBenchmark = true;
        else if (executionTime > TimeLimit)
          stopBenchmark = true;
      } while (!stopBenchmark);
    }

    private double abs(double d) {
      return (d >= 0) ? d : -d;
    }

    private double mathGen(double[][] a, int lda, int n, double[] b) {
      Random gen;
      double norma;
      int init, i, j;

      init = 1325;
      norma = 0.0;

      gen = new Random(init);

      if (cancellationToken.IsCancellationRequested) {
        throw new OperationCanceledException(cancellationToken);
      }

      // Next two for() statements switched.  Solver wants
      // matrix in column order. --dmd 3/3/97

      for (i = 0; i < n; i++) {
        for (j = 0; j < n; j++) {
          a[j][i] = gen.NextDouble() - .5;
          norma = (a[j][i] > norma) ? a[j][i] : norma;
        }
      }

      for (i = 0; i < n; i++) {
        b[i] = 0.0;
      }

      for (j = 0; j < n; j++) {
        for (i = 0; i < n; i++) {
          b[i] += a[j][i];
        }
      }

      return norma;
    }

    private int dgefa(double[][] a, int lda, int n, int[] ipvt) {
      double[] col_k, col_j;
      double t;
      int j, k, kp1, l, nm1;
      int info;

      if (cancellationToken.IsCancellationRequested) {
        throw new OperationCanceledException(cancellationToken);
      }

      // gaussian elimination with partial pivoting

      info = 0;
      nm1 = n - 1;
      if (nm1 >= 0) {
        for (k = 0; k < nm1; k++) {
          col_k = a[k];
          kp1 = k + 1;

          // find l = pivot index

          l = idamax(n - k, col_k, k, 1) + k;
          ipvt[k] = l;

          // zero pivot implies this column already triangularized

          if (col_k[l] != 0) {
            // interchange if necessary

            if (l != k) {
              t = col_k[l];
              col_k[l] = col_k[k];
              col_k[k] = t;
            }

            if (cancellationToken.IsCancellationRequested) {
              throw new OperationCanceledException(cancellationToken);
            }

            // compute multipliers

            t = -1.0 / col_k[k];
            dscal(n - (kp1), t, col_k, kp1, 1);

            if (cancellationToken.IsCancellationRequested) {
              throw new OperationCanceledException(cancellationToken);
            }

            // row elimination with column indexing

            for (j = kp1; j < n; j++) {
              col_j = a[j];
              t = col_j[l];
              if (l != k) {
                col_j[l] = col_j[k];
                col_j[k] = t;
              }
              daxpy(n - (kp1), t, col_k, kp1, 1,
                col_j, kp1, 1);
            }
          } else {
            info = k;
          }
        }
      }

      ipvt[n - 1] = n - 1;
      if (a[(n - 1)][(n - 1)] == 0) info = n - 1;

      return info;
    }

    private void dgesl(double[][] a, int lda, int n, int[] ipvt, double[] b, int job) {
      double t;
      int k, kb, l, nm1, kp1;

      if (cancellationToken.IsCancellationRequested) {
        throw new OperationCanceledException(cancellationToken);
      }

      nm1 = n - 1;
      if (job == 0) {
        // job = 0 , solve  a * x = b.  first solve  l*y = b

        if (nm1 >= 1) {
          for (k = 0; k < nm1; k++) {
            l = ipvt[k];
            t = b[l];
            if (l != k) {
              b[l] = b[k];
              b[k] = t;
            }
            kp1 = k + 1;
            daxpy(n - (kp1), t, a[k], kp1, 1, b, kp1, 1);
          }
        }

        if (cancellationToken.IsCancellationRequested) {
          throw new OperationCanceledException(cancellationToken);
        }

        // now solve  u*x = y

        for (kb = 0; kb < n; kb++) {
          k = n - (kb + 1);
          b[k] /= a[k][k];
          t = -b[k];
          daxpy(k, t, a[k], 0, 1, b, 0, 1);
        }
      } else {
        // job = nonzero, solve  trans(a) * x = b.  first solve  trans(u)*y = b

        for (k = 0; k < n; k++) {
          t = ddot(k, a[k], 0, 1, b, 0, 1);
          b[k] = (b[k] - t) / a[k][k];
        }

        if (cancellationToken.IsCancellationRequested) {
          throw new OperationCanceledException(cancellationToken);
        }

        // now solve trans(l)*x = y 

        if (nm1 >= 1) {
          //for (kb = 1; kb < nm1; kb++) {
          for (kb = 0; kb < nm1; kb++) {
            k = n - (kb + 1);
            kp1 = k + 1;
            b[k] += ddot(n - (kp1), a[k], kp1, 1, b, kp1, 1);
            l = ipvt[k];
            if (l != k) {
              t = b[l];
              b[l] = b[k];
              b[k] = t;
            }
          }
        }
      }
    }

    private void daxpy(int n, double da, double[] dx, int dx_off, int incx, double[] dy, int dy_off, int incy) {
      int i, ix, iy;

      if (cancellationToken.IsCancellationRequested) {
        throw new OperationCanceledException(cancellationToken);
      }

      if ((n > 0) && (da != 0)) {
        if (incx != 1 || incy != 1) {

          // code for unequal increments or equal increments not equal to 1

          ix = 0;
          iy = 0;
          if (incx < 0) ix = (-n + 1) * incx;
          if (incy < 0) iy = (-n + 1) * incy;
          for (i = 0; i < n; i++) {
            dy[iy + dy_off] += da * dx[ix + dx_off];
            ix += incx;
            iy += incy;
          }
          return;
        } else {
          // code for both increments equal to 1

          for (i = 0; i < n; i++)
            dy[i + dy_off] += da * dx[i + dx_off];
        }
      }
    }

    private double ddot(int n, double[] dx, int dx_off, int incx, double[] dy, int dy_off, int incy) {
      double dtemp = 0;
      int i, ix, iy;

      if (cancellationToken.IsCancellationRequested) {
        throw new OperationCanceledException(cancellationToken);
      }

      if (n > 0) {
        if (incx != 1 || incy != 1) {
          // code for unequal increments or equal increments not equal to 1

          ix = 0;
          iy = 0;
          if (incx < 0) ix = (-n + 1) * incx;
          if (incy < 0) iy = (-n + 1) * incy;
          for (i = 0; i < n; i++) {
            dtemp += dx[ix + dx_off] * dy[iy + dy_off];
            ix += incx;
            iy += incy;
          }
        } else {
          // code for both increments equal to 1

          for (i = 0; i < n; i++)
            dtemp += dx[i + dx_off] * dy[i + dy_off];
        }
      }
      return (dtemp);
    }

    private void dscal(int n, double da, double[] dx, int dx_off, int incx) {
      int i, nincx;

      if (cancellationToken.IsCancellationRequested) {
        throw new OperationCanceledException(cancellationToken);
      }

      if (n > 0) {
        if (incx != 1) {
          // code for increment not equal to 1

          nincx = n * incx;
          for (i = 0; i < nincx; i += incx)
            dx[i + dx_off] *= da;
        } else {
          // code for increment equal to 1

          for (i = 0; i < n; i++)
            dx[i + dx_off] *= da;
        }
      }
    }

    private int idamax(int n, double[] dx, int dx_off, int incx) {
      double dmax, dtemp;
      int i, ix, itemp = 0;

      if (cancellationToken.IsCancellationRequested) {
        throw new OperationCanceledException(cancellationToken);
      }

      if (n < 1) {
        itemp = -1;
      } else if (n == 1) {
        itemp = 0;
      } else if (incx != 1) {
        // code for increment not equal to 1

        dmax = (dx[dx_off] < 0.0) ? -dx[dx_off] : dx[dx_off];
        ix = 1 + incx;
        for (i = 0; i < n; i++) {
          dtemp = (dx[ix + dx_off] < 0.0) ? -dx[ix + dx_off] : dx[ix + dx_off];
          if (dtemp > dmax) {
            itemp = i;
            dmax = dtemp;
          }
          ix += incx;
        }
      } else {
        // code for increment equal to 1

        itemp = 0;
        dmax = (dx[dx_off] < 0.0) ? -dx[dx_off] : dx[dx_off];
        for (i = 0; i < n; i++) {
          dtemp = (dx[i + dx_off] < 0.0) ? -dx[i + dx_off] : dx[i + dx_off];
          if (dtemp > dmax) {
            itemp = i;
            dmax = dtemp;
          }
        }
      }
      return (itemp);
    }

    private double epslon(double x) {
      double a, b, c, eps;

      a = 4.0e0 / 3.0e0;
      eps = 0;
      while (eps == 0) {
        b = a - 1.0;
        c = b + b + b;
        eps = abs(c - 1.0);
      }
      return (eps * abs(x));
    }

    private void dmxpy(int n1, double[] y, int n2, int ldm, double[] x, double[][] m) {
      int j, i;

      // cleanup odd vector
      for (j = 0; j < n2; j++) {
        for (i = 0; i < n1; i++) {
          y[i] += x[j] * m[j][i];
        }
      }
    }
  }
}
