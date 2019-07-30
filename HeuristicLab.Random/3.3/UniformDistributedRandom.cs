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
  /// Unformliy distributed random variable.
  /// </summary>
  [Item("UniformDistributedRandom", "A pseudo random number generator to create uniform distributed random numbers.")]
  [StorableType("01239E33-7AAD-467A-A95C-6D7E001F5827")]
  public sealed class UniformDistributedRandom : Item, IRandom {
    [Storable]
    private double min;
    /// <summary>
    /// Gets or sets the value for min.
    /// </summary>
    public double Min {
      get { return min; }
      set { min = value; }
    }

    [Storable]
    private double max;
    /// <summary>
    /// Gets or sets the value for max.
    /// </summary>
    public double Max {
      get { return max; }
      set { max = value; }
    }

    [Storable]
    private IRandom uniform;

    /// <summary>
    /// Used by HeuristicLab.Persistence to initialize new instances during deserialization.
    /// </summary>
    /// <param name="deserializing">true, if the constructor is called during deserialization.</param>
    [StorableConstructor]
    private UniformDistributedRandom(StorableConstructorFlag _) : base(_) { }

    /// <summary>
    /// Initializes a new instance from an existing one (copy constructor).
    /// </summary>
    /// <param name="original">The original <see cref="UniformDistributedRandom"/> instance which is used to initialize the new instance.</param>
    /// <param name="cloner">A <see cref="Cloner"/> which is used to track all already cloned objects in order to avoid cycles.</param>
    private UniformDistributedRandom(UniformDistributedRandom original, Cloner cloner)
      : base(original, cloner) {
      uniform = cloner.Clone(original.uniform);
      min = original.min;
      max = original.max;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="UniformDistributedRandom"/> with the given parameters.
    /// </summary>    
    /// <param name="uniformRandom">The random number generator.</param>
    /// <param name="min">The minimal value (inclusive)</param>
    /// <param name="max">The maximal value (exclusive).</param>
    public UniformDistributedRandom(IRandom uniformRandom, double min, double max) {
      this.min = min;
      this.max = max;
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
      throw new NotSupportedException();
    }

    /// <summary>
    /// This method is not implemented.
    /// </summary>
    public int Next(int maxVal) {
      throw new NotSupportedException();
    }

    /// <summary>
    /// This method is not implemented.
    /// </summary>
    public int Next(int minVal, int maxVal) {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Generates a new double random number.
    /// </summary>
    /// <returns>A double random number.</returns>
    public double NextDouble() {
      return UniformDistributedRandom.NextDouble(uniform, min, max);
    }

    #endregion

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="cloner.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="UniformDistributedRandom"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformDistributedRandom(this, cloner);
    }

    public static double NextDouble(IRandom uniformRandom, double min, double max) {
      double range = max - min;
      return uniformRandom.NextDouble() * range + min;
    }
  }
}
