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

namespace HeuristicLab.RealVector {
  /// <summary>
  /// Blend alpha crossover for real vectors. Creates a new offspring by selecting a random value 
  /// from the interval between the two alleles of the parent solutions. The interval is increased 
  /// in both directions by the factor alpha.
  /// </summary>
  public class BlendAlphaCrossover : RealVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return
@"Blend alpha crossover for real vectors. Creates a new offspring by selecting a random value from the interval between the two alleles of the parent solutions. The interval is increased in both directions by the factor alpha.
Please use the operator BoundsChecker if necessary.";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BlendAlphaCrossover"/> with one variable info (<c>Alpha</c>).
    /// </summary>
    public BlendAlphaCrossover()
      : base() {
      VariableInfo alphaVarInfo = new VariableInfo("Alpha", "Value for alpha", typeof(DoubleData), VariableKind.In);
      alphaVarInfo.Local = true;
      AddVariableInfo(alphaVarInfo);
      AddVariable(new Variable("Alpha", new DoubleData(0.5)));
    }

    /// <summary>
    /// Performs a blend alpha crossover of two real vectors.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <param name="alpha">The alpha value for the crossover.</param>
    /// <returns>The newly created real vector resulting from the crossover operation.</returns>
    public static double[] Apply(IRandom random, double[] parent1, double[] parent2, double alpha) {
      int length = parent1.Length;
      double[] result = new double[length];
      double cMax = 0, cMin = 0, interval = 0, resMin = 0, resMax = 0;

      for (int i = 0; i < length; i++) {
        cMax = Math.Max(parent1[i], parent2[i]);
        cMin = Math.Min(parent1[i], parent2[i]);
        interval = Math.Abs(cMax - cMin);
        resMin = cMin - interval * alpha;
        resMax = cMax + interval * alpha;

        result[i] = resMin + random.NextDouble() * Math.Abs(resMax - resMin);
      }
      return result;
    }

    /// <summary>
    /// Performs a blend alpha crossover of two real vectors.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <returns>The newly created real vector, resulting from the blend alpha crossover.</returns>
    protected override double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      double alpha = GetVariableValue<DoubleData>("Alpha", scope, true).Data;
      return Apply(random, parent1, parent2, alpha);
    }
  }
}
