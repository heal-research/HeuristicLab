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

using HeuristicLab.Analysis;
using HeuristicLab.Core;
using HeuristicLab.Data;
//using HeuristicLab.Evolutionary;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
//using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.TS {
  /// <summary>
  /// An operator which represents a Tabu Search.
  /// </summary>
  [Item("TSOperator", "An operator which represents a Tabu Search.")]
  public class TSOperator : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolData> MaximizationParameter {
      get { return (ValueLookupParameter<BoolData>)Parameters["Maximization"]; }
    }
    public SubScopesLookupParameter<DoubleData> QualityParameter {
      get { return (SubScopesLookupParameter<DoubleData>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<IntData> MaximumIterationsParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["MaximumIterations"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    public TSOperator()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolData>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleData>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<IntData>("MaximumIterations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the TS should be applied."));
      #endregion

      #region Create operator graph
      VariableCreator variableCreator = new VariableCreator();
      ResultsCollector resultsCollector = new ResultsCollector();

      OperatorGraph.InitialOperator = variableCreator;

      variableCreator.CollectedValues.Add(new ValueParameter<IntData>("Iterations", new IntData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Best Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Best Move Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Average Move Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Worst Move Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DataTable>("Qualities", new DataTable("Qualities")));
      variableCreator.CollectedValues.Add(new ValueParameter<DataTable>("MoveQualities", new DataTable("MoveQualities")));
      variableCreator.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntData>("Iterations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Best Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Best Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Average Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Worst Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("MoveQualities"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      resultsCollector.Successor = null;
      #endregion
    }
  }
}
