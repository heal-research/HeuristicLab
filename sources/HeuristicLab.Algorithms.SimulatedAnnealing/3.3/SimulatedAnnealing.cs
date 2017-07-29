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

using System;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.SimulatedAnnealing
{
    [Item("Simulated Annealing", "An advanced simulated annealing algorithm.")]
    [Creatable("Algorithms")]
    [StorableClass]
    public sealed class SimulatedAnnealing : HeuristicOptimizationEngineAlgorithm, IStorableContent
    {
        #region Strings
        private const string SeedName = "Seed";
        private const string SetSeedRandomlyName = "SetSeedRandomly";
        private const string MoveGeneratorName = "MoveGenerator";
        private const string MoveEvaluatorName = "MoveEvaluator";
        private const string MoveMakerName = "MoveMaker";
        private const string AnnealingOperatorName = "AnnealingOperator";
        private const string ReheatingOperatorName = "ReheatingOperator";
        private const string MaximumIterationsName = "MaximumIterations";
        private const string InitialTemperatureName = "InitialTemperature";
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
        private const string TemperatureChartName = "Temperature Chart";
        private const string TemperatureAnalyzerName = "Temperature Analyzer";
        private const string TemperatureInitializerName = "TemperatureInitializer";
        #endregion

        public string Filename { get; set; }

        #region Problem Properties
        public override Type ProblemType
        {
            get { return typeof(ISingleObjectiveHeuristicOptimizationProblem); }
        }
        public new ISingleObjectiveHeuristicOptimizationProblem Problem
        {
            get { return (ISingleObjectiveHeuristicOptimizationProblem)base.Problem; }
            set { base.Problem = value; }
        }
        #endregion

        #region Parameter Properties
        private ValueParameter<IntValue> SeedParameter
        {
            get { return (ValueParameter<IntValue>)Parameters[SeedName]; }
        }
        private ValueParameter<BoolValue> SetSeedRandomlyParameter
        {
            get { return (ValueParameter<BoolValue>)Parameters[SetSeedRandomlyName]; }
        }
        public IConstrainedValueParameter<IMultiMoveGenerator> MoveGeneratorParameter
        {
            get { return (IConstrainedValueParameter<IMultiMoveGenerator>)Parameters[MoveGeneratorName]; }
        }
        public IConstrainedValueParameter<IMoveMaker> MoveMakerParameter
        {
            get { return (IConstrainedValueParameter<IMoveMaker>)Parameters[MoveMakerName]; }
        }
        public IConstrainedValueParameter<ISingleObjectiveMoveEvaluator> MoveEvaluatorParameter
        {
            get { return (IConstrainedValueParameter<ISingleObjectiveMoveEvaluator>)Parameters[MoveEvaluatorName]; }
        }
        public IConstrainedValueParameter<IDiscreteDoubleValueModifier> AnnealingOperatorParameter
        {
            get { return (IConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters[AnnealingOperatorName]; }
        }

        public IConstrainedValueParameter<IReheatingOperator> ReheatingOperatorParameter
        {
            get { return (IConstrainedValueParameter<IReheatingOperator>)Parameters[ReheatingOperatorName]; }
        }
        public IConstrainedValueParameter<ITemperatureInitializer> TemperatureInitializerParameter
        {
            get { return (IConstrainedValueParameter<ITemperatureInitializer>)Parameters[TemperatureInitializerName]; }
        }

        private ValueParameter<IntValue> MaximumIterationsParameter
        {
            get { return (ValueParameter<IntValue>)Parameters[MaximumIterationsName]; }
        }
        private ValueParameter<DoubleValue> LowerTemperatureParameter
        {
            get { return (ValueParameter<DoubleValue>)Parameters[LowerTemperatureName]; }
        }
        private ValueParameter<MultiAnalyzer> AnalyzerParameter
        {
            get { return (ValueParameter<MultiAnalyzer>)Parameters[AnalyzerName]; }
        }
        #endregion

        #region Properties
        public IntValue Seed
        {
            get { return SeedParameter.Value; }
            set { SeedParameter.Value = value; }
        }
        public BoolValue SetSeedRandomly
        {
            get { return SetSeedRandomlyParameter.Value; }
            set { SetSeedRandomlyParameter.Value = value; }
        }
        public IMultiMoveGenerator MoveGenerator
        {
            get { return MoveGeneratorParameter.Value; }
            set { MoveGeneratorParameter.Value = value; }
        }
        public IMoveMaker MoveMaker
        {
            get { return MoveMakerParameter.Value; }
            set { MoveMakerParameter.Value = value; }
        }
        public ISingleObjectiveMoveEvaluator MoveEvaluator
        {
            get { return MoveEvaluatorParameter.Value; }
            set { MoveEvaluatorParameter.Value = value; }
        }
        public IDiscreteDoubleValueModifier AnnealingOperator
        {
            get { return AnnealingOperatorParameter.Value; }
            set { AnnealingOperatorParameter.Value = value; }
        }

        public IReheatingOperator ReheatingOperator
        {
            get { return ReheatingOperatorParameter.Value; }
            set { ReheatingOperatorParameter.Value = value; }
        }
        public ITemperatureInitializer TemperatureInitializer
        {
            get { return TemperatureInitializerParameter.Value; }
            set { TemperatureInitializerParameter.Value = value; }
        }

        public IntValue MaximumIterations
        {
            get { return MaximumIterationsParameter.Value; }
            set { MaximumIterationsParameter.Value = value; }
        }
        public DoubleValue LowerTemperature
        {
            get { return LowerTemperatureParameter.Value; }
            set { LowerTemperatureParameter.Value = value; }
        }
        public MultiAnalyzer Analyzer
        {
            get { return AnalyzerParameter.Value; }
            set { AnalyzerParameter.Value = value; }
        }
        private RandomCreator RandomCreator
        {
            get { return (RandomCreator)OperatorGraph.InitialOperator; }
        }
        private SolutionsCreator SolutionsCreator
        {
            get { return (SolutionsCreator)RandomCreator.Successor; }
        }
        private SimulatedAnnealingMainLoop MainLoop
        {
            get { return OperatorGraph.Iterate().OfType<SimulatedAnnealingMainLoop>().First(); }
        }
        #endregion

        [StorableConstructor]
        private SimulatedAnnealing(bool deserializing) : base(deserializing) { }
        private SimulatedAnnealing(SimulatedAnnealing original, Cloner cloner)
          : base(original, cloner)
        {
            RegisterEventHandlers();
        }
        public SimulatedAnnealing()
          : base()
        {
            Parameters.Add(new ValueParameter<IntValue>(SeedName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
            Parameters.Add(new ValueParameter<BoolValue>(SetSeedRandomlyName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
            Parameters.Add(new ConstrainedValueParameter<IMultiMoveGenerator>(MoveGeneratorName, "The operator used to generate moves to the neighborhood of the current solution."));
            Parameters.Add(new ConstrainedValueParameter<ISingleObjectiveMoveEvaluator>(MoveEvaluatorName, "The operator used to evaluate a move."));
            Parameters.Add(new ConstrainedValueParameter<IMoveMaker>(MoveMakerName, "The operator used to perform a move."));
            Parameters.Add(new ConstrainedValueParameter<IDiscreteDoubleValueModifier>(AnnealingOperatorName, "The operator used to cool the temperature."));
            Parameters.Add(new ConstrainedValueParameter<IReheatingOperator>(ReheatingOperatorName, "The operator used to reheat the temperature, if necessary."));
            Parameters.Add(new ConstrainedValueParameter<ITemperatureInitializer>(TemperatureInitializerName, "The operator used to initialize the temperature."));
            Parameters.Add(new ValueParameter<IntValue>(MaximumIterationsName, "The maximum number of generations which should be processed.", new IntValue(10000)));
            Parameters.Add(new ValueParameter<DoubleValue>(LowerTemperatureName, "The lower bound for the temperature.", new DoubleValue(1e-6)));
            Parameters.Add(new ValueParameter<MultiAnalyzer>(AnalyzerName, "The operator used to analyze each iteration.", new MultiAnalyzer()));

            var randomCreator = new RandomCreator();
            var solutionsCreator = new SolutionsCreator();
            var variableCreator = new VariableCreator();
            var startTemperatureAssigner = new Assigner();
            var endTemperatureAssigner = new Assigner();
            var temperatureAssigner = new Assigner();
            var resultsCollector = new ResultsCollector();
            var mainLoop = new SimulatedAnnealingMainLoop();
            OperatorGraph.InitialOperator = randomCreator;

            randomCreator.RandomParameter.ActualName = RandomName;
            randomCreator.SeedParameter.ActualName = SeedParameter.Name;
            randomCreator.SeedParameter.Value = null;
            randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
            randomCreator.SetSeedRandomlyParameter.Value = null;
            randomCreator.Successor = solutionsCreator;

            solutionsCreator.NumberOfSolutions = new IntValue(1);
            solutionsCreator.Successor = variableCreator;

            variableCreator.Name = "Initialize Variables";
            variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>(InitialTemperatureName, new DoubleValue(Double.PositiveInfinity)));
            variableCreator.CollectedValues.Add(new ValueParameter<IntValue>(EvaluatedMovesName, new IntValue()));
            variableCreator.CollectedValues.Add(new ValueParameter<IntValue>(IterationsName, new IntValue(0)));
            variableCreator.CollectedValues.Add(new ValueParameter<IntValue>(TemperatureStartIndexName, new IntValue(0)));
            variableCreator.CollectedValues.Add(new ValueParameter<BoolValue>(CoolingName, new BoolValue(true)));

            variableCreator.Successor = startTemperatureAssigner;

            startTemperatureAssigner.Name = "Assign Start Temperature";
            startTemperatureAssigner.LeftSideParameter.ActualName = StartTemperatureName;
            startTemperatureAssigner.RightSideParameter.ActualName = InitialTemperatureName;
            startTemperatureAssigner.Successor = endTemperatureAssigner; 

            endTemperatureAssigner.Name = "Assign End Temperature";
            endTemperatureAssigner.LeftSideParameter.ActualName = EndTemperatureName;
            endTemperatureAssigner.RightSideParameter.ActualName = LowerTemperatureParameter.Name;
            endTemperatureAssigner.Successor = temperatureAssigner; 

            temperatureAssigner.Name = "Initialize Temperature";
            temperatureAssigner.LeftSideParameter.ActualName = TemperatureName;
            temperatureAssigner.RightSideParameter.ActualName = StartTemperatureName;
            temperatureAssigner.Successor =  resultsCollector;

            resultsCollector.CopyValue = new BoolValue(false);
            resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>(EvaluatedMovesName, null));
            resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>(IterationsName, null));
            resultsCollector.ResultsParameter.ActualName = ResultsName;
            resultsCollector.Successor = mainLoop;

            mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
            mainLoop.AnnealingOperatorParameter.ActualName = AnnealingOperatorParameter.Name;
            mainLoop.CoolingParameter.ActualName = CoolingName;
            mainLoop.EndTemperatureParameter.ActualName = EndTemperatureName;
            mainLoop.EvaluatedMovesParameter.ActualName = EvaluatedMovesName;

            mainLoop.IterationsParameter.ActualName = IterationsName;
            mainLoop.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
            mainLoop.MoveEvaluatorParameter.ActualName = MoveEvaluatorParameter.Name;
            mainLoop.MoveGeneratorParameter.ActualName = MoveGeneratorParameter.Name;
            mainLoop.MoveMakerParameter.ActualName = MoveMakerParameter.Name;
            mainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
            mainLoop.ResultsParameter.ActualName = ResultsName;
            mainLoop.StartTemperatureParameter.ActualName = StartTemperatureName;
            mainLoop.TemperatureStartIndexParameter.ActualName = TemperatureStartIndexName;
            mainLoop.TemperatureParameter.ActualName = TemperatureName;

            foreach (var op in ApplicationManager.Manager.GetInstances<IReheatingOperator>().OrderBy(x => x.Name))
            {
               
                ReheatingOperatorParameter.ValidValues.Add(op);
            }

            foreach (var op in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>().OrderBy(x => x.Name))
            {
                AnnealingOperatorParameter.ValidValues.Add(op);
            }

            foreach(var op in ApplicationManager.Manager.GetInstances<ITemperatureInitializer>().OrderBy(x => x.Name))
            {
                TemperatureInitializerParameter.ValidValues.Add(op);
            }

            UpdateAnalyzers();

            Parameterize();

            RegisterEventHandlers();
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new SimulatedAnnealing(this, cloner);
        }

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            if (Problem != null)
            {
                Problem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
                foreach (var op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>())
                {
                    op.MoveQualityParameter.ActualNameChanged += MoveEvaluator_MoveQualityParameter_ActualNameChanged;
                }
            }
            MoveGeneratorParameter.ValueChanged += MoveGeneratorParameter_ValueChanged;
            MoveEvaluatorParameter.ValueChanged += MoveEvaluatorParameter_ValueChanged;
        }

        public override void Prepare()
        {
            if (Problem != null && MoveGenerator != null && MoveMaker != null && MoveEvaluator != null)
                base.Prepare();
        }

        #region Events
        protected override void OnProblemChanged()
        {
            foreach (var op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>())
            {
                op.MoveQualityParameter.ActualNameChanged += MoveEvaluator_MoveQualityParameter_ActualNameChanged;
            }
            UpdateMoveGenerator();
            UpdateMoveOperators();
            UpdateAnalyzers();
            Parameterize();
            Problem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
            base.OnProblemChanged();
        }
        protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e)
        {
            Parameterize();
            base.Problem_SolutionCreatorChanged(sender, e);
        }
        protected override void Problem_EvaluatorChanged(object sender, EventArgs e)
        {
            Parameterize();
            Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
            base.Problem_EvaluatorChanged(sender, e);
        }
        protected override void Problem_OperatorsChanged(object sender, EventArgs e)
        {
            foreach (var op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>())
            {
                op.MoveQualityParameter.ActualNameChanged -= MoveEvaluator_MoveQualityParameter_ActualNameChanged;
                op.MoveQualityParameter.ActualNameChanged += MoveEvaluator_MoveQualityParameter_ActualNameChanged;
            }
            UpdateMoveGenerator();
            UpdateMoveOperators();
            UpdateAnalyzers();
            Parameterize();
            base.Problem_OperatorsChanged(sender, e);
        }
        private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e)
        {
            Parameterize();
        }
        private void MoveGeneratorParameter_ValueChanged(object sender, EventArgs e)
        {
            UpdateMoveOperators();
        }
        private void MoveEvaluatorParameter_ValueChanged(object sender, EventArgs e)
        {
            Parameterize();
        }
        private void MoveEvaluator_MoveQualityParameter_ActualNameChanged(object sender, EventArgs e)
        {
            if (sender == MoveEvaluator) Parameterize();
        }
        #endregion

        #region Helpers

        private void UpdateMoveGenerator()
        {
            var oldMoveGenerator = MoveGenerator;
            var defaultMoveGenerator = Problem.Operators.OfType<IMultiMoveGenerator>().FirstOrDefault();

            MoveGeneratorParameter.ValidValues.Clear();

            if (Problem != null)
            {
                foreach (var generator in Problem.Operators.OfType<IMultiMoveGenerator>().OrderBy(x => x.Name))
                    MoveGeneratorParameter.ValidValues.Add(generator);
            }

            if (oldMoveGenerator != null)
            {
                var newMoveGenerator = MoveGeneratorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveGenerator.GetType());
                if (newMoveGenerator != null) MoveGenerator = newMoveGenerator;
                else oldMoveGenerator = null;
            }
            if (oldMoveGenerator == null && defaultMoveGenerator != null)
                MoveGenerator = defaultMoveGenerator;

            UpdateMoveOperators();
        }

        private void UpdateMoveOperators()
        {
            var oldMoveMaker = MoveMaker;
            var oldMoveEvaluator = MoveEvaluator;

            MoveMakerParameter.ValidValues.Clear();
            MoveEvaluatorParameter.ValidValues.Clear();

            if (MoveGenerator != null)
            {
                var moveTypes = MoveGenerator.GetType().GetInterfaces().Where(x => typeof(IMoveOperator).IsAssignableFrom(x)).ToList();
                foreach (var type in moveTypes.ToList())
                {
                    if (moveTypes.Any(t => t != type && type.IsAssignableFrom(t)))
                        moveTypes.Remove(type);
                }
                var operators = Problem.Operators.Where(op => moveTypes.Any(m => m.IsInstanceOfType(op))).ToList();
                var defaultMoveMaker = operators.OfType<IMoveMaker>().FirstOrDefault();
                var defaultMoveEvaluator = operators.OfType<ISingleObjectiveMoveEvaluator>().FirstOrDefault();

                foreach (var moveMaker in operators.OfType<IMoveMaker>().OrderBy(op => op.Name))
                    MoveMakerParameter.ValidValues.Add(moveMaker);

                foreach (var moveEvaluator in operators.OfType<ISingleObjectiveMoveEvaluator>().OrderBy(op => op.Name))
                    MoveEvaluatorParameter.ValidValues.Add(moveEvaluator);

                if (oldMoveMaker != null)
                {
                    var mm = MoveMakerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveMaker.GetType());
                    if (mm != null) MoveMaker = mm;
                    else oldMoveMaker = null;
                }
                if (oldMoveMaker == null && defaultMoveMaker != null)
                    MoveMaker = defaultMoveMaker;

                if (oldMoveEvaluator != null)
                {
                    var me = MoveEvaluatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveEvaluator.GetType());
                    if (me != null) MoveEvaluator = me;
                    else oldMoveEvaluator = null;
                }
                if (oldMoveEvaluator == null & defaultMoveEvaluator != null)
                    MoveEvaluator = defaultMoveEvaluator;
            }
        }

        private void UpdateAnalyzers()
        {
            Analyzer.Operators.Clear();
            if (Problem != null)
            {
                foreach (var analyzer in Problem.Operators.OfType<IAnalyzer>())
                {
                    foreach (var param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
                        param.Depth = 0;
                    Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
                }
            }
            var qualityAnalyzer = new QualityAnalyzer();
            var temperatureAnalyzer = new SingleValueAnalyzer { Name = TemperatureAnalyzerName };
            Analyzer.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
            Analyzer.Operators.Add(temperatureAnalyzer, temperatureAnalyzer.EnabledByDefault);
        }

        private void Parameterize()
        {

            #region IStochasticOperator
            if (Problem != null)
            {
                foreach (var op in Problem.Operators.OfType<IStochasticOperator>())
                {
                    op.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
                    op.RandomParameter.Hidden = true;
                }
            }
            #endregion

            #region IIterationBasedOperator
            if (Problem != null)
            {
                foreach (var op in Problem.Operators.OfType<IIterationBasedOperator>())
                {
                    op.IterationsParameter.ActualName = IterationsName;
                    op.IterationsParameter.Hidden = true;
                    op.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
                    op.MaximumIterationsParameter.Hidden = true;
                }
            }
            #endregion

            #region Analyzers

            foreach (var qualityAnalyzer in Analyzer.Operators.OfType<QualityAnalyzer>())
            {
                qualityAnalyzer.ResultsParameter.ActualName = ResultsName;
                if (Problem != null)
                {
                    qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
                    qualityAnalyzer.MaximizationParameter.Hidden = true;
                    qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
                    qualityAnalyzer.QualityParameter.Depth = 0;
                    qualityAnalyzer.QualityParameter.Hidden = true;
                    qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
                    qualityAnalyzer.BestKnownQualityParameter.Hidden = true;
                }
                else
                {
                    qualityAnalyzer.MaximizationParameter.Hidden = false;
                    qualityAnalyzer.QualityParameter.Hidden = false;
                    qualityAnalyzer.BestKnownQualityParameter.Hidden = false;
                }
            }

            var temperatureAnalyzer = Analyzer.Operators.OfType<SingleValueAnalyzer>().FirstOrDefault(x => x.Name == TemperatureAnalyzerName);
            if (temperatureAnalyzer != null)
            {
                temperatureAnalyzer.ResultsParameter.ActualName = ResultsName;
                temperatureAnalyzer.ResultsParameter.Hidden = true;
                temperatureAnalyzer.ValueParameter.ActualName = TemperatureName;
                temperatureAnalyzer.ValueParameter.Hidden = true;
                temperatureAnalyzer.ValuesParameter.ActualName = TemperatureChartName;
                temperatureAnalyzer.ValuesParameter.Hidden = true;
            }

            #endregion

            #region SolutionCreator
            if (Problem != null)
            {
                SolutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
                SolutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
            }
            #endregion

            #region Annealing/Reheating Operators
            foreach (var op in AnnealingOperatorParameter.ValidValues)
            {
                op.IndexParameter.ActualName = IterationsName;
                op.IndexParameter.Hidden = true;
                op.StartIndexParameter.Value = null;
                op.StartIndexParameter.ActualName = TemperatureStartIndexName;
                op.EndIndexParameter.ActualName = MaximumIterationsParameter.Name;
                op.ValueParameter.ActualName = TemperatureName;
                op.ValueParameter.Hidden = true;
                op.StartValueParameter.ActualName = StartTemperatureName;
                op.StartValueParameter.Hidden = true;
                op.EndValueParameter.ActualName = LowerTemperatureParameter.Name;
                op.EndValueParameter.Hidden = true;
            }
            foreach (var op in ReheatingOperatorParameter.ValidValues)
            {
                op.Parameterize();
            }
            #endregion

            #region Move Operators
            if (Problem != null)
            {
                foreach (var op in Problem.Operators.OfType<IMultiMoveGenerator>())
                {
                    op.SampleSizeParameter.Value = new IntValue(1);
                    op.SampleSizeParameter.Hidden = true;
                }
                foreach (var op in Problem.Operators.OfType<IMoveMaker>())
                {
                    op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
                    op.QualityParameter.Hidden = true;
                    if (MoveEvaluator != null)
                    {
                        op.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
                        op.MoveQualityParameter.Hidden = true;
                    }
                    else
                    {
                        op.MoveQualityParameter.Hidden = false;
                    }
                }
                foreach (var op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>())
                {
                    op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
                    op.QualityParameter.Hidden = true;
                }
            }
            #endregion

            #region MainLoop
            if (Problem != null)
            {
                MainLoop.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
                MainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
                MainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
                if (MoveEvaluator != null)
                    MainLoop.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
            }
            #endregion

        }
        #endregion
    }
}