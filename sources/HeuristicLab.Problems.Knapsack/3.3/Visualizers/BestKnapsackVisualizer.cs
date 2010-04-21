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

namespace HeuristicLab.Problems.Knapsack.Visualizers {
  /// <summary>
  /// An operator for visualizing the best solution for a knapsack problem.
  /// </summary>
  [Item("BestKnapsackVisualizer", "An operator for visualizing the best solution for a knapsack problem.")]
  [StorableClass]
  class BestKnapsackVisualizer : SingleSuccessorOperator, IKnapsackSolutionsVisualizer {

    public ILookupParameter<ItemArray<BinaryVector>> BinaryVectorParameter {
      get { return (ILookupParameter<ItemArray<BinaryVector>>)Parameters["BinaryVector"]; }
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
    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["Quality"]; }
    }
    public ILookupParameter<KnapsackSolution> KnapsackSolutionParameter {
      get { return (ILookupParameter<KnapsackSolution>)Parameters["KnapsackSolutionVisualization"]; }
    }

    ILookupParameter ISolutionsVisualizer.VisualizationParameter {
      get { return KnapsackSolutionParameter; }
    }

    public BestKnapsackVisualizer()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<BinaryVector>("BinaryVector", "The knapsack solutions from which the best solution should be visualized."));
      Parameters.Add(new LookupParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack."));
      Parameters.Add(new LookupParameter<IntArray>("Weights", "The weights of the items."));
      Parameters.Add(new LookupParameter<IntArray>("Values", "The values of the items."));
      
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The qualities of the knapsack solutions which should be visualized."));
      Parameters.Add(new LookupParameter<KnapsackSolution>("KnapsackSolutionVisualization", "The visual representation of the best knapsack solution."));
    }

    public override IOperation Apply() {
      ItemArray<BinaryVector> binaryVectors = BinaryVectorParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      KnapsackSolution solution = KnapsackSolutionParameter.ActualValue;
      if (solution == null) KnapsackSolutionParameter.ActualValue = new KnapsackSolution(binaryVectors[i], 
        KnapsackCapacityParameter.ActualValue, WeightsParameter.ActualValue, ValuesParameter.ActualValue);
      else {
        solution.BinaryVector = binaryVectors[i];
        solution.Capacity = KnapsackCapacityParameter.ActualValue;
        solution.Weights = WeightsParameter.ActualValue;
        solution.Values = ValuesParameter.ActualValue;
      }

      return base.Apply();
    }
  }
}
