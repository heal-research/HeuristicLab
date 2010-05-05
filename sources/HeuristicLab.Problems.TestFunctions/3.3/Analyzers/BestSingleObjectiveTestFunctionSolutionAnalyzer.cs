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
  [Item("BestSingleObjectiveTestFunctionSolutionAnalyzer", "An operator for analyzing the best solution for a SingleObjectiveTestFunction problem.")]
  [StorableClass]
  class BestSingleObjectiveTestFunctionSolutionAnalyzer : SingleSuccessorOperator, IBestSingleObjectiveTestFunctionSolutionAnalyzer, ISolutionAnalyzer {

    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    ILookupParameter IBestSingleObjectiveTestFunctionSolutionAnalyzer.RealVectorParameter {
      get { return RealVectorParameter; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
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

    public BestSingleObjectiveTestFunctionSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The SingleObjectiveTestFunction solutions from which the best solution should be visualized."));

      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The qualities of the SingleObjectiveTestFunction solutions which should be visualized."));
      Parameters.Add(new LookupParameter<SingleObjectiveTestFunctionSolution>("BestSolution", "The best SingleObjectiveTestFunction solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the SingleObjectiveTestFunction solution should be stored."));
    }

    public override IOperation Apply() {
      RealVector RealVector = RealVectorParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      SingleObjectiveTestFunctionSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new SingleObjectiveTestFunctionSolution(RealVector, QualityParameter.ActualValue);
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best SingleObjectiveTestFunction Solution", solution));
      }  else {
        solution.RealVector = RealVector;
        solution.Quality = QualityParameter.ActualValue;

        results["Best SingleObjectiveTestFunction Solution"].Value = solution;
      }

      return base.Apply();
    }
  }
}
