#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    public bool EnabledByDefault {
      get { return true; }
    }

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
    public LookupParameter<ItemSet<Permutation>> BestKnownSolutionsParameter {
      get { return (LookupParameter<ItemSet<Permutation>>)Parameters["BestKnownSolutions"]; }
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
      Parameters.Add(new LookupParameter<ItemSet<Permutation>>("BestKnownSolutions", "The best known solutions (there may be multiple) of this QAP instance."));
      Parameters.Add(new LookupParameter<Permutation>("BestKnownSolution", "The best known solution of this QAP instance."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("BestKnownSolutions")) {
        Parameters.Add(new LookupParameter<ItemSet<Permutation>>("BestKnownSolutions", "The best known solutions of this QAP instance."));
      } else if (Parameters["BestKnownSolutions"].GetType().Equals(typeof(LookupParameter<ItemList<Permutation>>))) {
        string actualName = (Parameters["BestKnownSolutions"] as LookupParameter<ItemList<Permutation>>).ActualName;
        Parameters.Remove("BestKnownSolutions");
        Parameters.Add(new LookupParameter<ItemSet<Permutation>>("BestKnownSolutions", "The best known solutions of this QAP instance.", actualName));
      }
      #endregion
    }

    public override IOperation Apply() {
      DoubleMatrix distances = DistancesParameter.ActualValue;
      DoubleMatrix weights = WeightsParameter.ActualValue;
      ItemArray<Permutation> permutations = PermutationParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      var sorted = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).ToArray();
      if (max) sorted = sorted.Reverse().ToArray();
      int i = sorted.First().index;

      if (bestKnownQuality == null
          || max && qualities[i].Value > bestKnownQuality.Value
          || !max && qualities[i].Value < bestKnownQuality.Value) {
        // if there isn't a best-known quality or we improved the best-known quality we'll add the current solution as best-known
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (Permutation)permutations[i].Clone();
        BestKnownSolutionsParameter.ActualValue = new ItemSet<Permutation>(new PermutationEqualityComparer());
        BestKnownSolutionsParameter.ActualValue.Add((Permutation)permutations[i].Clone());
      } else if (bestKnownQuality.Value == qualities[i].Value) {
        // if we matched the best-known quality we'll try to set the best-known solution if it isn't null
        // and try to add it to the pool of best solutions if it is different
        if (BestKnownSolutionParameter.ActualValue == null)
          BestKnownSolutionParameter.ActualValue = (Permutation)permutations[i].Clone();
        if (BestKnownSolutionsParameter.ActualValue == null)
          BestKnownSolutionsParameter.ActualValue = new ItemSet<Permutation>(new PermutationEqualityComparer());
        foreach (var k in sorted) { // for each solution that we found check if it is in the pool of best-knowns
          if (!max && k.Value > qualities[i].Value
            || max && k.Value < qualities[i].Value) break; // stop when we reached a solution worse than the best-known quality
          Permutation p = permutations[k.index];
          if (!BestKnownSolutionsParameter.ActualValue.Contains(p))
            BestKnownSolutionsParameter.ActualValue.Add((Permutation)permutations[k.index].Clone());
        }
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
