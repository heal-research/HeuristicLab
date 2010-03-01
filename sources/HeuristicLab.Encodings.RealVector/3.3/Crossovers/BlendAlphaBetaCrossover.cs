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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// Blend alpha-beta crossover for real vectors. Creates a new offspring by selecting a 
  /// random value from the interval between the two alleles of the parent solutions. 
  /// The interval is increased in both directions as follows: Into the direction of the 'better' 
  /// solution by the factor alpha, into the direction of the 'worse' solution by the factor beta.
  /// </summary>
  [Item("BlendAlphaBetaCrossover", "FIXME: CHECK WITH LITERATURE AND VALIDATE IT.")]
  [EmptyStorableClass]
  public class BlendAlphaBetaCrossover : RealVectorCrossover {
    public ValueLookupParameter<BoolData> MaximizationParameter {
      get { return (ValueLookupParameter<BoolData>)Parameters["Maximization"]; }
    }
    public SubScopesLookupParameter<DoubleData> QualityParameter {
      get { return (SubScopesLookupParameter<DoubleData>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<DoubleData> AlphaParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Alpha"]; }
    }
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

    public static DoubleArrayData Apply(IRandom random, DoubleArrayData betterParent, DoubleArrayData worseParent, DoubleData alpha, DoubleData beta) {
      if (betterParent.Length != worseParent.Length) throw new ArgumentException("BlendAlphaBetaCrossover: The parents' vectors are of different length.", "betterParent");
      int length = betterParent.Length;
      DoubleArrayData result = new DoubleArrayData(length);

      for (int i = 0; i < length; i++) {
        double interval = Math.Abs(betterParent[i] - worseParent[i]);
        result[i] = SelectFromInterval(random, interval, betterParent[i], worseParent[i], alpha.Value, beta.Value);
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

    protected override DoubleArrayData Cross(IRandom random, ItemArray<DoubleArrayData> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("BlendAlphaBetaCrossover: Number of parents is not equal to 2.");
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
