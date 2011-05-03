#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  /// <summary>
  /// An operator for analyzing the best solution of Quadratic Assignment Problems.
  /// </summary>
  [Item("BestQAPSolutionAnalyzer", "An operator for analyzing the best solution of Quadratic Assignment Problems.")]
  [StorableClass]
  public sealed class BestQAPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public LookupParameter<DoubleMatrix> DistancesParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Distances"]; }
    }
    public LookupParameter<DoubleMatrix> WeightsParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Weights"]; }
    }
    public ScopeTreeLookupParameter<Permutation> PermutationParameter {
      get { return (ScopeTreeLookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public LookupParameter<QAPAssignment> BestSolutionParameter {
      get { return (LookupParameter<QAPAssignment>)Parameters["BestSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public LookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public LookupParameter<Permutation> BestKnownSolutionParameter {
      get { return (LookupParameter<Permutation>)Parameters["BestKnownSolution"]; }
    }

    [StorableConstructor]
    private BestQAPSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private BestQAPSolutionAnalyzer(BestQAPSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestQAPSolutionAnalyzer(this, cloner);
    }
    public BestQAPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The distances between the locations."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The weights between the facilities."));
      Parameters.Add(new ScopeTreeLookupParameter<Permutation>("Permutation", "The QAP solutions from which the best solution should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the QAP solutions which should be analyzed."));
      Parameters.Add(new LookupParameter<QAPAssignment>("BestSolution", "The best QAP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best QAP solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this QAP instance."));
      Parameters.Add(new LookupParameter<Permutation>("BestKnownSolution", "The best known solution of this QAP instance."));
    }

    public override IOperation Apply() {
      DoubleMatrix distances = DistancesParameter.ActualValue;
      DoubleMatrix weights = WeightsParameter.ActualValue;
      ItemArray<Permutation> permutations = PermutationParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      int i = -1;
      if (!max)
        i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      else i = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      if (bestKnownQuality == null ||
          max && qualities[i].Value > bestKnownQuality.Value ||
          !max && qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (Permutation)permutations[i].Clone();
      }

      QAPAssignment assignment = BestSolutionParameter.ActualValue;
      if (assignment == null) {
        assignment = new QAPAssignment(weights, (Permutation)permutations[i].Clone(), new DoubleValue(qualities[i].Value));
        assignment.Distances = distances;
        BestSolutionParameter.ActualValue = assignment;
        results.Add(new Result("Best QAP Solution", assignment));
      } else {
        if (max && assignment.Quality.Value < qualities[i].Value ||
          !max && assignment.Quality.Value > qualities[i].Value) {
          assignment.Distances = distances;
          assignment.Weights = weights;
          assignment.Assignment = (Permutation)permutations[i].Clone();
          assignment.Quality.Value = qualities[i].Value;
        }
      }

      return base.Apply();
    }
  }
}
