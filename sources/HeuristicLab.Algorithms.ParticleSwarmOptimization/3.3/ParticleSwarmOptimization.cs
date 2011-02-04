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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;
using System.Collections.Generic;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {

  [Item("Particle Swarm Optimization", "A particle swarm optimization algorithm based on the description in Pedersen, M.E.H. (2010). PhD thesis. University of Southampton.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public class ParticleSwarmOptimization : EngineAlgorithm, IStorableContent {

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(ISingleObjectiveProblem); }
    }
    public new ISingleObjectiveProblem Problem {
      get { return (ISingleObjectiveProblem)base.Problem; }
      set { base.Problem = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public IDiscreteDoubleValueModifier OmegaUpdater {
      get { return OmegaUpdaterParameter.Value; }
      set { OmegaUpdaterParameter.Value = value; }
    }
    public IDiscreteDoubleMatrixModifier VelocityBoundsUpdater {
      get { return VelocityBoundsUpdaterParameter.Value; }
      set { VelocityBoundsUpdaterParameter.Value = value; }
    }
    #endregion

    #region Parameter Properties
    public IValueParameter<IntValue> SeedParameter {
      get { return (IValueParameter<IntValue>)Parameters["Seed"]; }
    }
    public IValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    public IValueParameter<IntValue> SwarmSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["SwarmSize"]; }
    }
    public IValueParameter<IntValue> MaxIterationsParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaxIterations"]; }
    }
    public IValueParameter<DoubleValue> OmegaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Omega"]; }
    }
    public IValueParameter<DoubleValue> Phi_PParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Phi_P"]; }
    }
    public IValueParameter<DoubleValue> Phi_GParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Phi_G"]; }
    }
    public IValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (IValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    public IValueLookupParameter<DoubleMatrix> VelocityBoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["VelocityBounds"]; }
    }
    public ConstrainedValueParameter<IParticleUpdater> ParticleUpdaterParameter {
      get { return (ConstrainedValueParameter<IParticleUpdater>)Parameters["ParticleUpdater"]; }
    }
    public OptionalConstrainedValueParameter<ITopologyInitializer> TopologyInitializerParameter {
      get { return (OptionalConstrainedValueParameter<ITopologyInitializer>)Parameters["TopologyInitializer"]; }
    }
    public OptionalConstrainedValueParameter<ITopologyUpdater> TopologyUpdaterParameter {
      get { return (OptionalConstrainedValueParameter<ITopologyUpdater>)Parameters["TopologyUpdater"]; }
    }
    public OptionalConstrainedValueParameter<IDiscreteDoubleMatrixModifier> VelocityBoundsUpdaterParameter {
      get { return (OptionalConstrainedValueParameter<IDiscreteDoubleMatrixModifier>)Parameters["VelocityBoundsUpdater"]; }
    }
    public OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier> OmegaUpdaterParameter {
      get { return (OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters["OmegaUpdater"]; }
    }
    #endregion

    #region Properties

    public string Filename { get; set; }

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;

    public ITopologyInitializer TopologyInitializer {
      get { return TopologyInitializerParameter.Value; }
      set { TopologyInitializerParameter.Value = value; }
    }

    public ITopologyUpdater TopologyUpdater {
      get { return TopologyUpdaterParameter.Value; }
      set { TopologyUpdaterParameter.Value = value; }
    }

    public IParticleUpdater ParticleUpdater {
      get { return ParticleUpdaterParameter.Value; }
      set { ParticleUpdaterParameter.Value = value; }
    }

    #endregion

    [StorableConstructor]
    protected ParticleSwarmOptimization(bool deserializing)
      : base(deserializing) {
    }
    protected ParticleSwarmOptimization(ParticleSwarmOptimization original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    public ParticleSwarmOptimization()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("SwarmSize", "Size of the particle swarm.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaxIterations", "Maximal number of iterations.", new IntValue(1000)));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze each generation.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<DoubleValue>("Omega", "Weight for particle's velocity vector.", new DoubleValue(-0.2)));
      Parameters.Add(new ValueParameter<DoubleValue>("Phi_P", "Weight for particle's personal best position.", new DoubleValue(-0.01)));
      Parameters.Add(new ValueParameter<DoubleValue>("Phi_G", "Weight for global best position.", new DoubleValue(3.7)));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("VelocityBounds", "Maximum Velocity in every dimension", new DoubleMatrix(new double[,] { { -1, 1 } })));
      Parameters.Add(new ConstrainedValueParameter<IParticleUpdater>("ParticleUpdater", "Operator that calculates new position and velocity of a particle"));
      Parameters.Add(new OptionalConstrainedValueParameter<ITopologyInitializer>("TopologyInitializer", "Creates neighborhood description vectors"));
      Parameters.Add(new OptionalConstrainedValueParameter<ITopologyUpdater>("TopologyUpdater", "Updates the neighborhood description vectors"));
      Parameters.Add(new OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>("OmegaUpdater", "Updates the omega parameter"));
      Parameters.Add(new OptionalConstrainedValueParameter<IDiscreteDoubleMatrixModifier>("VelocityBoundsUpdater", "Adjusts the velocity bounds."));
      ParticleUpdaterParameter.ActualValue = ParticleUpdaterParameter.ValidValues.SingleOrDefault(v => v.GetType() == typeof(TotallyConnectedParticleUpdater));

      RandomCreator randomCreator = new RandomCreator();
      VariableCreator variableCreator = new VariableCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      CombinedOperator particleCreator = new CombinedOperator();
      Placeholder evaluatorPlaceholder = new Placeholder();
      Assigner bestPersonalQualityAssigner = new Assigner();
      BestPointInitializer bestPositionInitializer = new BestPointInitializer();
      Placeholder topologyInitializerPlaceholder = new Placeholder();
      NeighborUpdater neighborUpdater = new NeighborUpdater();
      Placeholder analyzerPlaceholder = new Placeholder();
      UniformSubScopesProcessor uniformSubScopeProcessor = new UniformSubScopesProcessor();
      Placeholder particleUpdaterPlaceholder = new Placeholder();
      Placeholder topologyUpdaterPlaceholder = new Placeholder();
      UniformSubScopesProcessor uniformSubscopesProcessor2 = new UniformSubScopesProcessor();
      UniformSubScopesProcessor evaluationProcessor = new UniformSubScopesProcessor();
      NeighborUpdater neighborUpdater2 = new NeighborUpdater();
      Placeholder evaluatorPlaceholder2 = new Placeholder();
      SwarmUpdater swarmUpdater = new SwarmUpdater();
      Placeholder analyzerPlaceholder2 = new Placeholder();
      IntCounter currentIterationCounter = new IntCounter();
      Comparator currentIterationComparator = new Comparator();
      ConditionalBranch conditionalBranch = new ConditionalBranch();
      Placeholder velocityBoundsUpdaterPlaceholder = new Placeholder();
      Placeholder omegaUpdaterPlaceholder = new Placeholder();

      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.SeedParameter.Value = null;
      randomCreator.Successor = variableCreator;

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("CurrentIteration", new IntValue(0)));
      variableCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = "SwarmSize";
      solutionsCreator.EvaluatorParameter.Value = evaluatorPlaceholder;
      solutionsCreator.SolutionCreatorParameter.Value = particleCreator;
      solutionsCreator.Successor = bestPositionInitializer;

      InitializeParticleCreator(particleCreator);

      evaluatorPlaceholder.Name = "(Evaluator)";
      evaluatorPlaceholder.OperatorParameter.ActualName = "Evaluator";
      evaluatorPlaceholder.Successor = bestPersonalQualityAssigner;

      bestPersonalQualityAssigner.LeftSideParameter.ActualName = "PersonalBestQuality";
      bestPersonalQualityAssigner.RightSideParameter.ActualName = "Quality";

      bestPositionInitializer.Successor = topologyInitializerPlaceholder;

      topologyInitializerPlaceholder.Name = "(TopologyInitializer)";
      topologyInitializerPlaceholder.OperatorParameter.ActualName = "TopologyInitializer";
      topologyInitializerPlaceholder.Successor = neighborUpdater;

      neighborUpdater.Successor = analyzerPlaceholder;

      analyzerPlaceholder.Name = "(Analyzer)";
      analyzerPlaceholder.OperatorParameter.ActualName = "Analyzer";
      analyzerPlaceholder.Successor = uniformSubScopeProcessor;

      uniformSubScopeProcessor.Operator = particleUpdaterPlaceholder;
      uniformSubScopeProcessor.Successor = evaluationProcessor;

      particleUpdaterPlaceholder.Name = "(ParticleUpdater)";
      particleUpdaterPlaceholder.OperatorParameter.ActualName = "ParticleUpdater";

      evaluationProcessor.Parallel = new BoolValue(true);
      evaluationProcessor.Operator = evaluatorPlaceholder2;
      evaluationProcessor.Successor = topologyUpdaterPlaceholder;

      evaluatorPlaceholder2.Name = "(Evaluator)";
      evaluatorPlaceholder2.OperatorParameter.ActualName = "Evaluator";

      topologyUpdaterPlaceholder.Name = "(TopologyUpdater)";
      topologyUpdaterPlaceholder.OperatorParameter.ActualName = "TopologyUpdater";
      topologyUpdaterPlaceholder.Successor = neighborUpdater2;

      neighborUpdater2.Successor = uniformSubscopesProcessor2;

      uniformSubscopesProcessor2.Operator = swarmUpdater;
      uniformSubscopesProcessor2.Successor = analyzerPlaceholder2;

      analyzerPlaceholder2.Name = "(Analyzer)";
      analyzerPlaceholder2.OperatorParameter.ActualName = "Analyzer";
      analyzerPlaceholder2.Successor = currentIterationCounter;

      currentIterationCounter.Name = "CurrentIteration++";
      currentIterationCounter.ValueParameter.ActualName = "CurrentIteration";
      currentIterationCounter.Successor = omegaUpdaterPlaceholder;

      omegaUpdaterPlaceholder.Name = "(Omega Updater)";
      omegaUpdaterPlaceholder.OperatorParameter.ActualName = "OmegaUpdater";
      omegaUpdaterPlaceholder.Successor = velocityBoundsUpdaterPlaceholder;

      velocityBoundsUpdaterPlaceholder.Name = "(Velocity Bounds Updater)";
      velocityBoundsUpdaterPlaceholder.OperatorParameter.ActualName = "VelocityBoundsUpdater";
      velocityBoundsUpdaterPlaceholder.Successor = currentIterationComparator;

      currentIterationComparator.LeftSideParameter.ActualName = "CurrentIteration";
      currentIterationComparator.Comparison = new Comparison(ComparisonType.Less);
      currentIterationComparator.RightSideParameter.ActualName = "MaxIterations";
      currentIterationComparator.ResultParameter.ActualName = "ContinueIteration";
      currentIterationComparator.Successor = conditionalBranch;

      conditionalBranch.Name = "ContinueIteration?";
      conditionalBranch.ConditionParameter.ActualName = "ContinueIteration";
      conditionalBranch.TrueBranch = uniformSubScopeProcessor;

      InitializeAnalyzers();
      InitVelocityBoundsUpdater();
      UpdateAnalyzers();
      UpdateOmegaUpdater();
      InitOmegaUpdater();
      UpdateTopologyInitializer();
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParticleSwarmOptimization(this, cloner);
    }

    public override void Prepare() {
      if (Problem != null) {
        base.Prepare();
        if (OmegaUpdater != null && OmegaUpdater.StartValueParameter.Value != null) {
          this.OmegaParameter.ActualValue = new DoubleValue(OmegaUpdaterParameter.Value.StartValueParameter.Value.Value);
        }
        if (VelocityBoundsUpdater != null && VelocityBoundsUpdater.StartValueParameter.Value != null && VelocityBoundsParameter.Value != null) {
          DoubleMatrix matrix = VelocityBoundsParameter.Value;
          for (int i = 0; i < matrix.Rows; i++) {
            matrix[i, 0] = -VelocityBoundsUpdater.StartValueParameter.Value.Value;
            matrix[i, 1] = VelocityBoundsUpdater.StartValueParameter.Value.Value;
          }
        }
      }
    }

    #region Events
    protected override void OnProblemChanged() {
      UpdateAnalyzers();
      ParameterizeAnalyzers();
      base.OnProblemChanged();
    }

    void TopologyInitializerParameter_ValueChanged(object sender, EventArgs e) {
      this.UpdateTopologyParameters();
    }

    void VelocityBoundsUpdaterParameter_ValueChanged(object sender, EventArgs e) {
      if (VelocityBoundsParameter.Value != null) {
        foreach (IDiscreteDoubleMatrixModifier matrixOp in VelocityBoundsUpdaterParameter.Value.ScalingOperatorParameter.ValidValues) {
          matrixOp.ValueParameter.ActualName = VelocityBoundsUpdater.ScaleParameter.Name;
          matrixOp.StartValueParameter.Value = new DoubleValue(VelocityBoundsUpdater.ScaleParameter.ActualValue.Value);
        }
      }
    }
    #endregion

    #region Helpers
    private void Initialize() {
      TopologyInitializerParameter.ValueChanged += new EventHandler(TopologyInitializerParameter_ValueChanged);
    }

    private static void InitializeParticleCreator(CombinedOperator particleCreator) {
      Placeholder positionCreator = new Placeholder();
      Assigner personalBestPositionAssigner = new Assigner();
      UniformRandomRealVectorCreator velocityCreator = new UniformRandomRealVectorCreator();

      particleCreator.Name = "Particle Creator";
      particleCreator.OperatorGraph.InitialOperator = positionCreator;

      positionCreator.Name = "(SolutionCreator)";
      positionCreator.OperatorParameter.ActualName = "SolutionCreator";
      positionCreator.Successor = personalBestPositionAssigner;

      personalBestPositionAssigner.LeftSideParameter.ActualName = "PersonalBestPoint";
      personalBestPositionAssigner.RightSideParameter.ActualName = "Point";
      personalBestPositionAssigner.Successor = velocityCreator;

      velocityCreator.LengthParameter.ActualName = "ProblemSize";
      velocityCreator.BoundsParameter.ActualName = "VelocityBounds";
      velocityCreator.RealVectorParameter.ActualName = "Velocity";
    }

    private void InitializeAnalyzers() {
      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      ParameterizeAnalyzers();
    }

    private void ParameterizeAnalyzers() {
      if (Problem != null) {
        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
      }
    }

    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>())
          Analyzer.Operators.Add(analyzer);
      }
      Analyzer.Operators.Add(qualityAnalyzer);
    }

    private void InitVelocityBoundsUpdater() {
      foreach (IDiscreteDoubleMatrixModifier matrixOp in ApplicationManager.Manager.GetInstances<IDiscreteDoubleMatrixModifier>()) {
        VelocityBoundsUpdaterParameter.ValidValues.Add(matrixOp);
        matrixOp.ValueParameter.ActualName = VelocityBoundsParameter.Name;
        matrixOp.EndIndexParameter.ActualName = MaxIterationsParameter.Name;
        matrixOp.StartIndexParameter.Value = new IntValue(0);
        matrixOp.IndexParameter.ActualName = "CurrentIteration";
        matrixOp.EndValueParameter.Value = new DoubleValue(0);
      }
      VelocityBoundsUpdaterParameter.ValueChanged += new EventHandler(VelocityBoundsUpdaterParameter_ValueChanged);
    }

    private void InitOmegaUpdater() {
      foreach (IDiscreteDoubleValueModifier updater in OmegaUpdaterParameter.ValidValues) {
        updater.EndIndexParameter.ActualName = MaxIterationsParameter.Name;
        updater.StartIndexParameter.Value = new IntValue(0);
        updater.IndexParameter.ActualName = "CurrentIteration";
        updater.ValueParameter.ActualName = OmegaParameter.Name;
        updater.StartValueParameter.Value = new DoubleValue(1);
        updater.EndValueParameter.Value = new DoubleValue(0);
      }
    }

    private void UpdateOmegaUpdater() {
      IDiscreteDoubleValueModifier oldOmegaUpdater = OmegaUpdater;
      OmegaUpdaterParameter.ValidValues.Clear();
      foreach (IDiscreteDoubleValueModifier updater in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>().OrderBy(x => x.Name)) {
        OmegaUpdaterParameter.ValidValues.Add(updater);
      }
      if (oldOmegaUpdater != null) {
        IDiscreteDoubleValueModifier updater = OmegaUpdaterParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldOmegaUpdater.GetType());
        if (updater != null) OmegaUpdaterParameter.Value = updater;
      }
    }

    private void UpdateTopologyInitializer() {
      ITopologyInitializer oldTopologyInitializer = TopologyInitializer;
      TopologyInitializerParameter.ValidValues.Clear();
      foreach (ITopologyInitializer topologyInitializer in ApplicationManager.Manager.GetInstances<ITopologyInitializer>().OrderBy(x => x.Name)) {
        TopologyInitializerParameter.ValidValues.Add(topologyInitializer);
      }
      if (oldTopologyInitializer != null && TopologyInitializerParameter.ValidValues.Any(x => x.GetType() == oldTopologyInitializer.GetType()))
        TopologyInitializer = TopologyInitializerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldTopologyInitializer.GetType());
      UpdateTopologyParameters();
    }

    private void UpdateTopologyParameters() {
      ITopologyUpdater oldTopologyUpdater = TopologyUpdater;
      IParticleUpdater oldParticleUpdater = ParticleUpdater;
      ClearTopologyParameters();
      if (TopologyInitializer != null) {
        foreach (ITopologyUpdater topologyUpdater in ApplicationManager.Manager.GetInstances<ITopologyUpdater>())
          TopologyUpdaterParameter.ValidValues.Add(topologyUpdater);
        foreach (IParticleUpdater particleUpdater in ApplicationManager.Manager.GetInstances<ILocalParticleUpdater>())
          ParticleUpdaterParameter.ValidValues.Add(particleUpdater);
      } else {
        foreach (IParticleUpdater particleUpdater in ApplicationManager.Manager.GetInstances<IGlobalParticleUpdater>())
          ParticleUpdaterParameter.ValidValues.Add(particleUpdater);
      }
      if (oldTopologyUpdater != null) {
        ITopologyUpdater newTopologyUpdater = TopologyUpdaterParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldParticleUpdater.GetType());
        if (newTopologyUpdater != null) TopologyUpdater = newTopologyUpdater;
      }
      if (oldParticleUpdater != null) {
        IParticleUpdater newParticleUpdater = ParticleUpdaterParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldParticleUpdater.GetType());
        if (newParticleUpdater != null) ParticleUpdater = newParticleUpdater;
      }
    }

    private void ClearTopologyParameters() {
      TopologyUpdaterParameter.ValidValues.Clear();
      ParticleUpdaterParameter.ValidValues.Clear();
    }
    #endregion

  }
}
