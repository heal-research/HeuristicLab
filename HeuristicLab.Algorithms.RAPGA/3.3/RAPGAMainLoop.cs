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
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.RAPGA {
  /// <summary>
  /// An operator which represents the main loop of a relevant alleles preserving genetic algorithm.
  /// </summary>
  [Item("RAPGAMainLoop", "An operator which represents the main loop of a relevant alleles preserving genetic algorithm.")]
  [StorableClass]
  public sealed class RAPGAMainLoop : AlgorithmOperator {
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
    public ValueLookupParameter<IntValue> ElitesParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["Elites"]; }
    }
    public ValueLookupParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public ValueLookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public ValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public IValueLookupParameter<IntValue> MinimumPopulationSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MinimumPopulationSize"]; }
    }
    public IValueLookupParameter<IntValue> MaximumPopulationSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumPopulationSize"]; }
    }
    public IValueLookupParameter<DoubleValue> ComparisonFactorParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }
    public IValueLookupParameter<IntValue> EffortParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Effort"]; }
    }
    public IValueLookupParameter<IntValue> BatchSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["BatchSize"]; }
    }
    public IValueLookupParameter<ISolutionSimilarityCalculator> SimilarityCalculatorParameter {
      get { return (IValueLookupParameter<ISolutionSimilarityCalculator>)Parameters["SimilarityCalculator"]; }
    }
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private RAPGAMainLoop(bool deserializing) : base(deserializing) { }
    private RAPGAMainLoop(RAPGAMainLoop original, Cloner cloner) : base(original, cloner) { }
    public RAPGAMainLoop()
      : base() {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RAPGAMainLoop(this, cloner);
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
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze each generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("EvaluatedSolutions", "The number of times solutions have been evaluated."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MinimumPopulationSize", "The minimum size of the population of solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumPopulationSize", "The maximum size of the population of solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Effort", "The maximum number of offspring created in each generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("BatchSize", "The number of children that should be created during one iteration of the offspring creation process."));
      Parameters.Add(new ValueLookupParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The operator used to calculate the similarity between two solutions."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the genetic algorithm should be applied."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      Assigner assigner1 = new Assigner();
      ResultsCollector resultsCollector = new ResultsCollector();
      Placeholder analyzer1 = new Placeholder();
      Placeholder selector = new Placeholder();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor = new UniformSubScopesProcessor();
      Placeholder crossover = new Placeholder();
      StochasticBranch stochasticBranch = new StochasticBranch();
      Placeholder mutator = new Placeholder();
      Placeholder evaluator = new Placeholder();
      WeightedParentsQualityComparator weightedParentsQualityComparator = new WeightedParentsQualityComparator();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      IntCounter intCounter1 = new IntCounter();
      IntCounter intCounter2 = new IntCounter();
      ConditionalSelector conditionalSelector = new ConditionalSelector();
      RightReducer rightReducer1 = new RightReducer();
      DuplicatesSelector duplicateSelector = new DuplicatesSelector();
      LeftReducer leftReducer1 = new LeftReducer();
      ProgressiveOffspringPreserver progressiveOffspringSelector = new ProgressiveOffspringPreserver();
      SubScopesCounter subScopesCounter2 = new SubScopesCounter();
      ExpressionCalculator calculator1 = new ExpressionCalculator();
      ConditionalBranch conditionalBranch1 = new ConditionalBranch();
      Comparator comparator1 = new Comparator();
      ConditionalBranch conditionalBranch2 = new ConditionalBranch();
      LeftReducer leftReducer2 = new LeftReducer();
      SubScopesProcessor subScopesProcessor2 = new SubScopesProcessor();
      BestSelector bestSelector = new BestSelector();
      RightReducer rightReducer2 = new RightReducer();
      ScopeCleaner scopeCleaner = new ScopeCleaner();
      ScopeRestorer scopeRestorer = new ScopeRestorer();
      MergingReducer mergingReducer = new MergingReducer();
      IntCounter intCounter3 = new IntCounter();
      SubScopesCounter subScopesCounter3 = new SubScopesCounter();
      ExpressionCalculator calculator2 = new ExpressionCalculator();
      Comparator comparator2 = new Comparator();
      ConditionalBranch conditionalBranch3 = new ConditionalBranch();
      Placeholder analyzer2 = new Placeholder();
      Comparator comparator3 = new Comparator();
      ConditionalBranch conditionalBranch4 = new ConditionalBranch();
      Comparator comparator4 = new Comparator();
      ConditionalBranch conditionalBranch5 = new ConditionalBranch();
      Assigner assigner3 = new Assigner();
      Assigner assigner4 = new Assigner();
      Assigner assigner5 = new Assigner();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0))); // Class RAPGA expects this to be called Generations
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("CurrentPopulationSize", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("NumberOfCreatedOffspring", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("NumberOfSuccessfulOffspring", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<ScopeList>("OffspringList", new ScopeList()));

      assigner1.Name = "Initialize CurrentPopulationSize";
      assigner1.LeftSideParameter.ActualName = "CurrentPopulationSize";
      assigner1.RightSideParameter.ActualName = PopulationSizeParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("CurrentPopulationSize"));
      resultsCollector.ResultsParameter.ActualName = "Results";

      analyzer1.Name = "Analyzer";
      analyzer1.OperatorParameter.ActualName = "Analyzer";

      selector.Name = "Selector";
      selector.OperatorParameter.ActualName = "Selector";

      childrenCreator.ParentsPerChild = new IntValue(2);

      uniformSubScopesProcessor.Parallel.Value = true;

      crossover.Name = "Crossover";
      crossover.OperatorParameter.ActualName = "Crossover";

      stochasticBranch.ProbabilityParameter.ActualName = "MutationProbability";
      stochasticBranch.RandomParameter.ActualName = "Random";

      mutator.Name = "Mutator";
      mutator.OperatorParameter.ActualName = "Mutator";

      evaluator.Name = "Evaluator";
      evaluator.OperatorParameter.ActualName = "Evaluator";

      weightedParentsQualityComparator.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      weightedParentsQualityComparator.LeftSideParameter.ActualName = QualityParameter.Name;
      weightedParentsQualityComparator.MaximizationParameter.ActualName = MaximizationParameter.Name;
      weightedParentsQualityComparator.RightSideParameter.ActualName = QualityParameter.Name;
      weightedParentsQualityComparator.ResultParameter.ActualName = "SuccessfulOffspring";

      subScopesRemover.RemoveAllSubScopes = true;

      intCounter1.Name = "Increment NumberOfCreatedOffspring";
      intCounter1.ValueParameter.ActualName = "NumberOfCreatedOffspring";
      intCounter1.Increment = null;
      intCounter1.IncrementParameter.ActualName = BatchSizeParameter.Name;

      intCounter2.Name = "Increment EvaluatedSolutions";
      intCounter2.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;
      intCounter2.Increment = null;
      intCounter2.IncrementParameter.ActualName = BatchSizeParameter.Name;

      conditionalSelector.ConditionParameter.ActualName = "SuccessfulOffspring";
      conditionalSelector.ConditionParameter.Depth = 1;
      conditionalSelector.CopySelected.Value = false;

      duplicateSelector.CopySelected.Value = false;

      progressiveOffspringSelector.OffspringListParameter.ActualName = "OffspringList";
      progressiveOffspringSelector.ElitesParameter.ActualName = ElitesParameter.Name;
      progressiveOffspringSelector.MaximumPopulationSizeParameter.ActualName = MaximumPopulationSizeParameter.Name;

      subScopesCounter2.Name = "Count Successful Offspring";
      subScopesCounter2.ValueParameter.ActualName = "NumberOfSuccessfulOffspring";

      calculator1.Name = "NumberOfSuccessfulOffspring == MaximumPopulationSize - Elites";
      calculator1.CollectedValues.Add(new ValueLookupParameter<IntValue>("NumberOfSuccessfulOffspring"));
      calculator1.CollectedValues.Add(new ValueLookupParameter<IntValue>("MaximumPopulationSize"));
      calculator1.CollectedValues.Add(new ValueLookupParameter<IntValue>("Elites"));
      calculator1.ExpressionParameter.Value = new StringValue("NumberOfSuccessfulOffspring MaximumPopulationSize Elites - ==");
      calculator1.ExpressionResultParameter.ActualName = "Break";

      conditionalBranch1.Name = "Break?";
      conditionalBranch1.ConditionParameter.ActualName = "Break";

      comparator1.Name = "NumberOfCreatedOffspring >= Effort";
      comparator1.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      comparator1.LeftSideParameter.ActualName = "NumberOfCreatedOffspring";
      comparator1.RightSideParameter.ActualName = EffortParameter.Name;
      comparator1.ResultParameter.ActualName = "Break";

      conditionalBranch2.Name = "Break?";
      conditionalBranch2.ConditionParameter.ActualName = "Break";

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = "Elites";
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;

      intCounter3.Name = "Increment Generations";
      intCounter3.Increment = new IntValue(1);
      intCounter3.ValueParameter.ActualName = "Generations";

      subScopesCounter3.Name = "Update CurrentPopulationSize";
      subScopesCounter3.ValueParameter.ActualName = "CurrentPopulationSize";
      subScopesCounter3.AccumulateParameter.Value = new BoolValue(false);

      calculator2.Name = "Evaluate ActualSelectionPressure";
      calculator2.CollectedValues.Add(new ValueLookupParameter<IntValue>("NumberOfCreatedOffspring"));
      calculator2.CollectedValues.Add(new ValueLookupParameter<IntValue>("Elites"));
      calculator2.CollectedValues.Add(new ValueLookupParameter<IntValue>("CurrentPopulationSize"));
      calculator2.ExpressionParameter.Value = new StringValue("NumberOfCreatedOffspring Elites + CurrentPopulationSize /");
      calculator2.ExpressionResultParameter.ActualName = "ActualSelectionPressure";

      comparator2.Name = "CurrentPopulationSize < 1";
      comparator2.Comparison = new Comparison(ComparisonType.Less);
      comparator2.LeftSideParameter.ActualName = "CurrentPopulationSize";
      comparator2.RightSideParameter.Value = new IntValue(1);
      comparator2.ResultParameter.ActualName = "Terminate";

      conditionalBranch3.Name = "Terminate?";
      conditionalBranch3.ConditionParameter.ActualName = "Terminate";

      analyzer2.Name = "Analyzer";
      analyzer2.OperatorParameter.ActualName = "Analyzer";

      comparator3.Name = "Generations >= MaximumGenerations";
      comparator3.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      comparator3.LeftSideParameter.ActualName = "Generations";
      comparator3.ResultParameter.ActualName = "Terminate";
      comparator3.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;

      conditionalBranch4.Name = "Terminate?";
      conditionalBranch4.ConditionParameter.ActualName = "Terminate";

      comparator4.Name = "CurrentPopulationSize < MinimumPopulationSize";
      comparator4.Comparison = new Comparison(ComparisonType.Less);
      comparator4.LeftSideParameter.ActualName = "CurrentPopulationSize";
      comparator4.RightSideParameter.ActualName = MinimumPopulationSizeParameter.Name;
      comparator4.ResultParameter.ActualName = "Terminate";

      conditionalBranch5.Name = "Terminate?";
      conditionalBranch5.ConditionParameter.ActualName = "Terminate";

      assigner3.Name = "Reset NumberOfCreatedOffspring";
      assigner3.LeftSideParameter.ActualName = "NumberOfCreatedOffspring";
      assigner3.RightSideParameter.Value = new IntValue(0);

      assigner4.Name = "Reset NumberOfSuccessfulOffspring";
      assigner4.LeftSideParameter.ActualName = "NumberOfSuccessfulOffspring";
      assigner4.RightSideParameter.Value = new IntValue(0);

      assigner5.Name = "Reset OffspringList";
      assigner5.LeftSideParameter.ActualName = "OffspringList";
      assigner5.RightSideParameter.Value = new ScopeList();
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = assigner1;
      assigner1.Successor = resultsCollector;
      resultsCollector.Successor = analyzer1;
      analyzer1.Successor = selector;
      selector.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = calculator1;
      childrenCreator.Successor = uniformSubScopesProcessor;
      uniformSubScopesProcessor.Operator = crossover;
      uniformSubScopesProcessor.Successor = intCounter1;
      crossover.Successor = stochasticBranch;
      stochasticBranch.FirstBranch = mutator;
      stochasticBranch.SecondBranch = null;
      mutator.Successor = null;
      stochasticBranch.Successor = evaluator;
      evaluator.Successor = weightedParentsQualityComparator;
      weightedParentsQualityComparator.Successor = subScopesRemover;
      intCounter1.Successor = intCounter2;
      intCounter2.Successor = conditionalSelector;
      conditionalSelector.Successor = rightReducer1;
      rightReducer1.Successor = duplicateSelector;
      duplicateSelector.Successor = leftReducer1;
      leftReducer1.Successor = progressiveOffspringSelector;
      progressiveOffspringSelector.Successor = subScopesCounter2;
      calculator1.Successor = conditionalBranch1;
      conditionalBranch1.FalseBranch = comparator1;
      conditionalBranch1.TrueBranch = subScopesProcessor2;
      comparator1.Successor = conditionalBranch2;
      conditionalBranch2.FalseBranch = leftReducer2;
      conditionalBranch2.TrueBranch = subScopesProcessor2;
      leftReducer2.Successor = selector;
      subScopesProcessor2.Operators.Add(bestSelector);
      subScopesProcessor2.Operators.Add(scopeCleaner);
      subScopesProcessor2.Successor = mergingReducer;
      bestSelector.Successor = rightReducer2;
      rightReducer2.Successor = null;
      scopeCleaner.Successor = scopeRestorer;
      mergingReducer.Successor = intCounter3;
      intCounter3.Successor = subScopesCounter3;
      subScopesCounter3.Successor = calculator2;
      calculator2.Successor = comparator2;
      comparator2.Successor = conditionalBranch3;
      conditionalBranch3.FalseBranch = analyzer2;
      conditionalBranch3.TrueBranch = null;
      analyzer2.Successor = comparator3;
      comparator3.Successor = conditionalBranch4;
      conditionalBranch4.FalseBranch = comparator4;
      conditionalBranch4.TrueBranch = null;
      conditionalBranch4.Successor = null;
      comparator4.Successor = conditionalBranch5;
      conditionalBranch5.FalseBranch = assigner3;
      conditionalBranch5.TrueBranch = null;
      conditionalBranch5.Successor = null;
      assigner3.Successor = assigner4;
      assigner4.Successor = assigner5;
      assigner5.Successor = selector;

      #endregion
    }

    public override IOperation Apply() {
      if (CrossoverParameter.ActualName == null)
        return null;
      return base.Apply();
    }
  }
}
