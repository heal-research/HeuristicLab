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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [Item("CMAMutator", "Mutates the solution vector according to the CMA-ES scheme.")]
  [StorableClass]
  public sealed class CMAMutator : SingleSuccessorOperator, IStochasticOperator, ICMAManipulator, IIterationBasedOperator {

    public Type CMAType {
      get { return typeof(CMAParameters); }
    }

    #region Parameter Properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<IntValue> PopulationSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }

    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }

    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }

    public ILookupParameter<RealVector> MeanParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Mean"]; }
    }

    public IScopeTreeLookupParameter<RealVector> RealVectorParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["RealVector"]; }
    }

    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    public ILookupParameter<CMAParameters> StrategyParametersParameter {
      get { return (ILookupParameter<CMAParameters>)Parameters["StrategyParameters"]; }
    }

    public IFixedValueParameter<IntValue> MaxTriesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["MaxTries"]; }
    }

    public IFixedValueParameter<BoolValue> TruncateAtBoundsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["TruncateAtBounds"]; }
    }
    #endregion

    [StorableConstructor]
    private CMAMutator(bool deserializing) : base(deserializing) { }
    private CMAMutator(CMAMutator original, Cloner cloner) : base(original, cloner) { }
    public CMAMutator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<IntValue>("PopulationSize", "The population size (lambda) determines how many offspring should be created."));
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "The current iteration that is being processed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of iterations to be processed."));
      Parameters.Add(new LookupParameter<RealVector>("Mean", "The current mean solution."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("RealVector", "The solution vector of real values."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The bounds for the dimensions."));
      Parameters.Add(new LookupParameter<CMAParameters>("StrategyParameters", "The CMA-ES strategy parameters used for mutation."));
      Parameters.Add(new FixedValueParameter<IntValue>("MaxTries", "The maximum number of tries a mutation should be performed if it was outside the bounds.", new IntValue(100)));
      Parameters.Add(new FixedValueParameter<BoolValue>("TruncateAtBounds", "Whether the point should be truncated at the bounds if none of the tries resulted in a point within the bounds.", new BoolValue(true)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CMAMutator(this, cloner);
    }

    public override IOperation Apply() {
      var maxTries = MaxTriesParameter.Value.Value;
      var truncateAtBounds = TruncateAtBoundsParameter.Value.Value;
      var random = RandomParameter.ActualValue;
      var lambda = PopulationSizeParameter.ActualValue.Value;
      var xmean = MeanParameter.ActualValue;
      var arx = RealVectorParameter.ActualValue;
      var sp = StrategyParametersParameter.ActualValue;
      var iterations = IterationsParameter.ActualValue.Value;
      var initialIterations = sp.InitialIterations;
      var bounds = BoundsParameter.ActualValue;

      if (arx == null || arx.Length == 0) {
        arx = new ItemArray<RealVector>(lambda);
        for (int i = 0; i < lambda; i++) arx[i] = new RealVector(xmean.Length);
        RealVectorParameter.ActualValue = arx;
      }
      var nd = new NormalDistributedRandom(random, 0, 1);

      var length = arx[0].Length;

      for (int i = 0; i < lambda; i++) {
        int tries = 0;
        bool inRange;
        if (initialIterations > iterations) {
          for (int k = 0; k < length; k++) {
            do {
              arx[i][k] = xmean[k] + sp.Sigma * sp.D[k] * nd.NextDouble();
              inRange = bounds[k % bounds.Rows, 0] <= arx[i][k] && arx[i][k] <= bounds[k % bounds.Rows, 1];
              if (!inRange) tries++;
            } while (!inRange && tries < maxTries);
            if (!inRange && truncateAtBounds) {
              if (bounds[k % bounds.Rows, 0] > arx[i][k]) arx[i][k] = bounds[k % bounds.Rows, 0];
              else if (bounds[k % bounds.Rows, 1] < arx[i][k]) arx[i][k] = bounds[k % bounds.Rows, 1];
            }
          }
        } else {
          var B = sp.B;
          do {
            tries++;
            inRange = true;
            var artmp = new double[length];
            for (int k = 0; k < length; ++k) {
              artmp[k] = sp.D[k] * nd.NextDouble();
            }

            for (int k = 0; k < length; k++) {
              var sum = 0.0;
              for (int j = 0; j < length; j++)
                sum += B[k, j] * artmp[j];
              arx[i][k] = xmean[k] + sp.Sigma * sum; // m + sig * Normal(0,C)
              if (bounds[k % bounds.Rows, 0] > arx[i][k] || arx[i][k] > bounds[k % bounds.Rows, 1])
                inRange = false;
            }
          } while (!inRange && tries < maxTries);
          if (!inRange && truncateAtBounds) {
            for (int k = 0; k < length; k++) {
              if (bounds[k % bounds.Rows, 0] > arx[i][k]) arx[i][k] = bounds[k % bounds.Rows, 0];
              else if (bounds[k % bounds.Rows, 1] < arx[i][k]) arx[i][k] = bounds[k % bounds.Rows, 1];
            }
          }
        }
      }
      return base.Apply();
    }
  }
}