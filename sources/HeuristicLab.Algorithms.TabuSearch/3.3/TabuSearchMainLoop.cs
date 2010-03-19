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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.TabuSearch {
  /// <summary>
  /// An operator which represents a tabu search.
  /// </summary>
  [Item("TabuSearchMainLoop", "An operator which represents the main loop of a tabu search.")]
  [StorableClass]
  public class TabuSearchMainLoop : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public LookupParameter<DoubleValue> QualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public LookupParameter<DoubleValue> MoveQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public LookupParameter<BoolValue> MoveTabuParameter {
      get { return (LookupParameter<BoolValue>)Parameters["MoveTabu"]; }
    }
    public ValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    public ValueLookupParameter<IntValue> TabuTenureParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["TabuTenure"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    public ValueLookupParameter<IOperator> MoveGeneratorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["MoveGenerator"]; }
    }
    public ValueLookupParameter<IOperator> MoveEvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["MoveEvaluator"]; }
    }
    public ValueLookupParameter<IOperator> MoveMakerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["MoveMaker"]; }
    }
    public ValueLookupParameter<IOperator> TabuMoveEvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["TabuMoveEvaluator"]; }
    }
    public ValueLookupParameter<IOperator> TabuMoveMakerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["TabuMoveMaker"]; }
    }

    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    public TabuSearchMainLoop()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The value which represents the quality of a move."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The value that indicates if a move is tabu or not."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("TabuTenure", "The length of the tabu list, and also means the number of iterations a move is kept tabu"));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveMaker", "The operator that performs a move and updates the quality."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveEvaluator", "The operator that evaluates a move."));
      Parameters.Add(new ValueLookupParameter<IOperator>("TabuMoveEvaluator", "The operator that evaluates whether a move is tabu."));
      Parameters.Add(new ValueLookupParameter<IOperator>("TabuMoveMaker", "The operator that declares a move tabu."));

      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the TS should be applied."));
      #endregion

      #region Create operators
      TabuListCreator tabuListCreator = new TabuListCreator();
      VariableCreator variableCreator = new VariableCreator();
      BestQualityMemorizer bestQualityMemorizer = new BestQualityMemorizer();
      ResultsCollector resultsCollector = new ResultsCollector();
      UniformSequentialSubScopesProcessor mainProcessor = new UniformSequentialSubScopesProcessor();
      Placeholder moveGenerator = new Placeholder();
      UniformSequentialSubScopesProcessor moveEvaluationProcessor = new UniformSequentialSubScopesProcessor();
      Placeholder moveEvaluator = new Placeholder();
      Placeholder tabuMoveEvaluator = new Placeholder();
      SubScopesSorter moveQualitySorter = new SubScopesSorter();
      BestAverageWorstQualityCalculator bestAverageWorstMoveQualityCalculator = new BestAverageWorstQualityCalculator();
      TabuSelector tabuSelector = new TabuSelector();
      RightReducer rightReducer = new RightReducer();
      UniformSequentialSubScopesProcessor moveMakingProcessor = new UniformSequentialSubScopesProcessor();
      Placeholder tabuMoveMaker = new Placeholder();
      Placeholder moveMaker = new Placeholder();
      DataTableValuesCollector valuesCollector = new DataTableValuesCollector();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      ConditionalBranch iterationsTermination = new ConditionalBranch();
      EmptyOperator finished = new EmptyOperator();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("Best Move Quality", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("Average Move Quality", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("Worst Move Quality", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DataTable>("MoveQualities", new DataTable("MoveQualities")));

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Iterations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality") { ActualName = "BestQuality" });
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Average Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Worst Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("MoveQualities"));
      resultsCollector.ResultsParameter.ActualName = "Results";

      mainProcessor.Name = "Solution processor (UniformSequentialSubScopesProcessor)";

      moveGenerator.Name = "MoveGenerator (placeholder)";
      moveGenerator.OperatorParameter.ActualName = "MoveGenerator";

      moveEvaluator.Name = "MoveEvaluator (placeholder)";
      moveEvaluator.OperatorParameter.ActualName = "MoveEvaluator";

      tabuMoveEvaluator.Name = "TabuMoveEvaluator (placeholder)";
      tabuMoveEvaluator.OperatorParameter.ActualName = "TabuMoveEvaluator";

      moveQualitySorter.DescendingParameter.ActualName = "Maximization";
      moveQualitySorter.ValueParameter.ActualName = "MoveQuality";

      bestAverageWorstMoveQualityCalculator.AverageQualityParameter.ActualName = "Average Move Quality";
      bestAverageWorstMoveQualityCalculator.BestQualityParameter.ActualName = "Best Move Quality";
      bestAverageWorstMoveQualityCalculator.MaximizationParameter.ActualName = "Maximization";
      bestAverageWorstMoveQualityCalculator.QualityParameter.ActualName = "MoveQuality";
      bestAverageWorstMoveQualityCalculator.WorstQualityParameter.ActualName = "Worst Move Quality";

      moveMakingProcessor.Name = "MoveMaking processor (UniformSequentialSubScopesProcessor)";

      tabuMoveMaker.Name = "TabuMoveMaker (placeholder)";
      tabuMoveMaker.OperatorParameter.ActualName = "TabuMoveMaker";

      moveMaker.Name = "MoveMaker (placeholder)";
      moveMaker.OperatorParameter.ActualName = "MoveMaker";

      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Move Quality"));
      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Average Move Quality"));
      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Worst Move Quality"));
      valuesCollector.DataTableParameter.ActualName = "MoveQualities";

      subScopesRemover.RemoveAllSubScopes = true;

      iterationsCounter.Name = "Iterations Counter";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = "Iterations";

      iterationsComparator.Name = "Iterations Comparator";
      iterationsComparator.Comparison = new Comparison(ComparisonType.Less);
      iterationsComparator.LeftSideParameter.ActualName = "Iterations";
      iterationsComparator.RightSideParameter.ActualName = "MaximumIterations";
      iterationsComparator.ResultParameter.ActualName = "IterationsCondition";

      iterationsTermination.Name = "Iterations Termination Condition";
      iterationsTermination.ConditionParameter.ActualName = "IterationsCondition";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = tabuListCreator;
      tabuListCreator.Successor = variableCreator;
      variableCreator.Successor = bestQualityMemorizer;
      bestQualityMemorizer.Successor = resultsCollector;
      resultsCollector.Successor = mainProcessor;
      mainProcessor.Operator = moveGenerator;
      mainProcessor.Successor = valuesCollector;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operator = moveEvaluator;
      moveEvaluationProcessor.Successor = moveQualitySorter;
      moveEvaluator.Successor = tabuMoveEvaluator;
      moveQualitySorter.Successor = bestAverageWorstMoveQualityCalculator;
      bestAverageWorstMoveQualityCalculator.Successor = tabuSelector;
      tabuSelector.Successor = rightReducer;
      rightReducer.Successor = moveMakingProcessor;
      moveMakingProcessor.Operator = tabuMoveMaker;
      moveMakingProcessor.Successor = subScopesRemover;
      tabuMoveMaker.Successor = moveMaker;
      valuesCollector.Successor = iterationsCounter;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = bestQualityMemorizer;
      iterationsTermination.FalseBranch = finished;
      #endregion
    }
  }
}
