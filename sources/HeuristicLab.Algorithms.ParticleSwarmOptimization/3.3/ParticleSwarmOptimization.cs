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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

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
    #endregion

    #region Properties
    [StorableAttribute]
    private ParticleSwarmOptimizationMainLoop mainLoop; // Check this !
    private ParticleSwarmOptimizationMainLoop MainLoop {
      get { return mainLoop; }
    }
    #endregion

    public ParticleSwarmOptimization()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("SwarmSize", "Size of the particle swarm.", new IntValue(1)));
      Parameters.Add(new ValueParameter<IntValue>("MaxIterations", "Maximal number of iterations.", new IntValue(1000)));
      Parameters.Add(new OptionalConstrainedValueParameter<IRealVectorEncoder>("Encoder", "The operator used to encode solutions as position vector."));
      //Parameters.Add(new ConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));

      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      //solutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreator.Name;
      mainLoop = new ParticleSwarmOptimizationMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = SwarmSizeParameter.Name;
      solutionsCreator.Successor = mainLoop;

      mainLoop.EncoderParameter.ActualName = EncoderParameter.Name;

      Initialize();
    }

    private void Initialize() {
      
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
      //ParameterizeStochasticOperator(Problem.SolutionCreator);
      //ParameterizeStochasticOperator(Problem.Evaluator);
      //ParameterizeStochasticOperator(Problem.Visualizer);
      //foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      //ParameterizeSolutionsCreator();
      //ParameterizeMainLoop();
      UpdateEncoders();
      //UpdateMutators();
      //Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.OnProblemChanged();
    }
    #endregion

    #region Helpers

    private void UpdateEncoders() {
      IRealVectorEncoder oldEncoder = EncoderParameter.Value;
      EncoderParameter.ValidValues.Clear();
      foreach (IRealVectorEncoder encoder in Problem.Operators.OfType<IRealVectorEncoder>().OrderBy(x => x.Name)) {
        EncoderParameter.ValidValues.Add(encoder);
        encoder.RealVectorParameter.ActualName = "Position"; 
      }
      if (oldEncoder != null) {
        IRealVectorEncoder encoder = EncoderParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldEncoder.GetType());
        if (encoder != null) EncoderParameter.Value = encoder;
      }
    }
    #endregion
  }
}
