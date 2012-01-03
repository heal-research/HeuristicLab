#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// Performs the simulated binary crossover (SBX) on a vector of real values such that each position is either crossed contracted or expanded with a certain probability.
  /// The probability distribution is designed such that the children will lie closer to their parents as is the case with the single point binary crossover.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Deb, K. and Agrawal, R. B. 1995. Simulated binary crossover for continuous search space. Complex Systems, 9, pp. 115-148.
  /// </remarks>
  [Item("SimulatedBinaryCrossover", "The simulated binary crossover (SBX) is implemented as described in Deb, K. and Agrawal, R. B. 1995. Simulated binary crossover for continuous search space. Complex Systems, 9, pp. 115-148.")]
  [StorableClass]
  public class SimulatedBinaryCrossover : RealVectorCrossover {
    /// <summary>
    /// The parameter must be greater or equal than 0. Common values are in the range [0;5] and more often just [2;5].
    /// </summary>
    public ValueLookupParameter<DoubleValue> ContiguityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Contiguity"]; }
    }

    [StorableConstructor]
    protected SimulatedBinaryCrossover(bool deserializing) : base(deserializing) { }
    protected SimulatedBinaryCrossover(SimulatedBinaryCrossover original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="SimulatedBinaryCrossover"/> with one 
    /// parameter (<c>Contiguity</c>).
    /// </summary>
    public SimulatedBinaryCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Contiguity", "Specifies whether the crossover should produce very different (small value) or very similar (large value) children. Valid values must be greater or equal to 0.", new DoubleValue(2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimulatedBinaryCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the simulated binary crossover on a real vector. Each position is crossed with a probability of 50% and if crossed either a contracting crossover or an expanding crossover is performed, again with equal probability.
    /// For more details refer to the paper by Deb and Agrawal.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the parents' vectors are of unequal length or when <paramref name="contiguity"/> is smaller than 0.</exception>
    /// <remarks>
    /// The manipulated value is not restricted by the (possibly) specified lower and upper bounds. Use the <see cref="BoundsChecker"/> to correct the values after performing the crossover.
    /// </remarks>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="parent1">The first parent vector.</param>
    /// <param name="parent2">The second parent vector.</param>
    /// <param name="contiguity">The contiguity value that specifies how close a child should be to its parents (larger value means closer). The value must be greater or equal than 0. Typical values are in the range [2;5].</param>
    /// <returns>The vector resulting from the crossover.</returns>
    public static RealVector Apply(IRandom random, RealVector parent1, RealVector parent2, DoubleValue contiguity) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("SimulatedBinaryCrossover: Parents are of unequal length");
      if (contiguity.Value < 0) throw new ArgumentException("SimulatedBinaryCrossover: Contiguity value is smaller than 0", "contiguity");
      int length = parent1.Length;
      RealVector result = new RealVector(length);
      for (int i = 0; i < length; i++) {
        if (length == 1 || random.NextDouble() < 0.5) { // cross this variable
          double u = random.NextDouble();
          double beta = 0;
          if (u < 0.5) { // if u is smaller than 0.5 perform a contracting crossover
            beta = Math.Pow(2 * u, 1.0 / (contiguity.Value + 1));
          } else if (u > 0.5) { // otherwise perform an expanding crossover
            beta = Math.Pow(0.5 / (1.0 - u), 1.0 / (contiguity.Value + 1));
          } else if (u == 0.5)
            beta = 1;

          if (random.NextDouble() < 0.5)
            result[i] = ((parent1[i] + parent2[i]) / 2.0) - beta * 0.5 * Math.Abs(parent1[i] - parent2[i]);
          else
            result[i] = ((parent1[i] + parent2[i]) / 2.0) + beta * 0.5 * Math.Abs(parent1[i] - parent2[i]);
        } else result[i] = parent1[i];
      }
      return result;
    }

    /// <summary>
    /// Checks number of parents, availability of the parameters and forwards the call to <see cref="Apply(IRandom, RealVector, RealVector, DoubleValue)"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when there are not exactly 2 parents or when the contiguity parameter could not be found.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The collection of parents (must be of size 2).</param>
    /// <returns>The real vector resulting from the crossover.</returns>
    protected override RealVector Cross(IRandom random, ItemArray<RealVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("SimulatedBinaryCrossover: The number of parents is not equal to 2");
      if (ContiguityParameter.ActualValue == null) throw new InvalidOperationException("SimulatedBinaryCrossover: Parameter " + ContiguityParameter.ActualName + " could not be found.");
      return Apply(random, parents[0], parents[1], ContiguityParameter.ActualValue);
    }
  }
}
