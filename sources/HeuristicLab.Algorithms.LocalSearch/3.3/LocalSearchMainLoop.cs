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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.LocalSearch {
  /// <summary>
  /// An operator which represents a local search.
  /// </summary>
  [Item("LocalSearchMainLoop", "An operator which represents the main loop of a best improvement local search (if only a single move is generated in each iteration it is a first improvement local search).")]
  [StorableClass]
  public sealed class LocalSearchMainLoop : AlgorithmOperator {
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
    public ValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public LookupParameter<DoubleValue> MoveQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
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
    public ValueLookupParameter<IOperator> MoveAnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["MoveAnalyzer"]; }
    }
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }

    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private LocalSearchMainLoop(bool deserializing) : base() { }
    public LocalSearchMainLoop()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The value which represents the quality of a move."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveMaker", "The operator that performs a move and updates the quality."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveEvaluator", "The operator that evaluates a move."));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveAnalyzer", "The operator used to analyze the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze the solution."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the TS should be applied."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      SubScopesProcessor subScopesProcessor0 = new SubScopesProcessor();
      Assigner bestQualityInitializer = new Assigner();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      ResultsCollector resultsCollector2 = new ResultsCollector();
      SubScopesProcessor mainProcessor = new SubScopesProcessor();
      Placeholder moveGenerator = new Placeholder();
      UniformSubScopesProcessor moveEvaluationProcessor = new UniformSubScopesProcessor();
      Placeholder moveEvaluator = new Placeholder();
      IntCounter evaluatedMovesCounter = new IntCounter();
      Placeholder moveAnalyzer = new Placeholder();
      BestSelector bestSelector = new BestSelector();
      RightReducer rightReducer = new RightReducer();
      SubScopesProcessor moveMakingProcessor = new SubScopesProcessor();
      QualityComparator qualityComparator = new QualityComparator();
      ConditionalBranch improvesQualityBranch = new ConditionalBranch();
      Placeholder moveMaker = new Placeholder();
      Assigner bestQualityUpdater = new Assigner();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      Placeholder analyzer2 = new Placeholder();
      ResultsCollector resultsCollector3 = new ResultsCollector();
      ConditionalBranch iterationsTermination = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedMoves", new IntValue(0)));

      bestQualityInitializer.Name = "Initialize BestQuality";
      bestQualityInitializer.LeftSideParameter.ActualName = "BestQuality";
      bestQualityInitializer.RightSideParameter.ActualName = QualityParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CopyValue = new BoolValue(false);
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Iterations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      resultsCollector2.CopyValue = new BoolValue(true);
      resultsCollector2.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Moves", null, "EvaluatedMoves"));
      resultsCollector2.ResultsParameter.ActualName = ResultsParameter.Name;

      moveGenerator.Name = "MoveGenerator (placeholder)";
      moveGenerator.OperatorParameter.ActualName = MoveGeneratorParameter.Name;

      moveEvaluator.Name = "MoveEvaluator (placeholder)";
      moveEvaluator.OperatorParameter.ActualName = MoveEvaluatorParameter.Name;

      evaluatedMovesCounter.Name = "EvaluatedMoves + 1";
      evaluatedMovesCounter.ValueParameter.ActualName = "EvaluatedMoves";
      evaluatedMovesCounter.Increment = new IntValue(1);

      moveAnalyzer.Name = "MoveAnalyzer (placeholder)";
      moveAnalyzer.OperatorParameter.ActualName = MoveAnalyzerParameter.Name;

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.Value = new IntValue(1);
      bestSelector.QualityParameter.ActualName = MoveQualityParameter.Name;

      qualityComparator.LeftSideParameter.ActualName = MoveQualityParameter.Name;
      qualityComparator.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparator.ResultParameter.ActualName = "IsBetter";

      improvesQualityBranch.ConditionParameter.ActualName = "IsBetter";

      moveMaker.Name = "MoveMaker (placeholder)";
      moveMaker.OperatorParameter.ActualName = MoveMakerParameter.Name;

      bestQualityUpdater.Name = "Update BestQuality";
      bestQualityUpdater.LeftSideParameter.ActualName = "BestQuality";
      bestQualityUpdater.RightSideParameter.ActualName = QualityParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      iterationsCounter.Name = "Iterations Counter";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = "Iterations";

      iterationsComparator.Name = "Iterations >= MaximumIterations";
      iterationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      iterationsComparator.LeftSideParameter.ActualName = "Iterations";
      iterationsComparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      iterationsComparator.ResultParameter.ActualName = "Terminate";

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector3.CopyValue = new BoolValue(true);
      resultsCollector3.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Moves", null, "EvaluatedMoves"));
      resultsCollector3.ResultsParameter.ActualName = ResultsParameter.Name;

      iterationsTermination.Name = "Iterations Termination Condition";
      iterationsTermination.ConditionParameter.ActualName = "Terminate";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = subScopesProcessor0;
      subScopesProcessor0.Operators.Add(bestQualityInitializer);
      subScopesProcessor0.Successor = resultsCollector1;
      bestQualityInitializer.Successor = analyzer1;
      analyzer1.Successor = null;
      resultsCollector1.Successor = resultsCollector2;
      resultsCollector2.Successor = mainProcessor;
      mainProcessor.Operators.Add(moveGenerator);
      mainProcessor.Successor = iterationsCounter;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operator = moveEvaluator;
      moveEvaluationProcessor.Successor = moveAnalyzer;
      moveEvaluator.Successor = evaluatedMovesCounter;
      evaluatedMovesCounter.Successor = null;
      moveAnalyzer.Successor = bestSelector;
      bestSelector.Successor = rightReducer;
      rightReducer.Successor = moveMakingProcessor;
      moveMakingProcessor.Operators.Add(qualityComparator);
      moveMakingProcessor.Successor = subScopesRemover;
      subScopesRemover.Successor = null;
      qualityComparator.Successor = improvesQualityBranch;
      improvesQualityBranch.TrueBranch = moveMaker;
      improvesQualityBranch.FalseBranch = null;
      improvesQualityBranch.Successor = null;
      moveMaker.Successor = bestQualityUpdater;
      bestQualityUpdater.Successor = null;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(analyzer2);
      subScopesProcessor1.Successor = resultsCollector3;
      analyzer2.Successor = null;
      resultsCollector3.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = null;
      iterationsTermination.FalseBranch = mainProcessor;
      #endregion
    }

    public override IOperation Apply() {
      if (MoveGeneratorParameter.ActualValue == null || MoveEvaluatorParameter.ActualValue == null || MoveMakerParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
