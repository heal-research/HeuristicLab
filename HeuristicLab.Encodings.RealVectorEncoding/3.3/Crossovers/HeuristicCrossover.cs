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
using HEAL.Fossil;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// Heuristic crossover for real vectors: Calculates the vector from the worse to the better parent and adds that to the better parent weighted with a factor in the interval [0;1).
  /// The idea is that going further in direction from the worse to the better leads to even better solutions (naturally this depends on the fitness landscape).
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Wright, A.H. (1994), Genetic algorithms for real parameter optimization, Foundations of Genetic Algorithms, G.J.E. Rawlins (Ed.), Morgan Kaufmann, San Mateo, CA, 205-218.
  /// </remarks>
  [Item("HeuristicCrossover", "The heuristic crossover produces offspring that extend the better parent in direction from the worse to the better parent. It is implemented as described in Wright, A.H. (1994), Genetic algorithms for real parameter optimization, Foundations of Genetic Algorithms, G.J.E. Rawlins (Ed.), Morgan Kaufmann, San Mateo, CA, 205-218.")]
  [StorableType("CBD8F372-73EA-4D52-8D35-A3D2D514B96A")]
  public class HeuristicCrossover : RealVectorCrossover, ISingleObjectiveOperator {
    /// <summary>
    /// Whether the problem is a maximization or minimization problem.
    /// </summary>
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    /// <summary>
    /// The quality of the parents.
    /// </summary>
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    [StorableConstructor]
    protected HeuristicCrossover(StorableConstructorFlag _) : base(_) { }
    protected HeuristicCrossover(HeuristicCrossover original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="HeuristicCrossover"/> with two variable infos
    /// (<c>Maximization</c> and <c>Quality</c>).
    /// </summary>
    public HeuristicCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "Whether the problem is a maximization problem or not."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality values of the parents."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HeuristicCrossover(this, cloner);
    }

    /// <summary>
    /// Perfomrs a heuristic crossover on the two given parents.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when two parents are not of the same length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="betterParent">The first parent for the crossover operation.</param>
    /// <param name="worseParent">The second parent for the crossover operation.</param>
    /// <returns>The newly created real vector, resulting from the heuristic crossover.</returns>
    public static RealVector Apply(IRandom random, RealVector betterParent, RealVector worseParent) {
      if (betterParent.Length != worseParent.Length)
        throw new ArgumentException("HeuristicCrossover: the two parents are not of the same length");

      int length = betterParent.Length;
      double[] result = new double[length];
      double factor = random.NextDouble();

      for (int i = 0; i < length; i++) {
        result[i] = betterParent[i] + factor * (betterParent[i] - worseParent[i]);
      }
      return new RealVector(result);
    }

    /// <summary>
    /// Performs a heuristic crossover operation for two given parent real vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the number of parents is not equal to 2.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when either:<br/>
    /// <list type="bullet">
    /// <item><description>Maximization parameter could not be found.</description></item>
    /// <item><description>Quality parameter could not be found or the number of quality values is not equal to the number of parents.</description></item>
    /// </list>
    /// </exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two real vectors that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected override RealVector Cross(IRandom random, ItemArray<RealVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("HeuristicCrossover: The number of parents is not equal to 2");

      if (MaximizationParameter.ActualValue == null) throw new InvalidOperationException("HeuristicCrossover: Parameter " + MaximizationParameter.ActualName + " could not be found.");
      if (QualityParameter.ActualValue == null || QualityParameter.ActualValue.Length != parents.Length) throw new InvalidOperationException("HeuristicCrossover: Parameter " + QualityParameter.ActualName + " could not be found, or not in the same quantity as there are parents.");

      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;

      if (maximization && qualities[0].Value >= qualities[1].Value || !maximization && qualities[0].Value <= qualities[1].Value)
        return Apply(random, parents[0], parents[1]);
      else
        return Apply(random, parents[1], parents[0]);
    }
  }
}
