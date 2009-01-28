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
  public class HeuristicCrossover : CrossoverBase {
    /// <summary>
    /// Initializes a new instance of <see cref="HeuristicCrossover"/> with three variable infos
    /// (<c>Maximization</c>, <c>Quality</c> and <c>RealVector</c>).
    /// </summary>
    public HeuristicCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("RealVector", "Parent and child real vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
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
    /// Perfomrs a heuristic crossover on the two given parents.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the parent vectors have different lengths.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <param name="child">The newly created real vector, resulting from the heuristic crossover.</param>
    protected sealed override void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child) {
      bool maximization = GetVariableValue<BoolData>("Maximization", scope, true).Data;
      DoubleArrayData vector1 = parent1.GetVariableValue<DoubleArrayData>("RealVector", false);
      DoubleData quality1 = parent1.GetVariableValue<DoubleData>("Quality", false);
      DoubleArrayData vector2 = parent2.GetVariableValue<DoubleArrayData>("RealVector", false);
      DoubleData quality2 = parent2.GetVariableValue<DoubleData>("Quality", false);

      if (vector1.Data.Length != vector2.Data.Length) throw new InvalidOperationException("Cannot apply crossover to real vectors of different length.");

      double[] result = Apply(random, maximization, vector1.Data, quality1.Data, vector2.Data, quality2.Data);
      child.AddVariable(new Variable(child.TranslateName("RealVector"), new DoubleArrayData(result)));
    }
  }
}
