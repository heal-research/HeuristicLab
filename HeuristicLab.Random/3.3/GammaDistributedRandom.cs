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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.ExpressionGenerator {
  /// <summary>
  /// Gamma distribution implemented after 
  /// "A Simple Method for Generating Gamma Variables" - Marsaglia & Tsang
  /// ACM Transactions on Mathematical Software, Vol. 26, No. 3, September 2000, Pages 363–372.
  /// </summary>
  [Item("GammaDistributedRandom", "A pseudo random number generator for gamma distributed random numbers.")]
  [StorableClass]
  public sealed class GammaDistributedRandom : Item, IRandom {
    [Storable]
    private double shape;
    public double Shape {
      get { return shape; }
      set { shape = value; }
    }

    [Storable]
    private double rate;
    public double Rate {
      get { return rate; }
      set { rate = value; }
    }

    [Storable]
    private readonly IRandom random;

    public GammaDistributedRandom() {
      random = new MersenneTwister();
    }

    public GammaDistributedRandom(IRandom random, double shape, double rate) {
      this.random = random;
      this.shape = shape;
      this.rate = rate;
    }

    [StorableConstructor]
    private GammaDistributedRandom(bool deserializing) : base(deserializing) { }

    private GammaDistributedRandom(GammaDistributedRandom original, Cloner cloner) : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GammaDistributedRandom(this, cloner);
    }

    public void Reset() {
      random.Reset();
    }

    public void Reset(int seed) {
      random.Reset(seed);
    }

    public int Next() {
      throw new NotImplementedException();
    }

    public int Next(int maxVal) {
      throw new NotImplementedException();
    }

    public int Next(int minVal, int maxVal) {
      throw new NotImplementedException();
    }

    public double NextDouble() {
      return NextDouble(random, shape, rate);
    }

    /// <summary>
    /// <para>Sample a value from a gamma distribution.</para>
    /// <para>Implementation of "A Simple Method for Generating Gamma Variables" - Marsaglia & Tsang
    /// ACM Transactions on Mathematical Software, Vol. 26, No. 3, September 2000, Pages 363–372.</para>
    /// </summary>
    /// <param name="uniformRandom">A uniformly-distributed random number generator.</param>
    /// <param name="shape">The shape (k, α) of the Gamma distribution. Range: α ≥ 0.</param>
    /// <param name="rate">The rate or inverse scale (β) of the Gamma distribution. Range: β ≥ 0.</param>
    /// <returns>A sample from a Gamma distributed random variable.</returns>
    public static double NextDouble(IRandom uniformRandom, double shape, double rate) {
      if (double.IsPositiveInfinity(rate)) {
        return shape;
      }
      var a = 1d;
      if (shape < 1) {
        a = Math.Pow(uniformRandom.NextDouble(), 1 / shape);
        shape += 1;
      }
      var d = shape - 1d / 3d;
      var c = 1 / Math.Sqrt(9 * d);

      for (;;) {
        double v, x;
        do {
          x = NormalDistributedRandom.NextDouble(uniformRandom, 0, 1);
          v = 1 + c * x;
        } while (v <= 0);

        v = v * v * v;
        x = x * x; // save a multiplication below
        var u = uniformRandom.NextDouble();
        if (u < 1 - 0.0331 * x * x || Math.Log(u) < 0.5 * x + d * (1 - v + Math.Log(v)))
          return a * d * v / rate;
      }
    }
  }
}
