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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.Analyzers {
  /// <summary>
  /// An operator for analyzing the best solution for a SingleObjectiveTestFunction problem.
  /// </summary>
  [Item("PopulationBestSingleObjectiveTestFunctionSolutionAnalyzer", "An operator for analyzing the best solution for a SingleObjectiveTestFunction problem.")]
  [StorableClass]
  class PopulationBestSingleObjectiveTestFunctionSolutionAnalyzer : SingleSuccessorOperator, IBestSingleObjectiveTestFunctionSolutionAnalyzer, IAnalyzer {

    public ILookupParameter<ItemArray<RealVector>> RealVectorParameter {
      get { return (ILookupParameter<ItemArray<RealVector>>)Parameters["RealVector"]; }
    }
    ILookupParameter IBestSingleObjectiveTestFunctionSolutionAnalyzer.RealVectorParameter {
      get { return RealVectorParameter; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["Quality"]; }
    }
    ILookupParameter IBestSingleObjectiveTestFunctionSolutionAnalyzer.QualityParameter {
      get { return QualityParameter; }
    }
    public ILookupParameter<SingleObjectiveTestFunctionSolution> BestSolutionParameter {
      get { return (ILookupParameter<SingleObjectiveTestFunctionSolution>)Parameters["BestSolution"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public PopulationBestSingleObjectiveTestFunctionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("RealVector", "The SingleObjectiveTestFunction solutions from which the best solution should be visualized."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the SingleObjectiveTestFunction solutions which should be visualized."));
      Parameters.Add(new LookupParameter<SingleObjectiveTestFunctionSolution>("BestSolution", "The best SingleObjectiveTestFunction solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the SingleObjectiveTestFunction solution should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<RealVector> RealVectors = RealVectorParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      SingleObjectiveTestFunctionSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new SingleObjectiveTestFunctionSolution(RealVectors[i], QualityParameter.ActualValue[i]);
        BestSolutionParameter.ActualValue = solution;

        results.Add(new Result("Best SingleObjectiveTestFunction Solution", solution));
      } else {
        solution.RealVector = RealVectors[i];
        solution.Quality = QualityParameter.ActualValue[i];

        results["Best SingleObjectiveTestFunction Solution"].Value = solution;
      }

      return base.Apply();
    }
  }
}
