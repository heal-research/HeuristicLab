﻿#region License Information
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
  /// Blend alpha-beta crossover for real vectors (BLX-a-b). Creates a new offspring by selecting a 
  /// random value from the interval between the two alleles of the parent solutions. 
  /// The interval is increased in both directions as follows: Into the direction of the 'better' 
  /// solution by the factor alpha, into the direction of the 'worse' solution by the factor beta.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Takahashi, M. and Kita, H. 2001. A crossover operator using independent component analysis for real-coded genetic algorithms Proceedings of the 2001 Congress on Evolutionary Computation, pp. 643-649.<br/>
  /// The default value for alpha is 0.75, the default value for beta is 0.25.
  /// </remarks>
  [Item("BlendAlphaBetaCrossover", "The blend alpha beta crossover (BLX-a-b) for real vectors is similar to the blend alpha crossover (BLX-a), but distinguishes between the better and worse of the parents. The interval from which to choose the new offspring can be extended more around the better parent by specifying a higher alpha value. It is implemented as described in Takahashi, M. and Kita, H. 2001. A crossover operator using independent component analysis for real-coded genetic algorithms Proceedings of the 2001 Congress on Evolutionary Computation, pp. 643-649.")]
  [EmptyStorableClass]
  public class BlendAlphaBetaCrossover : RealVectorCrossover {
    /// <summary>
    /// Whether the problem is a maximization or minimization problem.
    /// </summary>
    public ValueLookupParameter<BoolData> MaximizationParameter {
      get { return (ValueLookupParameter<BoolData>)Parameters["Maximization"]; }
    }
    /// <summary>
    /// The quality of the parents.
    /// </summary>
    public SubScopesLookupParameter<DoubleData> QualityParameter {
      get { return (SubScopesLookupParameter<DoubleData>)Parameters["Quality"]; }
    }
    /// <summary>
    /// The alpha parameter specifies how much the interval between the parents should be extended in direction of the better parent.
    /// </summary>
    public ValueLookupParameter<DoubleData> AlphaParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Alpha"]; }
    }
    /// <summary>
    /// The beta parameter specifies how much the interval between the parents should be extended in direction of the worse parent.
    /// </summary>
    public ValueLookupParameter<DoubleData> BetaParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Beta"]; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BlendAlphaBetaCrossover"/> with four additional parameters
    /// (<c>Maximization</c>, <c>Quality</c>, <c>Alpha</c> and <c>Beta</c>).
    /// </summary>
    public BlendAlphaBetaCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolData>("Maximization", "Whether the problem is a maximization problem or not."));
      Parameters.Add(new SubScopesLookupParameter<DoubleData>("Quality", "The quality values of the parents."));
      Parameters.Add(new ValueLookupParameter<DoubleData>("Alpha", "The value for alpha.", new DoubleData(0.75)));
      Parameters.Add(new ValueLookupParameter<DoubleData>("Beta", "The value for beta.", new DoubleData(0.25)));
    }

    /// <summary>
    /// Performs the blend alpha beta crossover (BLX-a-b) on two parent vectors.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when either:<br/>
    /// <list type="bullet">
    /// <item><description>The length of <paramref name="betterParent"/> and <paramref name="worseParent"/> is not equal.</description></item>
    /// <item><description>The parameter <paramref name="alpha"/> is smaller than 0.</description></item>
    /// <item><description>The parameter <paramref name="beta"/> is smaller than 0.</description></item>
    /// </list>
    /// </exception>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="betterParent">The better of the two parents with regard to their fitness.</param>
    /// <param name="worseParent">The worse of the two parents with regard to their fitness.</param>
    /// <param name="alpha">The parameter alpha.</param>
    /// <param name="beta">The parameter beta.</param>
    /// <returns>The real vector that results from the crossover.</returns>
    public static DoubleArrayData Apply(IRandom random, DoubleArrayData betterParent, DoubleArrayData worseParent, DoubleData alpha, DoubleData beta) {
      if (betterParent.Length != worseParent.Length) throw new ArgumentException("BlendAlphaBetaCrossover: The parents' vectors are of different length.", "betterParent");
      if (alpha.Value < 0) throw new ArgumentException("BlendAlphaBetaCrossover: Parameter alpha must be greater or equal to 0.", "alpha");
      if (beta.Value < 0) throw new ArgumentException("BlendAlphaBetaCrossover: Parameter beta must be greater or equal to 0.", "beta");
      int length = betterParent.Length;
      double min, max, d;
      DoubleArrayData result = new DoubleArrayData(length);

      for (int i = 0; i < length; i++) {
        d = Math.Abs(betterParent[i] - worseParent[i]);
        if (betterParent[i] <= worseParent[i]) {
          min = betterParent[i] - d * alpha.Value;
          max = worseParent[i] + d * beta.Value;
        } else {
          min = worseParent[i] - d * beta.Value;
          max = betterParent[i] + d * alpha.Value;
        }
        result[i] = min + random.NextDouble() * (max - min);
      }
      return result;
    }

    /// <summary>
    /// Checks if the number of parents is equal to 2, if all parameters are available and forwards the call to <see cref="Apply(IRandom, DoubleArrayData, DoubleArrayData, DoubleData, DoubleData)"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the number of parents is not equal to 2.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when either:<br/>
    /// <list type="bullet">
    /// <item><description>Maximization parameter could not be found.</description></item>
    /// <item><description>Quality parameter could not be found or the number of quality values is not equal to the number of parents.</description></item>
    /// <item><description>Alpha parameter could not be found.</description></item>
    /// <item><description>Beta parameter could not be found.</description></item>
    /// </list>
    /// </exception>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="parents">The collection of parents (must be of size 2).</param>
    /// <returns>The real vector that results from the crossover.</returns>
    protected override DoubleArrayData Cross(IRandom random, ItemArray<DoubleArrayData> parents) {
      if (parents.Length != 2) throw new ArgumentException("BlendAlphaBetaCrossover: Number of parents is not equal to 2.", "parents");
      if (MaximizationParameter.ActualValue == null) throw new InvalidOperationException("BlendAlphaBetaCrossover: Parameter " + MaximizationParameter.ActualName + " could not be found.");
      if (QualityParameter.ActualValue == null || QualityParameter.ActualValue.Length != parents.Length) throw new InvalidOperationException("BlendAlphaBetaCrossover: Parameter " + QualityParameter.ActualName + " could not be found, or not in the same quantity as there are parents.");
      if (AlphaParameter.ActualValue == null || BetaParameter.ActualValue == null) throw new InvalidOperationException("BlendAlphaBetaCrossover: Parameter " + AlphaParameter.ActualName + " or paramter " + BetaParameter.ActualName + " could not be found.");
      
      ItemArray<DoubleData> qualities = QualityParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      // the better parent 
      if (maximization && qualities[0].Value >= qualities[1].Value || !maximization && qualities[0].Value <= qualities[1].Value)
        return Apply(random, parents[0], parents[1], AlphaParameter.ActualValue, BetaParameter.ActualValue);
      else {
        return Apply(random, parents[1], parents[0], AlphaParameter.ActualValue, BetaParameter.ActualValue);
      }
    }
  }
}
