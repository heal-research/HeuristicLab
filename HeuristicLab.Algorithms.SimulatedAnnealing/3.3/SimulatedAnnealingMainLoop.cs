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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.SimulatedAnnealing {
  /// <summary>
  /// An operator which represents a simulated annealing.
  /// </summary>
  [Item("SimulatedAnnealingMainLoop", "An operator which represents the main loop of a simulated annealing algorithm.")]
  [StorableType("ADDF6A58-21CD-4224-94BF-FE6AE5CBEE3A")]
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
    public ILookupParameter<DoubleValue> TemperatureParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Temperature"]; }
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
    public LookupParameter<IntValue> IterationsParameter {
      get { return (LookupParameter<IntValue>)Parameters["Iterations"]; }
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
    private SimulatedAnnealingMainLoop(StorableConstructorFlag _) : base(_) { }
    private SimulatedAnnealingMainLoop(SimulatedAnnealingMainLoop original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimulatedAnnealingMainLoop(this, cloner);
    }
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
      Parameters.Add(new LookupParameter<DoubleValue>("Temperature", "The current temperature."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("StartTemperature", "The initial temperature."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("EndTemperature", "The end temperature."));
      Parameters.Add(new ValueLookupParameter<IntValue>("InnerIterations", "The amount of inner iterations (number of moves before temperature is adjusted again)."));
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "The number of iterations."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of iterations which should be processed."));

      Parameters.Add(new ValueLookupParameter<IOperator>("MoveGenerator", "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveEvaluator", "The operator that evaluates a move."));
      Parameters.Add(new ValueLookupParameter<IOperator>("MoveMaker", "The operator that performs a move and updates the quality."));
      Parameters.Add(new ValueLookupParameter<IOperator>("AnnealingOperator", "The operator that modifies the temperature."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze each generation."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedMoves", "The number of evaluated moves."));
      #endregion

      #region Create operators
      Assigner temperatureInitializer = new Assigner();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      SubScopesProcessor subScopesProcessor0 = new SubScopesProcessor();
      Placeholder analyzer1 = new Placeholder();
      SubScopesProcessor sssp = new SubScopesProcessor();
      ResultsCollector resultsCollector = new ResultsCollector();
      Placeholder annealingOperator = new Placeholder();
      UniformSubScopesProcessor mainProcessor = new UniformSubScopesProcessor();
      Placeholder moveGenerator = new Placeholder();
      UniformSubScopesProcessor moveEvaluationProcessor = new UniformSubScopesProcessor();
      Placeholder moveEvaluator = new Placeholder();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      ProbabilisticQualityComparator qualityComparator = new ProbabilisticQualityComparator();
      ConditionalBranch improvesQualityBranch = new ConditionalBranch();
      Placeholder moveMaker = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      Placeholder analyzer2 = new Placeholder();
      ConditionalBranch iterationsTermination = new ConditionalBranch();

      temperatureInitializer.LeftSideParameter.ActualName = TemperatureParameter.ActualName;
      temperatureInitializer.RightSideParameter.ActualName = StartTemperatureParameter.Name;

      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>(IterationsParameter.Name));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      annealingOperator.Name = "Annealing operator (placeholder)";
      annealingOperator.OperatorParameter.ActualName = AnnealingOperatorParameter.Name;

      moveGenerator.Name = "Move generator (placeholder)";
      moveGenerator.OperatorParameter.ActualName = MoveGeneratorParameter.Name;

      moveEvaluator.Name = "Move evaluator (placeholder)";
      moveEvaluator.OperatorParameter.ActualName = MoveEvaluatorParameter.Name;

      subScopesCounter.Name = "Increment EvaluatedMoves";
      subScopesCounter.ValueParameter.ActualName = EvaluatedMovesParameter.Name;

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
      iterationsCounter.ValueParameter.ActualName = IterationsParameter.Name;

      iterationsComparator.Name = "Iterations >= MaximumIterations";
      iterationsComparator.LeftSideParameter.ActualName = IterationsParameter.Name;
      iterationsComparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      iterationsComparator.ResultParameter.ActualName = "Terminate";
      iterationsComparator.Comparison.Value = ComparisonType.GreaterOrEqual;

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      iterationsTermination.Name = "Iterations termination condition";
      iterationsTermination.ConditionParameter.ActualName = "Terminate";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = temperatureInitializer;
      temperatureInitializer.Successor = resultsCollector1;
      resultsCollector1.Successor = subScopesProcessor0;
      subScopesProcessor0.Operators.Add(analyzer1);
      subScopesProcessor0.Successor = sssp;
      analyzer1.Successor = null;
      sssp.Operators.Add(resultsCollector);
      sssp.Successor = annealingOperator;
      resultsCollector.Successor = null;
      annealingOperator.Successor = mainProcessor;
      mainProcessor.Operator = moveGenerator;
      mainProcessor.Successor = iterationsCounter;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operator = moveEvaluator;
      moveEvaluationProcessor.Successor = subScopesCounter;
      moveEvaluator.Successor = qualityComparator;
      qualityComparator.Successor = improvesQualityBranch;
      improvesQualityBranch.TrueBranch = moveMaker;
      improvesQualityBranch.FalseBranch = null;
      improvesQualityBranch.Successor = null;
      moveMaker.Successor = null;
      subScopesCounter.Successor = subScopesRemover;
      subScopesRemover.Successor = null;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(analyzer2);
      subScopesProcessor1.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = null;
      iterationsTermination.FalseBranch = annealingOperator;
      #endregion
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (!Parameters.ContainsKey("Iterations"))
        Parameters.Add(new LookupParameter<IntValue>("Iterations", "The number of iterations."));
      if (!Parameters.ContainsKey("Temperature"))
        Parameters.Add(new LookupParameter<DoubleValue>("Temperature", "The current temperature."));
      #endregion
    }

    public override IOperation Apply() {
      if (MoveGeneratorParameter.ActualValue == null || MoveEvaluatorParameter.ActualValue == null || MoveMakerParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
