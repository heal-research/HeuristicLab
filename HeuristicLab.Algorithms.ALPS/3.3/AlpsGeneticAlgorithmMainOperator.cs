#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.ALPS {
  [Item("AlpsGeneticAlgorithmMainOperator", "An operator that represents the core of an ALPS genetic algorithm.")]
  [StorableType("3F16B0C0-9054-4FE8-A6B1-2CCED065AC26")]
  public sealed class AlpsGeneticAlgorithmMainOperator : AlgorithmOperator {
    #region Parameter properties
    public IValueLookupParameter<IRandom> RandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public IValueLookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public IValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public IValueLookupParameter<IOperator> SelectorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Selector"]; }
    }
    public IValueLookupParameter<IOperator> CrossoverParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Crossover"]; }
    }
    public IValueLookupParameter<IOperator> MutatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Mutator"]; }
    }
    public IValueLookupParameter<PercentValue> MutationProbabilityParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    public IValueLookupParameter<IntValue> ElitesParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Elites"]; }
    }
    public IValueLookupParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ReevaluateElites"]; }
    }
    public IValueLookupParameter<BoolValue> PlusSelectionParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["PlusSelection"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> AgeParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Age"]; }
    }
    public IValueLookupParameter<DoubleValue> AgeInheritanceParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["AgeInheritance"]; }
    }
    public IValueLookupParameter<DoubleValue> AgeIncrementParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["AgeIncrement"]; }
    }
    #endregion

    [StorableConstructor]
    private AlpsGeneticAlgorithmMainOperator(StorableConstructorFlag _) : base(_) { }
    private AlpsGeneticAlgorithmMainOperator(AlpsGeneticAlgorithmMainOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlpsGeneticAlgorithmMainOperator(this, cloner);
    }
    public AlpsGeneticAlgorithmMainOperator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<IntValue>("EvaluatedSolutions", "The number of times solutions have been evaluated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));

      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population of solutions in each layer."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      Parameters.Add(new ValueLookupParameter<BoolValue>("PlusSelection", "Include the parents in the selection of the invividuals for the next generation."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Age", "The age of individuals."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AgeInheritance", "A weight that determines the age of a child after crossover based on the older (1.0) and younger (0.0) parent."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AgeIncrement", "The value the age the individuals is incremented if they survives a generation."));


      var numberOfSelectedParentsCalculator = new ExpressionCalculator() { Name = "NumberOfSelectedParents = 2 * (PopulationSize - (PlusSelection ? 0 : Elites))" };
      var selector = new Placeholder() { Name = "Selector (Placeholder)" };
      var subScopesProcessor1 = new SubScopesProcessor();
      var childrenCreator = new ChildrenCreator();
      var uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      var crossover = new Placeholder() { Name = "Crossover (Placeholder)" };
      var stochasticBranch = new StochasticBranch() { Name = "MutationProbability" };
      var mutator = new Placeholder() { Name = "Mutator (Placeholder)" };
      var ageCalculator = new WeightingReducer() { Name = "Calculate Age" };
      var subScopesRemover = new SubScopesRemover();
      var uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      var evaluator = new Placeholder() { Name = "Evaluator (Placeholder)" };
      var subScopesCounter = new SubScopesCounter() { Name = "Increment EvaluatedSolutions" };
      var replacementBranch = new ConditionalBranch() { Name = "PlusSelection?" };
      var replacementMergingReducer = new MergingReducer();
      var replacementBestSelector = new BestSelector();
      var replacementRightReducer = new RightReducer();
      var subScopesProcessor2 = new SubScopesProcessor();
      var bestSelector = new BestSelector();
      var rightReducer = new RightReducer();
      var mergingReducer = new MergingReducer();
      var reevaluateElitesBranch = new ConditionalBranch() { Name = "Reevaluate elites ?" };
      var incrementAgeProcessor = new UniformSubScopesProcessor();
      var ageIncrementor = new DoubleCounter() { Name = "Increment Age" };

      OperatorGraph.InitialOperator = numberOfSelectedParentsCalculator;

      numberOfSelectedParentsCalculator.CollectedValues.Add(new LookupParameter<IntValue>(PopulationSizeParameter.Name));
      numberOfSelectedParentsCalculator.CollectedValues.Add(new LookupParameter<IntValue>(ElitesParameter.Name));
      numberOfSelectedParentsCalculator.CollectedValues.Add(new LookupParameter<BoolValue>(PlusSelectionParameter.Name));
      numberOfSelectedParentsCalculator.ExpressionResultParameter.ActualName = "NumberOfSelectedSubScopes";
      numberOfSelectedParentsCalculator.ExpressionParameter.Value = new StringValue("PopulationSize 0 Elites PlusSelection if - 2 * toint");
      numberOfSelectedParentsCalculator.Successor = selector;

      selector.OperatorParameter.ActualName = SelectorParameter.Name;
      selector.Successor = subScopesProcessor1;

      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = replacementBranch;

      childrenCreator.ParentsPerChild = new IntValue(2);
      childrenCreator.Successor = uniformSubScopesProcessor1;

      uniformSubScopesProcessor1.Operator = crossover;
      uniformSubScopesProcessor1.Successor = uniformSubScopesProcessor2;

      crossover.OperatorParameter.ActualName = CrossoverParameter.Name;
      crossover.Successor = stochasticBranch;

      stochasticBranch.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      stochasticBranch.RandomParameter.ActualName = RandomParameter.Name;
      stochasticBranch.FirstBranch = mutator;
      stochasticBranch.SecondBranch = null;
      stochasticBranch.Successor = ageCalculator;

      mutator.OperatorParameter.ActualName = MutatorParameter.Name;
      mutator.Successor = null;

      ageCalculator.ParameterToReduce.ActualName = AgeParameter.Name;
      ageCalculator.TargetParameter.ActualName = AgeParameter.Name;
      ageCalculator.WeightParameter.ActualName = AgeInheritanceParameter.Name;
      ageCalculator.Successor = subScopesRemover;

      subScopesRemover.RemoveAllSubScopes = true;
      subScopesRemover.Successor = null;

      uniformSubScopesProcessor2.Parallel.Value = true;
      uniformSubScopesProcessor2.Operator = evaluator;
      uniformSubScopesProcessor2.Successor = subScopesCounter;

      evaluator.OperatorParameter.ActualName = EvaluatorParameter.Name;
      evaluator.Successor = null;

      subScopesCounter.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;
      subScopesCounter.AccumulateParameter.Value = new BoolValue(true);
      subScopesCounter.Successor = null;

      replacementBranch.ConditionParameter.ActualName = PlusSelectionParameter.Name;
      replacementBranch.TrueBranch = replacementMergingReducer;
      replacementBranch.FalseBranch = subScopesProcessor2;
      replacementBranch.Successor = incrementAgeProcessor;

      replacementMergingReducer.Successor = replacementBestSelector;

      replacementBestSelector.NumberOfSelectedSubScopesParameter.ActualName = PopulationSizeParameter.Name;
      replacementBestSelector.CopySelected = new BoolValue(false);
      replacementBestSelector.Successor = replacementRightReducer;

      replacementRightReducer.Successor = reevaluateElitesBranch;

      subScopesProcessor2.Operators.Add(bestSelector);
      subScopesProcessor2.Operators.Add(new EmptyOperator());
      subScopesProcessor2.Successor = mergingReducer;

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = ElitesParameter.Name;
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;
      bestSelector.Successor = rightReducer;

      rightReducer.Successor = reevaluateElitesBranch;

      mergingReducer.Successor = null;

      reevaluateElitesBranch.ConditionParameter.ActualName = ReevaluateElitesParameter.Name;
      reevaluateElitesBranch.TrueBranch = uniformSubScopesProcessor2;
      reevaluateElitesBranch.FalseBranch = null;
      reevaluateElitesBranch.Successor = null;


      incrementAgeProcessor.Operator = ageIncrementor;
      incrementAgeProcessor.Successor = null;

      ageIncrementor.ValueParameter.ActualName = AgeParameter.Name;
      ageIncrementor.IncrementParameter.Value = null;
      ageIncrementor.IncrementParameter.ActualName = AgeIncrementParameter.Name;
      ageIncrementor.Successor = null;
    }
  }
}