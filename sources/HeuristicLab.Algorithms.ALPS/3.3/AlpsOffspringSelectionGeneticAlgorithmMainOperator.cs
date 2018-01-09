#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.ALPS {
  /// <summary>
  /// An operator which represents the main loop of an offspring selection genetic algorithm.
  /// </summary>
  [Item("AlpsOffspringSelectionGeneticAlgorithmMainOperator", "An operator that represents the core of an alps offspring selection genetic algorithm.")]
  [StorableClass]
  public sealed class AlpsOffspringSelectionGeneticAlgorithmMainOperator : AlgorithmOperator {
    #region Parameter properties
    public IValueLookupParameter<IRandom> RandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public ILookupParameter<IntValue> PopulationSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["PopulationSize"]; }
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

    public ILookupParameter<DoubleValue> ComparisonFactorParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }
    public ILookupParameter<DoubleValue> CurrentSuccessRatioParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["CurrentSuccessRatio"]; }
    }
    public IValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public ILookupParameter<DoubleValue> SelectionPressureParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["SelectionPressure"]; }
    }
    public IValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    public IValueLookupParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
    }
    public IValueLookupParameter<BoolValue> FillPopulationWithParentsParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["FillPopulationWithParents"]; }
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
    private AlpsOffspringSelectionGeneticAlgorithmMainOperator(bool deserializing) : base(deserializing) { }
    private AlpsOffspringSelectionGeneticAlgorithmMainOperator(AlpsOffspringSelectionGeneticAlgorithmMainOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlpsOffspringSelectionGeneticAlgorithmMainOperator(this, cloner);
    }
    public AlpsOffspringSelectionGeneticAlgorithmMainOperator()
      : base() {
      Initialize();
    }

    private void Initialize() {
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated solutions."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));

      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population of solutions in each layer."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));

      Parameters.Add(new LookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1]."));
      Parameters.Add(new LookupParameter<DoubleValue>("CurrentSuccessRatio", "The current success ratio."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new LookupParameter<DoubleValue>("SelectionPressure", "The actual selection pressure."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("FillPopulationWithParents", "True if the population should be filled with parent individual or false if worse children should be used when the maximum selection pressure is exceeded."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Age", "The age of individuals."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AgeInheritance", "A weight that determines the age of a child after crossover based on the older (1.0) and younger (0.0) parent."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AgeIncrement", "The value the age the individuals is incremented if they survives a generation."));


      var selector = new Placeholder();
      var subScopesProcessor1 = new SubScopesProcessor();
      var childrenCreator = new ChildrenCreator();
      var osBeforeMutationBranch = new ConditionalBranch();
      var uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      var crossover1 = new Placeholder();
      var uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      var evaluator1 = new Placeholder();
      var subScopesCounter1 = new SubScopesCounter();
      var qualityComparer1 = new WeightedParentsQualityComparator();
      var ageCalculator1 = new WeightingReducer() { Name = "Calculate Age" };
      var subScopesRemover1 = new SubScopesRemover();
      var uniformSubScopesProcessor3 = new UniformSubScopesProcessor();
      var mutationBranch1 = new StochasticBranch();
      var mutator1 = new Placeholder();
      var variableCreator1 = new VariableCreator();
      var variableCreator2 = new VariableCreator();
      var conditionalSelector = new ConditionalSelector();
      var subScopesProcessor2 = new SubScopesProcessor();
      var uniformSubScopesProcessor4 = new UniformSubScopesProcessor();
      var evaluator2 = new Placeholder();
      var subScopesCounter2 = new SubScopesCounter();
      var mergingReducer1 = new MergingReducer();
      var uniformSubScopesProcessor5 = new UniformSubScopesProcessor();
      var crossover2 = new Placeholder();
      var mutationBranch2 = new StochasticBranch();
      var mutator2 = new Placeholder();
      var uniformSubScopesProcessor6 = new UniformSubScopesProcessor();
      var evaluator3 = new Placeholder();
      var subScopesCounter3 = new SubScopesCounter();
      var qualityComparer2 = new WeightedParentsQualityComparator();
      var ageCalculator2 = new WeightingReducer() { Name = "Calculate Age" };
      var subScopesRemover2 = new SubScopesRemover();
      var offspringSelector = new AlpsOffspringSelector();
      var subScopesProcessor3 = new SubScopesProcessor();
      var bestSelector = new BestSelector();
      var worstSelector = new WorstSelector();
      var rightReducer = new RightReducer();
      var leftReducer = new LeftReducer();
      var mergingReducer2 = new MergingReducer();
      var reevaluateElitesBranch = new ConditionalBranch();
      var uniformSubScopesProcessor7 = new UniformSubScopesProcessor();
      var evaluator4 = new Placeholder();
      var subScopesCounter4 = new SubScopesCounter();
      var incrementAgeProcessor = new UniformSubScopesProcessor();
      var ageIncrementor = new DoubleCounter() { Name = "Increment Age" };


      OperatorGraph.InitialOperator = selector;

      selector.Name = "Selector (placeholder)";
      selector.OperatorParameter.ActualName = SelectorParameter.Name;
      selector.Successor = subScopesProcessor1;

      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = offspringSelector;

      childrenCreator.ParentsPerChild = new IntValue(2);
      childrenCreator.Successor = osBeforeMutationBranch;

      osBeforeMutationBranch.Name = "Apply OS before mutation?";
      osBeforeMutationBranch.ConditionParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      osBeforeMutationBranch.TrueBranch = uniformSubScopesProcessor1;
      osBeforeMutationBranch.FalseBranch = uniformSubScopesProcessor5;
      osBeforeMutationBranch.Successor = null;

      uniformSubScopesProcessor1.Operator = crossover1;
      uniformSubScopesProcessor1.Successor = uniformSubScopesProcessor2;

      crossover1.Name = "Crossover (placeholder)";
      crossover1.OperatorParameter.ActualName = CrossoverParameter.Name;
      crossover1.Successor = null;

      uniformSubScopesProcessor2.Parallel.Value = true;
      uniformSubScopesProcessor2.Operator = evaluator1;
      uniformSubScopesProcessor2.Successor = subScopesCounter1;

      evaluator1.Name = "Evaluator (placeholder)";
      evaluator1.OperatorParameter.ActualName = EvaluatorParameter.Name;
      evaluator1.Successor = qualityComparer1;

      subScopesCounter1.Name = "Increment EvaluatedSolutions";
      subScopesCounter1.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;
      subScopesCounter1.Successor = uniformSubScopesProcessor3;

      uniformSubScopesProcessor3.Operator = mutationBranch1;
      uniformSubScopesProcessor3.Successor = conditionalSelector;

      qualityComparer1.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      qualityComparer1.LeftSideParameter.ActualName = QualityParameter.Name;
      qualityComparer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      qualityComparer1.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparer1.ResultParameter.ActualName = "SuccessfulOffspring";
      qualityComparer1.Successor = ageCalculator1;

      ageCalculator1.ParameterToReduce.ActualName = AgeParameter.Name;
      ageCalculator1.TargetParameter.ActualName = AgeParameter.Name;
      ageCalculator1.WeightParameter.ActualName = AgeInheritanceParameter.Name;
      ageCalculator1.Successor = subScopesRemover1;

      subScopesRemover1.RemoveAllSubScopes = true;
      subScopesRemover1.Successor = null;

      mutationBranch1.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mutationBranch1.RandomParameter.ActualName = RandomParameter.Name;
      mutationBranch1.FirstBranch = mutator1;
      mutationBranch1.SecondBranch = variableCreator2;
      mutationBranch1.Successor = null;

      mutator1.Name = "Mutator (placeholder)";
      mutator1.OperatorParameter.ActualName = MutatorParameter.Name;
      mutator1.Successor = variableCreator1;

      variableCreator1.Name = "MutatedOffspring = true";
      variableCreator1.CollectedValues.Add(new ValueParameter<BoolValue>("MutatedOffspring", null, new BoolValue(true), false));
      variableCreator1.Successor = null;

      variableCreator2.Name = "MutatedOffspring = false";
      variableCreator2.CollectedValues.Add(new ValueParameter<BoolValue>("MutatedOffspring", null, new BoolValue(false), false));
      variableCreator2.Successor = null;

      conditionalSelector.ConditionParameter.ActualName = "MutatedOffspring";
      conditionalSelector.ConditionParameter.Depth = 1;
      conditionalSelector.CopySelected.Value = false;
      conditionalSelector.Successor = subScopesProcessor2;

      subScopesProcessor2.Operators.Add(new EmptyOperator());
      subScopesProcessor2.Operators.Add(uniformSubScopesProcessor4);
      subScopesProcessor2.Successor = mergingReducer1;

      mergingReducer1.Successor = null;

      uniformSubScopesProcessor4.Parallel.Value = true;
      uniformSubScopesProcessor4.Operator = evaluator2;
      uniformSubScopesProcessor4.Successor = subScopesCounter2;

      evaluator2.Name = "Evaluator (placeholder)";
      evaluator2.OperatorParameter.ActualName = EvaluatorParameter.Name;
      evaluator2.Successor = null;

      subScopesCounter2.Name = "Increment EvaluatedSolutions";
      subScopesCounter2.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;
      subScopesCounter2.Successor = null;

      uniformSubScopesProcessor5.Operator = crossover2;
      uniformSubScopesProcessor5.Successor = uniformSubScopesProcessor6;

      crossover2.Name = "Crossover (placeholder)";
      crossover2.OperatorParameter.ActualName = CrossoverParameter.Name;
      crossover2.Successor = mutationBranch2;

      mutationBranch2.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mutationBranch2.RandomParameter.ActualName = RandomParameter.Name;
      mutationBranch2.FirstBranch = mutator2;
      mutationBranch2.SecondBranch = null;
      mutationBranch2.Successor = null;

      mutator2.Name = "Mutator (placeholder)";
      mutator2.OperatorParameter.ActualName = MutatorParameter.Name;
      mutator2.Successor = null;

      uniformSubScopesProcessor6.Parallel.Value = true;
      uniformSubScopesProcessor6.Operator = evaluator3;
      uniformSubScopesProcessor6.Successor = subScopesCounter3;

      evaluator3.Name = "Evaluator (placeholder)";
      evaluator3.OperatorParameter.ActualName = EvaluatorParameter.Name;
      evaluator3.Successor = qualityComparer2;

      subScopesCounter3.Name = "Increment EvaluatedSolutions";
      subScopesCounter3.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      qualityComparer2.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      qualityComparer2.LeftSideParameter.ActualName = QualityParameter.Name;
      qualityComparer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      qualityComparer2.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparer2.ResultParameter.ActualName = "SuccessfulOffspring";
      qualityComparer2.Successor = ageCalculator2;

      ageCalculator2.ParameterToReduce.ActualName = AgeParameter.Name;
      ageCalculator2.TargetParameter.ActualName = AgeParameter.Name;
      ageCalculator2.WeightParameter.ActualName = AgeInheritanceParameter.Name;
      ageCalculator2.Successor = subScopesRemover2;

      subScopesRemover2.RemoveAllSubScopes = true;
      subScopesRemover2.Successor = null;

      subScopesCounter3.Successor = null;

      offspringSelector.CurrentSuccessRatioParameter.ActualName = CurrentSuccessRatioParameter.Name;
      offspringSelector.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      offspringSelector.SelectionPressureParameter.ActualName = SelectionPressureParameter.Name;
      offspringSelector.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      offspringSelector.OffspringPopulationParameter.ActualName = "OffspringPopulation";
      offspringSelector.OffspringPopulationWinnersParameter.ActualName = "OffspringPopulationWinners";
      offspringSelector.SuccessfulOffspringParameter.ActualName = "SuccessfulOffspring";
      offspringSelector.FillPopulationWithParentsParameter.ActualName = FillPopulationWithParentsParameter.Name;
      offspringSelector.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
      offspringSelector.OffspringCreator = selector;
      offspringSelector.Successor = subScopesProcessor3;

      subScopesProcessor3.Operators.Add(bestSelector);
      subScopesProcessor3.Operators.Add(worstSelector);
      subScopesProcessor3.Successor = mergingReducer2;

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = ElitesParameter.Name;
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;
      bestSelector.Successor = rightReducer;

      rightReducer.Successor = reevaluateElitesBranch;

      reevaluateElitesBranch.ConditionParameter.ActualName = "ReevaluateElites";
      reevaluateElitesBranch.Name = "Reevaluate elites ?";
      reevaluateElitesBranch.TrueBranch = uniformSubScopesProcessor7;
      reevaluateElitesBranch.FalseBranch = null;
      reevaluateElitesBranch.Successor = null;

      uniformSubScopesProcessor7.Parallel.Value = true;
      uniformSubScopesProcessor7.Operator = evaluator4;
      uniformSubScopesProcessor7.Successor = subScopesCounter4;

      evaluator4.Name = "Evaluator (placeholder)";
      evaluator4.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter4.Name = "Increment EvaluatedSolutions";
      subScopesCounter4.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;
      subScopesCounter4.Successor = null;

      worstSelector.CopySelected = new BoolValue(false);
      worstSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      worstSelector.NumberOfSelectedSubScopesParameter.ActualName = ElitesParameter.Name;
      worstSelector.QualityParameter.ActualName = QualityParameter.Name;
      worstSelector.Successor = leftReducer;

      leftReducer.Successor = null;

      mergingReducer2.Successor = incrementAgeProcessor;

      incrementAgeProcessor.Operator = ageIncrementor;
      incrementAgeProcessor.Successor = null;

      ageIncrementor.ValueParameter.ActualName = AgeParameter.Name;
      ageIncrementor.IncrementParameter.Value = null;
      ageIncrementor.IncrementParameter.ActualName = AgeIncrementParameter.Name;
      ageIncrementor.Successor = null;
    }

    public override IOperation Apply() {
      if (CrossoverParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
