#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// An operator which represents the main loop of an offspring selection genetic algorithm.
  /// </summary>
  [Item("OffspringSelectionGeneticAlgorithmMainLoop", "An operator which represents the main loop of an offspring selection genetic algorithm.")]
  [StorableClass]
  public sealed class OffspringSelectionGeneticAlgorithmMainLoop : AlgorithmOperator {
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
    public ValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public LookupParameter<DoubleValue> ComparisonFactorParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }
    public ValueLookupParameter<DoubleValue> ComparisonFactorStartParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorStart"]; }
    }
    public ValueLookupParameter<IOperator> ComparisonFactorModifierParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["ComparisonFactorModifier"]; }
    }
    public ValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    public ValueLookupParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
    }
    public LookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    #endregion

    [StorableConstructor]
    private OffspringSelectionGeneticAlgorithmMainLoop(bool deserializing) : base(deserializing) { }
    private OffspringSelectionGeneticAlgorithmMainLoop(OffspringSelectionGeneticAlgorithmMainLoop original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OffspringSelectionGeneticAlgorithmMainLoop(this, cloner);
    }
    public OffspringSelectionGeneticAlgorithmMainLoop()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze each generation."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new LookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1]."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorStart", "The initial value for the comparison factor."));
      Parameters.Add(new ValueLookupParameter<IOperator>("ComparisonFactorModifier", "The operator used to modify the comparison factor."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of times solutions have been evaluated."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      Assigner comparisonFactorInitializer = new Assigner();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      OffspringSelectionGeneticAlgorithmMainOperator mainOperator = new OffspringSelectionGeneticAlgorithmMainOperator();
      IntCounter generationsCounter = new IntCounter();
      Comparator maxGenerationsComparator = new Comparator();
      Comparator maxSelectionPressureComparator = new Comparator();
      Comparator maxEvaluatedSolutionsComparator = new Comparator();
      Placeholder comparisonFactorModifier = new Placeholder();
      Placeholder analyzer2 = new Placeholder();
      ConditionalBranch conditionalBranch1 = new ConditionalBranch();
      ConditionalBranch conditionalBranch2 = new ConditionalBranch();
      ConditionalBranch conditionalBranch3 = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0))); // Class OffspringSelectionGeneticAlgorithm expects this to be called Generations
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("SelectionPressure", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("CurrentSuccessRatio", new DoubleValue(0)));

      comparisonFactorInitializer.Name = "Initialize ComparisonFactor (placeholder)";
      comparisonFactorInitializer.LeftSideParameter.ActualName = ComparisonFactorParameter.Name;
      comparisonFactorInitializer.RightSideParameter.ActualName = ComparisonFactorStartParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CopyValue = new BoolValue(false);
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Comparison Factor", null, ComparisonFactorParameter.Name));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", "Displays the rising selection pressure during a generation.", "SelectionPressure"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", "Indicates how many successful children were already found during a generation (relative to the population size).", "CurrentSuccessRatio"));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      mainOperator.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      mainOperator.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainOperator.CurrentSuccessRatioParameter.ActualName = "CurrentSuccessRatio";
      mainOperator.ElitesParameter.ActualName = ElitesParameter.Name;
      mainOperator.EvaluatedSolutionsParameter.ActualName = EvaluatedSolutionsParameter.Name;
      mainOperator.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
      mainOperator.MaximizationParameter.ActualName = MaximizationParameter.Name;
      mainOperator.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      mainOperator.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainOperator.MutatorParameter.ActualName = MutatorParameter.Name;
      mainOperator.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      mainOperator.QualityParameter.ActualName = QualityParameter.Name;
      mainOperator.RandomParameter.ActualName = RandomParameter.Name;
      mainOperator.SelectionPressureParameter.ActualName = "SelectionPressure";
      mainOperator.SelectorParameter.ActualName = SelectorParameter.Name;
      mainOperator.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;

      generationsCounter.Increment = new IntValue(1);
      generationsCounter.ValueParameter.ActualName = "Generations";

      maxGenerationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxGenerationsComparator.LeftSideParameter.ActualName = "Generations";
      maxGenerationsComparator.ResultParameter.ActualName = "TerminateMaximumGenerations";
      maxGenerationsComparator.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;

      maxSelectionPressureComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxSelectionPressureComparator.LeftSideParameter.ActualName = "SelectionPressure";
      maxSelectionPressureComparator.ResultParameter.ActualName = "TerminateSelectionPressure";
      maxSelectionPressureComparator.RightSideParameter.ActualName = MaximumSelectionPressureParameter.Name;

      maxEvaluatedSolutionsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxEvaluatedSolutionsComparator.LeftSideParameter.ActualName = EvaluatedSolutionsParameter.Name;
      maxEvaluatedSolutionsComparator.ResultParameter.ActualName = "TerminateEvaluatedSolutions";
      maxEvaluatedSolutionsComparator.RightSideParameter.ActualName = "MaximumEvaluatedSolutions";

      comparisonFactorModifier.Name = "Update ComparisonFactor (placeholder)";
      comparisonFactorModifier.OperatorParameter.ActualName = ComparisonFactorModifierParameter.Name;

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      conditionalBranch1.Name = "MaximumSelectionPressure reached?";
      conditionalBranch1.ConditionParameter.ActualName = "TerminateSelectionPressure";

      conditionalBranch2.Name = "MaximumGenerations reached?";
      conditionalBranch2.ConditionParameter.ActualName = "TerminateMaximumGenerations";

      conditionalBranch3.Name = "MaximumEvaluatedSolutions reached?";
      conditionalBranch3.ConditionParameter.ActualName = "TerminateEvaluatedSolutions";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = comparisonFactorInitializer;
      comparisonFactorInitializer.Successor = analyzer1;
      analyzer1.Successor = resultsCollector1;
      resultsCollector1.Successor = mainOperator;
      mainOperator.Successor = generationsCounter;
      generationsCounter.Successor = maxGenerationsComparator;
      maxGenerationsComparator.Successor = maxSelectionPressureComparator;
      maxSelectionPressureComparator.Successor = maxEvaluatedSolutionsComparator;
      maxEvaluatedSolutionsComparator.Successor = comparisonFactorModifier;
      comparisonFactorModifier.Successor = analyzer2;
      analyzer2.Successor = conditionalBranch1;
      conditionalBranch1.FalseBranch = conditionalBranch2;
      conditionalBranch1.TrueBranch = null;
      conditionalBranch1.Successor = null;
      conditionalBranch2.FalseBranch = conditionalBranch3;
      conditionalBranch2.TrueBranch = null;
      conditionalBranch2.Successor = null;
      conditionalBranch3.FalseBranch = mainOperator;
      conditionalBranch3.TrueBranch = null;
      conditionalBranch3.Successor = null;
      #endregion
    }
  }
}
