#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.TabuSearch {
  /// <summary>
  /// An operator which represents a tabu search.
  /// </summary>
  [Item("TabuSearchMainLoop", "An operator which represents the main loop of a tabu search.")]
  [StorableClass]
  public sealed class TabuSearchMainLoop : AlgorithmOperator {
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
    public LookupParameter<BoolValue> MoveTabuParameter {
      get { return (LookupParameter<BoolValue>)Parameters["MoveTabu"]; }
    }
    public ValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    public ValueLookupParameter<IntValue> TabuTenureParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["TabuTenure"]; }
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
    public ValueLookupParameter<IOperator> TabuCheckerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["TabuChecker"]; }
    }
    public ValueLookupParameter<IOperator> TabuMakerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["TabuMaker"]; }
    }
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    public LookupParameter<IntValue> EvaluatedMovesParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedMoves"]; }
    }
    #endregion

    [StorableConstructor]
    private TabuSearchMainLoop(bool deserializing) : base(deserializing) { }
    public TabuSearchMainLoop()
      : base() {
      Initialize();
    }
    private TabuSearchMainLoop(TabuSearchMainLoop original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TabuSearchMainLoop(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      VariableCreator variableCreator = OperatorGraph.InitialOperator as VariableCreator;

      if (variableCreator != null) {
        if (!variableCreator.CollectedValues.ContainsKey("Memories")) {
          variableCreator.CollectedValues.Add(new ValueParameter<VariableCollection>("Memories", new VariableCollection()));
        }
      }
      #endregion
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The value which represents the quality of a move."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The value that indicates if a move is tabu or not."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("TabuTenure", "The length of the tabu list, and also means the number of iterations a move is kept tabu"));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveMaker", "The operator that performs a move and updates the quality."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveEvaluator", "The operator that evaluates a move."));
      Parameters.Add(new ValueLookupParameter<IOperator>("TabuChecker", "The operator that checks whether a move is tabu."));
      Parameters.Add(new ValueLookupParameter<IOperator>("TabuMaker", "The operator that declares a move tabu."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze the solution and moves."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedMoves", "The number of evaluated moves."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      SubScopesProcessor subScopesProcessor0 = new SubScopesProcessor();
      Assigner bestQualityInitializer = new Assigner();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      SubScopesProcessor solutionProcessor = new SubScopesProcessor();
      Placeholder moveGenerator = new Placeholder();
      UniformSubScopesProcessor moveEvaluationProcessor = new UniformSubScopesProcessor();
      Placeholder moveEvaluator = new Placeholder();
      Placeholder tabuChecker = new Placeholder();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      SubScopesSorter moveQualitySorter = new SubScopesSorter();
      TabuSelector tabuSelector = new TabuSelector();
      ConditionalBranch emptyNeighborhoodBranch1 = new ConditionalBranch();
      SubScopesProcessor moveMakingProcessor = new SubScopesProcessor();
      UniformSubScopesProcessor selectedMoveMakingProcesor = new UniformSubScopesProcessor();
      Placeholder tabuMaker = new Placeholder();
      Placeholder moveMaker = new Placeholder();
      MergingReducer mergingReducer = new MergingReducer();
      Placeholder analyzer2 = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      ConditionalBranch emptyNeighborhoodBranch2 = new ConditionalBranch();
      BestQualityMemorizer bestQualityUpdater = new BestQualityMemorizer();
      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      ConditionalBranch iterationsTermination = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0))); // Class TabuSearch expects this to be called Iterations
      variableCreator.CollectedValues.Add(new ValueParameter<BoolValue>("EmptyNeighborhood", new BoolValue(false)));
      variableCreator.CollectedValues.Add(new ValueParameter<ItemList<IItem>>("TabuList", new ItemList<IItem>()));
      variableCreator.CollectedValues.Add(new ValueParameter<VariableCollection>("Memories", new VariableCollection()));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(0)));

      bestQualityInitializer.Name = "Initialize BestQuality";
      bestQualityInitializer.LeftSideParameter.ActualName = "BestQuality";
      bestQualityInitializer.RightSideParameter.ActualName = QualityParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CopyValue = new BoolValue(false);
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Iterations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      moveGenerator.Name = "MoveGenerator (placeholder)";
      moveGenerator.OperatorParameter.ActualName = MoveGeneratorParameter.Name;

      moveEvaluationProcessor.Parallel = new BoolValue(true);

      moveEvaluator.Name = "MoveEvaluator (placeholder)";
      moveEvaluator.OperatorParameter.ActualName = MoveEvaluatorParameter.Name;

      tabuChecker.Name = "TabuChecker (placeholder)";
      tabuChecker.OperatorParameter.ActualName = TabuCheckerParameter.Name;

      subScopesCounter.Name = "Increment EvaluatedMoves";
      subScopesCounter.ValueParameter.ActualName = EvaluatedMovesParameter.Name;

      moveQualitySorter.DescendingParameter.ActualName = MaximizationParameter.Name;
      moveQualitySorter.ValueParameter.ActualName = MoveQualityParameter.Name;

      tabuSelector.AspirationParameter.Value = new BoolValue(true);
      tabuSelector.BestQualityParameter.ActualName = "BestQuality";
      tabuSelector.CopySelected = new BoolValue(false);
      tabuSelector.EmptyNeighborhoodParameter.ActualName = "EmptyNeighborhood";
      tabuSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      tabuSelector.MoveQualityParameter.ActualName = MoveQualityParameter.Name;
      tabuSelector.MoveTabuParameter.ActualName = MoveTabuParameter.Name;

      moveMakingProcessor.Name = "MoveMaking processor (UniformSubScopesProcessor)";

      emptyNeighborhoodBranch1.Name = "Neighborhood empty?";
      emptyNeighborhoodBranch1.ConditionParameter.ActualName = "EmptyNeighborhood";

      tabuMaker.Name = "TabuMaker (placeholder)";
      tabuMaker.OperatorParameter.ActualName = TabuMakerParameter.Name;

      moveMaker.Name = "MoveMaker (placeholder)";
      moveMaker.OperatorParameter.ActualName = MoveMakerParameter.Name;

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      bestQualityUpdater.Name = "Update BestQuality";
      bestQualityUpdater.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityUpdater.QualityParameter.ActualName = QualityParameter.Name;
      bestQualityUpdater.BestQualityParameter.ActualName = "BestQuality";

      iterationsCounter.Name = "Iterations Counter";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = "Iterations";

      iterationsComparator.Name = "Iterations >= MaximumIterations";
      iterationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      iterationsComparator.LeftSideParameter.ActualName = "Iterations";
      iterationsComparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      iterationsComparator.ResultParameter.ActualName = "Terminate";

      emptyNeighborhoodBranch2.Name = "Neighborhood empty?";
      emptyNeighborhoodBranch2.ConditionParameter.ActualName = "EmptyNeighborhood";

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
      resultsCollector1.Successor = solutionProcessor;
      solutionProcessor.Operators.Add(moveGenerator);
      solutionProcessor.Successor = iterationsCounter;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operator = moveEvaluator;
      moveEvaluationProcessor.Successor = subScopesCounter;
      moveEvaluator.Successor = tabuChecker;
      tabuChecker.Successor = null;
      subScopesCounter.Successor = moveQualitySorter;
      moveQualitySorter.Successor = tabuSelector;
      tabuSelector.Successor = emptyNeighborhoodBranch1;
      emptyNeighborhoodBranch1.FalseBranch = moveMakingProcessor;
      emptyNeighborhoodBranch1.TrueBranch = null;
      emptyNeighborhoodBranch1.Successor = subScopesRemover;
      moveMakingProcessor.Operators.Add(new EmptyOperator());
      moveMakingProcessor.Operators.Add(selectedMoveMakingProcesor);
      moveMakingProcessor.Successor = mergingReducer;
      selectedMoveMakingProcesor.Operator = tabuMaker;
      selectedMoveMakingProcesor.Successor = null;
      tabuMaker.Successor = moveMaker;
      moveMaker.Successor = null;
      mergingReducer.Successor = analyzer2;
      analyzer2.Successor = null;
      subScopesRemover.Successor = null;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = emptyNeighborhoodBranch2;
      emptyNeighborhoodBranch2.TrueBranch = null;
      emptyNeighborhoodBranch2.FalseBranch = iterationsTermination;
      emptyNeighborhoodBranch2.Successor = null;
      iterationsTermination.TrueBranch = null;
      iterationsTermination.FalseBranch = solutionProcessor;
      #endregion
    }

    public override IOperation Apply() {
      if (MoveGeneratorParameter.ActualValue == null || MoveEvaluatorParameter.ActualValue == null || MoveMakerParameter.ActualValue == null
        || TabuCheckerParameter.ActualValue == null || TabuMakerParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
