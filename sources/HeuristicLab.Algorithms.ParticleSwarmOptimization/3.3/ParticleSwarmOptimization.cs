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

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("Particle Swarm Optimization", "A particle swarm optimization algorithm based on the description in Pedersen, M.E.H. (2010). PhD thesis. University of Southampton.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class ParticleSwarmOptimization : HeuristicOptimizationEngineAlgorithm, IStorableContent {
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
    public IValueParameter<DoubleValue> InertiaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Inertia"]; }
    }
    public IValueParameter<DoubleValue> PersonalBestAttractionParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["PersonalBestAttraction"]; }
    }
    public IValueParameter<DoubleValue> NeighborBestAttractionParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["NeighborBestAttraction"]; }
    }
    public IValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (IValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    public ConstrainedValueParameter<IParticleCreator> ParticleCreatorParameter {
      get { return (ConstrainedValueParameter<IParticleCreator>)Parameters["ParticleCreator"]; }
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
    public OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier> InertiaUpdaterParameter {
      get { return (OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters["InertiaUpdater"]; }
    }
    public ConstrainedValueParameter<ISwarmUpdater> SwarmUpdaterParameter {
      get { return (ConstrainedValueParameter<ISwarmUpdater>)Parameters["SwarmUpdater"]; }

    }
    #endregion

    #region Properties

    public string Filename { get; set; }

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;

    [Storable]
    private SolutionsCreator solutionsCreator;

    [Storable]
    private ParticleSwarmOptimizationMainLoop mainLoop;

    public override Type ProblemType {
      get { return typeof(ISingleObjectiveHeuristicOptimizationProblem); }
    }
    public new ISingleObjectiveHeuristicOptimizationProblem Problem {
      get { return (ISingleObjectiveHeuristicOptimizationProblem)base.Problem; }
      set { base.Problem = value; }
    }
    public IntValue Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }
    public BoolValue SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
    }
    public IntValue SwarmSize {
      get { return SwarmSizeParameter.Value; }
      set { SwarmSizeParameter.Value = value; }
    }
    public IntValue MaxIterations {
      get { return MaxIterationsParameter.Value; }
      set { MaxIterationsParameter.Value = value; }
    }
    public DoubleValue Inertia {
      get { return InertiaParameter.Value; }
      set { InertiaParameter.Value = value; }
    }
    public DoubleValue PersonalBestAttraction {
      get { return PersonalBestAttractionParameter.Value; }
      set { PersonalBestAttractionParameter.Value = value; }
    }
    public DoubleValue NeighborBestAttraction {
      get { return NeighborBestAttractionParameter.Value; }
      set { NeighborBestAttractionParameter.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public IParticleCreator ParticleCreator {
      get { return ParticleCreatorParameter.Value; }
      set { ParticleCreatorParameter.Value = value; }
    }
    public IParticleUpdater ParticleUpdater {
      get { return ParticleUpdaterParameter.Value; }
      set { ParticleUpdaterParameter.Value = value; }
    }
    public ITopologyInitializer TopologyInitializer {
      get { return TopologyInitializerParameter.Value; }
      set { TopologyInitializerParameter.Value = value; }
    }
    public ITopologyUpdater TopologyUpdater {
      get { return TopologyUpdaterParameter.Value; }
      set { TopologyUpdaterParameter.Value = value; }
    }
    public IDiscreteDoubleValueModifier InertiaUpdater {
      get { return InertiaUpdaterParameter.Value; }
      set { InertiaUpdaterParameter.Value = value; }
    }
    public ISwarmUpdater SwarmUpdater {
      get { return SwarmUpdaterParameter.Value; }
      set { SwarmUpdaterParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private ParticleSwarmOptimization(bool deserializing) : base(deserializing) { }
    private ParticleSwarmOptimization(ParticleSwarmOptimization original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      solutionsCreator = cloner.Clone(original.solutionsCreator);
      mainLoop = cloner.Clone(original.mainLoop);
      Initialize();
    }
    public ParticleSwarmOptimization()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("SwarmSize", "Size of the particle swarm.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaxIterations", "Maximal number of iterations.", new IntValue(1000)));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze each generation.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<DoubleValue>("Inertia", "Inertia weight on a particle's movement (omega).", new DoubleValue(1)));
      Parameters.Add(new ValueParameter<DoubleValue>("PersonalBestAttraction", "Weight for particle's pull towards its personal best soution (phi_p).", new DoubleValue(-0.01)));
      Parameters.Add(new ValueParameter<DoubleValue>("NeighborBestAttraction", "Weight for pull towards the neighborhood best solution or global best solution in case of a totally connected topology (phi_g).", new DoubleValue(3.7)));
      Parameters.Add(new ConstrainedValueParameter<IParticleCreator>("ParticleCreator", "Operator that creates a new particle."));
      Parameters.Add(new ConstrainedValueParameter<IParticleUpdater>("ParticleUpdater", "Operator that updates a particle."));
      Parameters.Add(new OptionalConstrainedValueParameter<ITopologyInitializer>("TopologyInitializer", "Creates neighborhood description vectors."));
      Parameters.Add(new OptionalConstrainedValueParameter<ITopologyUpdater>("TopologyUpdater", "Updates the neighborhood description vectors."));
      Parameters.Add(new OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>("InertiaUpdater", "Updates the omega parameter."));
      Parameters.Add(new ConstrainedValueParameter<ISwarmUpdater>("SwarmUpdater", "Encoding-specific parameter which is provided by the problem. May provide additional encoding-specific parameters, such as velocity bounds for real valued problems"));
      ParticleUpdaterParameter.Hidden = true;

      RandomCreator randomCreator = new RandomCreator();
      VariableCreator variableCreator = new VariableCreator();
      Assigner currentInertiaAssigner = new Assigner();
      solutionsCreator = new SolutionsCreator();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      Placeholder topologyInitializerPlaceholder = new Placeholder();
      mainLoop = new ParticleSwarmOptimizationMainLoop();

      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.SeedParameter.Value = null;
      randomCreator.Successor = variableCreator;

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.Successor = currentInertiaAssigner;

      currentInertiaAssigner.Name = "CurrentInertia := Inertia";
      currentInertiaAssigner.LeftSideParameter.ActualName = "CurrentInertia";
      currentInertiaAssigner.RightSideParameter.ActualName = "Inertia";
      currentInertiaAssigner.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = "SwarmSize";
      ParameterizeSolutionsCreator();
      solutionsCreator.Successor = subScopesCounter;

      subScopesCounter.Name = "Initialize EvaluatedSolutions";
      subScopesCounter.ValueParameter.ActualName = "EvaluatedSolutions";
      subScopesCounter.Successor = topologyInitializerPlaceholder;

      topologyInitializerPlaceholder.Name = "(TopologyInitializer)";
      topologyInitializerPlaceholder.OperatorParameter.ActualName = "TopologyInitializer";
      topologyInitializerPlaceholder.Successor = mainLoop;

      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      mainLoop.InertiaParameter.ActualName = "CurrentInertia";
      mainLoop.MaxIterationsParameter.ActualName = MaxIterationsParameter.Name;
      mainLoop.NeighborBestAttractionParameter.ActualName = NeighborBestAttractionParameter.Name;
      mainLoop.InertiaUpdaterParameter.ActualName = InertiaUpdaterParameter.Name;
      mainLoop.ParticleUpdaterParameter.ActualName = ParticleUpdaterParameter.Name;
      mainLoop.PersonalBestAttractionParameter.ActualName = PersonalBestAttractionParameter.Name;
      mainLoop.RandomParameter.ActualName = randomCreator.RandomParameter.ActualName;
      mainLoop.SwarmSizeParameter.ActualName = SwarmSizeParameter.Name;
      mainLoop.TopologyUpdaterParameter.ActualName = TopologyUpdaterParameter.Name;
      mainLoop.RandomParameter.ActualName = randomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";

      InitializeAnalyzers();
      InitializeParticleCreator();
      InitializeSwarmUpdater();
      ParameterizeSolutionsCreator();
      UpdateAnalyzers();
      UpdateInertiaUpdater();
      InitInertiaUpdater();
      UpdateTopologyInitializer();
      Initialize();
      ParameterizeMainLoop();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParticleSwarmOptimization(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    #region Events
    protected override void OnProblemChanged() {
      UpdateAnalyzers();
      ParameterizeAnalyzers();
      UpdateTopologyParameters();
      InitializeParticleCreator();
      InitializeSwarmUpdater();
      ParameterizeSolutionsCreator();
      base.OnProblemChanged();
    }

    void TopologyInitializerParameter_ValueChanged(object sender, EventArgs e) {
      this.UpdateTopologyParameters();
    }
    #endregion

    #region Helpers
    private void Initialize() {
      TopologyInitializerParameter.ValueChanged += new EventHandler(TopologyInitializerParameter_ValueChanged);
    }

    private void InitializeParticleCreator() {
      if (Problem != null) {
        IParticleCreator oldParticleCreator = ParticleCreator;
        IParticleCreator defaultParticleCreator = Problem.Operators.OfType<IParticleCreator>().FirstOrDefault();
        ParticleCreatorParameter.ValidValues.Clear();
        foreach (IParticleCreator Creator in Problem.Operators.OfType<IParticleCreator>().OrderBy(x => x.Name)) {
          ParticleCreatorParameter.ValidValues.Add(Creator);
        }
        if (oldParticleCreator != null) {
          IParticleCreator creator = ParticleCreatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldParticleCreator.GetType());
          if (creator != null) ParticleCreator = creator;
          else oldParticleCreator = null;
        }
        if (oldParticleCreator == null && defaultParticleCreator != null)
          ParticleCreator = defaultParticleCreator;
      }
    }

    private void InitializeAnalyzers() {
      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
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
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
      }
      Analyzer.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
    }

    private void InitInertiaUpdater() {
      foreach (IDiscreteDoubleValueModifier updater in InertiaUpdaterParameter.ValidValues) {
        updater.EndIndexParameter.ActualName = MaxIterationsParameter.Name;
        updater.EndIndexParameter.Hidden = true;
        updater.StartIndexParameter.Value = new IntValue(0);
        updater.StartIndexParameter.Hidden = true;
        updater.IndexParameter.ActualName = "Iterations";
        updater.ValueParameter.ActualName = "CurrentInertia";
        updater.StartValueParameter.Value = new DoubleValue(1);
        updater.EndValueParameter.Value = new DoubleValue(1E-10);
      }
    }

    private void UpdateInertiaUpdater() {
      IDiscreteDoubleValueModifier oldInertiaUpdater = InertiaUpdater;
      InertiaUpdaterParameter.ValidValues.Clear();
      foreach (IDiscreteDoubleValueModifier updater in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>().OrderBy(x => x.Name)) {
        InertiaUpdaterParameter.ValidValues.Add(updater);
      }
      if (oldInertiaUpdater != null) {
        IDiscreteDoubleValueModifier updater = InertiaUpdaterParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldInertiaUpdater.GetType());
        if (updater != null) InertiaUpdaterParameter.Value = updater;
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

    private void ParameterizeTopologyUpdaters() {
      foreach (var updater in TopologyUpdaterParameter.ValidValues) {
        var multiPsoUpdater = updater as MultiPSOTopologyUpdater;
        if (multiPsoUpdater != null) {
          multiPsoUpdater.CurrentIterationParameter.ActualName = "Iterations";
        }
      }
    }

    private void UpdateTopologyParameters() {
      ITopologyUpdater oldTopologyUpdater = TopologyUpdater;
      IParticleUpdater oldParticleUpdater = ParticleUpdater;
      ClearTopologyParameters();
      if (Problem != null) {
        IParticleUpdater defaultParticleUpdater = null;
        if (TopologyInitializer != null) {
          foreach (ITopologyUpdater topologyUpdater in ApplicationManager.Manager.GetInstances<ITopologyUpdater>())
            TopologyUpdaterParameter.ValidValues.Add(topologyUpdater);
          defaultParticleUpdater = Problem.Operators.OfType<ILocalParticleUpdater>().FirstOrDefault();
          foreach (IParticleUpdater particleUpdater in Problem.Operators.OfType<ILocalParticleUpdater>().OrderBy(x => x.Name))
            ParticleUpdaterParameter.ValidValues.Add(particleUpdater);
        } else {
          defaultParticleUpdater = Problem.Operators.OfType<IGlobalParticleUpdater>().FirstOrDefault();
          foreach (IParticleUpdater particleUpdater in Problem.Operators.OfType<IGlobalParticleUpdater>().OrderBy(x => x.Name))
            ParticleUpdaterParameter.ValidValues.Add(particleUpdater);
        }
        if (oldTopologyUpdater != null) {
          ITopologyUpdater newTopologyUpdater = TopologyUpdaterParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldParticleUpdater.GetType());
          if (newTopologyUpdater != null) TopologyUpdater = newTopologyUpdater;
        }
        if (oldParticleUpdater != null) {
          IParticleUpdater newParticleUpdater = ParticleUpdaterParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldParticleUpdater.GetType());
          if (newParticleUpdater != null) ParticleUpdater = newParticleUpdater;
          else oldParticleUpdater = null;
        }
        if (oldParticleUpdater == null && defaultParticleUpdater != null)
          ParticleUpdater = defaultParticleUpdater;

        ParameterizeTopologyUpdaters();
      }
    }

    private void ClearTopologyParameters() {
      TopologyUpdaterParameter.ValidValues.Clear();
      ParticleUpdaterParameter.ValidValues.Clear();
    }

    private void ParameterizeSolutionsCreator() {
      if (Problem != null) {
        solutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
        if (ParticleCreatorParameter.Value != null)
          solutionsCreator.SolutionCreatorParameter.ActualName = ParticleCreatorParameter.Name;
        else
          solutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
      }
    }

    private void ParameterizeMainLoop() {
      if (Problem != null) {
        mainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      }
    }

    private void InitializeSwarmUpdater() {
      if (Problem != null) {
        ISwarmUpdater updater = Problem.Operators.OfType<ISwarmUpdater>().FirstOrDefault();
        SwarmUpdaterParameter.ValidValues.Clear();
        if (updater != null) {
          SwarmUpdaterParameter.ValidValues.Add(updater);
          SwarmUpdaterParameter.Value = updater;
        }
      }
    }
    #endregion

  }
}
