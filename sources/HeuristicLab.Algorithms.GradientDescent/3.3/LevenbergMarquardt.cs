#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.TestFunctions;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.GradientDescent {
  /// <summary>
  /// Linear regression data analysis algorithm.
  /// </summary>
  [Item("Levenberg Marquardt", "Levenberg Marquardt real vector optimization algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class LevenbergMarquardt : EngineAlgorithm, IStorableContent {
    public string Filename { get; set; }

    public override Type ProblemType {
      get { return typeof(SingleObjectiveTestFunctionProblem); }
    }
    public new SingleObjectiveTestFunctionProblem Problem {
      get { return (SingleObjectiveTestFunctionProblem)base.Problem; }
      set { base.Problem = value; }
    }

    private ValueParameter<IntValue> SeedParameter {
      get { return (ValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private ValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (ValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }

    public IntValue Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }
    public BoolValue SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
    }

    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)RandomCreator.Successor; }
    }

    #region storing and cloning
    [StorableConstructor]
    private LevenbergMarquardt(bool deserializing) : base(deserializing) { }
    private LevenbergMarquardt(LevenbergMarquardt original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LevenbergMarquardt(this, cloner);
    }
    #endregion

    public LevenbergMarquardt()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));


      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      UniformSubScopesProcessor subscopesProcessor = new UniformSubScopesProcessor();
      LevenbergMarquardtMove moveCreator = new LevenbergMarquardtMove();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutionsParameter.Value = new IntValue(1);
      solutionsCreator.Successor = subscopesProcessor;

      subscopesProcessor.Operator = moveCreator;
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    protected override void Problem_Reset(object sender, EventArgs e) {
      base.Problem_Reset(sender, e);
    }

    protected override void OnProblemChanged() {
      Problem.Reset += new EventHandler(Problem_Reset);
      base.OnProblemChanged();
    }

    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      SolutionsCreator.NumberOfSolutionsParameter.Value = new IntValue(1);
      base.Problem_SolutionCreatorChanged(sender, e);
    }


    private void Initialize() {

    }
  }
}
