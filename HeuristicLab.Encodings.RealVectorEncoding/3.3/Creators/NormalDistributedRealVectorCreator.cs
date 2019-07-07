#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// An operator which creates a new random real vector with each element normally distributed in a specified range.
  /// </summary>
  [Item("NormalDistributedRealVectorCreator", "An operator which creates a new random real vector with each element normally distributed in a specified range.")]
  [StorableType("790F7401-DD2A-4910-8632-F737E39CE6EE")]
  public class NormalDistributedRealVectorCreator : RealVectorCreator, IStrategyParameterCreator {

    public IValueLookupParameter<RealVector> MeanParameter {
      get { return (IValueLookupParameter<RealVector>)Parameters["Mean"]; }
    }

    public IValueLookupParameter<DoubleArray> SigmaParameter {
      get { return (IValueLookupParameter<DoubleArray>)Parameters["Sigma"]; }
    }

    public IValueParameter<IntValue> MaximumTriesParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumTries"]; }
    }

    [StorableConstructor]
    protected NormalDistributedRealVectorCreator(StorableConstructorFlag _) : base(_) { }
    protected NormalDistributedRealVectorCreator(NormalDistributedRealVectorCreator original, Cloner cloner) : base(original, cloner) { }
    public NormalDistributedRealVectorCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<RealVector>("Mean", "The mean vector around which the points will be sampled."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>("Sigma", "The standard deviations for all or for each dimension."));
      Parameters.Add(new ValueParameter<IntValue>("MaximumTries", "The maximum number of tries to sample within the specified bounds.", new IntValue(1000)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NormalDistributedRealVectorCreator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("MaximumTries"))
        Parameters.Add(new ValueParameter<IntValue>("MaximumTries", "The maximum number of tries to sample within the specified bounds.", new IntValue(1000)));
    }

    /// <summary>
    /// Generates a new random real vector normally distributed around the given mean with the given <paramref name="length"/> and in the interval [min,max).
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="random"/> is null.<br />
    /// Thrown when <paramref name="mean"/> is null or of length 0.<br />
    /// Thrown when <paramref name="sigma"/> is null or of length 0.<br />
    /// </exception>
    /// <remarks>
    /// If no bounds are given the bounds will be set to (double.MinValue;double.MaxValue).
    /// 
    /// If dimensions of the mean do not lie within the given bounds they're set to either to the min or max of the bounds depending on whether the given dimension
    /// for the mean is smaller or larger than the bounds. If min and max for a certain dimension are almost the same the resulting value will be set to min.
    /// 
    /// However, please consider that such static bounds are not really meaningful to optimize.
    /// 
    /// The sigma vector can contain 0 values in which case the dimension will be exactly the same as the given mean.
    /// </remarks>
    /// <param name="random">The random number generator.</param>
    /// <param name="means">The mean vector around which the resulting vector is sampled.</param>
    /// <param name="sigmas">The vector of standard deviations, must have at least one row.</param>
    /// <param name="bounds">The lower and upper bound (1st and 2nd column) of the positions in the vector. If there are less rows than dimensions, the rows are cycled.</param>
    /// <param name="maximumTries">The maximum number of tries to sample a value inside the bounds for each dimension. If a valid value cannot be obtained, the mean will be used.</param>
    /// <returns>The newly created real vector.</returns>
    public static RealVector Apply(IntValue lengthValue, IRandom random, RealVector means, DoubleArray sigmas, DoubleMatrix bounds, int maximumTries = 1000) {
      if (lengthValue == null || lengthValue.Value == 0) throw new ArgumentException("Length is not defined or zero");
      if (random == null) throw new ArgumentNullException("Random is not defined", "random");
      if (means == null || means.Length == 0) throw new ArgumentNullException("Mean is not defined", "mean");
      if (sigmas == null || sigmas.Length == 0) throw new ArgumentNullException("Sigma is not defined.", "sigma");
      if (bounds == null || bounds.Rows == 0) bounds = new DoubleMatrix(new[,] { { double.MinValue, double.MaxValue } });
      var length = lengthValue.Value;
      var nd = new NormalDistributedRandom(random, 0, 1);
      var result = new RealVector(length);
      for (int i = 0; i < result.Length; i++) {
        var min = bounds[i % bounds.Rows, 0];
        var max = bounds[i % bounds.Rows, 1];
        var mean = means[i % means.Length];
        var sigma = sigmas[i % sigmas.Length];
        if (min.IsAlmost(max) || mean < min) result[i] = min;
        else if (mean > max) result[i] = max;
        else {
          int count = 0;
          bool inRange;
          do {
            result[i] = mean + sigma * nd.NextDouble();
            inRange = result[i] >= min && result[i] < max;
            count++;
          } while (count < maximumTries && !inRange);
          if (count == maximumTries && !inRange)
            result[i] = mean;
        }
      }
      return result;
    }

    /// <summary>
    /// Forwards the call to <see cref="Apply(IRandom, RealVector, DoubleArray, DoubleMatrix)"/>.
    /// </summary>
    /// <param name="random">The pseudo random number generator to use.</param>
    /// <param name="length">The length of the real vector.</param>
    /// <param name="bounds">The lower and upper bound (1st and 2nd column) of the positions in the vector. If there are less rows than dimensions, the rows are cycled.</param>
    /// <returns>The newly created real vector.</returns>
    protected override RealVector Create(IRandom random, IntValue length, DoubleMatrix bounds) {
      return Apply(length, random, MeanParameter.ActualValue, SigmaParameter.ActualValue, bounds, MaximumTriesParameter.Value.Value);
    }
  }
}
