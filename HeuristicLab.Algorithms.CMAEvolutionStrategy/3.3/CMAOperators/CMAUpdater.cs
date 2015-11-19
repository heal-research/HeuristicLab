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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Linq;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [Item("CMAUpdater", "Updates the covariance matrix and strategy parameters of CMA-ES.")]
  [StorableClass]
  public class CMAUpdater : SingleSuccessorOperator, ICMAUpdater, IIterationBasedOperator, ISingleObjectiveOperator {

    public Type CMAType {
      get { return typeof(CMAParameters); }
    }

    #region Parameter Properties
    public ILookupParameter<CMAParameters> StrategyParametersParameter {
      get { return (ILookupParameter<CMAParameters>)Parameters["StrategyParameters"]; }
    }

    public ILookupParameter<RealVector> MeanParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Mean"]; }
    }

    public ILookupParameter<RealVector> OldMeanParameter {
      get { return (ILookupParameter<RealVector>)Parameters["OldMean"]; }
    }

    public IScopeTreeLookupParameter<RealVector> OffspringParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["Offspring"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }

    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }

    public IValueLookupParameter<IntValue> MaximumEvaluatedSolutionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumEvaluatedSolutions"]; }
    }

    public ILookupParameter<BoolValue> DegenerateStateParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["DegenerateState"]; }
    }
    #endregion

    [StorableConstructor]
    protected CMAUpdater(bool deserializing) : base(deserializing) { }
    protected CMAUpdater(CMAUpdater original, Cloner cloner) : base(original, cloner) { }
    public CMAUpdater()
      : base() {
      Parameters.Add(new LookupParameter<CMAParameters>("StrategyParameters", "The strategy parameters of CMA-ES."));
      Parameters.Add(new LookupParameter<RealVector>("Mean", "The new mean."));
      Parameters.Add(new LookupParameter<RealVector>("OldMean", "The old mean."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("Offspring", "The created offspring solutions."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality of the offspring."));
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "The number of iterations passed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of iterations."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumEvaluatedSolutions", "The maximum number of evaluated solutions."));
      Parameters.Add(new LookupParameter<BoolValue>("DegenerateState", "Whether the algorithm state has degenerated and should be terminated."));
      MeanParameter.ActualName = "XMean";
      OldMeanParameter.ActualName = "XOld";
      OffspringParameter.ActualName = "RealVector";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CMAUpdater(this, cloner);
    }

    public override IOperation Apply() {
      var iterations = IterationsParameter.ActualValue.Value;

      var xold = OldMeanParameter.ActualValue;
      var xmean = MeanParameter.ActualValue;
      var offspring = OffspringParameter.ActualValue;
      var quality = QualityParameter.ActualValue;
      var lambda = offspring.Length;

      var N = xmean.Length;
      var sp = StrategyParametersParameter.ActualValue;

      #region Initialize default values for strategy parameter adjustment
      if (sp.ChiN == 0) sp.ChiN = Math.Sqrt(N) * (1.0 - 1.0 / (4.0 * N) + 1.0 / (21.0 * N * N));
      if (sp.MuEff == 0) sp.MuEff = sp.Weights.Sum() * sp.Weights.Sum() / sp.Weights.Sum(x => x * x);
      if (sp.CS == 0) sp.CS = (sp.MuEff + 2) / (N + sp.MuEff + 3);
      if (sp.Damps == 0) {
        var maxIterations = MaximumIterationsParameter.ActualValue.Value;
        var maxEvals = MaximumEvaluatedSolutionsParameter.ActualValue.Value;
        sp.Damps = 2 * Math.Max(0, Math.Sqrt((sp.MuEff - 1) / (N + 1)) - 1)
                                * Math.Max(0.3, 1 - N / (1e-6 + Math.Min(maxIterations, maxEvals / lambda))) + sp.CS + 1;
      }
      if (sp.CC == 0) sp.CC = 4.0 / (N + 4);
      if (sp.MuCov == 0) sp.MuCov = sp.MuEff;
      if (sp.CCov == 0) sp.CCov = 2.0 / ((N + 1.41) * (N + 1.41) * sp.MuCov)
                             + (1 - (1.0 / sp.MuCov)) * Math.Min(1, (2 * sp.MuEff - 1) / (sp.MuEff + (N + 2) * (N + 2)));
      if (sp.CCovSep == 0) sp.CCovSep = Math.Min(1, sp.CCov * (N + 1.5) / 3);
      #endregion

      sp.QualityHistory.Enqueue(quality[0].Value);
      while (sp.QualityHistory.Count > sp.QualityHistorySize && sp.QualityHistorySize >= 0)
        sp.QualityHistory.Dequeue();

      for (int i = 0; i < N; i++) {
        sp.BDz[i] = Math.Sqrt(sp.MuEff) * (xmean[i] - xold[i]) / sp.Sigma;
      }

      if (sp.InitialIterations >= iterations) {
        for (int i = 0; i < N; i++) {
          sp.PS[i] = (1 - sp.CS) * sp.PS[i]
                     + Math.Sqrt(sp.CS * (2 - sp.CS)) * sp.BDz[i] / sp.D[i];
        }
      } else {
        var artmp = new double[N];
        for (int i = 0; i < N; i++) {
          var sum = 0.0;
          for (int j = 0; j < N; j++) {
            sum += sp.B[j, i] * sp.BDz[j];
          }
          artmp[i] = sum / sp.D[i];
        }
        for (int i = 0; i < N; i++) {
          var sum = 0.0;
          for (int j = 0; j < N; j++) {
            sum += sp.B[i, j] * artmp[j];
          }
          sp.PS[i] = (1 - sp.CS) * sp.PS[i] + Math.Sqrt(sp.CS * (2 - sp.CS)) * sum;
        }
      }
      var normPS = Math.Sqrt(sp.PS.Select(x => x * x).Sum());
      var hsig = normPS / Math.Sqrt(1 - Math.Pow(1 - sp.CS, 2 * iterations)) / sp.ChiN < 1.4 + 2.0 / (N + 1) ? 1.0 : 0.0;
      for (int i = 0; i < sp.PC.Length; i++) {
        sp.PC[i] = (1 - sp.CC) * sp.PC[i]
                   + hsig * Math.Sqrt(sp.CC * (2 - sp.CC)) * sp.BDz[i];
      }

      if (sp.CCov > 0) {
        if (sp.InitialIterations >= iterations) {
          for (int i = 0; i < N; i++) {
            sp.C[i, i] = (1 - sp.CCovSep) * sp.C[i, i]
                         + sp.CCov * (1 / sp.MuCov)
                         * (sp.PC[i] * sp.PC[i] + (1 - hsig) * sp.CC * (2 - sp.CC) * sp.C[i, i]);
            for (int k = 0; k < sp.Mu; k++) {
              sp.C[i, i] += sp.CCov * (1 - 1 / sp.MuCov) * sp.Weights[k] * (offspring[k][i] - xold[i]) *
                            (offspring[k][i] - xold[i]) / (sp.Sigma * sp.Sigma);
            }
          }
        } else {
          for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
              sp.C[i, j] = (1 - sp.CCov) * sp.C[i, j]
                           + sp.CCov * (1 / sp.MuCov)
                           * (sp.PC[i] * sp.PC[j] + (1 - hsig) * sp.CC * (2 - sp.CC) * sp.C[i, j]);
              for (int k = 0; k < sp.Mu; k++) {
                sp.C[i, j] += sp.CCov * (1 - 1 / sp.MuCov) * sp.Weights[k] * (offspring[k][i] - xold[i]) *
                              (offspring[k][j] - xold[j]) / (sp.Sigma * sp.Sigma);
              }
            }
          }
        }
      }
      sp.Sigma *= Math.Exp((sp.CS / sp.Damps) * (normPS / sp.ChiN - 1));

      double minSqrtdiagC = int.MaxValue, maxSqrtdiagC = int.MinValue;
      for (int i = 0; i < N; i++) {
        if (Math.Sqrt(sp.C[i, i]) < minSqrtdiagC) minSqrtdiagC = Math.Sqrt(sp.C[i, i]);
        if (Math.Sqrt(sp.C[i, i]) > maxSqrtdiagC) maxSqrtdiagC = Math.Sqrt(sp.C[i, i]);
      }

      // ensure maximal and minimal standard deviations
      if (sp.SigmaBounds != null && sp.SigmaBounds.GetLength(0) > 0) {
        for (int i = 0; i < N; i++) {
          var d = sp.SigmaBounds[Math.Min(i, sp.SigmaBounds.GetLength(0) - 1), 0];
          if (d > sp.Sigma * minSqrtdiagC) sp.Sigma = d / minSqrtdiagC;
        }
        for (int i = 0; i < N; i++) {
          var d = sp.SigmaBounds[Math.Min(i, sp.SigmaBounds.GetLength(0) - 1), 1];
          if (d > sp.Sigma * maxSqrtdiagC) sp.Sigma = d / maxSqrtdiagC;
        }
      }
      // end ensure ...

      // testAndCorrectNumerics
      double fac = 1;
      if (sp.D.Max() < 1e-6)
        fac = 1.0 / sp.D.Max();
      else if (sp.D.Min() > 1e4)
        fac = 1.0 / sp.D.Min();

      if (fac != 1.0) {
        sp.Sigma /= fac;
        for (int i = 0; i < N; i++) {
          sp.PC[i] *= fac;
          sp.D[i] *= fac;
          for (int j = 0; j < N; j++)
            sp.C[i, j] *= fac * fac;
        }
      }
      // end testAndCorrectNumerics


      if (sp.InitialIterations >= iterations) {
        for (int i = 0; i < N; i++)
          sp.D[i] = Math.Sqrt(sp.C[i, i]);
        DegenerateStateParameter.ActualValue = new BoolValue(false);
      } else {

        // set B <- C
        for (int i = 0; i < N; i++) {
          for (int j = 0; j < N; j++) {
            sp.B[i, j] = sp.C[i, j];
          }
        }
        var success = Eigendecomposition(N, sp.B, sp.D);

        DegenerateStateParameter.ActualValue = new BoolValue(!success);

        // assign D to eigenvalue square roots
        for (int i = 0; i < N; i++) {
          if (sp.D[i] < 0) { // numerical problem?
            DegenerateStateParameter.ActualValue.Value = true;
            sp.D[i] = 0;
          } else sp.D[i] = Math.Sqrt(sp.D[i]);
        }

        if (sp.D.Min() == 0.0) sp.AxisRatio = double.PositiveInfinity;
        else sp.AxisRatio = sp.D.Max() / sp.D.Min();
      }
      return base.Apply();
    }

    private bool Eigendecomposition(int N, double[,] B, double[] diagD) {
      bool result = true;
      // eigendecomposition
      var offdiag = new double[N];
      try {
        tred2(N, B, diagD, offdiag);
        tql2(N, diagD, offdiag, B);
      } catch { result = false; }

      return result;
    } // eigendecomposition


    // Symmetric Householder reduction to tridiagonal form, taken from JAMA package.
    private void tred2(int n, double[,] V, double[] d, double[] e) {

      //  This is derived from the Algol procedures tred2 by
      //  Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
      //  Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutine in EISPACK.

      for (int j = 0; j < n; j++) {
        d[j] = V[n - 1, j];
      }

      // Householder reduction to tridiagonal form.

      for (int i = n - 1; i > 0; i--) {

        // Scale to avoid under/overflow.

        double scale = 0.0;
        double h = 0.0;
        for (int k = 0; k < i; k++) {
          scale = scale + Math.Abs(d[k]);
        }
        if (scale == 0.0) {
          e[i] = d[i - 1];
          for (int j = 0; j < i; j++) {
            d[j] = V[i - 1, j];
            V[i, j] = 0.0;
            V[j, i] = 0.0;
          }
        } else {

          // Generate Householder vector.

          for (int k = 0; k < i; k++) {
            d[k] /= scale;
            h += d[k] * d[k];
          }
          double f = d[i - 1];
          double g = Math.Sqrt(h);
          if (f > 0) {
            g = -g;
          }
          e[i] = scale * g;
          h = h - f * g;
          d[i - 1] = f - g;
          for (int j = 0; j < i; j++) {
            e[j] = 0.0;
          }

          // Apply similarity transformation to remaining columns.

          for (int j = 0; j < i; j++) {
            f = d[j];
            V[j, i] = f;
            g = e[j] + V[j, j] * f;
            for (int k = j + 1; k <= i - 1; k++) {
              g += V[k, j] * d[k];
              e[k] += V[k, j] * f;
            }
            e[j] = g;
          }
          f = 0.0;
          for (int j = 0; j < i; j++) {
            e[j] /= h;
            f += e[j] * d[j];
          }
          double hh = f / (h + h);
          for (int j = 0; j < i; j++) {
            e[j] -= hh * d[j];
          }
          for (int j = 0; j < i; j++) {
            f = d[j];
            g = e[j];
            for (int k = j; k <= i - 1; k++) {
              V[k, j] -= (f * e[k] + g * d[k]);
            }
            d[j] = V[i - 1, j];
            V[i, j] = 0.0;
          }
        }
        d[i] = h;
      }

      // Accumulate transformations.

      for (int i = 0; i < n - 1; i++) {
        V[n - 1, i] = V[i, i];
        V[i, i] = 1.0;
        double h = d[i + 1];
        if (h != 0.0) {
          for (int k = 0; k <= i; k++) {
            d[k] = V[k, i + 1] / h;
          }
          for (int j = 0; j <= i; j++) {
            double g = 0.0;
            for (int k = 0; k <= i; k++) {
              g += V[k, i + 1] * V[k, j];
            }
            for (int k = 0; k <= i; k++) {
              V[k, j] -= g * d[k];
            }
          }
        }
        for (int k = 0; k <= i; k++) {
          V[k, i + 1] = 0.0;
        }
      }
      for (int j = 0; j < n; j++) {
        d[j] = V[n - 1, j];
        V[n - 1, j] = 0.0;
      }
      V[n - 1, n - 1] = 1.0;
      e[0] = 0.0;
    }

    // Symmetric tridiagonal QL algorithm, taken from JAMA package.
    private void tql2(int n, double[] d, double[] e, double[,] V) {

      //  This is derived from the Algol procedures tql2, by
      //  Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
      //  Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutine in EISPACK.

      for (int i = 1; i < n; i++) {
        e[i - 1] = e[i];
      }
      e[n - 1] = 0.0;

      double f = 0.0;
      double tst1 = 0.0;
      double eps = Math.Pow(2.0, -52.0);
      for (int l = 0; l < n; l++) {

        // Find small subdiagonal element

        tst1 = Math.Max(tst1, Math.Abs(d[l]) + Math.Abs(e[l]));
        int m = l;
        while (m < n) {
          if (Math.Abs(e[m]) <= eps * tst1) {
            break;
          }
          m++;
        }

        // If m == l, d[l] is an eigenvalue,
        // otherwise, iterate.

        if (m > l) {
          int iter = 0;
          do {
            iter = iter + 1;  // (Could check iteration count here.)

            // Compute implicit shift

            double g = d[l];
            double p = (d[l + 1] - g) / (2.0 * e[l]);
            double r = hypot(p, 1.0);
            if (p < 0) {
              r = -r;
            }
            d[l] = e[l] / (p + r);
            d[l + 1] = e[l] * (p + r);
            double dl1 = d[l + 1];
            double h = g - d[l];
            for (int i = l + 2; i < n; i++) {
              d[i] -= h;
            }
            f = f + h;

            // Implicit QL transformation.

            p = d[m];
            double c = 1.0;
            double c2 = c;
            double c3 = c;
            double el1 = e[l + 1];
            double s = 0.0;
            double s2 = 0.0;
            for (int i = m - 1; i >= l; i--) {
              c3 = c2;
              c2 = c;
              s2 = s;
              g = c * e[i];
              h = c * p;
              r = hypot(p, e[i]);
              e[i + 1] = s * r;
              s = e[i] / r;
              c = p / r;
              p = c * d[i] - s * g;
              d[i + 1] = h + s * (c * g + s * d[i]);

              // Accumulate transformation.

              for (int k = 0; k < n; k++) {
                h = V[k, i + 1];
                V[k, i + 1] = s * V[k, i] + c * h;
                V[k, i] = c * V[k, i] - s * h;
              }
            }
            p = -s * s2 * c3 * el1 * e[l] / dl1;
            e[l] = s * p;
            d[l] = c * p;

            // Check for convergence.

          } while (Math.Abs(e[l]) > eps * tst1);
        }
        d[l] = d[l] + f;
        e[l] = 0.0;
      }

      // Sort eigenvalues and corresponding vectors.

      for (int i = 0; i < n - 1; i++) {
        int k = i;
        double p = d[i];
        for (int j = i + 1; j < n; j++) {
          if (d[j] < p) { // NH find smallest k>i
            k = j;
            p = d[j];
          }
        }
        if (k != i) {
          d[k] = d[i]; // swap k and i 
          d[i] = p;
          for (int j = 0; j < n; j++) {
            p = V[j, i];
            V[j, i] = V[j, k];
            V[j, k] = p;
          }
        }
      }
    }

    /** sqrt(a^2 + b^2) without under/overflow. **/
    private double hypot(double a, double b) {
      double r = 0;
      if (Math.Abs(a) > Math.Abs(b)) {
        r = b / a;
        r = Math.Abs(a) * Math.Sqrt(1 + r * r);
      } else if (b != 0) {
        r = a / b;
        r = Math.Abs(b) * Math.Sqrt(1 + r * r);
      }
      return r;
    }
  }
}