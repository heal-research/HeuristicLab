#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [Item("CMAInitializer", "Initializes the covariance matrix and step size variables.")]
  [StorableClass]
  public class CMAInitializer : SingleSuccessorOperator, ICMAInitializer, IIterationBasedOperator {

    public Type CMAType {
      get { return typeof(CMAParameters); }
    }

    #region Parameter Properties
    public IValueLookupParameter<IntValue> DimensionParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Dimension"]; }
    }

    public IValueLookupParameter<DoubleArray> InitialSigmaParameter {
      get { return (IValueLookupParameter<DoubleArray>)Parameters["InitialSigma"]; }
    }

    public IValueLookupParameter<DoubleMatrix> SigmaBoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["SigmaBounds"]; }
    }

    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }

    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }

    public IValueLookupParameter<IntValue> InitialIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["InitialIterations"]; }
    }

    public ILookupParameter<IntValue> PopulationSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }

    public ILookupParameter<IntValue> MuParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Mu"]; }
    }

    public ILookupParameter<CMAParameters> StrategyParametersParameter {
      get { return (ILookupParameter<CMAParameters>)Parameters["StrategyParameters"]; }
    }
    #endregion

    [StorableConstructor]
    protected CMAInitializer(bool deserializing) : base(deserializing) { }
    protected CMAInitializer(CMAInitializer original, Cloner cloner) : base(original, cloner) { }
    public CMAInitializer()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("Dimension", "The problem dimension (N)."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>("InitialSigma", "The initial value for Sigma (need to be > 0), can be single dimensioned or an array that should be equal to the size of the vector."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("SigmaBounds", "The bounds for sigma value can be omitted, given as one value for all dimensions or a value for each dimension. First column specifies minimum, second column maximum value."));
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "The current iteration that is being processed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of iterations to be processed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("InitialIterations", "The number of iterations that should be performed using the diagonal covariance matrix only.", new IntValue(0)));
      Parameters.Add(new LookupParameter<IntValue>("PopulationSize", "The population size (lambda)."));
      Parameters.Add(new LookupParameter<IntValue>("Mu", "Optional, the number of offspring considered for updating of the strategy parameters."));
      Parameters.Add(new LookupParameter<CMAParameters>("StrategyParameters", "The strategy parameters for real-encoded CMA-ES."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CMAInitializer(this, cloner);
    }

    public override IOperation Apply() {
      var N = DimensionParameter.ActualValue.Value;
      var lambda = PopulationSizeParameter.ActualValue.Value;
      var mu = MuParameter.ActualValue;

      var sp = new CMAParameters();
      sp.Mu = mu == null ? (int)Math.Floor(lambda / 2.0) : mu.Value;
      sp.QualityHistorySize = 10 + 30 * N / lambda;
      sp.QualityHistory = new Queue<double>(sp.QualityHistorySize + 1);

      var s = InitialSigmaParameter.ActualValue;
      if (s == null || s.Length == 0) throw new InvalidOperationException("Initial standard deviation (sigma) must be given.");
      var sigma = s.Max();
      if (sigma <= 0) throw new InvalidOperationException("Initial standard deviation (sigma) must be > 0.");

      var pc = new double[N]; // evolution paths for C
      var ps = new double[N]; // evolution paths for sigma
      var B = new double[N, N]; // B defines the coordinate system
      var D = new double[N]; // diagonal D defines the scaling
      var C = new double[N, N]; // covariance matrix C
      var BDz = new double[N];
      double minSqrtdiagC = int.MaxValue, maxSqrtdiagC = int.MinValue;
      for (int i = 0; i < N; i++) {
        B[i, i] = 1;
        if (s.Length == 1) D[i] = 1;
        else if (s.Length == N) D[i] = s[i] / sigma;
        else throw new InvalidOperationException("Initial standard deviation (sigma) must either contain only one value for all dimension or for every dimension.");
        if (D[i] <= 0) throw new InvalidOperationException("Initial standard deviation (sigma) values must all be > 0.");
        C[i, i] = D[i] * D[i];
        if (Math.Sqrt(C[i, i]) < minSqrtdiagC) minSqrtdiagC = Math.Sqrt(C[i, i]);
        if (Math.Sqrt(C[i, i]) > maxSqrtdiagC) maxSqrtdiagC = Math.Sqrt(C[i, i]);
      }

      // ensure maximal and minimal standard deviations
      var sigmaBounds = SigmaBoundsParameter.ActualValue;
      if (sigmaBounds != null && sigmaBounds.Rows > 0) {
        for (int i = 0; i < N; i++) {
          var d = sigmaBounds[Math.Min(i, sigmaBounds.Rows - 1), 0];
          if (d > sigma * minSqrtdiagC) sigma = d / minSqrtdiagC;
        }
        for (int i = 0; i < N; i++) {
          var d = sigmaBounds[Math.Min(i, sigmaBounds.Rows - 1), 1];
          if (d > sigma * maxSqrtdiagC) sigma = d / maxSqrtdiagC;
        }
      }
      // end ensure ...

      // testAndCorrectNumerics
      double fac = 1;
      if (D.Max() < 1e-6)
        fac = 1.0 / D.Max();
      else if (D.Min() > 1e4)
        fac = 1.0 / D.Min();

      if (fac != 1.0) {
        sigma /= fac;
        for (int i = 0; i < N; i++) {
          pc[i] *= fac;
          D[i] *= fac;
          for (int j = 0; j < N; j++)
            C[i, j] *= fac * fac;
        }
      }
      // end testAndCorrectNumerics

      var initialIterations = InitialIterationsParameter.ActualValue;
      if (initialIterations == null) {
        initialIterations = new IntValue(0);
      }

      double maxD = D.Max(), minD = D.Min();
      if (minD == 0) sp.AxisRatio = double.PositiveInfinity;
      else sp.AxisRatio = maxD / minD;
      sp.PC = pc;
      sp.PS = ps;
      sp.B = B;
      sp.D = D;
      sp.C = C;
      sp.BDz = BDz;
      sp.Sigma = sigma;
      if (sigmaBounds != null) {
        sp.SigmaBounds = new double[sigmaBounds.Rows, sigmaBounds.Columns];
        for (int i = 0; i < sigmaBounds.Rows; i++)
          for (int j = 0; j < sigmaBounds.Columns; j++)
            sp.SigmaBounds[i, j] = sigmaBounds[i, j];
      }
      sp.InitialIterations = initialIterations.Value;

      StrategyParametersParameter.ActualValue = sp;
      return base.Apply();
    }
  }
}