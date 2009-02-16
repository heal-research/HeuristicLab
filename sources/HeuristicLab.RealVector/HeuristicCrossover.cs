#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.RealVector {
  /// <summary>
  /// Heuristic crossover for real vectors: Takes for each position the better parent and adds the difference
  /// of the two parents times a randomly chosen factor.
  /// </summary>
  public class HeuristicCrossover : RealVectorCrossoverBase {
    /// <summary>
    /// Initializes a new instance of <see cref="HeuristicCrossover"/> with two variable infos
    /// (<c>Maximization</c> and <c>Quality</c>).
    /// </summary>
    public HeuristicCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.In));
    }

    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Heuristic crossover for real vectors."; }
    }

    /// <summary>
    /// Perfomrs a heuristic crossover on the two given parents.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="maximization">Boolean flag whether it is a maximization problem.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="quality1">The quality of the first parent.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <param name="quality2">The quality of the second parent.</param>
    /// <returns>The newly created real vector, resulting from the heuristic crossover.</returns>
    public static double[] Apply(IRandom random, bool maximization, double[] parent1, double quality1, double[] parent2, double quality2) {
      int length = parent1.Length;
      double[] result = new double[length];
      double factor = random.NextDouble();

      for (int i = 0; i < length; i++) {
        if ((maximization && (quality1 > quality2)) || ((!maximization) && (quality1 < quality2)))
          result[i] = parent1[i] + factor * (parent1[i] - parent2[i]);
        else
          result[i] = parent2[i] + factor * (parent2[i] - parent1[i]);
      }
      return result;
    }

    /// <summary>
    /// Performs a heuristic crossover operation for two given parent real vectors.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two real vectors that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected override double[] Cross(IScope scope, IRandom random, double[][] parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in HeuristicCrossover: The number of parents is not equal to 2");
      bool maximization = GetVariableValue<BoolData>("Maximization", scope, true).Data;
      double quality1 = scope.SubScopes[0].GetVariableValue<DoubleData>("Quality", false).Data;
      double quality2 = scope.SubScopes[1].GetVariableValue<DoubleData>("Quality", false).Data;

      return Apply(random, maximization, parents[0], quality1, parents[1], quality2);
    }
  }
}
