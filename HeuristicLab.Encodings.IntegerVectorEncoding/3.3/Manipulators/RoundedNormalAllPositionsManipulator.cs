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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Manipulates each dimension in the integer vector with the mutation strength given 
  /// in the sigma parameter vector and rounds the result to the next feasible value.
  /// </summary>
  [Item("RoundedNormalAllPositionsManipulator", "This manipulation operator adds a value sigma_i * N_i(0,1) to the current value in each position i given the values for sigma_i in the parameter. The result is rounded to the next feasible value. If there are less elements in Sigma than positions, then Sigma is cycled.")]
  [StorableClass]
  public class RoundedNormalAllPositionsManipulator : BoundedIntegerVectorManipulator {

    public IValueParameter<DoubleArray> SigmaParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["Sigma"]; }
    }

    [StorableConstructor]
    protected RoundedNormalAllPositionsManipulator(bool deserializing) : base(deserializing) { }
    protected RoundedNormalAllPositionsManipulator(RoundedNormalAllPositionsManipulator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="RoundedNormalAllPositionsManipulator"/> with one
    /// parameter (<c>Sigma</c>).
    /// </summary>
    public RoundedNormalAllPositionsManipulator()
      : base() {
      Parameters.Add(new ValueParameter<DoubleArray>("Sigma", "The vector containing the standard deviations used for manipulating each dimension. If it is only of length one the same sigma will be used for every dimension.", new DoubleArray(new double[] { 1 })));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RoundedNormalAllPositionsManipulator(this, cloner);
    }

    /// <summary>
    /// Performs a normally distributed all position manipulation on the given 
    /// <paramref name="vector"/> and rounds the result to the next feasible value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the sigma vector is null or of length 0.</exception>
    /// <param name="sigma">The sigma vector determining the strength of the mutation.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The integer vector to manipulate.</param>#
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <returns>The manipulated integer vector.</returns>
    public static void Apply(IRandom random, IntegerVector vector, IntMatrix bounds, DoubleArray sigma) {
      if (sigma == null || sigma.Length == 0) throw new ArgumentException("RoundedNormalAllPositionsManipulator: Vector containing the standard deviations is not defined.", "sigma");
      if (bounds == null || bounds.Rows == 0 || bounds.Columns < 2) throw new ArgumentException("RoundedNormalAllPositionsManipulator: Invalid bounds specified.", "bounds");
      var N = new NormalDistributedRandom(random, 0.0, 1.0);
      for (int i = 0; i < vector.Length; i++) {
        int min = bounds[i % bounds.Rows, 0], max = bounds[i % bounds.Rows, 1], step = 1;
        if (bounds.Columns > 2) step = bounds[i % bounds.Rows, 2];

        int value = (vector[i] + (int)Math.Round((N.NextDouble() * sigma[i % sigma.Length])) - min) / step;
        max = FloorFeasible(min, max, step, max - 1);
        vector[i] = RoundFeasible(min, max, step, value);
      }
    }

    /// <summary>
    /// Retrieves the bounds and forwards the call to the static Apply method.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="vector">The vector of integer values that is manipulated.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    protected override void ManipulateBounded(IRandom random, IntegerVector vector, IntMatrix bounds) {
      Apply(random, vector, bounds, SigmaParameter.Value);
    }
  }
}
