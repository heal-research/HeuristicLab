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

namespace HeuristicLab.Algorithms.SimulatedAnnealing {
  /// <summary>
  /// An operator which represents a simulated annealing.
  /// </summary>
  [Item("SimulatedAnnealingMainLoop", "An operator which represents the main loop of a simulated annealing algorithm.")]
  [StorableClass]
  public sealed class SimulatedAnnealingMainLoop : AlgorithmOperator {
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
    public ValueLookupParameter<DoubleValue> StartTemperatureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["StartTemperature"]; }
    }
    public ValueLookupParameter<DoubleValue> EndTemperatureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["EndTemperature"]; }
    }
    public ValueLookupParameter<IntValue> InnerIterationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["InnerIterations"]; }
    }
    public ValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
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
    public ValueLookupParameter<IOperator> AnnealingOperatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["AnnealingOperator"]; }
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
    private SimulatedAnnealingMainLoop(bool deserializing) : base() { }
    public SimulatedAnnealingMainLoop()
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
      Parameters.Add(new ValueLookupParameter<DoubleValue>("StartTemperature", "The initial temperature."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("EndTemperature", "The end temperature."));
      Parameters.Add(new ValueLookupParameter<IntValue>("InnerIterations", "The amount of inner iterations (number of moves before temperature is adjusted again)."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of iterations which should be processed."));
      
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveEvaluator", "The operator that evaluates a move."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveMaker", "The operator that performs a move and updates the quality."));
      Parameters.Add(new ValueLookupParameter<IOperator>("AnnealingOperator", "The operator that modifies the temperature."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Visualizer", "The operator used to visualize solutions."));
      Parameters.Add(new LookupParameter<IItem>("Visualization", "The item which represents the visualization of solutions."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      QualityDifferenceCalculator qualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      Placeholder visualizer1 = new Placeholder();
      SequentialSubScopesProcessor sssp = new SequentialSubScopesProcessor();
      ResultsCollector resultsCollector = new ResultsCollector();
      Placeholder annealingOperator = new Placeholder();
      UniformSequentialSubScopesProcessor mainProcessor = new UniformSequentialSubScopesProcessor();
      DataTableValuesCollector valuesCollector = new DataTableValuesCollector();
      Placeholder moveGenerator = new Placeholder();
      UniformSequentialSubScopesProcessor moveEvaluationProcessor = new UniformSequentialSubScopesProcessor();
      Placeholder moveEvaluator = new Placeholder();
      ProbabilisticQualityComparator qualityComparator = new ProbabilisticQualityComparator();
      ConditionalBranch improvesQualityBranch = new ConditionalBranch();
      Placeholder moveMaker = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      BestQualityMemorizer bestQualityMemorizer3 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer4 = new BestQualityMemorizer();
      QualityDifferenceCalculator qualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      Placeholder visualizer2 = new Placeholder();
      ConditionalBranch iterationsTermination = new ConditionalBranch();
      
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DataTable>("Qualities", new DataTable("Qualities")));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("Temperature", new DoubleValue(double.MaxValue)));

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
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(QualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality") { ActualName = "BestQuality" });
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Temperature"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IItem>("Solution Visualization", null, VisualizationParameter.Name));
      
      annealingOperator.Name = "Annealing operator (placeholder)";
      annealingOperator.OperatorParameter.ActualName = AnnealingOperatorParameter.Name;

      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(QualityParameter.Name));
      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestQuality"));
      valuesCollector.DataTableParameter.ActualName = "Qualities";

      moveGenerator.Name = "Move generator (placeholder)";
      moveGenerator.OperatorParameter.ActualName = MoveGeneratorParameter.Name;

      moveEvaluator.Name = "Move evaluator (placeholder)";
      moveEvaluator.OperatorParameter.ActualName = MoveEvaluatorParameter.Name;

      qualityComparator.LeftSideParameter.ActualName = MoveQualityParameter.Name;
      qualityComparator.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparator.ResultParameter.ActualName = "IsBetter";
      qualityComparator.DampeningParameter.ActualName = "Temperature";

      improvesQualityBranch.ConditionParameter.ActualName = "IsBetter";

      moveMaker.Name = "Move maker (placeholder)";
      moveMaker.OperatorParameter.ActualName = MoveMakerParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      iterationsCounter.Name = "Increment Iterations";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = "Iterations";

      iterationsComparator.Name = "Iterations >= MaximumIterations";
      iterationsComparator.LeftSideParameter.ActualName = "Iterations";
      iterationsComparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      iterationsComparator.ResultParameter.ActualName = "Terminate";
      iterationsComparator.Comparison.Value = ComparisonType.GreaterOrEqual;

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

      iterationsTermination.Name = "Iterations termination condition";
      iterationsTermination.ConditionParameter.ActualName = "Terminate";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = bestQualityMemorizer1;
      bestQualityMemorizer1.Successor = bestQualityMemorizer2;
      bestQualityMemorizer2.Successor = qualityDifferenceCalculator1;
      qualityDifferenceCalculator1.Successor = visualizer1;
      visualizer1.Successor = sssp;
      sssp.Operators.Add(resultsCollector);
      resultsCollector.Successor = null;
      sssp.Successor = annealingOperator;
      annealingOperator.Successor = mainProcessor;
      mainProcessor.Operator = valuesCollector;
      mainProcessor.Successor = iterationsCounter;
      valuesCollector.Successor = moveGenerator;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operator = moveEvaluator;
      moveEvaluationProcessor.Successor = subScopesRemover;
      moveEvaluator.Successor = qualityComparator;
      qualityComparator.Successor = improvesQualityBranch;
      improvesQualityBranch.TrueBranch = moveMaker;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = bestQualityMemorizer3;
      bestQualityMemorizer3.Successor = bestQualityMemorizer4;
      bestQualityMemorizer4.Successor = qualityDifferenceCalculator2;
      qualityDifferenceCalculator2.Successor = visualizer2;
      visualizer2.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = null;
      iterationsTermination.FalseBranch = annealingOperator;
      #endregion
    }
  }
}
