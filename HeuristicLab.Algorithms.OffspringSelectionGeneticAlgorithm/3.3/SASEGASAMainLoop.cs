#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// A SASEGASA main loop operator.
  /// </summary>
  [Item("SASEGASAMainLoop", "A SASEGASA main loop operator.")]
  [StorableClass]
  public sealed class SASEGASAMainLoop : AlgorithmOperator {
    #region Parameter Properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<IntValue> NumberOfVillagesParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["NumberOfVillages"]; }
    }
    public ValueLookupParameter<IntValue> MigrationIntervalParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MigrationInterval"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
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
    public IValueLookupParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ReevaluateElites"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public ValueLookupParameter<IOperator> VillageAnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["VillageAnalyzer"]; }
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
    public ValueLookupParameter<DoubleValue> FinalMaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["FinalMaximumSelectionPressure"]; }
    }
    public ValueLookupParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    public ValueLookupParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
    }
    public LookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public IValueLookupParameter<BoolValue> FillPopulationWithParentsParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["FillPopulationWithParents"]; }
    }
    #endregion

    [StorableConstructor]
    private SASEGASAMainLoop(bool deserializing) : base(deserializing) { }
    private SASEGASAMainLoop(SASEGASAMainLoop original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SASEGASAMainLoop(this, cloner);
    }
    public SASEGASAMainLoop()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfVillages", "The initial number of villages."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MigrationInterval", "The fixed period after which migration occurs."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection to store the results."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to the analyze the villages."));
      Parameters.Add(new ValueLookupParameter<IOperator>("VillageAnalyzer", "The operator used to analyze each village."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new LookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1]."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorStart", "The lower bound of the comparison factor (start)."));
      Parameters.Add(new ValueLookupParameter<IOperator>("ComparisonFactorModifier", "The operator used to modify the comparison factor."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("FinalMaximumSelectionPressure", "The maximum selection pressure used when there is only one village left."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum genreation that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of times solutions have been evaluated."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("FillPopulationWithParents", "True if the population should be filled with parent individual or false if worse children should be used when the maximum selection pressure is exceeded."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      Assigner maxSelPressAssigner = new Assigner();
      Assigner villageCountAssigner = new Assigner();
      Assigner comparisonFactorInitializer = new Assigner();
      UniformSubScopesProcessor uniformSubScopesProcessor0 = new UniformSubScopesProcessor();
      VariableCreator villageVariableCreator = new VariableCreator();
      Placeholder villageAnalyzer1 = new Placeholder();
      ResultsCollector villageResultsCollector1 = new ResultsCollector();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      ConditionalBranch villageTerminatedBySelectionPressure1 = new ConditionalBranch();
      OffspringSelectionGeneticAlgorithmMainOperator mainOperator = new OffspringSelectionGeneticAlgorithmMainOperator();
      Placeholder villageAnalyzer2 = new Placeholder();
      ResultsCollector villageResultsCollector2 = new ResultsCollector();
      Comparator villageSelectionPressureComparator = new Comparator();
      ConditionalBranch villageTerminatedBySelectionPressure2 = new ConditionalBranch();
      IntCounter terminatedVillagesCounter = new IntCounter();
      IntCounter generationsCounter = new IntCounter();
      IntCounter generationsSinceLastReunificationCounter = new IntCounter();
      Comparator reunificationComparator1 = new Comparator();
      ConditionalBranch reunificationConditionalBranch1 = new ConditionalBranch();
      Comparator reunificationComparator2 = new Comparator();
      ConditionalBranch reunificationConditionalBranch2 = new ConditionalBranch();
      Comparator reunificationComparator3 = new Comparator();
      ConditionalBranch reunificationConditionalBranch3 = new ConditionalBranch();
      Assigner resetTerminatedVillagesAssigner = new Assigner();
      Assigner resetGenerationsSinceLastReunificationAssigner = new Assigner();
      SASEGASAReunificator reunificator = new SASEGASAReunificator();
      IntCounter reunificationCounter = new IntCounter();
      Placeholder comparisonFactorModifier = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      Assigner villageReviver = new Assigner();
      Comparator villageCountComparator = new Comparator();
      ConditionalBranch villageCountConditionalBranch = new ConditionalBranch();
      Assigner finalMaxSelPressAssigner = new Assigner();
      Comparator maximumGenerationsComparator = new Comparator();
      Comparator maximumEvaluatedSolutionsComparator = new Comparator();
      Placeholder analyzer2 = new Placeholder();
      ConditionalBranch terminationCondition = new ConditionalBranch();
      ConditionalBranch maximumGenerationsTerminationCondition = new ConditionalBranch();
      ConditionalBranch maximumEvaluatedSolutionsTerminationCondition = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Reunifications", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0))); // Class SASEGASA expects this to be called Generations
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("GenerationsSinceLastReunification", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("TerminatedVillages", new IntValue(0)));

      villageCountAssigner.LeftSideParameter.ActualName = "VillageCount";
      villageCountAssigner.RightSideParameter.ActualName = NumberOfVillagesParameter.Name;

      maxSelPressAssigner.LeftSideParameter.ActualName = "CurrentMaximumSelectionPressure";
      maxSelPressAssigner.RightSideParameter.ActualName = MaximumSelectionPressureParameter.Name;

      comparisonFactorInitializer.LeftSideParameter.ActualName = ComparisonFactorParameter.Name;
      comparisonFactorInitializer.RightSideParameter.ActualName = ComparisonFactorStartParameter.Name;

      villageVariableCreator.CollectedValues.Add(new ValueParameter<ResultCollection>("Results", new ResultCollection()));
      villageVariableCreator.CollectedValues.Add(new ValueParameter<BoolValue>("TerminateSelectionPressure", new BoolValue(false)));
      villageVariableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("SelectionPressure", new DoubleValue(0)));
      villageVariableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("CurrentSuccessRatio", new DoubleValue(0)));

      villageAnalyzer1.Name = "Village Analyzer (placeholder)";
      villageAnalyzer1.OperatorParameter.ActualName = VillageAnalyzerParameter.Name;

      villageResultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", "Indicates how many successful children were already found during a generation (relative to the population size).", "CurrentSuccessRatio"));
      villageResultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", "Displays the rising selection pressure during a generation.", "SelectionPressure"));
      villageResultsCollector1.ResultsParameter.ActualName = "Results";

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CopyValue = new BoolValue(false);
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("ComparisonFactor", null, ComparisonFactorParameter.Name));
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Terminated Villages", null, "TerminatedVillages"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Total Active Villages", null, "VillageCount"));
      resultsCollector1.CollectedValues.Add(new ScopeTreeLookupParameter<ResultCollection>("VillageResults", "Result set for each village", "Results"));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      villageTerminatedBySelectionPressure1.Name = "Village Terminated ?";
      villageTerminatedBySelectionPressure1.ConditionParameter.ActualName = "TerminateSelectionPressure";

      mainOperator.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      mainOperator.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainOperator.CurrentSuccessRatioParameter.ActualName = "CurrentSuccessRatio";
      mainOperator.ElitesParameter.ActualName = ElitesParameter.Name;
      mainOperator.ReevaluateElitesParameter.ActualName = ReevaluateElitesParameter.Name;
      mainOperator.EvaluatedSolutionsParameter.ActualName = EvaluatedSolutionsParameter.Name;
      mainOperator.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
      mainOperator.MaximizationParameter.ActualName = MaximizationParameter.Name;
      mainOperator.MaximumSelectionPressureParameter.ActualName = "CurrentMaximumSelectionPressure";
      mainOperator.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainOperator.MutatorParameter.ActualName = MutatorParameter.Name;
      mainOperator.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      mainOperator.QualityParameter.ActualName = QualityParameter.Name;
      mainOperator.RandomParameter.ActualName = RandomParameter.Name;
      mainOperator.SelectionPressureParameter.ActualName = "SelectionPressure";
      mainOperator.SelectorParameter.ActualName = SelectorParameter.Name;
      mainOperator.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      mainOperator.FillPopulationWithParentsParameter.ActualName = FillPopulationWithParentsParameter.Name;

      villageAnalyzer2.Name = "Village Analyzer (placeholder)";
      villageAnalyzer2.OperatorParameter.ActualName = VillageAnalyzerParameter.Name;

      villageResultsCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", "Indicates how many successful children were already found during a generation (relative to the population size).", "CurrentSuccessRatio"));
      villageResultsCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", "Displays the rising selection pressure during a generation.", "SelectionPressure"));
      villageResultsCollector2.ResultsParameter.ActualName = "Results";

      villageSelectionPressureComparator.Name = "SelectionPressure >= MaximumSelectionPressure ?";
      villageSelectionPressureComparator.LeftSideParameter.ActualName = "SelectionPressure";
      villageSelectionPressureComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      villageSelectionPressureComparator.RightSideParameter.ActualName = MaximumSelectionPressureParameter.Name;
      villageSelectionPressureComparator.ResultParameter.ActualName = "TerminateSelectionPressure";

      villageTerminatedBySelectionPressure2.Name = "Village Terminated ?";
      villageTerminatedBySelectionPressure2.ConditionParameter.ActualName = "TerminateSelectionPressure";

      terminatedVillagesCounter.Name = "TerminatedVillages + 1";
      terminatedVillagesCounter.ValueParameter.ActualName = "TerminatedVillages";
      terminatedVillagesCounter.Increment = new IntValue(1);

      generationsCounter.Name = "Generations + 1";
      generationsCounter.ValueParameter.ActualName = "Generations";
      generationsCounter.Increment = new IntValue(1);

      generationsSinceLastReunificationCounter.Name = "GenerationsSinceLastReunification + 1";
      generationsSinceLastReunificationCounter.ValueParameter.ActualName = "GenerationsSinceLastReunification";
      generationsSinceLastReunificationCounter.Increment = new IntValue(1);

      reunificationComparator1.Name = "TerminatedVillages = VillageCount ?";
      reunificationComparator1.LeftSideParameter.ActualName = "TerminatedVillages";
      reunificationComparator1.Comparison = new Comparison(ComparisonType.Equal);
      reunificationComparator1.RightSideParameter.ActualName = "VillageCount";
      reunificationComparator1.ResultParameter.ActualName = "Reunificate";

      reunificationConditionalBranch1.Name = "Reunificate ?";
      reunificationConditionalBranch1.ConditionParameter.ActualName = "Reunificate";

      reunificationComparator2.Name = "GenerationsSinceLastReunification = MigrationInterval ?";
      reunificationComparator2.LeftSideParameter.ActualName = "GenerationsSinceLastReunification";
      reunificationComparator2.Comparison = new Comparison(ComparisonType.Equal);
      reunificationComparator2.RightSideParameter.ActualName = "MigrationInterval";
      reunificationComparator2.ResultParameter.ActualName = "Reunificate";

      reunificationConditionalBranch2.Name = "Reunificate ?";
      reunificationConditionalBranch2.ConditionParameter.ActualName = "Reunificate";

      // if there's just one village left and we're getting to this point SASEGASA terminates
      reunificationComparator3.Name = "VillageCount <= 1 ?";
      reunificationComparator3.LeftSideParameter.ActualName = "VillageCount";
      reunificationComparator3.RightSideParameter.Value = new IntValue(1);
      reunificationComparator3.Comparison.Value = ComparisonType.LessOrEqual;
      reunificationComparator3.ResultParameter.ActualName = "TerminateSASEGASA";

      reunificationConditionalBranch3.Name = "Skip reunification?";
      reunificationConditionalBranch3.ConditionParameter.ActualName = "TerminateSASEGASA";

      resetTerminatedVillagesAssigner.Name = "Reset TerminatedVillages";
      resetTerminatedVillagesAssigner.LeftSideParameter.ActualName = "TerminatedVillages";
      resetTerminatedVillagesAssigner.RightSideParameter.Value = new IntValue(0);

      resetGenerationsSinceLastReunificationAssigner.Name = "Reset GenerationsSinceLastReunification";
      resetGenerationsSinceLastReunificationAssigner.LeftSideParameter.ActualName = "GenerationsSinceLastReunification";
      resetGenerationsSinceLastReunificationAssigner.RightSideParameter.Value = new IntValue(0);

      reunificator.VillageCountParameter.ActualName = "VillageCount";

      reunificationCounter.ValueParameter.ActualName = "Reunifications"; // this variable is referenced in SASEGASA, do not change!
      reunificationCounter.IncrementParameter.Value = new IntValue(1);

      comparisonFactorModifier.Name = "Update comparison factor (placeholder)";
      comparisonFactorModifier.OperatorParameter.ActualName = ComparisonFactorModifierParameter.Name;

      villageReviver.Name = "Village Reviver";
      villageReviver.LeftSideParameter.ActualName = "TerminateSelectionPressure";
      villageReviver.RightSideParameter.Value = new BoolValue(false);

      villageCountComparator.Name = "VillageCount == 1 ?";
      villageCountComparator.LeftSideParameter.ActualName = "VillageCount";
      villageCountComparator.RightSideParameter.Value = new IntValue(1);
      villageCountComparator.Comparison.Value = ComparisonType.Equal;
      villageCountComparator.ResultParameter.ActualName = "ChangeMaxSelPress";

      villageCountConditionalBranch.Name = "Change max selection pressure?";
      villageCountConditionalBranch.ConditionParameter.ActualName = "ChangeMaxSelPress";

      finalMaxSelPressAssigner.LeftSideParameter.ActualName = "CurrentMaximumSelectionPressure";
      finalMaxSelPressAssigner.RightSideParameter.ActualName = FinalMaximumSelectionPressureParameter.Name;

      // if Generations is reaching MaximumGenerations we're also terminating
      maximumGenerationsComparator.LeftSideParameter.ActualName = "Generations";
      maximumGenerationsComparator.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;
      maximumGenerationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maximumGenerationsComparator.ResultParameter.ActualName = "TerminateMaximumGenerations";

      maximumEvaluatedSolutionsComparator.Name = "EvaluatedSolutions >= MaximumEvaluatedSolutions";
      maximumEvaluatedSolutionsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maximumEvaluatedSolutionsComparator.LeftSideParameter.ActualName = EvaluatedSolutionsParameter.Name;
      maximumEvaluatedSolutionsComparator.ResultParameter.ActualName = "TerminateEvaluatedSolutions";
      maximumEvaluatedSolutionsComparator.RightSideParameter.ActualName = "MaximumEvaluatedSolutions";

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      terminationCondition.ConditionParameter.ActualName = "TerminateSASEGASA";
      maximumGenerationsTerminationCondition.ConditionParameter.ActualName = "TerminateMaximumGenerations";
      maximumEvaluatedSolutionsTerminationCondition.ConditionParameter.ActualName = "TerminateEvaluatedSolutions";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = maxSelPressAssigner;
      maxSelPressAssigner.Successor = villageCountAssigner;
      villageCountAssigner.Successor = comparisonFactorInitializer;
      comparisonFactorInitializer.Successor = uniformSubScopesProcessor0;
      uniformSubScopesProcessor0.Operator = villageVariableCreator;
      uniformSubScopesProcessor0.Successor = analyzer1;
      villageVariableCreator.Successor = villageAnalyzer1;
      villageAnalyzer1.Successor = villageResultsCollector1;
      analyzer1.Successor = resultsCollector1;
      resultsCollector1.Successor = uniformSubScopesProcessor1;
      uniformSubScopesProcessor1.Operator = villageTerminatedBySelectionPressure1;
      uniformSubScopesProcessor1.Successor = generationsCounter;
      villageTerminatedBySelectionPressure1.TrueBranch = null;
      villageTerminatedBySelectionPressure1.FalseBranch = mainOperator;
      villageTerminatedBySelectionPressure1.Successor = null;
      mainOperator.Successor = villageAnalyzer2;
      villageAnalyzer2.Successor = villageResultsCollector2;
      villageResultsCollector2.Successor = villageSelectionPressureComparator;
      villageSelectionPressureComparator.Successor = villageTerminatedBySelectionPressure2;
      villageTerminatedBySelectionPressure2.TrueBranch = terminatedVillagesCounter;
      villageTerminatedBySelectionPressure2.FalseBranch = null;
      villageTerminatedBySelectionPressure2.Successor = null;
      terminatedVillagesCounter.Successor = null;
      generationsCounter.Successor = generationsSinceLastReunificationCounter;
      generationsSinceLastReunificationCounter.Successor = reunificationComparator1;
      reunificationComparator1.Successor = reunificationConditionalBranch1;
      reunificationConditionalBranch1.TrueBranch = reunificationComparator3;
      reunificationConditionalBranch1.FalseBranch = reunificationComparator2;
      reunificationConditionalBranch1.Successor = maximumGenerationsComparator;
      reunificationComparator2.Successor = reunificationConditionalBranch2;
      reunificationConditionalBranch2.TrueBranch = reunificationComparator3;
      reunificationConditionalBranch2.FalseBranch = null;
      reunificationConditionalBranch2.Successor = null;
      reunificationComparator3.Successor = reunificationConditionalBranch3;
      reunificationConditionalBranch3.TrueBranch = null;
      reunificationConditionalBranch3.FalseBranch = resetTerminatedVillagesAssigner;
      reunificationConditionalBranch3.Successor = null;
      resetTerminatedVillagesAssigner.Successor = resetGenerationsSinceLastReunificationAssigner;
      resetGenerationsSinceLastReunificationAssigner.Successor = reunificator;
      reunificator.Successor = reunificationCounter;
      reunificationCounter.Successor = comparisonFactorModifier;
      comparisonFactorModifier.Successor = uniformSubScopesProcessor2;
      uniformSubScopesProcessor2.Operator = villageReviver;
      uniformSubScopesProcessor2.Successor = villageCountComparator;
      villageReviver.Successor = null;
      villageCountComparator.Successor = villageCountConditionalBranch;
      villageCountConditionalBranch.TrueBranch = finalMaxSelPressAssigner;
      villageCountConditionalBranch.FalseBranch = null;
      villageCountConditionalBranch.Successor = null;
      finalMaxSelPressAssigner.Successor = null;
      maximumGenerationsComparator.Successor = maximumEvaluatedSolutionsComparator;
      maximumEvaluatedSolutionsComparator.Successor = analyzer2;
      analyzer2.Successor = terminationCondition;
      terminationCondition.TrueBranch = null;
      terminationCondition.FalseBranch = maximumGenerationsTerminationCondition;
      terminationCondition.Successor = null;
      maximumGenerationsTerminationCondition.TrueBranch = null;
      maximumGenerationsTerminationCondition.FalseBranch = maximumEvaluatedSolutionsTerminationCondition;
      maximumGenerationsTerminationCondition.Successor = null;
      maximumEvaluatedSolutionsTerminationCondition.TrueBranch = null;
      maximumEvaluatedSolutionsTerminationCondition.FalseBranch = uniformSubScopesProcessor1;
      maximumEvaluatedSolutionsTerminationCondition.Successor = null;
      #endregion
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("ReevaluateElites")) {
        Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      }
      if (!Parameters.ContainsKey("FillPopulationWithParents"))
        Parameters.Add(new ValueLookupParameter<BoolValue>("FillPopulationWithParents", "True if the population should be filled with parent individual or false if worse children should be used when the maximum selection pressure is exceeded."));
      #endregion
    }

    public override IOperation Apply() {
      if (CrossoverParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
