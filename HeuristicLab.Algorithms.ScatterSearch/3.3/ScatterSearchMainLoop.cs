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
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.ScatterSearch {
  /// <summary>
  /// An operator which represents a scatter search.
  /// </summary>
  [Item("ScatterSearchMainLoop", "An operator which represents a scatter search.")]
  [StorableType("CDEB69B9-A0D9-4E84-80FD-347E16A1A8FE")]
  public sealed class ScatterSearchMainLoop : AlgorithmOperator {
    #region Parameter properties
    public IValueLookupParameter<IMultiAnalyzer> AnalyzerParameter {
      get { return (IValueLookupParameter<IMultiAnalyzer>)Parameters["Analyzer"]; }
    }
    public IValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public IValueLookupParameter<ICrossover> CrossoverParameter {
      get { return (IValueLookupParameter<ICrossover>)Parameters["Crossover"]; }
    }
    public IValueLookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public IValueLookupParameter<IEvaluator> EvaluatorParameter {
      get { return (IValueLookupParameter<IEvaluator>)Parameters["Evaluator"]; }
    }
    public IValueLookupParameter<BoolValue> ExecutePathRelinkingParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ExecutePathRelinking"]; }
    }
    public IValueLookupParameter<IImprovementOperator> ImproverParameter {
      get { return (IValueLookupParameter<IImprovementOperator>)Parameters["Improver"]; }
    }
    public IValueLookupParameter<IntValue> IterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Iterations"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    public IValueLookupParameter<IntValue> NumberOfHighQualitySolutionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NumberOfHighQualitySolutions"]; }
    }
    public IValueLookupParameter<IPathRelinker> PathRelinkerParameter {
      get { return (IValueLookupParameter<IPathRelinker>)Parameters["PathRelinker"]; }
    }
    public IValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public IValueLookupParameter<IntValue> ReferenceSetSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["ReferenceSetSize"]; }
    }
    public IValueLookupParameter<DoubleValue> QualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<IRandom> RandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (IValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    public IValueLookupParameter<ISolutionSimilarityCalculator> SimilarityCalculatorParameter {
      get { return (IValueLookupParameter<ISolutionSimilarityCalculator>)Parameters["SimilarityCalculator"]; }
    }
    #endregion

    #region Properties
    private IMultiAnalyzer Analyzer {
      get { return AnalyzerParameter.ActualValue; }
      set { AnalyzerParameter.ActualValue = value; }
    }
    private DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.ActualValue; }
      set { BestKnownQualityParameter.ActualValue = value; }
    }
    private ICrossover Crossover {
      get { return CrossoverParameter.ActualValue; }
      set { CrossoverParameter.ActualValue = value; }
    }
    private IntValue EvaluatedSolutions {
      get { return EvaluatedSolutionsParameter.ActualValue; }
      set { EvaluatedSolutionsParameter.ActualValue = value; }
    }
    private IEvaluator Evaluator {
      get { return EvaluatorParameter.ActualValue; }
      set { EvaluatorParameter.ActualValue = value; }
    }
    private BoolValue ExecutePathRelinking {
      get { return ExecutePathRelinkingParameter.ActualValue; }
      set { ExecutePathRelinkingParameter.ActualValue = value; }
    }
    private IImprovementOperator Improver {
      get { return ImproverParameter.ActualValue; }
      set { ImproverParameter.ActualValue = value; }
    }
    private IntValue Iterations {
      get { return IterationsParameter.ActualValue; }
      set { IterationsParameter.ActualValue = value; }
    }
    private BoolValue Maximization {
      get { return MaximizationParameter.ActualValue; }
      set { MaximizationParameter.ActualValue = value; }
    }
    private IntValue MaximumIterations {
      get { return MaximumIterationsParameter.ActualValue; }
      set { MaximumIterationsParameter.ActualValue = value; }
    }
    private IntValue NumberOfHighQualitySolutions {
      get { return NumberOfHighQualitySolutionsParameter.ActualValue; }
      set { NumberOfHighQualitySolutionsParameter.ActualValue = value; }
    }
    private IPathRelinker PathRelinker {
      get { return PathRelinkerParameter.ActualValue; }
      set { PathRelinkerParameter.ActualValue = value; }
    }
    private IntValue PopulationSize {
      get { return PopulationSizeParameter.ActualValue; }
      set { PopulationSizeParameter.ActualValue = value; }
    }
    private IntValue ReferenceSetSize {
      get { return ReferenceSetSizeParameter.ActualValue; }
      set { ReferenceSetSizeParameter.ActualValue = value; }
    }
    private DoubleValue Quality {
      get { return QualityParameter.ActualValue; }
      set { QualityParameter.ActualValue = value; }
    }
    private IRandom Random {
      get { return RandomParameter.ActualValue; }
      set { RandomParameter.ActualValue = value; }
    }
    private VariableCollection Results {
      get { return ResultsParameter.ActualValue; }
      set { ResultsParameter.ActualValue = value; }
    }
    private ISolutionSimilarityCalculator SimilarityCalculator {
      get { return SimilarityCalculatorParameter.ActualValue; }
      set { SimilarityCalculatorParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    private ScatterSearchMainLoop(StorableConstructorFlag _) : base(_) { }
    private ScatterSearchMainLoop(ScatterSearchMainLoop original, Cloner cloner) : base(original, cloner) { }
    public ScatterSearchMainLoop() : base() { Initialize(); }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterSearchMainLoop(this, cloner);
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IMultiAnalyzer>("Analyzer", "The analyzer used to analyze each iteration."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("EvaluatedSolutions", "The number of times solutions have been evaluated."));
      Parameters.Add(new ValueLookupParameter<IEvaluator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ExecutePathRelinking", "True if path relinking should be executed instead of crossover, otherwise false."));
      Parameters.Add(new ValueLookupParameter<IImprovementOperator>("Improver", "The operator used to improve solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Iterations", "The number of iterations performed."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of iterations which should be processed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfHighQualitySolutions", "The number of high quality solutions in the reference set."));
      Parameters.Add(new ValueLookupParameter<IPathRelinker>("PathRelinker", "The operator used to execute path relinking."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population of solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("ReferenceSetSize", "The size of the reference set."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Quality", "This parameter is used for name translation only."));
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The operator used to calculate the similarity between two solutions."));
      #endregion

      #region Create operators
      Placeholder analyzer = new Placeholder();
      Assigner assigner1 = new Assigner();
      Assigner assigner2 = new Assigner();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      Placeholder crossover = new Placeholder();
      Comparator iterationsChecker = new Comparator();
      IntCounter iterationsCounter = new IntCounter();
      MergingReducer mergingReducer = new MergingReducer();
      ConditionalBranch executePathRelinkingBranch = new ConditionalBranch();
      ConditionalBranch newSolutionsBranch = new ConditionalBranch();
      OffspringProcessor offspringProcessor = new OffspringProcessor();
      Placeholder pathRelinker = new Placeholder();
      PopulationRebuildMethod populationRebuildMethod = new PopulationRebuildMethod();
      ReferenceSetUpdateMethod referenceSetUpdateMethod = new ReferenceSetUpdateMethod();
      ResultsCollector resultsCollector = new ResultsCollector();
      RightSelector rightSelector = new RightSelector();
      Placeholder solutionEvaluator1 = new Placeholder();
      Placeholder solutionEvaluator2 = new Placeholder();
      Placeholder solutionImprover1 = new Placeholder();
      Placeholder solutionImprover2 = new Placeholder();
      SolutionPoolUpdateMethod solutionPoolUpdateMethod = new SolutionPoolUpdateMethod();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      DataReducer dataReducer1 = new DataReducer();
      DataReducer dataReducer2 = new DataReducer();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      SubScopesProcessor subScopesProcessor2 = new SubScopesProcessor();
      SubScopesProcessor subScopesProcessor3 = new SubScopesProcessor();
      SubScopesProcessor subScopesProcessor4 = new SubScopesProcessor();
      ConditionalBranch terminateBranch = new ConditionalBranch();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      UniformSubScopesProcessor uniformSubScopesProcessor3 = new UniformSubScopesProcessor();
      VariableCreator variableCreator = new VariableCreator();
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>(IterationsParameter.Name, new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<BoolValue>("NewSolutions", new BoolValue(false)));
      variableCreator.Successor = resultsCollector;

      resultsCollector.CopyValue = new BoolValue(false);
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>(IterationsParameter.Name));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
      resultsCollector.Successor = iterationsChecker;

      iterationsChecker.Name = "IterationsChecker";
      iterationsChecker.Comparison.Value = ComparisonType.GreaterOrEqual;
      iterationsChecker.LeftSideParameter.ActualName = IterationsParameter.Name;
      iterationsChecker.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      iterationsChecker.ResultParameter.ActualName = "Terminate";
      iterationsChecker.Successor = terminateBranch;

      terminateBranch.Name = "TerminateChecker";
      terminateBranch.ConditionParameter.ActualName = "Terminate";
      terminateBranch.FalseBranch = referenceSetUpdateMethod;

      referenceSetUpdateMethod.Successor = assigner1;

      assigner1.Name = "NewSolutions = true";
      assigner1.LeftSideParameter.ActualName = "NewSolutions";
      assigner1.RightSideParameter.Value = new BoolValue(true);
      assigner1.Successor = subScopesProcessor1;

      subScopesProcessor1.DepthParameter.Value = new IntValue(1);
      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = newSolutionsBranch;

      childrenCreator.Name = "SubsetGenerator";
      childrenCreator.ParentsPerChildParameter.Value = new IntValue(2);
      childrenCreator.Successor = assigner2;

      assigner2.Name = "NewSolutions = false";
      assigner2.LeftSideParameter.ActualName = "NewSolutions";
      assigner2.RightSideParameter.Value = new BoolValue(false);
      assigner2.Successor = uniformSubScopesProcessor1;

      uniformSubScopesProcessor1.DepthParameter.Value = new IntValue(1);
      uniformSubScopesProcessor1.Operator = executePathRelinkingBranch;
      uniformSubScopesProcessor1.Successor = solutionPoolUpdateMethod;

      executePathRelinkingBranch.Name = "ExecutePathRelinkingChecker";
      executePathRelinkingBranch.ConditionParameter.ActualName = ExecutePathRelinkingParameter.ActualName;
      executePathRelinkingBranch.TrueBranch = pathRelinker;
      executePathRelinkingBranch.FalseBranch = crossover;

      pathRelinker.Name = "PathRelinker";
      pathRelinker.OperatorParameter.ActualName = PathRelinkerParameter.Name;
      pathRelinker.Successor = rightSelector;

      crossover.Name = "Crossover";
      crossover.OperatorParameter.ActualName = CrossoverParameter.Name;
      crossover.Successor = offspringProcessor;

      offspringProcessor.Successor = rightSelector;

      rightSelector.NumberOfSelectedSubScopesParameter.Value = new IntValue(1);
      rightSelector.CopySelected = new BoolValue(false);
      rightSelector.Successor = subScopesProcessor2;

      subScopesProcessor2.DepthParameter.Value = new IntValue(1);
      subScopesProcessor2.Operators.Add(new EmptyOperator());
      subScopesProcessor2.Operators.Add(uniformSubScopesProcessor2);
      subScopesProcessor2.Successor = mergingReducer;

      uniformSubScopesProcessor2.DepthParameter.Value = new IntValue(2);
      uniformSubScopesProcessor2.Operator = solutionImprover1;
      uniformSubScopesProcessor2.ParallelParameter.Value = new BoolValue(true);
      uniformSubScopesProcessor2.Successor = subScopesProcessor4;

      solutionImprover1.Name = "SolutionImprover";
      solutionImprover1.OperatorParameter.ActualName = ImproverParameter.Name;
      solutionImprover1.Successor = solutionEvaluator1;

      solutionEvaluator1.Name = "SolutionEvaluator";
      solutionEvaluator1.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesProcessor4.Operators.Add(dataReducer1);

      dataReducer1.Name = "Increment EvaluatedSolutions";
      dataReducer1.ParameterToReduce.ActualName = "LocalEvaluatedSolutions";
      dataReducer1.TargetParameter.ActualName = EvaluatedSolutionsParameter.Name;
      dataReducer1.ReductionOperation.Value = new ReductionOperation(ReductionOperations.Sum);
      dataReducer1.TargetOperation.Value = new ReductionOperation(ReductionOperations.Sum);

      solutionPoolUpdateMethod.QualityParameter.ActualName = QualityParameter.ActualName;
      solutionPoolUpdateMethod.Successor = analyzer;

      analyzer.Name = "Analyzer";
      analyzer.OperatorParameter.ActualName = AnalyzerParameter.Name;

      newSolutionsBranch.Name = "NewSolutionsChecker";
      newSolutionsBranch.ConditionParameter.ActualName = "NewSolutions";
      newSolutionsBranch.TrueBranch = subScopesProcessor1;
      newSolutionsBranch.FalseBranch = populationRebuildMethod;

      populationRebuildMethod.QualityParameter.ActualName = QualityParameter.ActualName;
      populationRebuildMethod.Successor = subScopesProcessor3;

      subScopesProcessor3.DepthParameter.Value = new IntValue(1);
      subScopesProcessor3.Operators.Add(solutionsCreator);
      subScopesProcessor3.Operators.Add(new EmptyOperator());
      subScopesProcessor3.Successor = iterationsCounter;

      solutionsCreator.Name = "DiversificationGenerationMethod";
      solutionsCreator.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      solutionsCreator.Successor = uniformSubScopesProcessor3;

      uniformSubScopesProcessor3.DepthParameter.Value = new IntValue(1);
      uniformSubScopesProcessor3.Operator = solutionImprover2;
      uniformSubScopesProcessor3.ParallelParameter.Value = new BoolValue(true);
      uniformSubScopesProcessor3.Successor = dataReducer2;

      solutionImprover2.Name = "SolutionImprover";
      solutionImprover2.OperatorParameter.ActualName = ImproverParameter.Name;
      solutionImprover2.Successor = solutionEvaluator2;

      solutionEvaluator2.Name = "SolutionEvaluator";
      solutionEvaluator2.OperatorParameter.ActualName = EvaluatorParameter.Name;

      dataReducer2.Name = "Increment EvaluatedSolutions";
      dataReducer2.ParameterToReduce.ActualName = "LocalEvaluatedSolutions";
      dataReducer2.TargetParameter.ActualName = EvaluatedSolutionsParameter.Name;
      dataReducer2.ReductionOperation.Value = new ReductionOperation(ReductionOperations.Sum);
      dataReducer2.TargetOperation.Value = new ReductionOperation(ReductionOperations.Sum);

      iterationsCounter.Name = "IterationCounter";
      iterationsCounter.IncrementParameter.Value = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = IterationsParameter.Name;
      iterationsCounter.Successor = resultsCollector;
      #endregion
    }

    public override IOperation Apply() {
      if (ImproverParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
