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

using HeuristicLab.Analysis;
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
    private OffspringSelectionGeneticAlgorithmMainOperator(bool deserializing) : base() { }
    public OffspringSelectionGeneticAlgorithmMainOperator()
      : base() {
      Initialize();
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
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
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
      UniformSubScopesProcessor uniformSubScopesProcessor = new UniformSubScopesProcessor();
      Placeholder crossover = new Placeholder();
      ConditionalBranch osBeforeMutationBranch = new ConditionalBranch();
      Placeholder evaluator1 = new Placeholder();
      IntCounter evaluationCounter1 = new IntCounter();
      WeightedParentsQualityComparator qualityComparer1 = new WeightedParentsQualityComparator();
      StochasticBranch mutationBranch1 = new StochasticBranch();
      Placeholder mutator1 = new Placeholder();
      Placeholder evaluator2 = new Placeholder();
      IntCounter evaluationCounter2 = new IntCounter();
      StochasticBranch mutationBranch2 = new StochasticBranch();
      Placeholder mutator2 = new Placeholder();
      Placeholder evaluator3 = new Placeholder();
      IntCounter evaluationCounter3 = new IntCounter();
      WeightedParentsQualityComparator qualityComparer2 = new WeightedParentsQualityComparator();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      OffspringSelector offspringSelector = new OffspringSelector();
      SubScopesProcessor subScopesProcessor2 = new SubScopesProcessor();
      BestSelector bestSelector = new BestSelector();
      WorstSelector worstSelector = new WorstSelector();
      RightReducer rightReducer = new RightReducer();
      LeftReducer leftReducer = new LeftReducer();
      MergingReducer mergingReducer = new MergingReducer();
      
      selector.Name = "Selector (placeholder)";
      selector.OperatorParameter.ActualName = SelectorParameter.Name;

      childrenCreator.ParentsPerChild = new IntValue(2);

      crossover.Name = "Crossover (placeholder)";
      crossover.OperatorParameter.ActualName = CrossoverParameter.Name;

      osBeforeMutationBranch.Name = "Apply OS before mutation?";
      osBeforeMutationBranch.ConditionParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;

      evaluator1.Name = "Evaluator (placeholder)";
      evaluator1.OperatorParameter.ActualName = EvaluatorParameter.Name;

      evaluationCounter1.Name = "EvaluatedSolutions++";
      evaluationCounter1.Increment = new IntValue(1);
      evaluationCounter1.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      qualityComparer1.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      qualityComparer1.LeftSideParameter.ActualName = QualityParameter.Name;
      qualityComparer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      qualityComparer1.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparer1.ResultParameter.ActualName = "SuccessfulOffspring";

      mutationBranch1.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mutationBranch1.RandomParameter.ActualName = RandomParameter.Name;

      mutator1.Name = "Mutator (placeholder)";
      mutator1.OperatorParameter.ActualName = MutatorParameter.Name;

      evaluator2.Name = "Evaluator (placeholder)";
      evaluator2.OperatorParameter.ActualName = EvaluatorParameter.Name;

      evaluationCounter2.Name = "EvaluatedSolutions++";
      evaluationCounter2.Increment = new IntValue(1);
      evaluationCounter2.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      mutationBranch2.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mutationBranch2.RandomParameter.ActualName = RandomParameter.Name;

      mutator2.Name = "Mutator (placeholder)";
      mutator2.OperatorParameter.ActualName = MutatorParameter.Name;

      evaluator3.Name = "Evaluator (placeholder)";
      evaluator3.OperatorParameter.ActualName = EvaluatorParameter.Name;

      evaluationCounter3.Name = "EvaluatedSolutions++";
      evaluationCounter3.Increment = new IntValue(1);
      evaluationCounter3.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      qualityComparer2.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      qualityComparer2.LeftSideParameter.ActualName = QualityParameter.Name;
      qualityComparer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      qualityComparer2.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparer2.ResultParameter.ActualName = "SuccessfulOffspring";

      subScopesRemover.RemoveAllSubScopes = true;

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
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = selector;
      selector.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = offspringSelector;
      childrenCreator.Successor = uniformSubScopesProcessor;
      uniformSubScopesProcessor.Operator = crossover;
      uniformSubScopesProcessor.Successor = null;
      crossover.Successor = osBeforeMutationBranch;
      osBeforeMutationBranch.TrueBranch = evaluator1;
      osBeforeMutationBranch.FalseBranch = mutationBranch2;
      osBeforeMutationBranch.Successor = subScopesRemover;
      evaluator1.Successor = evaluationCounter1;
      evaluationCounter1.Successor = qualityComparer1;
      qualityComparer1.Successor = mutationBranch1;
      mutationBranch1.FirstBranch = mutator1;
      mutationBranch1.SecondBranch = null;
      mutationBranch1.Successor = null;
      mutator1.Successor = evaluator2;
      evaluator2.Successor = evaluationCounter2;
      evaluationCounter2.Successor = null;
      mutationBranch2.FirstBranch = mutator2;
      mutationBranch2.SecondBranch = null;
      mutationBranch2.Successor = evaluator3;
      mutator2.Successor = null;
      evaluator3.Successor = evaluationCounter3;
      evaluationCounter3.Successor = qualityComparer2;
      subScopesRemover.Successor = null;
      offspringSelector.OffspringCreator = selector;
      offspringSelector.Successor = subScopesProcessor2;
      subScopesProcessor2.Operators.Add(bestSelector);
      subScopesProcessor2.Operators.Add(worstSelector);
      subScopesProcessor2.Successor = mergingReducer;
      bestSelector.Successor = rightReducer;
      rightReducer.Successor = null;
      worstSelector.Successor = leftReducer;
      leftReducer.Successor = null;
      mergingReducer.Successor = null;
      #endregion
    }

    public override IOperation Apply() {
      if (CrossoverParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
