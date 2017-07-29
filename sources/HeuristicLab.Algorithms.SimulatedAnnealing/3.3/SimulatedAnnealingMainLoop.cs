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

namespace HeuristicLab.Algorithms.SimulatedAnnealing
{
    [Item("SimulatedAnnealingMainLoop", "An operator which represents the main loop of a simulated annealing algorithm.")]
    [StorableClass]
    public sealed class SimulatedAnnealingMainLoop : AlgorithmOperator
    {
        #region Strings
        private const string MoveGeneratorName = "MoveGenerator";
        private const string MoveEvaluatorName = "MoveEvaluator";
        private const string MoveMakerName = "MoveMaker";
        private const string AnnealingOperatorName = "AnnealingOperator";
        private const string ReheatingOperatorName = "ReheatingOperator";
        private const string MaximumIterationsName = "MaximumIterations";
        private const string LowerTemperatureName = "LowerTemperature";
        private const string AnalyzerName = "Analyzer";
        private const string RandomName = "Random";
        private const string EvaluatedMovesName = "EvaluatedMoves";
        private const string IterationsName = "Iterations";
        private const string TemperatureStartIndexName = "TemperatureStartIndex";
        private const string CoolingName = "Cooling";
        private const string InitialTemperatureName = "InitialTemperature";
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
        private const string AcceptanceMemoryName = "AcceptanceMemory";
        private const string ConsecutiveRejectedSolutionsCountName = "ConsecutiveRejectedSolutions";
        private const string AverageAcceptanceRatioName = "AverageAcceptanceRatio";
        private const string LastQualityName = "LastQuality";
        private const string UphillMovesMemoryName = "UphillMovesMemory";
        private const string TemperatureBeforeReheatName = "TemperatureBeforeReheat";
        private const string CurrentRandomWalkStepName = "CurrentRandomWalkStep";
        private const string QualitiesBeforeReheatingName = "QualitiesBeforeReheating";
        private const string LastAcceptedQualityName = "LastAcceptedQuality";
        private const string TemperatureInitializerName = "TemperatureInitializer";
        private const string TemperatureInitializedName = "TemperatureInitialized";

        #endregion

        #region Parameter properties
        public IValueLookupParameter<IRandom> RandomParameter
        {
            get { return (IValueLookupParameter<IRandom>)Parameters[RandomName]; }
        }
        public IValueLookupParameter<BoolValue> MaximizationParameter
        {
            get { return (IValueLookupParameter<BoolValue>)Parameters[MaximizationName]; }
        }
        public ILookupParameter<DoubleValue> QualityParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[QualityName]; }
        }
        public IValueLookupParameter<DoubleValue> BestKnownQualityParameter
        {
            get { return (IValueLookupParameter<DoubleValue>)Parameters[BestKnownQualityName]; }
        }
        public IValueLookupParameter<DoubleValue> LastQualityParameter
        {
            get { return (IValueLookupParameter<DoubleValue>)Parameters[LastQualityName]; }
        }
        public ILookupParameter<DoubleValue> InitialTemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[InitialTemperatureName]; }
        }
        public ILookupParameter<DoubleValue> MoveQualityParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[MoveQualityName]; }
        }
        public ILookupParameter<DoubleValue> TemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[TemperatureName]; }
        }
        public IValueLookupParameter<DoubleValue> LowerTemperatureParameter
        {
            get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerTemperatureName]; }
        }
        public ILookupParameter<IntValue> IterationsParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[IterationsName]; }
        }
        public IValueLookupParameter<IntValue> MaximumIterationsParameter
        {
            get { return (IValueLookupParameter<IntValue>)Parameters[MaximumIterationsName]; }
        }
        public IValueLookupParameter<IOperator> MoveGeneratorParameter
        {
            get { return (IValueLookupParameter<IOperator>)Parameters[MoveGeneratorName]; }
        }
        public IValueLookupParameter<IOperator> MoveEvaluatorParameter
        {
            get { return (IValueLookupParameter<IOperator>)Parameters[MoveEvaluatorName]; }
        }
        public IValueLookupParameter<IOperator> MoveMakerParameter
        {
            get { return (IValueLookupParameter<IOperator>)Parameters[MoveMakerName]; }
        }
        public IValueLookupParameter<IOperator> AnnealingOperatorParameter
        {
            get { return (IValueLookupParameter<IOperator>)Parameters[AnnealingOperatorName]; }
        }
        public IValueLookupParameter<IOperator> ReheatingOperatorParameter
        {
            get { return (IValueLookupParameter<IOperator>)Parameters[ReheatingOperatorName]; }
        }
        public IValueLookupParameter<IOperator> AnalyzerParameter
        {
            get { return (IValueLookupParameter<IOperator>)Parameters[AnalyzerName]; }
        }
        public IValueLookupParameter<VariableCollection> ResultsParameter
        {
            get { return (IValueLookupParameter<VariableCollection>)Parameters[ResultsName]; }
        }
        public ILookupParameter<IntValue> EvaluatedMovesParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[EvaluatedMovesName]; }
        }
        public ILookupParameter<DoubleValue> StartTemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[StartTemperatureName]; }
        }
        public ILookupParameter<DoubleValue> EndTemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[EndTemperatureName]; }
        }
        public ILookupParameter<IntValue> TemperatureStartIndexParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[TemperatureStartIndexName]; }
        }
        public ILookupParameter<BoolValue> CoolingParameter
        {
            get { return (ILookupParameter<BoolValue>)Parameters[CoolingName]; }
        }
        #endregion

        [StorableConstructor]
        private SimulatedAnnealingMainLoop(bool deserializing) : base(deserializing) { }
        private SimulatedAnnealingMainLoop(SimulatedAnnealingMainLoop original, Cloner cloner)
          : base(original, cloner)
        {
        }
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new SimulatedAnnealingMainLoop(this, cloner);
        }
        public SimulatedAnnealingMainLoop()
          : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            #region Create parameters
            Parameters.Add(new ValueLookupParameter<IRandom>(RandomName, "A pseudo random number generator."));
            Parameters.Add(new ValueLookupParameter<BoolValue>(MaximizationName, "True if the problem is a maximization problem, otherwise false."));
            Parameters.Add(new LookupParameter<DoubleValue>(QualityName, "The value which represents the quality of a solution."));
            Parameters.Add(new ValueLookupParameter<DoubleValue>(BestKnownQualityName, "The best known quality value found so far."));
            Parameters.Add(new LookupParameter<DoubleValue>(MoveQualityName, "The value which represents the quality of a move."));
            Parameters.Add(new LookupParameter<DoubleValue>(TemperatureName, "The current temperature."));
            Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerTemperatureName, "The lower bound of the temperature."));
            Parameters.Add(new LookupParameter<IntValue>(IterationsName, "The number of iterations."));
            Parameters.Add(new ValueLookupParameter<IntValue>(MaximumIterationsName, "The maximum number of iterations which should be processed."));

            Parameters.Add(new ValueLookupParameter<IOperator>(MoveGeneratorName, "The operator that generates the moves."));
            Parameters.Add(new ValueLookupParameter<IOperator>(MoveEvaluatorName, "The operator that evaluates a move."));
            Parameters.Add(new ValueLookupParameter<IOperator>(MoveMakerName, "The operator that performs a move and updates the quality."));
            Parameters.Add(new ValueLookupParameter<IOperator>(AnnealingOperatorName, "The operator that cools the temperature."));
            Parameters.Add(new ValueLookupParameter<IOperator>(ReheatingOperatorName, "The operator that reheats the temperature if necessary."));
            Parameters.Add(new ValueLookupParameter<IOperator>(TemperatureInitializerName, "The operator that initialized the temperature."));

            Parameters.Add(new ValueLookupParameter<IOperator>(AnalyzerName, "The operator used to analyze each generation."));
            Parameters.Add(new ValueLookupParameter<VariableCollection>(ResultsName, "The variable collection where results should be stored."));
            Parameters.Add(new LookupParameter<IntValue>(EvaluatedMovesName, "The number of evaluated moves."));

            Parameters.Add(new LookupParameter<DoubleValue>(InitialTemperatureName, "The initial temperature."));
            Parameters.Add(new LookupParameter<IntValue>(TemperatureStartIndexName, "The index where the annealing or heating was last changed."));
            Parameters.Add(new LookupParameter<BoolValue>(CoolingName, "True when the temperature should be cooled, false otherwise."));
            Parameters.Add(new LookupParameter<DoubleValue>(StartTemperatureName, "The temperature from which cooling or reheating should occur."));
            Parameters.Add(new LookupParameter<DoubleValue>(EndTemperatureName, "The temperature to which should be cooled or heated."));


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
            variableCreator.CollectedValues.Add(new ValueParameter<BoolValue>(TemperatureInitializedName, new BoolValue(false)));
            variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>(AverageAcceptanceRatioName, new DoubleValue(0d)));
            variableCreator.CollectedValues.Add(new ValueParameter<IntValue>(ConsecutiveRejectedSolutionsCountName, new IntValue(0)));
            variableCreator.CollectedValues.Add(new ValueParameter<ItemList<BoolValue>>(AcceptanceMemoryName, new ItemList<BoolValue>()));
            variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>(LastQualityName, new DoubleValue(-1)));
            variableCreator.CollectedValues.Add(new ValueParameter<ItemList<DoubleValue>>(UphillMovesMemoryName, new ItemList<DoubleValue>()));
            variableCreator.CollectedValues.Add(new ValueParameter<IntValue>(CurrentRandomWalkStepName, new IntValue(0)));
            variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>(TemperatureBeforeReheatName, new DoubleValue(0)));
            variableCreator.CollectedValues.Add(new ValueParameter<ItemList<DoubleValue>>(QualitiesBeforeReheatingName, new ItemList<DoubleValue>()));
            variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>(LastAcceptedQualityName, new DoubleValue(-1)));

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

        public override IOperation Apply()
        {
            if (MoveGeneratorParameter.ActualValue == null || MoveEvaluatorParameter.ActualValue == null || MoveMakerParameter.ActualValue == null)
                return null;
            return base.Apply();
        }
    }
}
