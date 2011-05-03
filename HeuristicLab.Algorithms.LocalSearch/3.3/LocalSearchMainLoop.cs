#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    public LookupParameter<DoubleValue> BestLocalQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["BestLocalQuality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public LookupParameter<DoubleValue> MoveQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public LookupParameter<IntValue> IterationsParameter {
      get { return (LookupParameter<IntValue>)Parameters["Iterations"]; }
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
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public LookupParameter<IntValue> EvaluatedMovesParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedMoves"]; }
    }
    #endregion

    [StorableConstructor]
    private LocalSearchMainLoop(bool deserializing) : base(deserializing) { }
    public LocalSearchMainLoop()
      : base() {
      Initialize();
    }
    private LocalSearchMainLoop(LocalSearchMainLoop original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LocalSearchMainLoop(this, cloner);
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestLocalQuality", "The value which represents the best quality found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The problem's best known quality value found so far."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The value which represents the quality of a move."));
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "The number of iterations performed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveMaker", "The operator that performs a move and updates the quality."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveEvaluator", "The operator that evaluates a move."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze the solution and moves."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedMoves", "The number of evaluated moves."));
      #endregion

      #region Create operators
      SubScopesProcessor subScopesProcessor0 = new SubScopesProcessor();
      Assigner bestQualityInitializer = new Assigner();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      SubScopesProcessor mainProcessor = new SubScopesProcessor();
      Placeholder moveGenerator = new Placeholder();
      UniformSubScopesProcessor moveEvaluationProcessor = new UniformSubScopesProcessor();
      Placeholder moveEvaluator = new Placeholder();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      BestSelector bestSelector = new BestSelector();
      SubScopesProcessor moveMakingProcessor = new SubScopesProcessor();
      UniformSubScopesProcessor selectedMoveMakingProcessor = new UniformSubScopesProcessor();
      QualityComparator qualityComparator = new QualityComparator();
      ConditionalBranch improvesQualityBranch = new ConditionalBranch();
      Placeholder moveMaker = new Placeholder();
      Assigner bestQualityUpdater = new Assigner();
      ResultsCollector resultsCollector2 = new ResultsCollector();
      MergingReducer mergingReducer = new MergingReducer();
      Placeholder analyzer2 = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      ConditionalBranch iterationsTermination = new ConditionalBranch();

      bestQualityInitializer.Name = "Initialize BestQuality";
      bestQualityInitializer.LeftSideParameter.ActualName = BestLocalQualityParameter.Name;
      bestQualityInitializer.RightSideParameter.ActualName = QualityParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CopyValue = new BoolValue(false);
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>(IterationsParameter.Name));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>(BestLocalQualityParameter.Name, null, BestLocalQualityParameter.Name));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      moveGenerator.Name = "MoveGenerator (placeholder)";
      moveGenerator.OperatorParameter.ActualName = MoveGeneratorParameter.Name;

      moveEvaluationProcessor.Parallel = new BoolValue(true);

      moveEvaluator.Name = "MoveEvaluator (placeholder)";
      moveEvaluator.OperatorParameter.ActualName = MoveEvaluatorParameter.Name;

      subScopesCounter.Name = "Increment EvaluatedMoves";
      subScopesCounter.ValueParameter.ActualName = EvaluatedMovesParameter.Name;

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
      bestQualityUpdater.LeftSideParameter.ActualName = BestLocalQualityParameter.Name;
      bestQualityUpdater.RightSideParameter.ActualName = QualityParameter.Name;

      resultsCollector2.CopyValue = new BoolValue(false);
      resultsCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>(BestLocalQualityParameter.Name, null, BestLocalQualityParameter.Name));
      resultsCollector2.ResultsParameter.ActualName = ResultsParameter.Name;

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      iterationsCounter.Name = "Iterations Counter";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = IterationsParameter.Name;

      iterationsComparator.Name = "Iterations >= MaximumIterations";
      iterationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      iterationsComparator.LeftSideParameter.ActualName = IterationsParameter.Name;
      iterationsComparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      iterationsComparator.ResultParameter.ActualName = "Terminate";

      iterationsTermination.Name = "Iterations Termination Condition";
      iterationsTermination.ConditionParameter.ActualName = "Terminate";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = subScopesProcessor0; // don't change this without adapting the constructor of LocalSearchImprovementOperator
      subScopesProcessor0.Operators.Add(bestQualityInitializer);
      subScopesProcessor0.Successor = resultsCollector1;
      bestQualityInitializer.Successor = analyzer1;
      analyzer1.Successor = null;
      resultsCollector1.Successor = mainProcessor;
      mainProcessor.Operators.Add(moveGenerator);
      mainProcessor.Successor = iterationsCounter;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operator = moveEvaluator;
      moveEvaluationProcessor.Successor = subScopesCounter;
      moveEvaluator.Successor = null;
      subScopesCounter.Successor = bestSelector;
      bestSelector.Successor = moveMakingProcessor;
      moveMakingProcessor.Operators.Add(new EmptyOperator());
      moveMakingProcessor.Operators.Add(selectedMoveMakingProcessor);
      moveMakingProcessor.Successor = mergingReducer;
      selectedMoveMakingProcessor.Operator = qualityComparator;
      qualityComparator.Successor = improvesQualityBranch;
      improvesQualityBranch.TrueBranch = moveMaker;
      improvesQualityBranch.FalseBranch = null;
      improvesQualityBranch.Successor = null;
      moveMaker.Successor = bestQualityUpdater;
      bestQualityUpdater.Successor = null;
      mergingReducer.Successor = analyzer2;
      analyzer2.Successor = subScopesRemover;
      subScopesRemover.Successor = null;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = null;
      iterationsTermination.FalseBranch = mainProcessor;
      #endregion
    }

    public override IOperation Apply() {
      if (MoveGeneratorParameter.ActualValue == null || MoveEvaluatorParameter.ActualValue == null || MoveMakerParameter.ActualValue == null)
        return null;
      return base.Apply();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("BestLocalQuality"))
        Parameters.Add(new LookupParameter<DoubleValue>("BestLocalQuality", "The value which represents the best quality found so far."));
      if (!Parameters.ContainsKey("Iterations"))
        Parameters.Add(new LookupParameter<IntValue>("Iterations", "The number of iterations performed."));
      #endregion
    }
  }
}
