#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// Manipulates each dimension in the real vector with the mutation strength given 
  /// in the strategy parameter vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Beyer, H.-G. and Schwefel, H.-P. 2002. Evolution Strategies - A Comprehensive Introduction Natural Computing, 1, pp. 3-52.<br/>
  /// The strategy vector can be of smaller length than the solution vector, in which case values are taken from the beginning again once the end of the strategy vector is reached.
  /// </remarks>
  [Item("FixedNormalAllPositionsManipulator", "This manipulation operator adds a value sigma_i * N_i(0,1) to the current value in each position i given the values for sigma_i in the parameter. If there are less elements in Sigma than positions, then Sigma is cycled. It is implemented as described in Beyer, H.-G. and Schwefel, H.-P. 2002. Evolution Strategies - A Comprehensive Introduction Natural Computing, 1, pp. 3-52.")]
  [StorableClass]
  public class FixedNormalAllPositionsManipulator : RealVectorManipulator {

    public IValueParameter<RealVector> SigmaParameter {
      get { return (IValueParameter<RealVector>)Parameters["Sigma"]; }
    }

    [StorableConstructor]
    protected FixedNormalAllPositionsManipulator(bool deserializing) : base(deserializing) { }
    protected FixedNormalAllPositionsManipulator(FixedNormalAllPositionsManipulator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="NormalAllPositionsManipulator"/> with one
    /// parameter (<c>StrategyVector</c>).
    /// </summary>
    public FixedNormalAllPositionsManipulator()
      : base() {
      Parameters.Add(new ValueParameter<RealVector>("Sigma", "The vector containing the standard deviations used for manipulating each dimension. If it is only of length one the same sigma will be used for every dimension.", new RealVector(new double[] { 1 })));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FixedNormalAllPositionsManipulator(this, cloner);
    }

    /// <summary>
    /// Performs an adaptive normally distributed all position manipulation on the given 
    /// <paramref name="vector"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the strategy vector is not
    /// as long as the vector to get manipulated.</exception>
    /// <param name="sigma">The strategy vector determining the strength of the mutation.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <returns>The manipulated real vector.</returns>
    public static void Apply(IRandom random, RealVector vector, RealVector sigma) {
      if (sigma == null || sigma.Length == 0) throw new ArgumentException("ERROR: Vector containing the standard deviations is not defined.", "sigma");
      NormalDistributedRandom N = new NormalDistributedRandom(random, 0.0, 1.0);
      for (int i = 0; i < vector.Length; i++) {
        vector[i] = vector[i] + (N.NextDouble() * sigma[i % sigma.Length]);
      }
    }

    /// <summary>
    /// Checks that the strategy vector is not null and forwards the call to <see cref="Apply(IRandom, RealVector, RealVector)"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="realVector">The vector of real values that is manipulated.</param>
    protected override void Manipulate(IRandom random, RealVector realVector) {
      Apply(random, realVector, SigmaParameter.Value);
    }
  }
}
