#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// The self adaptive discrete crossover is similar to the <see cref="DiscreteCrossover"/>, but will also copy values from an exising strategy vector (such as in the sigma self adaptive evolution strategy)
  /// from the same parent to the offspring in each position. The strategy vector can be of different length to the parent vector. The idea is that if it is shorter, it will be cycled.
  /// </summary>
  /// <remarks>
  /// This operator is not mentioned in the literature, as typically intermediate recombination (<see cref="AverageCrossover"/>) should be used to recombine the strategy vector (Beyer and Schwefel 2002).
  /// Since <see cref="AverageCrossover"/> is deterministic this can be done for solution vector and strategy vector separately.<br/>
  /// However if one wishes to use discrete (dominant) recombination on the strategy vectors the idea is probably that the same parent should donate strategy value and solution value in a position. In this case use this operator.
  /// Otherwise, you can also apply <see cref="DiscreteCrossover"/> independently on the solution vector and on the strategy vector.
  /// </remarks>
  public class SelfAdaptiveDiscreteCrossover : RealVectorCrossover {
    /// <summary>
    /// Parameter for the parents' strategy vectors.
    /// </summary>
    public SubScopesLookupParameter<DoubleArrayData> ParentsStrategyVectorParameter {
      get { return (SubScopesLookupParameter<DoubleArrayData>)Parameters["ParentsStrategyVector"]; }
    }
    /// <summary>
    /// Parameter for the child's strategy vector.
    /// </summary>
    public LookupParameter<DoubleArrayData> ChildStrategyVectorParameter {
      get { return (LookupParameter<DoubleArrayData>)Parameters["ChildStrategyVector"]; }
    }
    /// <summary>
    /// Initializes a new instance of <see cref="SelfAdaptiveDiscreteCrossover"/> with two parameters (<c>ParentsStrategyVector</c>, and <c>ChildStrategyVector</c>).
    /// </summary>
    public SelfAdaptiveDiscreteCrossover()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<DoubleArrayData>("ParentsStrategyVector", "The vector containing the parents' endogenous strategy parameters."));
      ParentsStrategyVectorParameter.ActualName = "StrategyVector";
      Parameters.Add(new LookupParameter<DoubleArrayData>("ChildStrategyVector", "The vector containing the child's endogenous strategy parameters."));
      ChildStrategyVectorParameter.ActualName = "StrategyVector";
    }

    /// <summary>
    /// Performs a discrete crossover operation for multiple given parent vectors in a way that the same parent
    /// is used to donate the value of the solution and of the strategy vector in each position.
    /// </summary>
    /// <remarks>
    /// The parent vectors can be of a different length to the strategy vectors, but among each group the length has to be the same.
    /// </remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the parents that should be crossed.</param>
    /// <param name="strategies">An array containing the strategy vectors of the parents.</param>
    /// <param name="child">The newly created real vector, resulting from the crossover operation (output parameter).</param>
    /// <param name="strategy">The newly created strategy vector, resulting from the crossover operation (output parameter).</param>
    public static void Apply(IRandom random, ItemArray<DoubleArrayData> parents, ItemArray<DoubleArrayData> strategies, out DoubleArrayData child, out DoubleArrayData strategy) {
      if (strategies.Length != parents.Length) throw new ArgumentException("SelfAdaptiveDiscreteCrossover: Number of parents is not equal to the number of strategy vectors", "parents");
      int length = parents[0].Length, strategyLength = strategies[0].Length, parentsCount = parents.Length;
      child = new DoubleArrayData(length);
      strategy = new DoubleArrayData(strategyLength);
      int loopEnd = Math.Max(length, strategyLength);
      try {
        for (int i = 0; i < loopEnd; i++) {
          int nextParent = random.Next(parentsCount);
          if (i < length) child[i] = parents[nextParent][i];
          if (i < strategyLength) strategy[i] = strategies[nextParent][i];
        }
      } catch (IndexOutOfRangeException) {
        throw new ArgumentException("SelfAdaptiveDiscreteCrossover: There are different lengths among either the parent vectors or the parents' strategy vectors.");
      }
    }

    /// <summary>
    /// Checks that there are at least two parents, that the strategy vector is not null and forwards the call to <see cref="Apply(IRandom, ItemArray<DoubleArrayData>, ItemArray<DoubleArrayData>, out DoubleArrayData, out DoubleArrayData)"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The collection of parents (at least of size 2).</param>
    /// <returns>The child vector that results from the crossover.</returns>
    protected override DoubleArrayData Cross(IRandom random, ItemArray<DoubleArrayData> parents) {
      if (parents.Length < 2) throw new ArgumentException("SelfAdaptiveDiscreteCrossover: The number of parents is less than 2", "parents");
      if (ParentsStrategyVectorParameter.ActualValue == null) throw new InvalidOperationException("SelfAdaptiveDiscreteCrossover: Parameter " + ParentsStrategyVectorParameter.ActualName + " could not be found.");
      ItemArray<DoubleArrayData> strategies = ParentsStrategyVectorParameter.ActualValue;      
      DoubleArrayData result, resultStrategyVector;
      Apply(random, parents, strategies, out result, out resultStrategyVector);
      ChildStrategyVectorParameter.ActualValue = resultStrategyVector;
      return result;
    }
  }
}
