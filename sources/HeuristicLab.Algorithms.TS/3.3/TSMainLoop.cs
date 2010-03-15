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

namespace HeuristicLab.Algorithms.TS {
  /// <summary>
  /// An operator which represents a tabu search.
  /// </summary>
  [Item("TSMainLoop", "An operator which represents the main loop of a tabu search.")]
  [StorableClass]
  public class TSMainLoop : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public SubScopesLookupParameter<DoubleValue> QualityParameter {
      get { return (SubScopesLookupParameter<DoubleValue>)Parameters["Quality"]; }
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
    public ValueLookupParameter<IOperator> MoveQualityEvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["MoveQualityEvaluator"]; }
    }
    public ValueLookupParameter<IOperator> MoveTabuEvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["MoveTabuEvaluator"]; }
    }
    public ValueLookupParameter<IOperator> TabuSelectorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["TabuSelector"]; }
    }
    public ValueLookupParameter<IOperator> MoveTabuMakerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["MoveTabuMaker"]; }
    }
    public ValueLookupParameter<IOperator> MoveMakerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["MoveMaker"]; }
    }

    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    public TSMainLoop()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("TabuTenure", "The length of the tabu list, and also means the number of iterations a move is kept tabu"));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveQualityEvaluator", "The operator that evaluates the quality of a move."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveTabuEvaluator", "The operator that evaluates whether a move is tabu."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveTabuMaker", "The operator that declares a move tabu."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveMaker", "The operator that performs a move and updates the quality."));

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
      Placeholder moveQualityEvaluator = new Placeholder();
      Placeholder moveTabuEvaluator = new Placeholder();
      SubScopesSorter moveQualitySorter = new SubScopesSorter();
      BestAverageWorstQualityCalculator bestAverageWorstMoveQualityCalculator = new BestAverageWorstQualityCalculator();
      TabuSelector tabuSelector = new TabuSelector();
      RightReducer rightReducer = new RightReducer();
      UniformSequentialSubScopesProcessor moveMakingProcessor = new UniformSequentialSubScopesProcessor();
      Placeholder moveTabuMaker = new Placeholder();
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

      moveQualityEvaluator.Name = "MoveQualityEvaluator (placeholder)";
      moveQualityEvaluator.OperatorParameter.ActualName = "MoveQualityEvaluator";

      moveTabuEvaluator.Name = "MoveTabuEvaluator (placeholder)";
      moveTabuEvaluator.OperatorParameter.ActualName = "MoveTabuEvaluator";

      moveQualitySorter.DescendingParameter.ActualName = "Maximization";
      moveQualitySorter.ValueParameter.ActualName = "MoveQuality";

      bestAverageWorstMoveQualityCalculator.AverageQualityParameter.ActualName = "Average Move Quality";
      bestAverageWorstMoveQualityCalculator.BestQualityParameter.ActualName = "Best Move Quality";
      bestAverageWorstMoveQualityCalculator.MaximizationParameter.ActualName = "Maximization";
      bestAverageWorstMoveQualityCalculator.QualityParameter.ActualName = "MoveQuality";
      bestAverageWorstMoveQualityCalculator.WorstQualityParameter.ActualName = "Worst Move Quality";

      tabuSelector.NumberOfSelectedSubScopes = new IntValue(1);

      moveMakingProcessor.Name = "MoveMaking processor (UniformSequentialSubScopesProcessor)";

      moveTabuMaker.Name = "MoveTabuMaker (placeholder)";
      moveTabuMaker.OperatorParameter.ActualName = "MoveTabuMaker";

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
      moveEvaluationProcessor.Operator = moveQualityEvaluator;
      moveEvaluationProcessor.Successor = moveQualitySorter;
      moveQualityEvaluator.Successor = moveTabuEvaluator;
      moveQualitySorter.Successor = bestAverageWorstMoveQualityCalculator;
      bestAverageWorstMoveQualityCalculator.Successor = tabuSelector;
      tabuSelector.Successor = rightReducer;
      rightReducer.Successor = moveMakingProcessor;
      moveMakingProcessor.Operator = moveTabuMaker;
      moveMakingProcessor.Successor = subScopesRemover;
      moveTabuMaker.Successor = moveMaker;
      valuesCollector.Successor = iterationsCounter;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = bestQualityMemorizer;
      iterationsTermination.FalseBranch = finished;
      #endregion
    }
  }
}
