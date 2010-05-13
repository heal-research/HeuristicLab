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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;
using HeuristicLab.Analysis;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("Particle Swarm Optimization", "A particle swarm optimization algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class ParticleSwarmOptimization : EngineAlgorithm {
    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(ISingleObjectiveProblem); }
    }
    public new ISingleObjectiveProblem Problem {
      get { return (ISingleObjectiveProblem)base.Problem; }
      set { base.Problem = value; }
    }
    public IRealVectorEncoder Encoder {
      get { return EncoderParameter.Value; }
      set { EncoderParameter.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    #endregion 

    #region Parameter Properties
    private ValueParameter<IntValue> SeedParameter {
      get { return (ValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private ValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (ValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    private ValueParameter<IntValue> SwarmSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["SwarmSize"]; }
    }
    private ValueParameter<IntValue> MaxIterationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxIterations"]; }
    }
    private OptionalConstrainedValueParameter<IRealVectorEncoder> EncoderParameter {
      get { return (OptionalConstrainedValueParameter<IRealVectorEncoder>)Parameters["Encoder"]; }
    }
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    #endregion

    #region Properties
    [Storable]
    private ParticleSwarmOptimizationMainLoop mainLoop; // Check this !
    private ParticleSwarmOptimizationMainLoop MainLoop {
      get { return mainLoop; }
    }
    [Storable]
    private Assigner bestLocalQualityInitalizer; // Check this !
    private Assigner BestLocalQualityInitalizer {
      get { return bestLocalQualityInitalizer; }
    }
    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    #endregion

    public ParticleSwarmOptimization()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("SwarmSize", "Size of the particle swarm.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaxIterations", "Maximal number of iterations.", new IntValue(1000)));
      Parameters.Add(new ConstrainedValueParameter<IRealVectorEncoder>("Encoder", "The operator used to encode solutions as position vector."));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze each generation.", new MultiAnalyzer()));
      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor = new UniformSubScopesProcessor();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      VariableCreator variableCreator = new VariableCreator();
      VariableCreator localVariableCreator = new VariableCreator();
      Placeholder encoder = new Placeholder();
      UniformRandomRealVectorCreator velocityVectorCreator = new UniformRandomRealVectorCreator();
      bestLocalQualityInitalizer = new Assigner();
      Assigner bestLocalPositionInitalizer = new Assigner();
      Assigner bestGlobalPositionInitalizer = new Assigner();
      mainLoop = new ParticleSwarmOptimizationMainLoop();
      BestAverageWorstQualityCalculator bawCalculator = new BestAverageWorstQualityCalculator();
      Comparator comparator = new Comparator();
      ConditionalBranch branch = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<RealVector>("CurrentBestPosition", new RealVector()));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleMatrix>("ZeroBounds", new DoubleMatrix(new double[,] { { 0, 0 } })));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Length", new IntValue(2)));

      localVariableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(0)));
      localVariableCreator.CollectedValues.Add(new ValueParameter<RealVector>("BestPosition", new RealVector()));

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      
      solutionsCreator.NumberOfSolutionsParameter.ActualName = SwarmSizeParameter.Name;
      
      encoder.OperatorParameter.ActualName = "Encoder";
      
      velocityVectorCreator.BoundsParameter.ActualName = "ZeroBounds";
      velocityVectorCreator.RealVectorParameter.ActualName = "Velocity";

      bestLocalQualityInitalizer.LeftSideParameter.ActualName = "BestQuality"; // cloned value
      bestLocalQualityInitalizer.RightSideParameter.ActualName = "Quality"; // FIXME!!! Should be mapped

      bestLocalPositionInitalizer.LeftSideParameter.ActualName = "BestPosition";
      bestLocalPositionInitalizer.RightSideParameter.ActualName = "Position"; // FixMe

      bestGlobalPositionInitalizer.LeftSideParameter.ActualName = "CurrentBestPosition";
      bestGlobalPositionInitalizer.RightSideParameter.ActualName = "BestPosition";

      bawCalculator.AverageQualityParameter.ActualName = "CurrentAverageBestQuality";
      bawCalculator.BestQualityParameter.ActualName = "CurrentBestBestQuality";
      bawCalculator.MaximizationParameter.ActualName = "Maximization"; // FIXME
      bawCalculator.QualityParameter.ActualName = "Quality";
      bawCalculator.WorstQualityParameter.ActualName = "CurrentWorstBestQuality";

      comparator.Comparison = new Comparison(ComparisonType.Equal);
      comparator.LeftSideParameter.ActualName = "Quality";
      comparator.ResultParameter.ActualName = "NewGlobalBest";
      comparator.RightSideParameter.ActualName = "CurrentBestBestQuality";

      branch.ConditionParameter.ActualName = "NewGlobalBest";
      branch.TrueBranch = bestGlobalPositionInitalizer; // copy position vector

      mainLoop.MaximumGenerationsParameter.ActualName = MaxIterationsParameter.Name;
      mainLoop.ResultsParameter.ActualName = "Results";
      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;

      OperatorGraph.InitialOperator = randomCreator;
      randomCreator.Successor = solutionsCreator;
      solutionsCreator.Successor = variableCreator;
      variableCreator.Successor = uniformSubScopesProcessor;
      uniformSubScopesProcessor.Operator = encoder;
      encoder.Successor = velocityVectorCreator;
      velocityVectorCreator.Successor = localVariableCreator;
      localVariableCreator.Successor = bestLocalQualityInitalizer;
      bestLocalQualityInitalizer.Successor = bestLocalPositionInitalizer;
      uniformSubScopesProcessor.Successor = bawCalculator; // mainLoop;
      bawCalculator.Successor = uniformSubScopesProcessor2;
      uniformSubScopesProcessor2.Operator = comparator;
      comparator.Successor = branch;
      uniformSubScopesProcessor2.Successor = mainLoop;
      InitializeAnalyzers();
      UpdateAnalyzers();
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      EncoderParameter.ValueChanged += new EventHandler(EncoderParameter_ValueChanged);
      if (Problem != null) {
        bestLocalQualityInitalizer.RightSideParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      }
    }

    [StorableConstructor]
    private ParticleSwarmOptimization(bool deserializing) : base(deserializing) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      ParticleSwarmOptimization clone = (ParticleSwarmOptimization)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      UpdateEncoders();
      UpdateAnalyzers();
      ParameterizeAnalyzers(); 
      bestLocalQualityInitalizer.RightSideParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      MainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      base.OnProblemChanged();
    }

    private void EncoderParameter_ValueChanged(object sender, EventArgs e) {
      //MainLoop.EncoderParameter.ActualValue = (IRealVectorEncoder) EncoderParameter.ActualValue;
      //((UniformSubScopesProcessor)((VariableCreator)((SolutionsCreator)((RandomCreator)OperatorGraph.InitialOperator).Successor).Successor).Successor).Operator = EncoderParameter.Value;
      //((SingleSuccessorOperator)EncoderParameter.Value).Successor = ((SingleSuccessorOperator)old).Successor;
    }
    #endregion

    #region Helpers
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      // 
      //
    }

    private void UpdateEncoders() {
      IRealVectorEncoder oldEncoder = EncoderParameter.Value;
      EncoderParameter.ValidValues.Clear();
      List<IRealVectorEncoder> encoders = Problem.Operators.OfType<IRealVectorEncoder>().OrderBy(x => x.Name).ToList<IRealVectorEncoder>();
      if (encoders.Count > 0) {  // ToDo: Add wiring; else: use Position Vector directly --> name matching
        foreach (IRealVectorEncoder encoder in Problem.Operators.OfType<IRealVectorEncoder>().OrderBy(x => x.Name)) {
          EncoderParameter.ValidValues.Add(encoder);
          ((ILookupParameter)encoder.RealVectorParameter).ActualName = "Position";
        }
        if (oldEncoder != null) {
          IRealVectorEncoder encoder = EncoderParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldEncoder.GetType());
          if (encoder != null) EncoderParameter.Value = encoder;
        }
      }
    }

    private void InitializeAnalyzers() {
      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      ParameterizeAnalyzers();
    }

    private void ParameterizeAnalyzers() {
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      if (Problem != null) {
        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
      }
    }

    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>().OrderBy(x => x.Name))
          Analyzer.Operators.Add(analyzer);
      }
      Analyzer.Operators.Add(qualityAnalyzer);
    }

    #endregion
  }
}
