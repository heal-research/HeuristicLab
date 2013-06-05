#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// An operator which represents the main loop of an offspring selection genetic algorithm.
  /// </summary>
  [Item("OffspringSelectionGeneticAlgorithmMainOperator", "An operator that represents the core of an offspring selection genetic algorithm.")]
  [StorableClass]
  public sealed class OffspringSelectionGeneticAlgorithmMainOperator : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<IOperator> SelectorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Selector"]; }
    }
    public ValueLookupParameter<IOperator> CrossoverParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Crossover"]; }
    }
    public ValueLookupParameter<PercentValue> MutationProbabilityParameter {
      get { return (ValueLookupParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    public ValueLookupParameter<IOperator> MutatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Mutator"]; }
    }
    public ValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public LookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public ValueLookupParameter<IntValue> ElitesParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["Elites"]; }
    }
    public IValueLookupParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ReevaluateElites"]; }
    }
    public LookupParameter<DoubleValue> ComparisonFactorParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }
    public LookupParameter<DoubleValue> CurrentSuccessRatioParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["CurrentSuccessRatio"]; }
    }
    public ValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public LookupParameter<DoubleValue> SelectionPressureParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["SelectionPressure"]; }
    }
    public ValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    public ValueLookupParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
    }
    #endregion

    [StorableConstructor]
    private OffspringSelectionGeneticAlgorithmMainOperator(bool deserializing) : base(deserializing) { }
    private OffspringSelectionGeneticAlgorithmMainOperator(OffspringSelectionGeneticAlgorithmMainOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OffspringSelectionGeneticAlgorithmMainOperator(this, cloner);
    }
    public OffspringSelectionGeneticAlgorithmMainOperator()
      : base() {
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("ReevaluateElites")) {
        Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      }
      #endregion
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      Parameters.Add(new LookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1]."));
      Parameters.Add(new LookupParameter<DoubleValue>("CurrentSuccessRatio", "The current success ratio."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new LookupParameter<DoubleValue>("SelectionPressure", "The actual selection pressure."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      #endregion

      #region Create operators
      Placeholder selector = new Placeholder();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      ConditionalBranch osBeforeMutationBranch = new ConditionalBranch();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      Placeholder crossover1 = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      Placeholder evaluator1 = new Placeholder();
      SubScopesCounter subScopesCounter1 = new SubScopesCounter();
      WeightedParentsQualityComparator qualityComparer1 = new WeightedParentsQualityComparator();
      SubScopesRemover subScopesRemover1 = new SubScopesRemover();
      UniformSubScopesProcessor uniformSubScopesProcessor3 = new UniformSubScopesProcessor();
      StochasticBranch mutationBranch1 = new StochasticBranch();
      Placeholder mutator1 = new Placeholder();
      VariableCreator variableCreator1 = new VariableCreator();
      VariableCreator variableCreator2 = new VariableCreator();
      ConditionalSelector conditionalSelector = new ConditionalSelector();
      SubScopesProcessor subScopesProcessor2 = new SubScopesProcessor();
      UniformSubScopesProcessor uniformSubScopesProcessor4 = new UniformSubScopesProcessor();
      Placeholder evaluator2 = new Placeholder();
      SubScopesCounter subScopesCounter2 = new SubScopesCounter();
      MergingReducer mergingReducer1 = new MergingReducer();
      UniformSubScopesProcessor uniformSubScopesProcessor5 = new UniformSubScopesProcessor();
      Placeholder crossover2 = new Placeholder();
      StochasticBranch mutationBranch2 = new StochasticBranch();
      Placeholder mutator2 = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor6 = new UniformSubScopesProcessor();
      Placeholder evaluator3 = new Placeholder();
      SubScopesCounter subScopesCounter3 = new SubScopesCounter();
      WeightedParentsQualityComparator qualityComparer2 = new WeightedParentsQualityComparator();
      SubScopesRemover subScopesRemover2 = new SubScopesRemover();
      OffspringSelector offspringSelector = new OffspringSelector();
      SubScopesProcessor subScopesProcessor3 = new SubScopesProcessor();
      BestSelector bestSelector = new BestSelector();
      WorstSelector worstSelector = new WorstSelector();
      RightReducer rightReducer = new RightReducer();
      LeftReducer leftReducer = new LeftReducer();
      MergingReducer mergingReducer2 = new MergingReducer();
      ConditionalBranch reevaluateElitesBranch = new ConditionalBranch();
      UniformSubScopesProcessor uniformSubScopesProcessor7 = new UniformSubScopesProcessor();
      Placeholder evaluator4 = new Placeholder();
      SubScopesCounter subScopesCounter4 = new SubScopesCounter();

      selector.Name = "Selector (placeholder)";
      selector.OperatorParameter.ActualName = SelectorParameter.Name;

      childrenCreator.ParentsPerChild = new IntValue(2);

      osBeforeMutationBranch.Name = "Apply OS before mutation?";
      osBeforeMutationBranch.ConditionParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;

      crossover1.Name = "Crossover (placeholder)";
      crossover1.OperatorParameter.ActualName = CrossoverParameter.Name;

      uniformSubScopesProcessor2.Parallel.Value = true;

      evaluator1.Name = "Evaluator (placeholder)";
      evaluator1.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter1.Name = "Increment EvaluatedSolutions";
      subScopesCounter1.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      qualityComparer1.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      qualityComparer1.LeftSideParameter.ActualName = QualityParameter.Name;
      qualityComparer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      qualityComparer1.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparer1.ResultParameter.ActualName = "SuccessfulOffspring";

      subScopesRemover1.RemoveAllSubScopes = true;

      mutationBranch1.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mutationBranch1.RandomParameter.ActualName = RandomParameter.Name;

      mutator1.Name = "Mutator (placeholder)";
      mutator1.OperatorParameter.ActualName = MutatorParameter.Name;

      variableCreator1.Name = "MutatedOffspring = true";
      variableCreator1.CollectedValues.Add(new ValueParameter<BoolValue>("MutatedOffspring", null, new BoolValue(true), false));

      variableCreator2.Name = "MutatedOffspring = false";
      variableCreator2.CollectedValues.Add(new ValueParameter<BoolValue>("MutatedOffspring", null, new BoolValue(false), false));

      conditionalSelector.ConditionParameter.ActualName = "MutatedOffspring";
      conditionalSelector.ConditionParameter.Depth = 1;
      conditionalSelector.CopySelected.Value = false;

      uniformSubScopesProcessor4.Parallel.Value = true;

      evaluator2.Name = "Evaluator (placeholder)";
      evaluator2.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter2.Name = "Increment EvaluatedSolutions";
      subScopesCounter2.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      crossover2.Name = "Crossover (placeholder)";
      crossover2.OperatorParameter.ActualName = CrossoverParameter.Name;

      mutationBranch2.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mutationBranch2.RandomParameter.ActualName = RandomParameter.Name;

      mutator2.Name = "Mutator (placeholder)";
      mutator2.OperatorParameter.ActualName = MutatorParameter.Name;

      uniformSubScopesProcessor6.Parallel.Value = true;

      evaluator3.Name = "Evaluator (placeholder)";
      evaluator3.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter3.Name = "Increment EvaluatedSolutions";
      subScopesCounter3.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      qualityComparer2.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      qualityComparer2.LeftSideParameter.ActualName = QualityParameter.Name;
      qualityComparer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      qualityComparer2.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparer2.ResultParameter.ActualName = "SuccessfulOffspring";

      subScopesRemover2.RemoveAllSubScopes = true;

      offspringSelector.CurrentSuccessRatioParameter.ActualName = CurrentSuccessRatioParameter.Name;
      offspringSelector.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      offspringSelector.SelectionPressureParameter.ActualName = SelectionPressureParameter.Name;
      offspringSelector.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      offspringSelector.OffspringPopulationParameter.ActualName = "OffspringPopulation";
      offspringSelector.OffspringPopulationWinnersParameter.ActualName = "OffspringPopulationWinners";
      offspringSelector.SuccessfulOffspringParameter.ActualName = "SuccessfulOffspring";

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = ElitesParameter.Name;
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;

      worstSelector.CopySelected = new BoolValue(false);
      worstSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      worstSelector.NumberOfSelectedSubScopesParameter.ActualName = ElitesParameter.Name;
      worstSelector.QualityParameter.ActualName = QualityParameter.Name;

      reevaluateElitesBranch.ConditionParameter.ActualName = "ReevaluateElites";
      reevaluateElitesBranch.Name = "Reevaluate elites ?";

      uniformSubScopesProcessor7.Parallel.Value = true;

      evaluator4.Name = "Evaluator (placeholder)";
      evaluator4.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter4.Name = "Increment EvaluatedSolutions";
      subScopesCounter4.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = selector;
      selector.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = offspringSelector;
      childrenCreator.Successor = osBeforeMutationBranch;
      osBeforeMutationBranch.TrueBranch = uniformSubScopesProcessor1;
      osBeforeMutationBranch.FalseBranch = uniformSubScopesProcessor5;
      osBeforeMutationBranch.Successor = null;
      uniformSubScopesProcessor1.Operator = crossover1;
      uniformSubScopesProcessor1.Successor = uniformSubScopesProcessor2;
      crossover1.Successor = null;
      uniformSubScopesProcessor2.Operator = evaluator1;
      uniformSubScopesProcessor2.Successor = subScopesCounter1;
      evaluator1.Successor = qualityComparer1;
      qualityComparer1.Successor = subScopesRemover1;
      subScopesRemover1.Successor = null;
      subScopesCounter1.Successor = uniformSubScopesProcessor3;
      uniformSubScopesProcessor3.Operator = mutationBranch1;
      uniformSubScopesProcessor3.Successor = conditionalSelector;
      mutationBranch1.FirstBranch = mutator1;
      mutationBranch1.SecondBranch = variableCreator2;
      mutationBranch1.Successor = null;
      mutator1.Successor = variableCreator1;
      variableCreator1.Successor = null;
      variableCreator2.Successor = null;
      conditionalSelector.Successor = subScopesProcessor2;
      subScopesProcessor2.Operators.Add(new EmptyOperator());
      subScopesProcessor2.Operators.Add(uniformSubScopesProcessor4);
      subScopesProcessor2.Successor = mergingReducer1;
      uniformSubScopesProcessor4.Operator = evaluator2;
      uniformSubScopesProcessor4.Successor = subScopesCounter2;
      evaluator2.Successor = null;
      subScopesCounter2.Successor = null;
      mergingReducer1.Successor = null;
      uniformSubScopesProcessor5.Operator = crossover2;
      uniformSubScopesProcessor5.Successor = uniformSubScopesProcessor6;
      crossover2.Successor = mutationBranch2;
      mutationBranch2.FirstBranch = mutator2;
      mutationBranch2.SecondBranch = null;
      mutationBranch2.Successor = null;
      mutator2.Successor = null;
      uniformSubScopesProcessor6.Operator = evaluator3;
      uniformSubScopesProcessor6.Successor = subScopesCounter3;
      evaluator3.Successor = qualityComparer2;
      qualityComparer2.Successor = subScopesRemover2;
      subScopesRemover2.Successor = null;
      subScopesCounter3.Successor = null;
      offspringSelector.OffspringCreator = selector;
      offspringSelector.Successor = subScopesProcessor3;
      subScopesProcessor3.Operators.Add(bestSelector);
      subScopesProcessor3.Operators.Add(worstSelector);
      subScopesProcessor3.Successor = mergingReducer2;
      bestSelector.Successor = rightReducer;
      rightReducer.Successor = reevaluateElitesBranch;
      reevaluateElitesBranch.TrueBranch = uniformSubScopesProcessor7;
      uniformSubScopesProcessor7.Operator = evaluator4;
      uniformSubScopesProcessor7.Successor = subScopesCounter4;
      subScopesCounter4.Successor = null;
      reevaluateElitesBranch.FalseBranch = null;
      reevaluateElitesBranch.Successor = null;
      worstSelector.Successor = leftReducer;
      leftReducer.Successor = null;
      mergingReducer2.Successor = null;
      #endregion
    }

    public override IOperation Apply() {
      if (CrossoverParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
