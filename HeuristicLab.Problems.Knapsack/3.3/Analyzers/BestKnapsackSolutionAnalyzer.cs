#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Problems.Knapsack {
  /// <summary>
  /// An operator for analyzing the best solution for a Knapsack problem.
  /// </summary>
  [Item("BestKnapsackSolutionAnalyzer", "An operator for analyzing the best solution for a Knapsack problem.")]
  [StorableType("40BCB11E-F9C9-4749-A4FF-831BD8342D1A")]
  public class BestKnapsackSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer, ISingleObjectiveOperator {
    public virtual bool EnabledByDefault {
      get { return true; }
    }

    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ScopeTreeLookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }
    public LookupParameter<IntValue> KnapsackCapacityParameter {
      get { return (LookupParameter<IntValue>)Parameters["KnapsackCapacity"]; }
    }
    public LookupParameter<IntArray> WeightsParameter {
      get { return (LookupParameter<IntArray>)Parameters["Weights"]; }
    }
    public LookupParameter<IntArray> ValuesParameter {
      get { return (LookupParameter<IntArray>)Parameters["Values"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public LookupParameter<KnapsackSolution> BestSolutionParameter {
      get { return (LookupParameter<KnapsackSolution>)Parameters["BestSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public LookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public LookupParameter<BinaryVector> BestKnownSolutionParameter {
      get { return (LookupParameter<BinaryVector>)Parameters["BestKnownSolution"]; }
    }

    [StorableConstructor]
    protected BestKnapsackSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected BestKnapsackSolutionAnalyzer(BestKnapsackSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public BestKnapsackSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<BinaryVector>("BinaryVector", "The Knapsack solutions from which the best solution should be visualized."));
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
      ItemArray<BinaryVector> binaryVectors = BinaryVectorParameter.ActualValue;
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
        BestKnownSolutionParameter.ActualValue = (BinaryVector)binaryVectors[i].Clone();
      }

      KnapsackSolution solution = BestSolutionParameter.ActualValue;
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
