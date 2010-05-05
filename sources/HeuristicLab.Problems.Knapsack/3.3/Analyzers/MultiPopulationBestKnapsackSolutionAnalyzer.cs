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
  [Item("MultiPopulationBestKnapsackSolutionAnalyzer", "An operator for analyzing the best solution for a knapsack problem.")]
  [StorableClass]
  class MultiPopulationBestKnapsackSolutionAnalyzer : SingleSuccessorOperator, IBestKnapsackSolutionAnalyzer, IAnalyzer {

    public ILookupParameter<ItemArray<ItemArray<BinaryVector>>> BinaryVectorParameter {
      get { return (ILookupParameter<ItemArray<ItemArray<BinaryVector>>>)Parameters["BinaryVector"]; }
    }
    ILookupParameter IBestKnapsackSolutionAnalyzer.BinaryVectorParameter {
      get { return BinaryVectorParameter; }
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
    public ILookupParameter<ItemArray<ItemArray<DoubleValue>>> QualityParameter {
      get { return (ILookupParameter<ItemArray<ItemArray<DoubleValue>>>)Parameters["Quality"]; }
    }
    ILookupParameter IBestKnapsackSolutionAnalyzer.QualityParameter {
      get { return QualityParameter; }
    }
    public ILookupParameter<KnapsackSolution> BestSolutionParameter {
      get { return (ILookupParameter<KnapsackSolution>)Parameters["BestSolution"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public MultiPopulationBestKnapsackSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<ItemArray<BinaryVector>>("BinaryVector", "The knapsack solutions from which the best solution should be visualized."));
      Parameters.Add(new LookupParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack."));
      Parameters.Add(new LookupParameter<IntArray>("Weights", "The weights of the items."));
      Parameters.Add(new LookupParameter<IntArray>("Values", "The values of the items."));
      
      Parameters.Add(new ScopeTreeLookupParameter<ItemArray<DoubleValue>>("Quality", "The qualities of the knapsack solutions which should be visualized."));
      Parameters.Add(new LookupParameter<KnapsackSolution>("BestSolution", "The best knapsack solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the knapsack solution should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<ItemArray<BinaryVector>> binaryVectors = BinaryVectorParameter.ActualValue;
      ItemArray<ItemArray<DoubleValue>> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      DoubleValue bestQuality = new DoubleValue(double.MaxValue);
      BinaryVector bestBinaryVector = null;

      for (int i = 0; i < qualities.Length; i++) {
        for (int j = 0; j < qualities[i].Length; j++) {
          if (qualities[i][j].Value < bestQuality.Value) {
            bestQuality = qualities[i][j];
            bestBinaryVector = binaryVectors[i][j];
          }
        }
      }

      KnapsackSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new KnapsackSolution(bestBinaryVector, bestQuality,
          KnapsackCapacityParameter.ActualValue, WeightsParameter.ActualValue, ValuesParameter.ActualValue);
        BestSolutionParameter.ActualValue = solution;

        results.Add(new Result("Best Knapsack Solution", solution));
      } else {
        if (solution.Quality.Value > bestQuality.Value) {
          solution.BinaryVector = bestBinaryVector;
          solution.Quality = bestQuality;
          solution.Capacity = KnapsackCapacityParameter.ActualValue;
          solution.Weights = WeightsParameter.ActualValue;
          solution.Values = ValuesParameter.ActualValue;

          results["Best Knapsack Solution"].Value = solution;
        }
      }

      return base.Apply();
    }
  }
}
