using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Analysis;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Encodings.RealVectorEncoding;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  /// <summary>
  /// An operator which represents the main loop of a PSO.
  /// </summary> 
  [Item("ParticleSwarmOptimizationMainLoop", "An operator which represents the main loop of a PSO.")]
  [StorableClass]
  public class ParticleSwarmOptimizationMainLoop : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    public ValueLookupParameter<IOperator> EncoderParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Encoder"]; }
    }
    public ValueLookupParameter<IOperator> DecoderParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Decoder"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    public ValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    #endregion

    [StorableConstructor]
    private ParticleSwarmOptimizationMainLoop(bool deserializing) : base() { }
    public ParticleSwarmOptimizationMainLoop()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Encoder", "The encoding operator that maps a solution to a position vector."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Decoder", "The decoding operator that maps a position vector to a solution."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze each generation."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      IntCounter intCounter = new IntCounter();
      ConditionalBranch conditionalBranch = new ConditionalBranch();
      ParticleUpdater velocityUpdater = new ParticleUpdater(); 
      UniformSubScopesProcessor uniformSubScopesProcessor = new UniformSubScopesProcessor();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      Placeholder encPlaceholder = new Placeholder();
      Placeholder decPlaceholder = new Placeholder(); 
      Placeholder evaluator = new Placeholder();
      Comparator comparator = new Comparator();
      SwarmUpdater swarmUpdater = new SwarmUpdater();
      Placeholder analyzer1 = new Placeholder();

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(1))); // Initial generation already built

      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.ResultsParameter.ActualName = "Results";

      intCounter.Increment = new IntValue(1);
      intCounter.ValueParameter.ActualName = "Generations";

      comparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      comparator.LeftSideParameter.ActualName = "Generations";
      comparator.ResultParameter.ActualName = "Terminate";
      comparator.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;

      conditionalBranch.ConditionParameter.ActualName = "Terminate";

      velocityUpdater.BestGlobalParameter.ActualName = "CurrentBestPosition";
      velocityUpdater.BestLocalParameter.ActualName = "BestPosition";
      velocityUpdater.CurrentPositionParameter.ActualName = "Position";
      velocityUpdater.VelocityParameter.ActualName = "Velocity";

      encPlaceholder.OperatorParameter.ActualName = EncoderParameter.ActualName;
      decPlaceholder.OperatorParameter.ActualName = DecoderParameter.ActualName; 

      evaluator.Name = "Evaluator (placeholder)";
      evaluator.OperatorParameter.ActualName = EvaluatorParameter.Name;

      swarmUpdater.CurrentPositionParameter.ActualName = "Position";
      swarmUpdater.CurrentQualityParameter.ActualName = "Quality";
      swarmUpdater.BestGlobalParameter.ActualName = "CurrentBestPosition";
      swarmUpdater.BestLocalParameter.ActualName = "BestPosition";
      swarmUpdater.LocalBestQualityParameter.ActualName = "BestQuality";
      swarmUpdater.GlobalBestQualityParameter.ActualName = "CurrentBestBestQuality";

      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = comparator;
      comparator.Successor = conditionalBranch;
      conditionalBranch.FalseBranch = uniformSubScopesProcessor;
      uniformSubScopesProcessor.Operator = velocityUpdater;
      velocityUpdater.Successor = decPlaceholder;
      uniformSubScopesProcessor.Successor = uniformSubScopesProcessor2;
      uniformSubScopesProcessor2.Operator = evaluator;
      evaluator.Successor = swarmUpdater;
      swarmUpdater.Successor = intCounter;
      intCounter.Successor = resultsCollector1;
      resultsCollector1.Successor = comparator; //analyzer1; 
      //analyzer1.Successor = comparator;
      #endregion
    }
  }
}
