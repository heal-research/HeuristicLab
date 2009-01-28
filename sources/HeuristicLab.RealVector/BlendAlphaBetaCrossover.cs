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
using HeuristicLab.Evolutionary;
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  /// <summary>
  /// Blend alpha-beta crossover for real vectors. Creates a new offspring by selecting a 
  /// random value from the interval between the two alleles of the parent solutions. 
  /// The interval is increased in both directions as follows: Into the direction of the 'better' 
  /// solution by the factor alpha, into the direction of the 'worse' solution by the factor beta.
  /// </summary>
  public class BlendAlphaBetaCrossover : CrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return
@"Blend alpha-beta crossover for real vectors. Creates a new offspring by selecting a random value from the interval between the two alleles of the parent solutions. The interval is increased in both directions as follows: Into the direction of the 'better' solution by the factor alpha, into the direction of the 'worse' solution by the factor beta.
Please use the operator BoundsChecker if necessary.";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BlendAlphaBetaCrossover"/> with five variable infos
    /// (<c>Maximization</c>, <c>Quality</c>, <c>RealVector</c>, <c>Alpha</c> and <c>Beta</c>).
    /// </summary>
    public BlendAlphaBetaCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("RealVector", "Parent and child real vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
      VariableInfo alphaVarInfo = new VariableInfo("Alpha", "Value for alpha", typeof(DoubleData), VariableKind.In);
      alphaVarInfo.Local = true;
      AddVariableInfo(alphaVarInfo);
      AddVariable(new Variable("Alpha", new DoubleData(0.75)));
      VariableInfo betaVarInfo = new VariableInfo("Beta", "Value for beta", typeof(DoubleData), VariableKind.In);
      betaVarInfo.Local = true;
      AddVariableInfo(betaVarInfo);
      AddVariable(new Variable("Beta", new DoubleData(0.25)));
    }

    /// <summary>
    /// Performs a blend alpha beta crossover of two real vectors.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="maximization">Boolean flag whether it is a maximization problem.</param>
    /// <param name="parent1">The first parent for the crossover.</param>
    /// <param name="quality1">The quality of the first parent.</param>
    /// <param name="parent2">The second parent for the crossover.</param>
    /// <param name="quality2">The quality of the second parent.</param>
    /// <param name="alpha">The alpha value for the crossover.</param>
    /// <param name="beta">The beta value for the crossover operation.</param>
    /// <returns>The newly created real vector resulting from the crossover.</returns>
    public static double[] Apply(IRandom random, bool maximization, double[] parent1, double quality1, double[] parent2, double quality2, double alpha, double beta) {
      int length = parent1.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++) {
        double interval = Math.Abs(parent1[i] - parent2[i]);

        if ((maximization && (quality1 > quality2)) || ((!maximization) && (quality1 < quality2))) {
          result[i] = SelectFromInterval(random, interval, parent1[i], parent2[i], alpha, beta);
        } else {
          result[i] = SelectFromInterval(random, interval, parent2[i], parent1[i], alpha, beta);
        }
      }
      return result;
    }

    private static double SelectFromInterval(IRandom random, double interval, double val1, double val2, double alpha, double beta) {
      double resMin = 0;
      double resMax = 0;

      if (val1 <= val2) {
        resMin = val1 - interval * alpha;
        resMax = val2 + interval * beta;
      } else {
        resMin = val2 - interval * beta;
        resMax = val1 + interval * alpha;
      }

      return SelectRandomFromInterval(random, resMin, resMax);
    }

    private static double SelectRandomFromInterval(IRandom random, double resMin, double resMax) {
      return resMin + random.NextDouble() * Math.Abs(resMax - resMin);
    }

    /// <summary>
    /// Performs a blend alpha beta crossover of two real vectors.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover.</param>
    /// <param name="parent2">The second parent for the crossover.</param>
    /// <param name="child">The created child generated through the blend alpha beta crossover.</param>
    protected sealed override void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child) {
      bool maximization = GetVariableValue<BoolData>("Maximization", scope, true).Data;
      DoubleArrayData vector1 = parent1.GetVariableValue<DoubleArrayData>("RealVector", false);
      DoubleData quality1 = parent1.GetVariableValue<DoubleData>("Quality", false);
      DoubleArrayData vector2 = parent2.GetVariableValue<DoubleArrayData>("RealVector", false);
      DoubleData quality2 = parent2.GetVariableValue<DoubleData>("Quality", false);
      double alpha = GetVariableValue<DoubleData>("Alpha", scope, true).Data;
      double beta = GetVariableValue<DoubleData>("Beta", scope, true).Data;

      if (vector1.Data.Length != vector2.Data.Length) throw new InvalidOperationException("Cannot apply crossover to real vectors of different length.");

      double[] result = Apply(random, maximization, vector1.Data, quality1.Data, vector2.Data, quality2.Data, alpha, beta);
      child.AddVariable(new Variable(child.TranslateName("RealVector"), new DoubleArrayData(result)));
    }
  }
}
