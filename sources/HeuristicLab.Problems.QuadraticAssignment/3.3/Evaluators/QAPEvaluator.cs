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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [StorableClass]
  public class QAPEvaluator : SingleSuccessorOperator, IQAPEvaluator {

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DoubleMatrix> DistancesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Distances"]; }
    }
    public ILookupParameter<DoubleMatrix> WeightsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Weights"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    [StorableConstructor]
    protected QAPEvaluator(bool deserializing) : base(deserializing) { }
    protected QAPEvaluator(QAPEvaluator original, Cloner cloner) : base(original, cloner) { }
    public QAPEvaluator() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation that represents the current solution."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The distance matrix that contains the distances between the locations."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The matrix with the weights between the facilities, that is how strongly they're connected to each other."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value aka fitness value of the solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPEvaluator(this, cloner);
    }

    public static double Apply(Permutation assignment, DoubleMatrix weights, DoubleMatrix distances) {
      double quality = 0;
      for (int i = 0; i < assignment.Length; i++) {
        for (int j = 0; j < assignment.Length; j++) {
          quality += weights[i, j] * distances[assignment[i], assignment[j]];
        }
      }
      return quality;
    }

    public static double Impact(int facility, Permutation assignment, DoubleMatrix weights, DoubleMatrix distances) {
      double impact = 0;
      for (int i = 0; i < assignment.Length; i++) {
        impact += weights[facility, i] * distances[assignment[facility], assignment[i]];
        impact += weights[i, facility] * distances[assignment[i], assignment[facility]];
      }
      return impact;
    }

    public override IOperation Apply() {
      Permutation assignment = PermutationParameter.ActualValue;
      DoubleMatrix weights = WeightsParameter.ActualValue;
      DoubleMatrix distanceMatrix = DistancesParameter.ActualValue;

      double quality = Apply(assignment, weights, distanceMatrix);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.Apply();
    }
  }
}
