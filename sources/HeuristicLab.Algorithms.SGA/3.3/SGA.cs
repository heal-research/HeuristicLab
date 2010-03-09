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
    //#region Private Members
    //// store operator references in order to be able to access them easily after deserialization
    //[Storable]
    //private PopulationCreator populationCreator;
    //[Storable]
    //private SGAOperator sgaOperator;
    //#endregion

    //#region Problem Properties
    //public override Type ProblemType {
    //  get { return typeof(ISingleObjectiveProblem); }
    //}
    //public new ISingleObjectiveProblem Problem {
    //  get { return (ISingleObjectiveProblem)base.Problem; }
    //  set { base.Problem = value; }
    //}
    //#endregion

    //#region Parameter Properties
    //private ValueParameter<IntData> SeedParameter {
    //  get { return (ValueParameter<IntData>)Parameters["Seed"]; }
    //}
    //private ValueParameter<BoolData> SetSeedRandomlyParameter {
    //  get { return (ValueParameter<BoolData>)Parameters["SetSeedRandomly"]; }
    //}
    //private ValueParameter<IntData> PopulationSizeParameter {
    //  get { return (ValueParameter<IntData>)Parameters["PopulationSize"]; }
    //}
    //private ConstrainedValueParameter<ISelector> SelectorParameter {
    //  get { return (ConstrainedValueParameter<ISelector>)Parameters["Selector"]; }
    //}
    //private ConstrainedValueParameter<ICrossover> CrossoverParameter {
    //  get { return (ConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    //}
    //private ValueParameter<DoubleData> MutationProbabilityParameter {
    //  get { return (ValueParameter<DoubleData>)Parameters["MutationProbability"]; }
    //}
    //private OptionalConstrainedValueParameter<IManipulator> MutatorParameter {
    //  get { return (OptionalConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    //}
    //private ValueParameter<IntData> ElitesParameter {
    //  get { return (ValueParameter<IntData>)Parameters["Elites"]; }
    //}
    //private ValueParameter<IntData> MaximumGenerationsParameter {
    //  get { return (ValueParameter<IntData>)Parameters["MaximumGenerations"]; }
    //}
    //#endregion

    //#region Properties
    //public IntData Seed {
    //  get { return SeedParameter.Value; }
    //  set { SeedParameter.Value = value; }
    //}
    //public BoolData SetSeedRandomly {
    //  get { return SetSeedRandomlyParameter.Value; }
    //  set { SetSeedRandomlyParameter.Value = value; }
    //}
    //public IntData PopulationSize {
    //  get { return PopulationSizeParameter.Value; }
    //  set { PopulationSizeParameter.Value = value; }
    //}
    //public ISelector Selector {
    //  get { return SelectorParameter.Value; }
    //  set { SelectorParameter.Value = value; }
    //}
    //public ICrossover Crossover {
    //  get { return CrossoverParameter.Value; }
    //  set { CrossoverParameter.Value = value; }
    //}
    //public DoubleData MutationProbability {
    //  get { return MutationProbabilityParameter.Value; }
    //  set { MutationProbabilityParameter.Value = value; }
    //}
    //public IManipulator Mutator {
    //  get { return MutatorParameter.Value; }
    //  set { MutatorParameter.Value = value; }
    //}
    //public IntData Elites {
    //  get { return ElitesParameter.Value; }
    //  set { ElitesParameter.Value = value; }
    //}
    //public IntData MaximumGenerations {
    //  get { return MaximumGenerationsParameter.Value; }
    //  set { MaximumGenerationsParameter.Value = value; }
    //}
    //#endregion

    //#region Persistence Properties
    //[Storable]
    //private object RestoreEvents {
    //  get { return null; }
    //  set { RegisterEvents(); }
    //}
    //#endregion

    //public SGA()
    //  : base() {
    //  Parameters.Add(new ValueParameter<IntData>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntData(0)));
    //  Parameters.Add(new ValueParameter<BoolData>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolData(true)));
    //  Parameters.Add(new ValueParameter<IntData>("PopulationSize", "The size of the population of solutions.", new IntData(100)));
    //  Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
    //  Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
    //  Parameters.Add(new ValueParameter<DoubleData>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new DoubleData(0.05)));
    //  Parameters.Add(new OptionalConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
    //  Parameters.Add(new ValueParameter<IntData>("Elites", "The numer of elite solutions which are kept in each generation.", new IntData(1)));
    //  Parameters.Add(new ValueParameter<IntData>("MaximumGenerations", "The maximum number of generations which should be processed.", new IntData(1000)));

    //  RandomCreator randomCreator = new RandomCreator();
    //  populationCreator = new PopulationCreator();
    //  sgaOperator = new SGAOperator();

    //  randomCreator.RandomParameter.ActualName = "Random";
    //  randomCreator.SeedParameter.ActualName = "Seed";
    //  randomCreator.SeedParameter.Value = null;
    //  randomCreator.SetSeedRandomlyParameter.ActualName = "SetSeedRandomly";
    //  randomCreator.SetSeedRandomlyParameter.Value = null;
    //  randomCreator.Successor = populationCreator;

    //  populationCreator.PopulationSizeParameter.ActualName = "PopulationSize";
    //  populationCreator.PopulationSizeParameter.Value = null;
    //  populationCreator.Successor = sgaOperator;

    //  sgaOperator.SelectorParameter.ActualName = "Selector";
    //  sgaOperator.CrossoverParameter.ActualName = "Crossover";
    //  sgaOperator.ElitesParameter.ActualName = "Elites";
    //  sgaOperator.MaximumGenerationsParameter.ActualName = "MaximumGenerations";
    //  sgaOperator.MutatorParameter.ActualName = "Mutator";
    //  sgaOperator.MutationProbabilityParameter.ActualName = "MutationProbability";
    //  sgaOperator.RandomParameter.ActualName = "Random";
    //  sgaOperator.ResultsParameter.ActualName = "Results";

    //  OperatorGraph.InitialOperator = randomCreator;

    //  if (ApplicationManager.Manager != null) {
    //    var selectors = ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name);
    //    foreach (ISelector selector in selectors)
    //      SelectorParameter.ValidValues.Add(selector);
    //    ParameterizeSelectors();
    //  }

    //  RegisterEvents();
    //}

    //public override IDeepCloneable Clone(Cloner cloner) {
    //  SGA clone = (SGA)base.Clone(cloner);
    //  clone.populationCreator = (PopulationCreator)cloner.Clone(populationCreator);
    //  clone.sgaOperator = (SGAOperator)cloner.Clone(sgaOperator);
    //  clone.RegisterEvents();
    //  return clone;
    //}

    //#region Events
    //protected override void OnProblemChanged() {
    //  if (Problem.SolutionCreator is IStochasticOperator) ((IStochasticOperator)Problem.SolutionCreator).RandomParameter.ActualName = "Random";
    //  if (Problem.Evaluator is IStochasticOperator) ((IStochasticOperator)Problem.Evaluator).RandomParameter.ActualName = "Random";
    //  foreach (IStochasticOperator op in Problem.Operators.OfType<IStochasticOperator>())
    //    op.RandomParameter.ActualName = "Random";

    //  populationCreator.SolutionCreatorParameter.Value = Problem.SolutionCreator;
    //  populationCreator.EvaluatorParameter.Value = Problem.Evaluator;
    //  sgaOperator.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
    //  sgaOperator.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    //  sgaOperator.EvaluatorParameter.Value = Problem.Evaluator;

    //  foreach (ISingleObjectiveSelector op in SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>()) {
    //    op.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
    //    op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    //  }

    //  ICrossover oldCrossover = CrossoverParameter.Value;
    //  CrossoverParameter.ValidValues.Clear();
    //  foreach (ICrossover crossover in Problem.Operators.OfType<ICrossover>().OrderBy(x => x.Name))
    //    CrossoverParameter.ValidValues.Add(crossover);
    //  if (oldCrossover != null) {
    //    CrossoverParameter.Value = CrossoverParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldCrossover.GetType());
    //  }

    //  IManipulator oldMutator = MutatorParameter.Value;
    //  MutatorParameter.ValidValues.Clear();
    //  foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name))
    //    MutatorParameter.ValidValues.Add(mutator);
    //  if (oldMutator != null) {
    //    MutatorParameter.Value = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
    //  }

    //  base.OnProblemChanged();
    //}
    //protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
    //  if (Problem.SolutionCreator is IStochasticOperator) ((IStochasticOperator)Problem.SolutionCreator).RandomParameter.ActualName = "Random";
    //  populationCreator.SolutionCreatorParameter.Value = Problem.SolutionCreator;
    //  base.Problem_SolutionCreatorChanged(sender, e);
    //}
    //protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
    //  if (Problem.Evaluator is IStochasticOperator) ((IStochasticOperator)Problem.Evaluator).RandomParameter.ActualName = "Random";

    //  foreach (ISingleObjectiveSelector op in SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>()) {
    //    op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    //  }

    //  populationCreator.EvaluatorParameter.Value = Problem.Evaluator;
    //  sgaOperator.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    //  sgaOperator.EvaluatorParameter.Value = Problem.Evaluator;
    //  base.Problem_EvaluatorChanged(sender, e);
    //}
    //private void ElitesParameter_ValueChanged(object sender, EventArgs e) {
    //  foreach (ISelector selector in SelectorParameter.ValidValues)
    //    selector.NumberOfSelectedSubScopesParameter.Value = new IntData(2 * (PopulationSizeParameter.Value.Value - ElitesParameter.Value.Value));
    //}
    //private void PopulationSizeParameter_ValueChanged(object sender, EventArgs e) {
    //  foreach (ISelector selector in SelectorParameter.ValidValues)
    //    selector.NumberOfSelectedSubScopesParameter.Value = new IntData(2 * (PopulationSizeParameter.Value.Value - ElitesParameter.Value.Value));
    //}
    //#endregion

    //#region Helpers
    //private void RegisterEvents() {
    //  PopulationSizeParameter.ValueChanged += new EventHandler(PopulationSizeParameter_ValueChanged);
    //  ElitesParameter.ValueChanged += new EventHandler(ElitesParameter_ValueChanged);
    //}
    //private void ParameterizeSelectors() {
    //  foreach (ISelector selector in SelectorParameter.ValidValues) {
    //    selector.CopySelected = new BoolData(true);
    //    selector.NumberOfSelectedSubScopesParameter.Value = new IntData(2 * (PopulationSizeParameter.Value.Value - ElitesParameter.Value.Value));
    //    if (selector is IStochasticOperator) ((IStochasticOperator)selector).RandomParameter.ActualName = "Random";
    //  }
    //}
    //private void ParameterizePopulationCreator() {
    //  populationCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    //  populationCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
    //}
    //private void ParameterizeSGAOperator() {
    //  sgaOperator.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
    //  sgaOperator.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    //  sgaOperator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
    //}
    //private void ParameterizeSolutionCreator() {
    //}
    //#endregion
  }
}
