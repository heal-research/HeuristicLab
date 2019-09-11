#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Random {

  /// <summary>
  /// Normally distributed random variable.
  /// Uses Marsaglia's polar method
  /// </summary>
  [Item("NormalDistributedRandomPolar", "A pseudo random number generator which uses Marsaglia's polar method to create normally distributed random numbers.")]
  public sealed class NormalDistributedRandomPolar : Item, IRandom {
    [Storable]
    private double mu;
    /// <summary>
    /// Gets or sets the value for µ.
    /// </summary>
    public double Mu {
      get { return mu; }
      set { mu = value; }
    }

    [Storable]
    private double sigma;
    /// <summary>
    /// Gets or sets the value for sigma.
    /// </summary>
    public double Sigma {
      get { return sigma; }
      set { sigma = value; }
    }

    [Storable]
    private IRandom uniform;


    [StorableConstructor]
    private NormalDistributedRandomPolar(StorableConstructorFlag _) : base(_) { }

    private NormalDistributedRandomPolar(NormalDistributedRandomPolar original, Cloner cloner)
      : base(original, cloner) {
      uniform = cloner.Clone(original.uniform);
      mu = original.mu;
      sigma = original.sigma;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NormalDistributedRandomPolar"/> with µ = 0 and sigma = 1
    /// and a new random number generator.
    /// </summary>
    public NormalDistributedRandomPolar() {
      this.mu = 0.0;
      this.sigma = 1.0;
      this.uniform = new MersenneTwister();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NormalDistributedRandomPolar"/> with the given parameters.
    /// <note type="caution"> The random number generator is not copied!</note>
    /// </summary>    
    /// <param name="uniformRandom">The random number generator.</param>
    /// <param name="mu">The value for µ.</param>
    /// <param name="sigma">The value for sigma.</param>
    public NormalDistributedRandomPolar(IRandom uniformRandom, double mu, double sigma) {
      this.mu = mu;
      this.sigma = sigma;
      this.uniform = uniformRandom;
    }

    #region IRandom Members

    /// <inheritdoc cref="IRandom.Reset()"/>
    public void Reset() {
      uniform.Reset();
    }

    /// <inheritdoc cref="IRandom.Reset(int)"/>
    public void Reset(int seed) {
      uniform.Reset(seed);
    }

    /// <summary>
    /// This method is not implemented.
    /// </summary>
    public int Next() {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method is not implemented.
    /// </summary>
    public int Next(int maxVal) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method is not implemented.
    /// </summary>
    public int Next(int minVal, int maxVal) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Generates a new double random number.
    /// </summary>
    /// <returns>A double random number.</returns>
    public double NextDouble() {
      return NormalDistributedRandomPolar.NextDouble(uniform, mu, sigma);
    }

    #endregion

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <returns>The cloned object as <see cref="NormalDistributedRandomPolar"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new NormalDistributedRandomPolar(this, cloner);
    }


    /**
     * Polar method due to Marsaglia.
     *
     * Devroye, L. Non-Uniform Random Variates Generation. Springer-Verlag,
     * New York, 1986, Ch. V, Sect. 4.4.
     */
    public static double NextDouble(IRandom uniformRandom, double mu, double sigma) {
      // we don't use spare numbers (efficency loss but easier for multi-threaded code)
      double u, v, s;
      do {
        u = uniformRandom.NextDouble() * 2 - 1;
        v = uniformRandom.NextDouble() * 2 - 1;
        s = u * u + v * v;
      } while (s >= 1 || s == 0);
      s = Math.Sqrt(-2.0 * Math.Log(s) / s);
      return mu + sigma * u * s; 
    }
  }
}
