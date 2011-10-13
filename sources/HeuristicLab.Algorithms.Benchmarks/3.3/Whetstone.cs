using System;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.Benchmarks {
  [Item("Whetstone Algorithm", "A Whetstone benchmark algorithm.")]
  [Creatable("Benchmarks")]
  [StorableClass]
  public class Whetstone : Algorithm {
    private DateTime lastUpdateTime;

    [Storable]
    private ResultCollection results;

    #region Benchmark Fields

    private long begin_time;
    private long end_time;
    //private long total_time;

    private int ITERATIONS;
    private int numberOfCycles;
    private int cycleNo;
    private double x1, x2, x3, x4, x, y, t, t1, t2;
    private double[] z = new double[1];
    private double[] e1 = new double[4];
    private int i, j, k, l, n1, n2, n3, n4, n6, n7, n8, n9, n10, n11;

    #endregion

    #region Properties

    public override ResultCollection Results {
      get { return results; }
    }

    #endregion

    #region Costructors

    public Whetstone()
      : base() {
      results = new ResultCollection();
    }

    private Whetstone(Whetstone original, Cloner cloner)
      : base(original, cloner) {
      results = new ResultCollection();
    }

    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Whetstone(this, cloner);
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

    #region Whetstone Benchmark

    private void RunBenchmark() {
      ITERATIONS = 100; // ITERATIONS / 10 = Millions Whetstone instructions

      numberOfCycles = 100;
      int numberOfRuns = 10;
      float elapsedTime = 0;
      float meanTime = 0;
      float rating = 0;
      float meanRating = 0;
      int intRating = 0;

      for (int runNumber = 1; runNumber <= numberOfRuns; runNumber++) {
        //System.Console.WriteLine(runNumber + ". Test");
        elapsedTime = (float)(MainCalc() / 1000);
        meanTime = meanTime + (elapsedTime * 1000 / numberOfCycles);
        rating = (1000 * numberOfCycles) / elapsedTime;
        meanRating = meanRating + rating;
        intRating = (int)rating;
        numberOfCycles += 10;
      }

      meanTime = meanTime / numberOfRuns;
      meanRating = meanRating / numberOfRuns;
      intRating = (int)meanRating;
      //System.Console.WriteLine("Number of Runs " + numberOfRuns);
      //System.Console.WriteLine("Average time per cycle " + meanTime + " ms.");
      //System.Console.WriteLine("Average Whetstone Rating " + intRating + " KWIPS");
      //Results.Add(new Result("KWIPS", new IntValue(intRating)));
      Results.Add(new Result("MWIPS", new IntValue(intRating / 1000)));
    }

    private double MainCalc() {
      // initialize constants
      t = 0.499975;
      t1 = 0.50025;
      t2 = 2.0;

      // set values of module weights
      n1 = 0 * ITERATIONS;
      n2 = 12 * ITERATIONS;
      n3 = 14 * ITERATIONS;
      n4 = 345 * ITERATIONS;
      n6 = 210 * ITERATIONS;
      n7 = 32 * ITERATIONS;
      n8 = 899 * ITERATIONS;
      n9 = 616 * ITERATIONS;
      n10 = 0 * ITERATIONS;
      n11 = 93 * ITERATIONS;

      begin_time = DateTime.Now.Ticks / 10000; // get ms

      for (cycleNo = 1; cycleNo <= numberOfCycles; cycleNo++) {
        /* MODULE 1: simple identifiers */
        x1 = 1.0;
        x2 = x3 = x4 = -1.0;
        for (i = 1; i <= n1; i += 1) {
          x1 = (x1 + x2 + x3 - x4) * t;
          x2 = (x1 + x2 - x3 + x4) * t; // correction: x2 = ( x1 + x2 - x3 - x4 ) * t;
          x3 = (x1 - x2 + x3 + x4) * t; // correction: x3 = ( x1 - x2 + x3 + x4 ) * t;
          x4 = (-x1 + x2 + x3 + x4) * t;
        }

        /* MODULE 2: array elements */
        e1[0] = 1.0;
        e1[1] = e1[2] = e1[3] = -1.0;
        for (i = 1; i <= n2; i += 1) {
          e1[0] = (e1[0] + e1[1] + e1[2] - e1[3]) * t;
          e1[1] = (e1[0] + e1[1] - e1[2] + e1[3]) * t;
          e1[2] = (e1[0] - e1[1] + e1[2] + e1[3]) * t;
          e1[3] = (-e1[0] + e1[1] + e1[2] + e1[3]) * t;
        }

        /* MODULE 3: array as parameter */
        for (i = 1; i <= n3; i += 1)
          pa(e1);

        /* MODULE 4: conditional jumps */
        j = 1;
        for (i = 1; i <= n4; i += 1) {
          if (j == 1)
            j = 2;
          else
            j = 3;
          if (j > 2)
            j = 0;
          else
            j = 1;
          if (j < 1)
            j = 1;
          else
            j = 0;
        }

        /* MODULE 5: omitted */

        /* MODULE 6: integer arithmetic */
        j = 1;
        k = 2;
        l = 3;
        for (i = 1; i <= n6; i += 1) {
          j = j * (k - j) * (l - k);
          k = l * k - (l - j) * k;
          l = (l - k) * (k + j);
          e1[l - 2] = j + k + l; /* C arrays are zero based */
          e1[k - 2] = j * k * l;
        }

        /* MODULE 7: trig. functions */
        x = y = 0.5;
        for (i = 1; i <= n7; i += 1) {
          x = t * Math.Atan(t2 * Math.Sin(x) * Math.Cos(x) / (Math.Cos(x + y) + Math.Cos(x - y) - 1.0));
          y = t * Math.Atan(t2 * Math.Sin(y) * Math.Cos(y) / (Math.Cos(x + y) + Math.Cos(x - y) - 1.0));
        }

        /* MODULE 8: procedure calls */
        x = y = z[0] = 1.0;
        for (i = 1; i <= n8; i += 1)
          p3(x, y, z);

        /* MODULE9: array references */
        j = 0;
        k = 1;
        l = 2;
        e1[0] = 1.0;
        e1[1] = 2.0;
        e1[2] = 3.0;
        for (i = 1; i <= n9; i++)
          p0();

        /* MODULE10: integer arithmetic */
        j = 2;
        k = 3;
        for (i = 1; i <= n10; i += 1) {
          j = j + k;
          k = j + k;
          j = k - j;
          k = k - j - j;
        }

        /* MODULE11: standard functions */
        x = 0.75;
        for (i = 1; i <= n11; i += 1)
          x = Math.Sqrt(Math.Exp(Math.Log(x) / t1));
      } /* for */

      end_time = DateTime.Now.Ticks / 10000; // get ms
      //System.Console.WriteLine(" (time for " + numberOfCycles + " cycles): " + (end_time - begin_time) + " millisec.");

      return (end_time - begin_time);
    }

    public void pa(double[] e) {
      int j;
      j = 0;
      do {
        e[0] = (e[0] + e[1] + e[2] - e[3]) * t;
        e[1] = (e[0] + e[1] - e[2] + e[3]) * t;
        e[2] = (e[0] - e[1] + e[2] + e[3]) * t;
        e[3] = (-e[0] + e[1] + e[2] + e[3]) / t2;
        j += 1;
      }
      while (j < 6);
    }

    public void p3(double x, double y, double[] z) {
      x = t * (x + y);
      y = t * (x + y);
      z[0] = (x + y) / t2;
    }

    public void p0() {
      e1[j] = e1[k];
      e1[k] = e1[l];
      e1[l] = e1[j];
    }

    #endregion
  }
}
