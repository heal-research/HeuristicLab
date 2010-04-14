using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using HeuristicLab.Data;

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
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      //variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));
      UniformSubScopesProcessor uniformSubScopesProcessor = new UniformSubScopesProcessor();
      Placeholder encoder = new Placeholder();

      encoder.Name = "Encoder (placeholder)";
      encoder.OperatorParameter.ActualName = EncoderParameter.Name;
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = uniformSubScopesProcessor;
      uniformSubScopesProcessor.Operator = encoder;
      uniformSubScopesProcessor.Successor = null;
      #endregion
    }
  }
}
