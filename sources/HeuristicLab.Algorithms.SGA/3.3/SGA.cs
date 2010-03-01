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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Evolutionary;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.SGA {
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

    private ValueParameter<IntData> PopulationSizeParameter {
      get { return (ValueParameter<IntData>)Parameters["PopulationSize"]; }
    }
    private ConstrainedValueParameter<ISelector> SelectorParameter {
      get { return (ConstrainedValueParameter<ISelector>)Parameters["Selector"]; }
    }
    private ConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (ConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }
    private ConstrainedValueParameter<IManipulator> MutatorParameter {
      get { return (ConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    }
    private ValueParameter<IntData> ElitesParameter {
      get { return (ValueParameter<IntData>)Parameters["Elites"]; }
    }

    public override Type ProblemType {
      get { return typeof(ISingleObjectiveProblem); }
    }
    public new ISingleObjectiveProblem Problem {
      get { return (ISingleObjectiveProblem)base.Problem; }
      set { base.Problem = value; }
    }

    public SGA()
      : base() {
      Parameters.Add(new ValueParameter<IntData>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntData(0)));
      Parameters.Add(new ValueParameter<BoolData>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolData(true)));
      Parameters.Add(new ValueParameter<IntData>("PopulationSize", "The size of the population of solutions.", new IntData(100)));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<DoubleData>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new DoubleData(0.05)));
      Parameters.Add(new ConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueParameter<IntData>("Elites", "The numer of elite solutions which are kept in each generation.", new IntData(1)));
      Parameters.Add(new ValueParameter<IntData>("MaximumGenerations", "The maximum number of generations which should be processed.", new IntData(1000)));

      PopulationSizeParameter.ValueChanged += new EventHandler(PopulationSizeParameter_ValueChanged);
      ElitesParameter.ValueChanged += new EventHandler(ElitesParameter_ValueChanged);

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

      sgaOperator.SelectorParameter.ActualName = "Selector";
      sgaOperator.CrossoverParameter.ActualName = "Crossover";
      sgaOperator.ElitesParameter.ActualName = "Elites";
      sgaOperator.MaximumGenerationsParameter.ActualName = "MaximumGenerations";
      sgaOperator.MutatorParameter.ActualName = "Mutator";
      sgaOperator.MutationProbabilityParameter.ActualName = "MutationProbability";
      sgaOperator.RandomParameter.ActualName = "Random";
      sgaOperator.ResultsParameter.ActualName = "Results";

      OperatorGraph.InitialOperator = randomCreator;

      var selectors = ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector));
      selectors.Select(x => x.CopySelected = new BoolData(true));
      selectors.Select(x => x.NumberOfSelectedSubScopesParameter.Value = new IntData(2 * (PopulationSizeParameter.Value.Value - ElitesParameter.Value.Value)));
      selectors.OfType<IStochasticOperator>().Select(x => x.RandomParameter.ActualName = "Random");
      foreach (ISelector selector in selectors)
        SelectorParameter.ValidValues.Add(selector);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SGA clone = (SGA)base.Clone(cloner);
      clone.populationCreator = (PopulationCreator)cloner.Clone(populationCreator);
      clone.sgaOperator = (SGAOperator)cloner.Clone(sgaOperator);
      return clone;
    }

    private void ElitesParameter_ValueChanged(object sender, EventArgs e) {
      SelectorParameter.ValidValues.Select(x => x.NumberOfSelectedSubScopesParameter.Value = new IntData(2 * (PopulationSizeParameter.Value.Value - ElitesParameter.Value.Value)));
    }
    private void PopulationSizeParameter_ValueChanged(object sender, EventArgs e) {
      SelectorParameter.ValidValues.Select(x => x.NumberOfSelectedSubScopesParameter.Value = new IntData(2 * (PopulationSizeParameter.Value.Value - ElitesParameter.Value.Value)));
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
      if (Problem.SolutionCreator is IStochasticOperator) ((IStochasticOperator)Problem.SolutionCreator).RandomParameter.ActualName = "Random";
      if (Problem.Evaluator is IStochasticOperator) ((IStochasticOperator)Problem.Evaluator).RandomParameter.ActualName = "Random";
      Problem.Operators.OfType<IStochasticOperator>().Select(x => x.RandomParameter.ActualName = "Random");

      populationCreator.SolutionCreatorParameter.Value = Problem.SolutionCreator;
      populationCreator.EvaluatorParameter.Value = Problem.Evaluator;
      sgaOperator.MaximizationParameter.Value = Problem.Maximization;
      sgaOperator.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      sgaOperator.EvaluatorParameter.Value = Problem.Evaluator;

      SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>().Select(x => x.MaximizationParameter.Value = Problem.Maximization);
      SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>().Select(x => x.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName);

      CrossoverParameter.ValidValues.Clear();
      foreach (ICrossover crossover in Problem.Operators.OfType<ICrossover>())
        CrossoverParameter.ValidValues.Add(crossover);

      MutatorParameter.ValidValues.Clear();
      foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>())
        MutatorParameter.ValidValues.Add(mutator);

      base.OnProblemChanged();
    }
    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      if (Problem.SolutionCreator is IStochasticOperator) ((IStochasticOperator)Problem.SolutionCreator).RandomParameter.ActualName = "Random";
      populationCreator.SolutionCreatorParameter.Value = Problem.SolutionCreator;
      base.Problem_SolutionCreatorChanged(sender, e);
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      if (Problem.Evaluator is IStochasticOperator) ((IStochasticOperator)Problem.Evaluator).RandomParameter.ActualName = "Random";
      SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>().Select(x => x.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName);
      populationCreator.EvaluatorParameter.Value = Problem.Evaluator;
      sgaOperator.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      sgaOperator.EvaluatorParameter.Value = Problem.Evaluator;
      base.Problem_EvaluatorChanged(sender, e);
    }
    private void Problem_MaximizationChanged(object sender, EventArgs e) {
      sgaOperator.MaximizationParameter.Value = Problem.Maximization;
      SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>().Select(x => x.MaximizationParameter.Value = Problem.Maximization);
    }
  }
}
