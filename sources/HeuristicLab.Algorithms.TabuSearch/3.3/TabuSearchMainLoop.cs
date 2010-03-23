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
    public ValueLookupParameter<IOperator> TabuMoveEvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["TabuMoveEvaluator"]; }
    }
    public ValueLookupParameter<IOperator> TabuMoveMakerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["TabuMoveMaker"]; }
    }
    public ValueLookupParameter<IOperator> VisualizerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Visualizer"]; }
    }
    public LookupParameter<IItem> VisualizationParameter {
      get { return (LookupParameter<IItem>)Parameters["Visualization"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    #endregion

    [StorableConstructor]
    private TabuSearchMainLoop(bool deserializing) : base() { }
    public TabuSearchMainLoop()
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
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The value that indicates if a move is tabu or not."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("TabuTenure", "The length of the tabu list, and also means the number of iterations a move is kept tabu"));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveMaker", "The operator that performs a move and updates the quality."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveEvaluator", "The operator that evaluates a move."));
      Parameters.Add(new ValueLookupParameter<IOperator>("TabuMoveEvaluator", "The operator that evaluates whether a move is tabu."));
      Parameters.Add(new ValueLookupParameter<IOperator>("TabuMoveMaker", "The operator that declares a move tabu."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Visualizer", "The operator used to visualize solutions."));
      Parameters.Add(new LookupParameter<IItem>("Visualization", "The item which represents the visualization of solutions."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      #endregion

      #region Create operators
      TabuListCreator tabuListCreator = new TabuListCreator();
      VariableCreator variableCreator = new VariableCreator();
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      QualityDifferenceCalculator qualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      Placeholder visualizer1 = new Placeholder();
      ResultsCollector resultsCollector = new ResultsCollector();
      UniformSubScopesProcessor mainProcessor = new UniformSubScopesProcessor();
      Placeholder moveGenerator = new Placeholder();
      UniformSubScopesProcessor moveEvaluationProcessor = new UniformSubScopesProcessor();
      Placeholder moveEvaluator = new Placeholder();
      Placeholder tabuMoveEvaluator = new Placeholder();
      SubScopesSorter moveQualitySorter = new SubScopesSorter();
      BestAverageWorstQualityCalculator bestAverageWorstMoveQualityCalculator = new BestAverageWorstQualityCalculator();
      TabuSelector tabuSelector = new TabuSelector();
      ConditionalBranch emptyNeighborhoodBranch1 = new ConditionalBranch();
      RightReducer rightReducer = new RightReducer();
      UniformSubScopesProcessor moveMakingProcessor = new UniformSubScopesProcessor();
      Placeholder tabuMoveMaker = new Placeholder();
      Placeholder moveMaker = new Placeholder();
      DataTableValuesCollector valuesCollector = new DataTableValuesCollector();
      SubScopesRemover subScopesRemover1 = new SubScopesRemover();
      ConditionalBranch emptyNeighborhoodBranch2 = new ConditionalBranch();
      UniformSubScopesProcessor removeMoves = new UniformSubScopesProcessor();
      SubScopesRemover subScopesRemover2 = new SubScopesRemover();
      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      BestQualityMemorizer bestQualityMemorizer3 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer4 = new BestQualityMemorizer();
      QualityDifferenceCalculator qualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      Placeholder visualizer2 = new Placeholder();
      ConditionalBranch iterationsTermination = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("Best Move Quality", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("Average Move Quality", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("Worst Move Quality", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DataTable>("MoveQualities", new DataTable("MoveQualities")));
      variableCreator.CollectedValues.Add(new ValueParameter<BoolValue>("EmptyNeighborhood", new BoolValue(false)));

      bestQualityMemorizer1.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer1.QualityParameter.ActualName = QualityParameter.Name;

      bestQualityMemorizer2.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestQualityMemorizer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer2.QualityParameter.ActualName = QualityParameter.Name;

      qualityDifferenceCalculator1.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator1.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.SecondQualityParameter.ActualName = "BestQuality";

      visualizer1.Name = "Visualizer (placeholder)";
      visualizer1.OperatorParameter.ActualName = VisualizerParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Iterations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Average Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Worst Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("MoveQualities"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IItem>("Solution Visualization", null, VisualizationParameter.Name));
      resultsCollector.ResultsParameter.ActualName = "Results";

      mainProcessor.Name = "Solution processor (UniformSubScopesProcessor)";

      moveGenerator.Name = "MoveGenerator (placeholder)";
      moveGenerator.OperatorParameter.ActualName = MoveGeneratorParameter.Name;

      moveEvaluator.Name = "MoveEvaluator (placeholder)";
      moveEvaluator.OperatorParameter.ActualName = MoveEvaluatorParameter.Name;

      tabuMoveEvaluator.Name = "TabuMoveEvaluator (placeholder)";
      tabuMoveEvaluator.OperatorParameter.ActualName = TabuMoveEvaluatorParameter.Name;

      moveQualitySorter.DescendingParameter.ActualName = MaximizationParameter.Name;
      moveQualitySorter.ValueParameter.ActualName = MoveQualityParameter.Name;

      bestAverageWorstMoveQualityCalculator.AverageQualityParameter.ActualName = "Average Move Quality";
      bestAverageWorstMoveQualityCalculator.BestQualityParameter.ActualName = "Best Move Quality";
      bestAverageWorstMoveQualityCalculator.MaximizationParameter.ActualName = "Maximization";
      bestAverageWorstMoveQualityCalculator.QualityParameter.ActualName = MoveQualityParameter.Name;
      bestAverageWorstMoveQualityCalculator.WorstQualityParameter.ActualName = "Worst Move Quality";

      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Move Quality"));
      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Average Move Quality"));
      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Worst Move Quality"));
      valuesCollector.DataTableParameter.ActualName = "MoveQualities";

      moveMakingProcessor.Name = "MoveMaking processor (UniformSubScopesProcessor)";

      emptyNeighborhoodBranch1.Name = "Neighborhood empty?";
      emptyNeighborhoodBranch1.ConditionParameter.ActualName = "EmptyNeighborhood";

      tabuMoveMaker.Name = "TabuMoveMaker (placeholder)";
      tabuMoveMaker.OperatorParameter.ActualName = TabuMoveMakerParameter.Name;

      moveMaker.Name = "MoveMaker (placeholder)";
      moveMaker.OperatorParameter.ActualName = MoveMakerParameter.Name;

      subScopesRemover1.RemoveAllSubScopes = true;

      emptyNeighborhoodBranch2.Name = "Neighborhood empty?";
      emptyNeighborhoodBranch2.ConditionParameter.ActualName = "EmptyNeighborhood";

      subScopesRemover2.RemoveAllSubScopes = true;

      iterationsCounter.Name = "Iterations Counter";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = "Iterations";

      iterationsComparator.Name = "Iterations >= MaximumIterations";
      iterationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      iterationsComparator.LeftSideParameter.ActualName = "Iterations";
      iterationsComparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      iterationsComparator.ResultParameter.ActualName = "Terminate";

      bestQualityMemorizer3.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer3.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer3.QualityParameter.ActualName = QualityParameter.Name;

      bestQualityMemorizer4.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestQualityMemorizer4.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer4.QualityParameter.ActualName = QualityParameter.Name;

      qualityDifferenceCalculator2.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator2.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.SecondQualityParameter.ActualName = "BestQuality";

      visualizer2.Name = "Visualizer (placeholder)";
      visualizer2.OperatorParameter.ActualName = VisualizerParameter.Name;

      iterationsTermination.Name = "Iterations Termination Condition";
      iterationsTermination.ConditionParameter.ActualName = "Terminate";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = tabuListCreator;
      tabuListCreator.Successor = variableCreator;
      variableCreator.Successor = bestQualityMemorizer1;
      bestQualityMemorizer1.Successor = bestQualityMemorizer2;
      bestQualityMemorizer2.Successor = qualityDifferenceCalculator1;
      qualityDifferenceCalculator1.Successor = visualizer1;
      visualizer1.Successor = resultsCollector;
      resultsCollector.Successor = mainProcessor;
      mainProcessor.Operator = moveGenerator;
      mainProcessor.Successor = emptyNeighborhoodBranch2;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operator = moveEvaluator;
      moveEvaluationProcessor.Successor = moveQualitySorter;
      moveEvaluator.Successor = tabuMoveEvaluator;
      tabuMoveEvaluator.Successor = null;
      moveQualitySorter.Successor = bestAverageWorstMoveQualityCalculator;
      bestAverageWorstMoveQualityCalculator.Successor = valuesCollector;
      valuesCollector.Successor = tabuSelector;
      tabuSelector.Successor = emptyNeighborhoodBranch1;
      emptyNeighborhoodBranch1.FalseBranch = rightReducer;
      emptyNeighborhoodBranch1.TrueBranch = null;
      emptyNeighborhoodBranch1.Successor = null;
      rightReducer.Successor = moveMakingProcessor;
      moveMakingProcessor.Operator = tabuMoveMaker;
      moveMakingProcessor.Successor = subScopesRemover1;
      tabuMoveMaker.Successor = moveMaker;
      moveMaker.Successor = null;
      subScopesRemover1.Successor = null;
      emptyNeighborhoodBranch2.FalseBranch = iterationsCounter;
      emptyNeighborhoodBranch2.TrueBranch = removeMoves;
      emptyNeighborhoodBranch2.Successor = null;
      removeMoves.Operator = subScopesRemover2;
      removeMoves.Successor = null;
      subScopesRemover2.Successor = null;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = bestQualityMemorizer3;
      bestQualityMemorizer3.Successor = bestQualityMemorizer4;
      bestQualityMemorizer4.Successor = qualityDifferenceCalculator2;
      qualityDifferenceCalculator2.Successor = visualizer2;
      visualizer2.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = null;
      iterationsTermination.FalseBranch = mainProcessor;
      #endregion
    }
  }
}
