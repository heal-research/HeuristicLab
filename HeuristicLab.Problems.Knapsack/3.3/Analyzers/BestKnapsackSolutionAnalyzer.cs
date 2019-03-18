#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Knapsack {
  /// <summary>
  /// An operator for analyzing the best solution for a Knapsack problem.
  /// </summary>
  [Item("BestKnapsackSolutionAnalyzer", "An operator for analyzing the best solution for a Knapsack problem.")]
  [StorableClass]
  public class BestKnapsackSolutionAnalyzer : SingleSuccessorOperator, IBinaryVectorSolutionsOperator, IAnalyzer, ISingleObjectiveOperator {
    public virtual bool EnabledByDefault {
      get { return true; }
    }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public IScopeTreeLookupParameter<BinaryVector> BinaryVectorsParameter {
      get { return (IScopeTreeLookupParameter<BinaryVector>)Parameters["BinaryVectors"]; }
    }
    public ILookupParameter<IntValue> KnapsackCapacityParameter {
      get { return (ILookupParameter<IntValue>)Parameters["KnapsackCapacity"]; }
    }
    public ILookupParameter<IntArray> WeightsParameter {
      get { return (ILookupParameter<IntArray>)Parameters["Weights"]; }
    }
    public ILookupParameter<IntArray> ValuesParameter {
      get { return (ILookupParameter<IntArray>)Parameters["Values"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<KnapsackSolution> BestSolutionParameter {
      get { return (ILookupParameter<KnapsackSolution>)Parameters["BestSolution"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public ILookupParameter<BinaryVector> BestKnownSolutionParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BestKnownSolution"]; }
    }

    [StorableConstructor]
    protected BestKnapsackSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected BestKnapsackSolutionAnalyzer(BestKnapsackSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public BestKnapsackSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<BinaryVector>("BinaryVectors", "The Knapsack solutions from which the best solution should be visualized."));
      Parameters.Add(new LookupParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack."));
      Parameters.Add(new LookupParameter<IntArray>("Weights", "The weights of the items."));
      Parameters.Add(new LookupParameter<IntArray>("Values", "The values of the items."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the Knapsack solutions which should be visualized."));
      Parameters.Add(new LookupParameter<KnapsackSolution>("BestSolution", "The best Knapsack solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the knapsack solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution."));
      Parameters.Add(new LookupParameter<BinaryVector>("BestKnownSolution", "The best known solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestKnapsackSolutionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var binaryVectors = BinaryVectorsParameter.ActualValue;
      var qualities = QualityParameter.ActualValue;
      var results = ResultsParameter.ActualValue;
      var max = MaximizationParameter.ActualValue.Value;
      var bestKnownQuality = BestKnownQualityParameter.ActualValue;

      int i = -1;
      i = !max ? qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index
               : qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      if (bestKnownQuality == null ||
          max && qualities[i].Value > bestKnownQuality.Value ||
          !max && qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (BinaryVector)binaryVectors[i].Clone();
      }

      var solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new KnapsackSolution((BinaryVector)binaryVectors[i].Clone(), new DoubleValue(qualities[i].Value),
          KnapsackCapacityParameter.ActualValue, WeightsParameter.ActualValue, ValuesParameter.ActualValue);
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best Knapsack Solution", solution));
      } else {
        if (max && qualities[i].Value > solution.Quality.Value ||
          !max && qualities[i].Value < solution.Quality.Value) {
          solution.BinaryVector = (BinaryVector)binaryVectors[i].Clone();
          solution.Quality = new DoubleValue(qualities[i].Value);
          solution.Capacity = KnapsackCapacityParameter.ActualValue;
          solution.Weights = WeightsParameter.ActualValue;
          solution.Values = ValuesParameter.ActualValue;
        }
      }

      return base.Apply();
    }
  }
}
