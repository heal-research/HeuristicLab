#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.VariableNeighborhoodSearch {
  /// <summary>
  /// An operator which represents a variable neighborhood search.
  /// </summary>
  [Item("VariableNeighborhoodSearchMainLoop", "An operator which represents the main loop of a variable neighborhood search.")]
  [StorableClass]
  public sealed class VariableNeighborhoodSearchMainLoop : AlgorithmOperator {
    #region Parameter properties
    public IValueLookupParameter<IRandom> RandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public IValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    public IValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (IValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    public IValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public ILookupParameter<IntValue> CurrentNeighborhoodIndexParameter {
      get { return (ILookupParameter<IntValue>)Parameters["CurrentNeighborhoodIndex"]; }
    }
    public ILookupParameter<IntValue> NeighborhoodCountParameter {
      get { return (ILookupParameter<IntValue>)Parameters["NeighborhoodCount"]; }
    }
    public IValueLookupParameter<ILocalImprovementOperator> LocalImprovementParameter {
      get { return (IValueLookupParameter<ILocalImprovementOperator>)Parameters["LocalImprovement"]; }
    }
    public IValueLookupParameter<IMultiNeighborhoodShakingOperator> ShakingOperatorParameter {
      get { return (IValueLookupParameter<IMultiNeighborhoodShakingOperator>)Parameters["ShakingOperator"]; }
    }
    #endregion

    [StorableConstructor]
    private VariableNeighborhoodSearchMainLoop(bool deserializing) : base(deserializing) { }
    public VariableNeighborhoodSearchMainLoop()
      : base() {
      Initialize();
    }
    private VariableNeighborhoodSearchMainLoop(VariableNeighborhoodSearchMainLoop original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new VariableNeighborhoodSearchMainLoop(this, cloner);
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "The iterations to count."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze the solution."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated solutions."));
      Parameters.Add(new ValueLookupParameter<ILocalImprovementOperator>("LocalImprovement", "The local improvement operation."));
      Parameters.Add(new ValueLookupParameter<IMultiNeighborhoodShakingOperator>("ShakingOperator", "The shaking operation."));
      Parameters.Add(new LookupParameter<IntValue>("CurrentNeighborhoodIndex", "The index of the current shaking operation that should be applied."));
      Parameters.Add(new LookupParameter<IntValue>("NeighborhoodCount", "The number of neighborhood operators used for shaking."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      SubScopesProcessor subScopesProcessor0 = new SubScopesProcessor();
      Assigner bestQualityInitializer = new Assigner();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();

      CombinedOperator iteration = new CombinedOperator();
      Assigner iterationInit = new Assigner();

      SubScopesCloner createChild = new SubScopesCloner();
      SubScopesProcessor childProcessor = new SubScopesProcessor();

      Assigner qualityAssigner = new Assigner();
      Placeholder shaking = new Placeholder();
      Placeholder localImprovement = new Placeholder();
      Placeholder evaluator = new Placeholder();
      IntCounter evalCounter = new IntCounter();

      QualityComparator qualityComparator = new QualityComparator();
      ConditionalBranch improvesQualityBranch = new ConditionalBranch();

      Assigner bestQualityUpdater = new Assigner();

      BestSelector bestSelector = new BestSelector();
      RightReducer rightReducer = new RightReducer();

      IntCounter indexCounter = new IntCounter();
      Assigner indexResetter = new Assigner();

      Placeholder analyzer2 = new Placeholder();

      Comparator indexComparator = new Comparator();
      ConditionalBranch indexTermination = new ConditionalBranch();

      IntCounter iterationsCounter = new IntCounter();
      Comparator iterationsComparator = new Comparator();
      ConditionalBranch iterationsTermination = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<BoolValue>("IsBetter", new BoolValue(false)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(0)));

      bestQualityInitializer.Name = "Initialize BestQuality";
      bestQualityInitializer.LeftSideParameter.ActualName = "BestQuality";
      bestQualityInitializer.RightSideParameter.ActualName = QualityParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CopyValue = new BoolValue(false);
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      iteration.Name = "MainLoop Body";

      iterationInit.Name = "Init k = 0";
      iterationInit.LeftSideParameter.ActualName = CurrentNeighborhoodIndexParameter.Name;
      iterationInit.RightSideParameter.Value = new IntValue(0);

      createChild.Name = "Clone solution";

      qualityAssigner.Name = "Assign quality";
      qualityAssigner.LeftSideParameter.ActualName = "OriginalQuality";
      qualityAssigner.RightSideParameter.ActualName = QualityParameter.Name;

      shaking.Name = "Shaking operator (placeholder)";
      shaking.OperatorParameter.ActualName = ShakingOperatorParameter.Name;

      localImprovement.Name = "Local improvement operator (placeholder)";
      localImprovement.OperatorParameter.ActualName = LocalImprovementParameter.Name;

      evaluator.Name = "Evaluation operator (placeholder)";
      evaluator.OperatorParameter.ActualName = EvaluatorParameter.Name;

      evalCounter.Name = "Count evaluations";
      evalCounter.Increment.Value = 1;
      evalCounter.ValueParameter.ActualName = EvaluatedSolutionsParameter.ActualName;

      qualityComparator.LeftSideParameter.ActualName = QualityParameter.Name;
      qualityComparator.RightSideParameter.ActualName = "OriginalQuality";
      qualityComparator.ResultParameter.ActualName = "IsBetter";

      improvesQualityBranch.ConditionParameter.ActualName = "IsBetter";

      bestQualityUpdater.Name = "Update BestQuality";
      bestQualityUpdater.LeftSideParameter.ActualName = "BestQuality";
      bestQualityUpdater.RightSideParameter.ActualName = QualityParameter.Name;

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.Value = new IntValue(1);
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;

      indexCounter.Name = "Count neighborhood index";
      indexCounter.Increment.Value = 1;
      indexCounter.ValueParameter.ActualName = CurrentNeighborhoodIndexParameter.Name;

      indexResetter.Name = "Reset neighborhood index";
      indexResetter.LeftSideParameter.ActualName = CurrentNeighborhoodIndexParameter.Name;
      indexResetter.RightSideParameter.Value = new IntValue(0);

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      iterationsCounter.Name = "Iterations Counter";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.ValueParameter.ActualName = IterationsParameter.Name;

      iterationsComparator.Name = "Iterations >= MaximumIterations";
      iterationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      iterationsComparator.LeftSideParameter.ActualName = IterationsParameter.Name;
      iterationsComparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      iterationsComparator.ResultParameter.ActualName = "Terminate";

      iterationsTermination.Name = "Iterations Termination Condition";
      iterationsTermination.ConditionParameter.ActualName = "Terminate";

      indexComparator.Name = "k < k_max (index condition)";
      indexComparator.LeftSideParameter.ActualName = CurrentNeighborhoodIndexParameter.Name;
      indexComparator.RightSideParameter.ActualName = NeighborhoodCountParameter.Name;
      indexComparator.Comparison = new Comparison(ComparisonType.Less);
      indexComparator.ResultParameter.ActualName = "ContinueIteration";

      indexTermination.Name = "Index Termination Condition";
      indexTermination.ConditionParameter.ActualName = "ContinueIteration";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = subScopesProcessor0;
      subScopesProcessor0.Operators.Add(bestQualityInitializer);
      subScopesProcessor0.Successor = analyzer1;
      analyzer1.Successor = resultsCollector1;
      /////////
      resultsCollector1.Successor = iteration;

      iteration.OperatorGraph.InitialOperator = iterationInit;
      iteration.Successor = iterationsCounter;
      iterationInit.Successor = createChild;

      createChild.Successor = childProcessor;
      childProcessor.Operators.Add(new EmptyOperator());
      childProcessor.Operators.Add(qualityAssigner);
      childProcessor.Successor = bestSelector;
      /////////
      qualityAssigner.Successor = shaking;
      shaking.Successor = evaluator;
      evaluator.Successor = evalCounter;
      evalCounter.Successor = localImprovement;
      localImprovement.Successor = qualityComparator;
      qualityComparator.Successor = improvesQualityBranch;
      improvesQualityBranch.TrueBranch = bestQualityUpdater;
      improvesQualityBranch.FalseBranch = indexCounter;

      bestQualityUpdater.Successor = indexResetter;
      indexResetter.Successor = null;

      indexCounter.Successor = null;
      /////////
      bestSelector.Successor = rightReducer;
      rightReducer.Successor = analyzer2;
      analyzer2.Successor = indexComparator;
      indexComparator.Successor = indexTermination;
      indexTermination.TrueBranch = createChild;
      indexTermination.FalseBranch = null;

      iterationsCounter.Successor = iterationsComparator;
      iterationsComparator.Successor = iterationsTermination;
      iterationsTermination.TrueBranch = null;
      iterationsTermination.FalseBranch = iteration;
      #endregion
    }

    public override IOperation Apply() {
      if (LocalImprovementParameter.ActualValue == null || ShakingOperatorParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
