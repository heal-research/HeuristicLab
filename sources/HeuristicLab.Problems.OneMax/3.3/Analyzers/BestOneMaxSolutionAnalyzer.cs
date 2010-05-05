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
  [Item("BestOneMaxSolutionAnalyzer", "An operator for analyzing the best solution for a OneMax problem.")]
  [StorableClass]
  class BestOneMaxSolutionAnalyzer : SingleSuccessorOperator, IBestOneMaxSolutionAnalyzer, IAnalyzer {

    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }
    ILookupParameter IBestOneMaxSolutionAnalyzer.BinaryVectorParameter {
      get { return BinaryVectorParameter; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
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

    public BestOneMaxSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The Onemax solutions from which the best solution should be visualized."));

      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The qualities of the Onemax solutions which should be visualized."));
      Parameters.Add(new LookupParameter<OneMaxSolution>("BestSolution", "The best Onemax solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the Onemax solution should be stored."));
    }

    public override IOperation Apply() {
      BinaryVector binaryVector = BinaryVectorParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      OneMaxSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new OneMaxSolution(binaryVector, QualityParameter.ActualValue);
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best OneMax Solution", solution));
      }  else {
        solution.BinaryVector = binaryVector;
        solution.Quality = QualityParameter.ActualValue;

        results["Best OneMax Solution"].Value = solution;
      }

      return base.Apply();
    }
  }
}
