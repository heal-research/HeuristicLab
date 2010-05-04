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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Selection;

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
    public SubScopesLookupParameter<DoubleValue> QualityParameter {
      get { return (SubScopesLookupParameter<DoubleValue>)Parameters["Quality"]; }
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
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ValueLookupParameter<IOperator> VisualizerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Visualizer"]; }
    }
    public LookupParameter<IItem> VisualizationParameter {
      get { return (LookupParameter<IItem>)Parameters["Visualization"]; }
    }
    public ValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public LookupParameter<DoubleValue> ComparisonFactorParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }
    public ValueLookupParameter<DoubleValue> ComparisonFactorLowerBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorLowerBound"]; }
    }
    public ValueLookupParameter<DoubleValue> ComparisonFactorUpperBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorUpperBound"]; }
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
    #endregion

    [StorableConstructor]
    private SASEGASAMainLoop(bool deserializing) : base() { }
    public SASEGASAMainLoop()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfVillages", "The initial number of villages."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MigrationInterval", "The fixed period after which migration occurs."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection to store the results."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Visualizer", "The operator used to visualize solutions."));
      Parameters.Add(new LookupParameter<IItem>("Visualization", "The item which represents the visualization of solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new LookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1]."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorLowerBound", "The lower bound of the comparison factor (start)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorUpperBound", "The upper bound of the comparison factor (end)."));
      Parameters.Add(new ValueLookupParameter<IOperator>("ComparisonFactorModifier", "The operator used to modify the comparison factor."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("FinalMaximumSelectionPressure", "The maximum selection pressure used when there is only one village left."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum genreation that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      Assigner maxSelPressAssigner = new Assigner();
      Assigner villageCountAssigner = new Assigner();
      Assigner comparisonFactorInitializer = new Assigner();
      UniformSubScopesProcessor uniformSubScopesProcessor0 = new UniformSubScopesProcessor();
      VariableCreator villageVariableCreator = new VariableCreator();
      BestQualityMemorizer villageBestQualityMemorizer1 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator villageBestAverageWorstQualityCalculator1 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector villageDataTableValuesCollector1 = new DataTableValuesCollector();
      DataTableValuesCollector villageDataTableValuesCollector2 = new DataTableValuesCollector();
      QualityDifferenceCalculator villageQualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      ResultsCollector villageResultsCollector = new ResultsCollector();
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator1 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector1 = new DataTableValuesCollector();
      DataTableValuesCollector dataTableValuesCollector2 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      ConditionalBranch villageTerminatedBySelectionPressure1 = new ConditionalBranch();
      OffspringSelectionGeneticAlgorithmMainOperator mainOperator = new OffspringSelectionGeneticAlgorithmMainOperator();
      BestQualityMemorizer villageBestQualityMemorizer2 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator villageBestAverageWorstQualityCalculator2 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector villageDataTableValuesCollector3 = new DataTableValuesCollector();
      DataTableValuesCollector villageDataTableValuesCollector4 = new DataTableValuesCollector();
      QualityDifferenceCalculator villageQualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      IntCounter evaluatedSolutionsCounter = new IntCounter();
      Assigner villageEvaluatedSolutionsAssigner = new Assigner();
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
      BestQualityMemorizer bestQualityMemorizer3 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer4 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator2 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector3 = new DataTableValuesCollector();
      DataTableValuesCollector dataTableValuesCollector4 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      ConditionalBranch terminationCondition = new ConditionalBranch();
      ConditionalBranch maximumGenerationsTerminationCondition = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Reunifications", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("GenerationsSinceLastReunification", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("TerminatedVillages", new IntValue(0)));

      villageCountAssigner.LeftSideParameter.ActualName = "VillageCount";
      villageCountAssigner.RightSideParameter.ActualName = NumberOfVillagesParameter.Name;

      maxSelPressAssigner.LeftSideParameter.ActualName = "CurrentMaximumSelectionPressure";
      maxSelPressAssigner.RightSideParameter.ActualName = MaximumSelectionPressureParameter.Name;

      comparisonFactorInitializer.LeftSideParameter.ActualName = "ComparisonFactor";
      comparisonFactorInitializer.RightSideParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;

      villageVariableCreator.CollectedValues.Add(new ValueParameter<ResultCollection>("VillageResults", new ResultCollection()));
      villageVariableCreator.CollectedValues.Add(new ValueParameter<IntValue>("VillageEvaluatedSolutions", new IntValue(0)));
      villageVariableCreator.CollectedValues.Add(new ValueParameter<BoolValue>("TerminateSelectionPressure", new BoolValue(false)));
      villageVariableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("SelectionPressure", new DoubleValue(0)));
      villageVariableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("CurrentSuccessRatio", new DoubleValue(0)));

      villageBestQualityMemorizer1.BestQualityParameter.ActualName = "BestQuality";
      villageBestQualityMemorizer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      villageBestQualityMemorizer1.QualityParameter.ActualName = QualityParameter.Name;

      villageBestAverageWorstQualityCalculator1.AverageQualityParameter.ActualName = "CurrentAverageQuality";
      villageBestAverageWorstQualityCalculator1.BestQualityParameter.ActualName = "CurrentBestQuality";
      villageBestAverageWorstQualityCalculator1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      villageBestAverageWorstQualityCalculator1.QualityParameter.ActualName = QualityParameter.Name;
      villageBestAverageWorstQualityCalculator1.WorstQualityParameter.ActualName = "CurrentWorstQuality";

      villageDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestQuality"));
      villageDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageQuality"));
      villageDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstQuality"));
      villageDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      villageDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      villageDataTableValuesCollector1.DataTableParameter.ActualName = "Qualities";

      villageDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Selection Pressure", null, "SelectionPressure"));
      villageDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, "MaximumSelectionPressure"));
      villageDataTableValuesCollector2.DataTableParameter.ActualName = "SelectionPressures";

      villageQualityDifferenceCalculator1.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      villageQualityDifferenceCalculator1.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      villageQualityDifferenceCalculator1.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      villageQualityDifferenceCalculator1.SecondQualityParameter.ActualName = "BestQuality";

      villageResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", null, "CurrentSuccessRatio"));
      villageResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality", null, "CurrentBestQuality"));
      villageResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality", null, "CurrentAverageQuality"));
      villageResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality", null, "CurrentWorstQuality"));
      villageResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      villageResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      villageResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      villageResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      villageResultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      villageResultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("SelectionPressures"));
      villageResultsCollector.ResultsParameter.ActualName = "VillageResults";

      bestQualityMemorizer1.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer1.QualityParameter.ActualName = "BestQuality";

      bestQualityMemorizer2.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestQualityMemorizer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer2.QualityParameter.ActualName = "BestQuality";

      bestAverageWorstQualityCalculator1.AverageQualityParameter.ActualName = "CurrentAverageBestQuality";
      bestAverageWorstQualityCalculator1.BestQualityParameter.ActualName = "CurrentBestBestQuality";
      bestAverageWorstQualityCalculator1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator1.QualityParameter.ActualName = "CurrentBestQuality";
      bestAverageWorstQualityCalculator1.WorstQualityParameter.ActualName = "CurrentWorstBestQuality";

      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector1.DataTableParameter.ActualName = "BestQualities";

      dataTableValuesCollector2.CollectedValues.Add(new SubScopesLookupParameter<DoubleValue>("Selection Pressure Village", null, "SelectionPressure"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, "MaximumSelectionPressure"));
      dataTableValuesCollector2.DataTableParameter.ActualName = "VillageSelectionPressures";

      qualityDifferenceCalculator1.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator1.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.SecondQualityParameter.ActualName = "BestQuality";

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Active Villages", null, "VillageCount"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("BestQualities"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("VillageSelectionPressures"));
      resultsCollector.CollectedValues.Add(new SubScopesLookupParameter<ResultCollection>("VillageResults", "Result set for each village"));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;

      villageTerminatedBySelectionPressure1.Name = "Village Terminated ?";
      villageTerminatedBySelectionPressure1.ConditionParameter.ActualName = "TerminateSelectionPressure";

      mainOperator.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      mainOperator.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainOperator.CurrentSuccessRatioParameter.ActualName = "CurrentSuccessRatio";
      mainOperator.ElitesParameter.ActualName = ElitesParameter.Name;
      mainOperator.EvaluatedSolutionsParameter.ActualName = "VillageEvaluatedSolutions";
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

      villageBestQualityMemorizer2.BestQualityParameter.ActualName = "BestQuality";
      villageBestQualityMemorizer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      villageBestQualityMemorizer2.QualityParameter.ActualName = QualityParameter.Name;

      villageBestAverageWorstQualityCalculator2.AverageQualityParameter.ActualName = "CurrentAverageQuality";
      villageBestAverageWorstQualityCalculator2.BestQualityParameter.ActualName = "CurrentBestQuality";
      villageBestAverageWorstQualityCalculator2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      villageBestAverageWorstQualityCalculator2.QualityParameter.ActualName = QualityParameter.Name;
      villageBestAverageWorstQualityCalculator2.WorstQualityParameter.ActualName = "CurrentWorstQuality";

      villageDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestQuality"));
      villageDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageQuality"));
      villageDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstQuality"));
      villageDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      villageDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      villageDataTableValuesCollector3.DataTableParameter.ActualName = "Qualities";

      villageDataTableValuesCollector4.CollectedValues.Add(new LookupParameter<DoubleValue>("Selection Pressure", null, "SelectionPressure"));
      villageDataTableValuesCollector4.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, "MaximumSelectionPressure"));
      villageDataTableValuesCollector4.DataTableParameter.ActualName = "SelectionPressures";

      villageQualityDifferenceCalculator2.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      villageQualityDifferenceCalculator2.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      villageQualityDifferenceCalculator2.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      villageQualityDifferenceCalculator2.SecondQualityParameter.ActualName = "BestQuality";

      evaluatedSolutionsCounter.Name = "Update EvaluatedSolutions";
      evaluatedSolutionsCounter.ValueParameter.ActualName = "EvaluatedSolutions";
      evaluatedSolutionsCounter.Increment = null;
      evaluatedSolutionsCounter.IncrementParameter.ActualName = "VillageEvaluatedSolutions";

      villageEvaluatedSolutionsAssigner.Name = "Reset EvaluatedSolutions";
      villageEvaluatedSolutionsAssigner.LeftSideParameter.ActualName = "VillageEvaluatedSolutions";
      villageEvaluatedSolutionsAssigner.RightSideParameter.Value = new IntValue(0);

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

      bestQualityMemorizer3.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer3.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer3.QualityParameter.ActualName = "BestQuality";

      bestQualityMemorizer4.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer4.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer4.QualityParameter.ActualName = "BestQuality";

      bestAverageWorstQualityCalculator2.AverageQualityParameter.ActualName = "CurrentAverageBestQuality";
      bestAverageWorstQualityCalculator2.BestQualityParameter.ActualName = "CurrentBestBestQuality";
      bestAverageWorstQualityCalculator2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator2.QualityParameter.ActualName = "CurrentBestQuality";
      bestAverageWorstQualityCalculator2.WorstQualityParameter.ActualName = "CurrentWorstBestQuality";

      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector3.DataTableParameter.ActualName = "BestQualities";

      dataTableValuesCollector4.CollectedValues.Add(new SubScopesLookupParameter<DoubleValue>("Selection Pressure Village", null, "SelectionPressure"));
      dataTableValuesCollector4.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, "MaximumSelectionPressure"));
      dataTableValuesCollector4.DataTableParameter.ActualName = "VillageSelectionPressures";

      qualityDifferenceCalculator2.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator2.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.SecondQualityParameter.ActualName = "BestQuality";

      terminationCondition.ConditionParameter.ActualName = "TerminateSASEGASA";
      maximumGenerationsTerminationCondition.ConditionParameter.ActualName = "TerminateMaximumGenerations";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = maxSelPressAssigner;
      maxSelPressAssigner.Successor = villageCountAssigner;
      villageCountAssigner.Successor = comparisonFactorInitializer;
      comparisonFactorInitializer.Successor = uniformSubScopesProcessor0;
      uniformSubScopesProcessor0.Operator = villageVariableCreator;
      uniformSubScopesProcessor0.Successor = bestQualityMemorizer1;
      villageVariableCreator.Successor = villageBestQualityMemorizer1;
      villageBestQualityMemorizer1.Successor = villageBestAverageWorstQualityCalculator1;
      villageBestAverageWorstQualityCalculator1.Successor = villageDataTableValuesCollector1;
      villageDataTableValuesCollector1.Successor = villageDataTableValuesCollector2;
      villageDataTableValuesCollector2.Successor = villageQualityDifferenceCalculator1;
      villageQualityDifferenceCalculator1.Successor = villageResultsCollector;
      bestQualityMemorizer1.Successor = bestQualityMemorizer2;
      bestQualityMemorizer2.Successor = bestAverageWorstQualityCalculator1;
      bestAverageWorstQualityCalculator1.Successor = dataTableValuesCollector1;
      dataTableValuesCollector1.Successor = dataTableValuesCollector2;
      dataTableValuesCollector2.Successor = qualityDifferenceCalculator1;
      qualityDifferenceCalculator1.Successor = resultsCollector;
      resultsCollector.Successor = uniformSubScopesProcessor1;
      uniformSubScopesProcessor1.Operator = villageTerminatedBySelectionPressure1;
      uniformSubScopesProcessor1.Successor = generationsCounter;
      villageTerminatedBySelectionPressure1.TrueBranch = null;
      villageTerminatedBySelectionPressure1.FalseBranch = mainOperator;
      villageTerminatedBySelectionPressure1.Successor = null;
      mainOperator.Successor = villageBestQualityMemorizer2;
      villageBestQualityMemorizer2.Successor = villageBestAverageWorstQualityCalculator2;
      villageBestAverageWorstQualityCalculator2.Successor = villageDataTableValuesCollector3;
      villageDataTableValuesCollector3.Successor = villageDataTableValuesCollector4;
      villageDataTableValuesCollector4.Successor = villageQualityDifferenceCalculator2;
      villageQualityDifferenceCalculator2.Successor = evaluatedSolutionsCounter;
      evaluatedSolutionsCounter.Successor = villageEvaluatedSolutionsAssigner;
      villageEvaluatedSolutionsAssigner.Successor = villageSelectionPressureComparator;
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
      maximumGenerationsComparator.Successor = bestQualityMemorizer3;
      bestQualityMemorizer3.Successor = bestQualityMemorizer4;
      bestQualityMemorizer4.Successor = bestAverageWorstQualityCalculator2;
      bestAverageWorstQualityCalculator2.Successor = dataTableValuesCollector3;
      dataTableValuesCollector3.Successor = dataTableValuesCollector4;
      dataTableValuesCollector4.Successor = qualityDifferenceCalculator2;
      qualityDifferenceCalculator2.Successor = terminationCondition;
      terminationCondition.TrueBranch = null;
      terminationCondition.FalseBranch = maximumGenerationsTerminationCondition;
      terminationCondition.Successor = null;
      maximumGenerationsTerminationCondition.TrueBranch = null;
      maximumGenerationsTerminationCondition.FalseBranch = uniformSubScopesProcessor1;
      maximumGenerationsTerminationCondition.Successor = null;
      #endregion
    }
  }
}
