#region License Information
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Optimization;
using HeuristicLab.Data;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Knapsack.Analyzers {
  /// <summary>
  /// An operator for analyzing the best solution for a knapsack problem.
  /// </summary>
  [Item("BestKnapsackSolutionAnalyzer", "An operator for analyzing the best solution for a knapsack problem.")]
  [StorableClass]
  class BestKnapsackSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {

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

    public BestKnapsackSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<BinaryVector>("BinaryVector", "The knapsack solutions from which the best solution should be visualized."));
      Parameters.Add(new LookupParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack."));
      Parameters.Add(new LookupParameter<IntArray>("Weights", "The weights of the items."));
      Parameters.Add(new LookupParameter<IntArray>("Values", "The values of the items."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the knapsack solutions which should be visualized."));
      Parameters.Add(new LookupParameter<KnapsackSolution>("BestSolution", "The best knapsack solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the knapsack solution should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<BinaryVector> binaryVectors = BinaryVectorParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      KnapsackSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new KnapsackSolution(binaryVectors[i], qualities[i],
          KnapsackCapacityParameter.ActualValue, WeightsParameter.ActualValue, ValuesParameter.ActualValue);
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best Knapsack Solution", solution));
      }  else {
        solution.BinaryVector = binaryVectors[i];
        solution.Quality = qualities[i];
        solution.Capacity = KnapsackCapacityParameter.ActualValue;
        solution.Weights = WeightsParameter.ActualValue;
        solution.Values = ValuesParameter.ActualValue;

        results["Best Knapsack Solution"].Value = solution;
      }

      return base.Apply();
    }
  }
}
