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
  class BestSingleObjectiveTestFunctionSolutionAnalyzer : SingleSuccessorOperator, IBestSingleObjectiveTestFunctionSolutionAnalyzer, IAnalyzer {

    public ScopeTreeLookupParameter<RealVector> RealVectorParameter {
      get { return (ScopeTreeLookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    ILookupParameter IBestSingleObjectiveTestFunctionSolutionAnalyzer.RealVectorParameter {
      get { return RealVectorParameter; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
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
    public IValueLookupParameter<ISingleObjectiveTestFunctionProblemEvaluator> EvaluatorParameter {
      get { return (IValueLookupParameter<ISingleObjectiveTestFunctionProblemEvaluator>)Parameters["Evaluator"]; }
    }

    public BestSingleObjectiveTestFunctionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("RealVector", "The SingleObjectiveTestFunction solutions from which the best solution should be visualized."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the SingleObjectiveTestFunction solutions which should be visualized."));
      Parameters.Add(new LookupParameter<SingleObjectiveTestFunctionSolution>("BestSolution", "The best SingleObjectiveTestFunction solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the SingleObjectiveTestFunction solution should be stored."));
      Parameters.Add(new ValueLookupParameter<ISingleObjectiveTestFunctionProblemEvaluator>("Evaluator", "The evaluator with which the solution is evaluated."));
    }

    public override IOperation Apply() {
      ItemArray<RealVector> realVectors = RealVectorParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      ISingleObjectiveTestFunctionProblemEvaluator evaluator = EvaluatorParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      SingleObjectiveTestFunctionSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new SingleObjectiveTestFunctionSolution(realVectors[i], qualities[i], evaluator);
        solution.Population = realVectors;
        BestSolutionParameter.ActualValue = solution;

        results.Add(new Result("Best SingleObjectiveTestFunction Solution", solution));
      } else {
        if (evaluator.Maximization && qualities[i].Value > solution.BestQuality.Value
          || !evaluator.Maximization && qualities[i].Value < solution.BestQuality.Value) {
          solution.BestRealVector = realVectors[i];
          solution.BestQuality = qualities[i];
        }
        solution.Population = realVectors;

        //results["Best SingleObjectiveTestFunction Solution"].Value = solution;
      }

      return base.Apply();
    }
  }
}
