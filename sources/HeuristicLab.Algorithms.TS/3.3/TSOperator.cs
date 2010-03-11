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
  /// An operator which represents a Tabu Search.
  /// </summary>
  [Item("TSOperator", "An operator which represents a Tabu Search.")]
  [StorableClass(StorableClassType.Empty)]
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
    public ValueLookupParameter<IntData> TabuTenureParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["TabuTenure"]; }
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

    public TSOperator()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolData>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleData>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<IntData>("MaximumIterations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<IntData>("TabuTenure", "The length of the tabu list, and also means the number of iterations a move is kept tabu"));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveQualityEvaluator", "The operator that evaluates the quality of a move."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveTabuEvaluator", "The operator that evaluates whether a move is tabu."));
      TabuSelector tabuSelectorOp = new TabuSelector();
      tabuSelectorOp.NumberOfSelectedSubScopes = new IntData(1);
      Parameters.Add(new ValueLookupParameter<IOperator>("TabuSelector", "The operator that selects among the moves the next best.", tabuSelectorOp));
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
      Placeholder tabuSelector = new Placeholder();
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

      variableCreator.CollectedValues.Add(new ValueParameter<IntData>("Iterations", new IntData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Best Move Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Average Move Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Worst Move Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DataTable>("MoveQualities", new DataTable("MoveQualities")));

      resultsCollector.CollectedValues.Add(new LookupParameter<IntData>("Iterations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Best Quality") { ActualName = "BestQuality" });
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Best Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Average Move Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Worst Move Quality"));
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

      tabuSelector.Name = "TabuSelector (placeholder)";
      tabuSelector.OperatorParameter.ActualName = "TabuSelector";

      moveMakingProcessor.Name = "MoveMaking processor (UniformSequentialSubScopesProcessor)";

      moveTabuMaker.Name = "MoveTabuMaker (placeholder)";
      moveTabuMaker.OperatorParameter.ActualName = "MoveTabuMaker";

      moveMaker.Name = "MoveMaker (placeholder)";
      moveMaker.OperatorParameter.ActualName = "MoveMaker";

      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Best Move Quality"));
      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Average Move Quality"));
      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Worst Move Quality"));
      valuesCollector.DataTableParameter.ActualName = "MoveQualities";

      subScopesRemover.RemoveAllSubScopes = true;

      iterationsCounter.Name = "Iterations Counter";
      iterationsCounter.Increment = new IntData(1);
      iterationsCounter.ValueParameter.ActualName = "Iterations";

      iterationsComparator.Name = "Iterations Comparator";
      iterationsComparator.Comparison = new ComparisonData(Comparison.Less);
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
