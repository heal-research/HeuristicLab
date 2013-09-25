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
  [Item("Whetstone", "Whetstone performance benchmark.")]
  [StorableClass]
  public sealed class Whetstone : Benchmark {
    private long begin_time;
    private long end_time;

    private int ITERATIONS;
    private int numberOfCycles;
    private int cycleNo;
    private double x1, x2, x3, x4, x, y, t, t1, t2;
    private double[] z = new double[1];
    private double[] e1 = new double[4];
    private int i, j, k, l, n1, n2, n3, n4, n6, n7, n8, n9, n10, n11;

    [StorableConstructor]
    private Whetstone(bool deserializing) : base(deserializing) { }
    private Whetstone(Whetstone original, Cloner cloner) : base(original, cloner) { }
    public Whetstone() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Whetstone(this, cloner);
    }

    // implementation based on Java version: www.aicas.com/download/Whetstone.java
    public override void Run(CancellationToken cancellationToken, ResultCollection results) {
      bool stopBenchmark = false;

      ITERATIONS = 100; // ITERATIONS / 10 = Millions Whetstone instructions

      numberOfCycles = 100;
      int defaultNumberOfRuns = 10;
      float elapsedTime = 0;
      float meanTime = 0;
      float rating = 0;
      float meanRating = 0;
      int intRating = 0;

      long runNumber = 1;
      Stopwatch sw = new Stopwatch();
      sw.Start();

      while (!stopBenchmark) {
        elapsedTime = (float)(MainCalc() / 1000);
        meanTime = meanTime + (elapsedTime * 1000 / numberOfCycles);
        rating = (1000 * numberOfCycles) / elapsedTime;
        meanRating = meanRating + rating;
        intRating = (int)rating;
        numberOfCycles += 10;

        if (cancellationToken.IsCancellationRequested) {
          throw new OperationCanceledException(cancellationToken);
        }

        if ((TimeLimit == null) || (TimeLimit.TotalMilliseconds == 0)) {
          if (runNumber > defaultNumberOfRuns) {
            stopBenchmark = true;
          }
        } else if (sw.Elapsed > TimeLimit) {
          stopBenchmark = true;
        }

        runNumber++;
      }
      sw.Stop();
      meanTime = meanTime / runNumber;
      meanRating = meanRating / runNumber;
      intRating = (int)meanRating;

      results.Add(new Result("MWIPS", new IntValue(intRating / 1000)));
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
  }
}
