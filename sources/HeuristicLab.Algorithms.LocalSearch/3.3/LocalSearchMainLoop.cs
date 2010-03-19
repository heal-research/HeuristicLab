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
  public class LocalSearchMainLoop : AlgorithmOperator {
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
    public ValueLookupParameter<IOperator> VisualizerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Visualizer"]; }
    }
    public LookupParameter<IItem> VisualizationParameter {
      get { return (LookupParameter<IItem>)Parameters["Visualization"]; }
    }

    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    public LocalSearchMainLoop()
      : base() {
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

      Parameters.Add(new ValueLookupParameter<IOperator>("Visualizer", "The operator used to visualize solutions."));
      Parameters.Add(new LookupParameter<IItem>("Visualization", "The item which represents the visualization of solutions."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the TS should be applied."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      DataTableValuesCollector dataTableValuesCollector1 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      Placeholder visualizer1 = new Placeholder();
      ResultsCollector resultsCollector = new ResultsCollector();
      UniformSequentialSubScopesProcessor mainProcessor = new UniformSequentialSubScopesProcessor();
      Placeholder moveGenerator = new Placeholder();
      UniformSequentialSubScopesProcessor moveEvaluationProcessor = new UniformSequentialSubScopesProcessor();
      Placeholder moveEvaluator = new Placeholder();
      BestSelector bestSelector = new BestSelector();
      RightReducer rightReducer = new RightReducer();
      UniformSequentialSubScopesProcessor moveMakingProcessor = new UniformSequentialSubScopesProcessor();
      QualityComparator qualityComparator = new QualityComparator();
      ConditionalBranch improvesQualityBranch = new ConditionalBranch();
      Placeholder moveMaker = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      BestQualityMemorizer bestQualityMemorizer3 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer4 = new BestQualityMemorizer();
      DataTableValuesCollector dataTableValuesCollector2 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      Placeholder visualizer2 = new Placeholder();
      ConditionalBranch iterationsTermination = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));

      bestQualityMemorizer1.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestQualityMemorizer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer1.QualityParameter.ActualName = QualityParameter.Name;

      bestQualityMemorizer2.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer2.QualityParameter.ActualName = QualityParameter.Name;

      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("BestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector1.DataTableParameter.ActualName = "Qualities";

      qualityDifferenceCalculator1.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator1.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.SecondQualityParameter.ActualName = "BestQuality";

      visualizer1.Name = "Visualizer (placeholder)";
      visualizer1.OperatorParameter.ActualName = VisualizerParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Iterations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IItem>("Solution Visualization", null, VisualizationParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;

      mainProcessor.Name = "Solution processor (UniformSequentialSubScopesProcessor)";

      moveGenerator.Name = "MoveGenerator (placeholder)";
      moveGenerator.OperatorParameter.ActualName = "MoveGenerator";

      moveEvaluator.Name = "MoveEvaluator (placeholder)";
      moveEvaluator.OperatorParameter.ActualName = "MoveEvaluator";

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = "Maximization";
      bestSelector.NumberOfSelectedSubScopesParameter.Value = new IntValue(1);
      bestSelector.QualityParameter.ActualName = "MoveQuality";

      moveMakingProcessor.Name = "MoveMaking processor (UniformSequentialSubScopesProcessor)";

      qualityComparator.LeftSideParameter.ActualName = "MoveQuality";
      qualityComparator.RightSideParameter.ActualName = "Quality";
      qualityComparator.ResultParameter.ActualName = "IsBetter";

      improvesQualityBranch.ConditionParameter.ActualName = "IsBetter";

      moveMaker.Name = "MoveMaker (placeholder)";
      moveMaker.OperatorParameter.ActualName = "MoveMaker";

      subScopesRemover.RemoveAllSubScopes = true;

      iterationsCounter.Name = "Iterations Counter";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = "Iterations";

      iterationsComparator.Name = "Iterations Comparator";
      iterationsComparator.Comparison = new Comparison(ComparisonType.Less);
      iterationsComparator.LeftSideParameter.ActualName = "Iterations";
      iterationsComparator.RightSideParameter.ActualName = "MaximumIterations";
      iterationsComparator.ResultParameter.ActualName = "IterationsCondition";

      bestQualityMemorizer3.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestQualityMemorizer3.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer3.QualityParameter.ActualName = QualityParameter.Name;

      bestQualityMemorizer4.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer4.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer4.QualityParameter.ActualName = QualityParameter.Name;

      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("BestQuality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector2.DataTableParameter.ActualName = "Qualities";

      qualityDifferenceCalculator2.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator2.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.SecondQualityParameter.ActualName = "BestQuality";

      visualizer2.Name = "Visualizer (placeholder)";
      visualizer2.OperatorParameter.ActualName = VisualizerParameter.Name;

      iterationsTermination.Name = "Iterations Termination Condition";
      iterationsTermination.ConditionParameter.ActualName = "IterationsCondition";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = bestQualityMemorizer1;
      bestQualityMemorizer1.Successor = bestQualityMemorizer2;
      bestQualityMemorizer2.Successor = dataTableValuesCollector1;
      dataTableValuesCollector1.Successor = qualityDifferenceCalculator1;
      qualityDifferenceCalculator1.Successor = visualizer1;
      visualizer1.Successor = resultsCollector;
      resultsCollector.Successor = mainProcessor;
      mainProcessor.Operator = moveGenerator;
      mainProcessor.Successor = iterationsCounter;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operator = moveEvaluator;
      moveEvaluationProcessor.Successor = bestSelector;
      bestSelector.Successor = rightReducer;
      rightReducer.Successor = moveMakingProcessor;
      moveMakingProcessor.Operator = qualityComparator;
      moveMakingProcessor.Successor = subScopesRemover;
      subScopesRemover.Successor = null;
      qualityComparator.Successor = improvesQualityBranch;
      improvesQualityBranch.TrueBranch = moveMaker;
      improvesQualityBranch.FalseBranch = null;
      improvesQualityBranch.Successor = null;
      moveMaker.Successor = null;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = bestQualityMemorizer3;
      bestQualityMemorizer3.Successor = bestQualityMemorizer4;
      bestQualityMemorizer4.Successor = dataTableValuesCollector2;
      dataTableValuesCollector2.Successor = qualityDifferenceCalculator2;
      qualityDifferenceCalculator2.Successor = visualizer2;
      visualizer2.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = mainProcessor;
      iterationsTermination.FalseBranch = null;
      #endregion
    }
  }
}
