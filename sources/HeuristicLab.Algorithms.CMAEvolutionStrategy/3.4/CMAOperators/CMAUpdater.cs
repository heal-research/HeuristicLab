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

        double[] d;
        double[,] b;
        var success = alglib.smatrixevd(sp.C, N, 1, true, out d, out b);
        Array.Copy(d, sp.D, N);
        for (var i = 0; i < N; i++)
          for (var j = 0; j < N; j++)
            sp.B[i, j] = b[i, j];

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
  }
}