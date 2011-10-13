using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.Benchmarks {
  [Item("Linpack Algorithm", "A Linpack benchmark algorithm.")]
  [Creatable("Benchmarks")]
  [StorableClass]
  public class Linpack : Algorithm {
    private DateTime lastUpdateTime;

    [Storable]
    private ResultCollection results;

    #region Benchmark Fields

    private const int DEFAULT_PSIZE = 1500;

    private double eps_result = 0.0;
    private double mflops_result = 0.0;
    private double residn_result = 0.0;
    private double time_result = 0.0;
    private double total = 0.0;

    private Stopwatch sw = new Stopwatch();

    #endregion

    #region Properties

    public override ResultCollection Results {
      get { return results; }
    }

    #endregion

    #region Costructors

    public Linpack()
      : base() {
      results = new ResultCollection();
    }

    private Linpack(Linpack original, Cloner cloner)
      : base(original, cloner) {
      results = new ResultCollection();
    }

    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Linpack(this, cloner);
    }

    public override void Prepare() {
      results.Clear();
      OnPrepared();
    }

    public override void Start() {
      var cancellationTokenSource = new CancellationTokenSource();
      OnStarted();
      Task task = Task.Factory.StartNew(Run, cancellationTokenSource.Token, cancellationTokenSource.Token);
      task.ContinueWith(t => {
        try {
          t.Wait();
        }
        catch (AggregateException ex) {
          try {
            ex.Flatten().Handle(x => x is OperationCanceledException);
          }
          catch (AggregateException remaining) {
            if (remaining.InnerExceptions.Count == 1) OnExceptionOccurred(remaining.InnerExceptions[0]);
            else OnExceptionOccurred(remaining);
          }
        }
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
        OnStopped();
      });
    }

    private void Run(object state) {
      CancellationToken cancellationToken = (CancellationToken)state;
      lastUpdateTime = DateTime.Now;
      System.Timers.Timer timer = new System.Timers.Timer(250);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
      timer.Start();
      try {
        RunBenchmark();
      }
      finally {
        timer.Elapsed -= new System.Timers.ElapsedEventHandler(timer_Elapsed);
        timer.Stop();
        ExecutionTime += DateTime.Now - lastUpdateTime;
      }

      cancellationToken.ThrowIfCancellationRequested();
    }

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      System.Timers.Timer timer = (System.Timers.Timer)sender;
      timer.Enabled = false;
      DateTime now = DateTime.Now;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
      timer.Enabled = true;
    }

    #region Linpack Benchmark

    private void RunBenchmark() {
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

      sw.Reset();
      sw.Start();

      info = dgefa(a, lda, n, ipvt);

      dgesl(a, lda, n, ipvt, b, 0);

      sw.Stop();
      total = sw.Elapsed.TotalMilliseconds / 1000;

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

      //System.Console.WriteLine("Mflops/s: " + mflops_result + "  Time: " + time_result + " secs" + "  Norm Res: " + residn_result + "  Precision: " + eps_result);

      Results.Add(new Result("Mflops/s", new DoubleValue(mflops_result)));
      //Results.Add(new Result("ca. Mflops/s", new DoubleValue(mflops_result * Environment.ProcessorCount)));
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

            // compute multipliers

            t = -1.0 / col_k[k];
            dscal(n - (kp1), t, col_k, kp1, 1);

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

    #endregion
  }
}
