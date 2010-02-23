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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Evolutionary;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;

namespace HeuristicLab.SGA {
  /// <summary>
  /// A standard genetic algorithm.
  /// </summary>
  [Item("SGA", "A standard genetic algorithm.")]
  [Creatable("Algorithms")]
  public sealed class SGA : EngineAlgorithm {
    [Storable]
    private PopulationCreator populationCreator;
    [Storable]
    private SGAOperator sgaOperator;

    public override Type ProblemType {
      get { return typeof(ISingleObjectiveProblem); }
    }
    public new ISingleObjectiveProblem Problem {
      get { return (ISingleObjectiveProblem)base.Problem; }
      set { base.Problem = value; }
    }

    public new IScope GlobalScope {
      get { return base.GlobalScope; }
    }

    public SGA()
      : base() {
      Parameters.Add(new ValueParameter<IntData>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntData(0)));
      Parameters.Add(new ValueParameter<BoolData>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolData(true)));
      Parameters.Add(new ValueParameter<IntData>("PopulationSize", "The size of the population of solutions.", new IntData(100)));
      Parameters.Add(new OperatorParameter("CrossoverOperator", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<DoubleData>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new DoubleData(0.05)));
      Parameters.Add(new OperatorParameter("MutationOperator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueParameter<IntData>("Elites", "The numer of elite solutions which are kept in each generation.", new IntData(1)));
      Parameters.Add(new ValueParameter<IntData>("MaximumGenerations", "The maximum number of generations which should be processed.", new IntData(1000)));

      RandomCreator randomCreator = new RandomCreator();
      populationCreator = new PopulationCreator();
      sgaOperator = new SGAOperator();

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = "Seed";
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = "SetSeedRandomly";
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = populationCreator;

      populationCreator.PopulationSizeParameter.ActualName = "PopulationSize";
      populationCreator.PopulationSizeParameter.Value = null;
      populationCreator.Successor = sgaOperator;

      sgaOperator.CrossoverOperatorParameter.ActualName = "CrossoverOperator";
      sgaOperator.ElitesParameter.ActualName = "Elites";
      sgaOperator.MaximumGenerationsParameter.ActualName = "MaximumGenerations";
      sgaOperator.MutationOperatorParameter.ActualName = "MutationOperator";
      sgaOperator.MutationProbabilityParameter.ActualName = "MutationProbability";
      sgaOperator.RandomParameter.ActualName = "Random";

      OperatorGraph.InitialOperator = randomCreator;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SGA clone = (SGA)base.Clone(cloner);
      clone.populationCreator = (PopulationCreator)cloner.Clone(populationCreator);
      clone.sgaOperator = (SGAOperator)cloner.Clone(sgaOperator);
      return clone;
    }

    protected override void DeregisterProblemEvents() {
      Problem.MaximizationChanged -= new EventHandler(Problem_MaximizationChanged);
      base.DeregisterProblemEvents();
    }
    protected override void RegisterProblemEvents() {
      base.RegisterProblemEvents();
      Problem.MaximizationChanged += new EventHandler(Problem_MaximizationChanged);
    }

    protected override void OnProblemChanged() {
      Problem.SolutionCreator.RandomParameter.ActualName = "Random";
      populationCreator.SolutionCreatorParameter.Value = Problem.SolutionCreator;
      populationCreator.SolutionEvaluatorParameter.Value = Problem.Evaluator;
      sgaOperator.MaximizationParameter.Value = Problem.Maximization;
      sgaOperator.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.Name;
      sgaOperator.SolutionEvaluatorParameter.Value = Problem.Evaluator;
      base.OnProblemChanged();
    }
    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      Problem.SolutionCreator.RandomParameter.ActualName = "Random";
      populationCreator.SolutionCreatorParameter.Value = Problem.SolutionCreator;
      base.Problem_SolutionCreatorChanged(sender, e);
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      populationCreator.SolutionEvaluatorParameter.Value = Problem.Evaluator;
      sgaOperator.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.Name;
      sgaOperator.SolutionEvaluatorParameter.Value = Problem.Evaluator;
      base.Problem_EvaluatorChanged(sender, e);
    }
    private void Problem_MaximizationChanged(object sender, EventArgs e) {
      sgaOperator.MaximizationParameter.Value = Problem.Maximization;
    }
  }
}
