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

namespace HeuristicLab.Problems.OneMax.Analyzers {
  /// <summary>
  /// An operator for analyzing the best solution for a OneMax problem.
  /// </summary>
  [Item("MultiPopulationBestOneMaxSolutionAnalyzer", "An operator for analyzing the best solution for a OneMax problem.")]
  [StorableClass]
  class MultiPopulationBestOneMaxSolutionAnalyzer : SingleSuccessorOperator, IBestOneMaxSolutionAnalyzer, IAnalyzer {

    public ILookupParameter<ItemArray<ItemArray<BinaryVector>>> BinaryVectorParameter {
      get { return (ILookupParameter<ItemArray<ItemArray<BinaryVector>>>)Parameters["BinaryVector"]; }
    }
    ILookupParameter IBestOneMaxSolutionAnalyzer.BinaryVectorParameter {
      get { return BinaryVectorParameter; }
    }
    public ILookupParameter<ItemArray<ItemArray<DoubleValue>>> QualityParameter {
      get { return (ILookupParameter<ItemArray<ItemArray<DoubleValue>>>)Parameters["Quality"]; }
    }
    ILookupParameter IBestOneMaxSolutionAnalyzer.QualityParameter {
      get { return QualityParameter; }
    }
    public ILookupParameter<OneMaxSolution> BestSolutionParameter {
      get { return (ILookupParameter<OneMaxSolution>)Parameters["BestSolution"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public MultiPopulationBestOneMaxSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<ItemArray<BinaryVector>>("BinaryVector", "The Onemax solutions from which the best solution should be visualized."));

      Parameters.Add(new ScopeTreeLookupParameter<ItemArray<DoubleValue>>("Quality", "The qualities of the Onemax solutions which should be visualized."));
      Parameters.Add(new LookupParameter<OneMaxSolution>("BestSolution", "The best Onemax solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the Onemax solution should be stored."));
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

      OneMaxSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new OneMaxSolution(bestBinaryVector, bestQuality);
        BestSolutionParameter.ActualValue = solution;

        results.Add(new Result("Best OneMax Solution", solution));
      } else {
        if (solution.Quality.Value > bestQuality.Value) {
          solution.BinaryVector = bestBinaryVector;
          solution.Quality = bestQuality;

          results["Best OneMax Solution"].Value = solution;
        }
      }

      return base.Apply();
    }
  }
}
