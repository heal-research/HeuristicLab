#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.SimulatedAnnealing {
  [Item("SimulatedAnnealingMainLoop", "An operator which represents the main loop of a simulated annealing algorithm.")]
  [StorableClass]
  public sealed class SimulatedAnnealingMainLoop : AlgorithmOperator {
    #region Strings
    private const string MoveGeneratorName = "MoveGenerator";
    private const string MoveEvaluatorName = "MoveEvaluator";
    private const string MoveMakerName = "MoveMaker";
    private const string AnnealingOperatorName = "AnnealingOperator";
    private const string HeatingOperatorName = "HeatingOperator";
    private const string MaximumIterationsName = "MaximumIterations";
    private const string UpperTemperatureName = "UpperTemperature";
    private const string LowerTemperatureName = "LowerTemperature";
    private const string AnalyzerName = "Analyzer";
    private const string RandomName = "Random";
    private const string EvaluatedMovesName = "EvaluatedMoves";
    private const string IterationsName = "Iterations";
    private const string TemperatureStartIndexName = "TemperatureStartIndex";
    private const string CoolingName = "Cooling";
    private const string StartTemperatureName = "StartTemperature";
    private const string EndTemperatureName = "EndTemperature";
    private const string TemperatureName = "Temperature";
    private const string ResultsName = "Results";
    private const string MaximizationName = "Maximization";
    private const string QualityName = "Quality";
    private const string BestKnownQualityName = "BestKnownQuality";
    private const string MoveQualityName = "MoveQuality";
    private const string IsAcceptedName = "IsAccepted";
    private const string TerminateName = "Terminate";
    private const string ThresholdName = "Threshold";
    private const string MemorySizeName = "MemorySize";
    private const string AcceptanceMemoryName = "AcceptanceMemory";
    #endregion

    #region Parameter properties
    public IValueLookupParameter<IRandom> RandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters[RandomName]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[MaximizationName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityName]; }
    }
    public IValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[BestKnownQualityName]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[MoveQualityName]; }
    }
    public ILookupParameter<DoubleValue> TemperatureParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[TemperatureName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperTemperatureParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperTemperatureName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerTemperatureParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerTemperatureName]; }
    }
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[IterationsName]; }
    }
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumIterationsName]; }
    }
    public IValueLookupParameter<IOperator> MoveGeneratorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters[MoveGeneratorName]; }
    }
    public IValueLookupParameter<IOperator> MoveEvaluatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters[MoveEvaluatorName]; }
    }
    public IValueLookupParameter<IOperator> MoveMakerParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters[MoveMakerName]; }
    }
    public IValueLookupParameter<IOperator> AnnealingOperatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters[AnnealingOperatorName]; }
    }
    public IValueLookupParameter<IOperator> HeatingOperatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters[HeatingOperatorName]; }
    }
    public IValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters[AnalyzerName]; }
    }
    public IValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (IValueLookupParameter<VariableCollection>)Parameters[ResultsName]; }
    }
    public ILookupParameter<IntValue> EvaluatedMovesParameter {
      get { return (ILookupParameter<IntValue>)Parameters[EvaluatedMovesName]; }
    }
    public ILookupParameter<DoubleValue> StartTemperatureParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[StartTemperatureName]; }
    }
    public ILookupParameter<DoubleValue> EndTemperatureParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[EndTemperatureName]; }
    }
    public ILookupParameter<IntValue> TemperatureStartIndexParameter {
      get { return (ILookupParameter<IntValue>)Parameters[TemperatureStartIndexName]; }
    }
    public ILookupParameter<BoolValue> CoolingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[CoolingName]; }
    }
    public IValueLookupParameter<DoubleRange> ThresholdParameter {
      get { return (IValueLookupParameter<DoubleRange>)Parameters[ThresholdName]; }
    }
    public IValueLookupParameter<IntValue> MemorySizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MemorySizeName]; }
    }
    #endregion

    [StorableConstructor]
    private SimulatedAnnealingMainLoop(bool deserializing) : base(deserializing) { }
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
      Parameters.Add(new ValueLookupParameter<IRandom>(RandomName, "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>(MaximizationName, "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityName, "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(BestKnownQualityName, "The best known quality value found so far."));
      Parameters.Add(new LookupParameter<DoubleValue>(MoveQualityName, "The value which represents the quality of a move."));
      Parameters.Add(new LookupParameter<DoubleValue>(TemperatureName, "The current temperature."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperTemperatureName, "The upper bound of the temperature."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerTemperatureName, "The lower bound of the temperature."));
      Parameters.Add(new LookupParameter<IntValue>(IterationsName, "The number of iterations."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumIterationsName, "The maximum number of iterations which should be processed."));

      Parameters.Add(new ValueLookupParameter<IOperator>(MoveGeneratorName, "The operator that generates the moves."));
      Parameters.Add(new ValueLookupParameter<IOperator>(MoveEvaluatorName, "The operator that evaluates a move."));
      Parameters.Add(new ValueLookupParameter<IOperator>(MoveMakerName, "The operator that performs a move and updates the quality."));
      Parameters.Add(new ValueLookupParameter<IOperator>(AnnealingOperatorName, "The operator that cools the temperature."));
      Parameters.Add(new ValueLookupParameter<IOperator>(HeatingOperatorName, "The operator that heats the temperature."));

      Parameters.Add(new ValueLookupParameter<IOperator>(AnalyzerName, "The operator used to analyze each generation."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>(ResultsName, "The variable collection where results should be stored."));
      Parameters.Add(new LookupParameter<IntValue>(EvaluatedMovesName, "The number of evaluated moves."));

      Parameters.Add(new LookupParameter<IntValue>(TemperatureStartIndexName, "The index where the annealing or heating was last changed."));
      Parameters.Add(new LookupParameter<BoolValue>(CoolingName, "True when the temperature should be cooled, false otherwise."));
      Parameters.Add(new LookupParameter<DoubleValue>(StartTemperatureName, "The temperature from which cooling or reheating should occur."));
      Parameters.Add(new LookupParameter<DoubleValue>(EndTemperatureName, "The temperature to which should be cooled or heated."));
      Parameters.Add(new ValueLookupParameter<DoubleRange>(ThresholdName, "The threshold controls the temperature in case a heating operator is specified. If the average ratio of accepted moves goes below the start of the range the temperature is heated. If the the average ratio of accepted moves goes beyond the end of the range the temperature is cooled again."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MemorySizeName, "The maximum size of the acceptance memory."));
      #endregion

      #region Create operators
      var variableCreator = new VariableCreator();
      var ssp1 = new SubScopesProcessor();
      var analyzer1 = new Placeholder();
      var ssp2 = new SubScopesProcessor();
      var resultsCollector = new ResultsCollector();
      var mainProcessor = new SubScopesProcessor();
      var moveGenerator = new Placeholder();
      var moveEvaluationProcessor = new SubScopesProcessor();
      var moveEvaluator = new Placeholder();
      var evaluatedMovesCounter = new IntCounter();
      var qualityComparator = new ProbabilisticQualityComparator();
      var acceptsQualityBranch = new ConditionalBranch();
      var moveMaker = new Placeholder();
      var temperatureController = new TemperatureController();
      var subScopesRemover = new SubScopesRemover();
      var iterationsCounter = new IntCounter();
      var iterationsComparator = new Comparator();
      var ssp3 = new SubScopesProcessor();
      var analyzer2 = new Placeholder();
      var iterationsTermination = new ConditionalBranch();

      variableCreator.Name = "Initialize Memory";
      variableCreator.CollectedValues.Add(new ValueParameter<ItemList<BoolValue>>(AcceptanceMemoryName, new ItemList<BoolValue>()));

      analyzer1.Name = "Analyzer";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      moveGenerator.Name = "Move generator";
      moveGenerator.OperatorParameter.ActualName = MoveGeneratorParameter.Name;

      moveEvaluator.Name = "Move evaluator";
      moveEvaluator.OperatorParameter.ActualName = MoveEvaluatorParameter.Name;

      evaluatedMovesCounter.Name = "EvaluatedMoves++";
      evaluatedMovesCounter.ValueParameter.ActualName = EvaluatedMovesParameter.Name;
      evaluatedMovesCounter.Increment = new IntValue(1);

      qualityComparator.LeftSideParameter.ActualName = MoveQualityParameter.Name;
      qualityComparator.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparator.ResultParameter.ActualName = IsAcceptedName;
      qualityComparator.DampeningParameter.ActualName = TemperatureParameter.Name;

      acceptsQualityBranch.ConditionParameter.ActualName = IsAcceptedName;

      moveMaker.Name = "Move maker";
      moveMaker.OperatorParameter.ActualName = MoveMakerParameter.Name;

      temperatureController.AnnealingOperatorParameter.ActualName = AnnealingOperatorParameter.Name;
      temperatureController.ThresholdParameter.ActualName = ThresholdParameter.Name;
      temperatureController.AcceptanceMemoryParameter.ActualName = AcceptanceMemoryName;
      temperatureController.MemorySizeParameter.ActualName = MemorySizeParameter.Name;
      temperatureController.CoolingParameter.ActualName = CoolingParameter.Name;
      temperatureController.EndTemperatureParameter.ActualName = EndTemperatureParameter.Name;
      temperatureController.HeatingOperatorParameter.ActualName = HeatingOperatorParameter.Name;
      temperatureController.IsAcceptedParameter.ActualName = IsAcceptedName;
      temperatureController.IterationsParameter.ActualName = IterationsParameter.Name;
      temperatureController.LowerTemperatureParameter.ActualName = LowerTemperatureParameter.Name;
      temperatureController.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      temperatureController.StartTemperatureParameter.ActualName = StartTemperatureParameter.Name;
      temperatureController.TemperatureParameter.ActualName = TemperatureParameter.Name;
      temperatureController.TemperatureStartIndexParameter.ActualName = TemperatureStartIndexParameter.Name;
      temperatureController.UpperTemperatureParameter.ActualName = UpperTemperatureParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      iterationsCounter.Name = "Iterations++";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = IterationsParameter.Name;

      iterationsComparator.Name = "Iterations >= MaximumIterations";
      iterationsComparator.LeftSideParameter.ActualName = IterationsParameter.Name;
      iterationsComparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      iterationsComparator.ResultParameter.ActualName = TerminateName;
      iterationsComparator.Comparison.Value = ComparisonType.GreaterOrEqual;

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      iterationsTermination.Name = "Iterations termination condition";
      iterationsTermination.ConditionParameter.ActualName = TerminateName;
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = ssp1;
      ssp1.Operators.Add(analyzer1);
      ssp1.Successor = ssp2;
      analyzer1.Successor = null;
      ssp2.Operators.Add(resultsCollector);
      ssp2.Successor = mainProcessor;
      resultsCollector.Successor = null;
      mainProcessor.Operators.Add(moveGenerator);
      mainProcessor.Successor = iterationsCounter;
      moveGenerator.Successor = moveEvaluationProcessor;
      moveEvaluationProcessor.Operators.Add(moveEvaluator);
      moveEvaluationProcessor.Successor = evaluatedMovesCounter;
      moveEvaluator.Successor = qualityComparator;
      qualityComparator.Successor = acceptsQualityBranch;
      acceptsQualityBranch.TrueBranch = moveMaker;
      acceptsQualityBranch.FalseBranch = null;
      acceptsQualityBranch.Successor = temperatureController;
      moveMaker.Successor = null;
      temperatureController.Successor = null;
      evaluatedMovesCounter.Successor = subScopesRemover;
      subScopesRemover.Successor = null;
      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = ssp3;
      ssp3.Operators.Add(analyzer2);
      ssp3.Successor = iterationsTermination;
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
