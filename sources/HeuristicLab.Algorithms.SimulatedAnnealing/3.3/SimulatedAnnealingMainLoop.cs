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
    public LookupParameter<DoubleValue> MoveQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ValueLookupParameter<DoubleValue> StartTemperatureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["StartTemperature"]; }
    }
    public ValueLookupParameter<DoubleValue> EndTemperatureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["EndTemperature"]; }
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
    public ValueLookupParameter<IOperator> AnnealingOperatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["AnnealingOperator"]; }
    }

    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
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
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The value which represents the quality of a move."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("StartTemperature", "The initial temperature."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("EndTemperature", "The end temperature."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of iterations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveEvaluator", "The operator that evaluates a move."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveMaker", "The operator that performs a move and updates the quality."));
      Parameters.Add(new ValueLookupParameter<IOperator>("AnnealingOperator", "The operator that modifies the temperature."));

      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the TS should be applied."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      BestQualityMemorizer initializeBestQuality = new BestQualityMemorizer();
      SequentialSubScopesProcessor sssp = new SequentialSubScopesProcessor();
      ResultsCollector resultsCollector = new ResultsCollector();
      BestQualityMemorizer bestQualityMemorizer = new BestQualityMemorizer();
      Placeholder annealingOperator = new Placeholder();
      UniformSequentialSubScopesProcessor mainProcessor = new UniformSequentialSubScopesProcessor();
      Placeholder moveGenerator = new Placeholder();
      SequentialSubScopesProcessor moveEvaluationProcessor = new SequentialSubScopesProcessor();
      Placeholder moveEvaluator = new Placeholder();
      ProbabilisticQualityComparator qualityComparator = new ProbabilisticQualityComparator();
      ConditionalBranch improvesQualityBranch = new ConditionalBranch();
      Placeholder moveMaker = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      DataTableValuesCollector valuesCollector = new DataTableValuesCollector();
      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      ConditionalBranch iterationsTermination = new ConditionalBranch();
      EmptyOperator finished = new EmptyOperator();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DataTable>("Qualities", new DataTable("Qualities")));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("Temperature", new DoubleValue(double.MaxValue)));

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Iterations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality") { ActualName = "BestQuality" });
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Temperature"));

      annealingOperator.Name = "Annealing operator (placeholder)";
      annealingOperator.OperatorParameter.ActualName = "AnnealingOperator";

      moveGenerator.Name = "Move generator (placeholder)";
      moveGenerator.OperatorParameter.ActualName = "MoveGenerator";

      moveEvaluator.Name = "Move evaluator (placeholder)";
      moveEvaluator.OperatorParameter.ActualName = "MoveEvaluator";

      qualityComparator.LeftSideParameter.ActualName = "MoveQuality";
      qualityComparator.RightSideParameter.ActualName = "Quality";
      qualityComparator.ResultParameter.ActualName = "IsBetter";
      qualityComparator.DampeningParameter.ActualName = "Temperature";

      improvesQualityBranch.ConditionParameter.ActualName = "IsBetter";

      moveMaker.Name = "Move maker (placeholder)";
      moveMaker.OperatorParameter.ActualName = "MoveMaker";

      subScopesRemover.RemoveAllSubScopes = true;

      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Quality"));
      valuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestQuality"));
      valuesCollector.DataTableParameter.ActualName = "Qualities";

      iterationsCounter.Name = "Increment Iterations";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = "Iterations";

      iterationsComparator.Name = "Iterations >= MaximumIterations";
      iterationsComparator.LeftSideParameter.ActualName = "Iterations";
      iterationsComparator.RightSideParameter.ActualName = "MaximumIterations";
      iterationsComparator.ResultParameter.ActualName = "IterationsCondition";
      iterationsComparator.Comparison.Value = ComparisonType.GreaterOrEqual;

      iterationsTermination.Name = "Iterations termination condition";
      iterationsTermination.ConditionParameter.ActualName = "IterationsCondition";

      finished.Name = "Finished";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = initializeBestQuality;
      initializeBestQuality.Successor = sssp;
      sssp.Operators.Add(resultsCollector);
      sssp.Successor = bestQualityMemorizer;
      bestQualityMemorizer.Successor = annealingOperator;
      annealingOperator.Successor = mainProcessor;
      mainProcessor.Operator = moveGenerator;
      mainProcessor.Successor = valuesCollector;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operators.Add(moveEvaluator);
      moveEvaluationProcessor.Successor = subScopesRemover;
      moveEvaluator.Successor = qualityComparator;
      qualityComparator.Successor = improvesQualityBranch;
      improvesQualityBranch.TrueBranch = moveMaker;
      valuesCollector.Successor = iterationsCounter;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = finished;
      iterationsTermination.FalseBranch = bestQualityMemorizer;
      #endregion
    }
  }
}
