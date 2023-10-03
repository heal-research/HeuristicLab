using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Dynamic;

[StorableType("92934716-3F99-4102-881F-794EA841A054")]
public enum PartitionsUpdateMode {
  Wrapping,
  KeepLast
}

[Item("DynamicRegressionProblemData", "")]
[StorableType("279D12A9-8E7A-49C6-8C70-B3CBADD85249")]
public class DynamicRegressionProblemData : RegressionProblemData {
  
  public IValueParameter<IntMatrix> TrainingPartitionsParameter => (IValueParameter<IntMatrix>)Parameters["TrainingPartitions"];
  public IValueParameter<IntMatrix> TestPartitionsParameter => (IValueParameter<IntMatrix>)Parameters["TestPartitions"];
  public IFixedValueParameter<EnumValue<PartitionsUpdateMode>> ProgressModeParameter => (IFixedValueParameter<EnumValue<PartitionsUpdateMode>>)Parameters["PartitionsUpdate"];
  
  
  public IntMatrix TrainingPartitions {
    get { return TrainingPartitionsParameter.Value; }
    set { TrainingPartitionsParameter.Value = value; }
  }
  public IntMatrix TestPartitions {
    get { return TestPartitionsParameter.Value; }
    set { TestPartitionsParameter.Value = value; }
  }
  public PartitionsUpdateMode PartitionsUpdate {
    get { return ProgressModeParameter.Value.Value; }
    set { ProgressModeParameter.Value.Value = value; }
  }


  public DynamicRegressionProblemData(IDataset dataset, IEnumerable<string> allowedInputVariables, string targetVariable) 
    : base(dataset, allowedInputVariables, targetVariable) {
    Parameters.Add(new ValueParameter<IntMatrix>("TrainingPartitions", new IntMatrix()));
    Parameters.Add(new ValueParameter<IntMatrix>("TestPartitions",     new IntMatrix()));
    Parameters.Add(new FixedValueParameter<EnumValue<PartitionsUpdateMode>>("PartitionsUpdate", new EnumValue<PartitionsUpdateMode>(PartitionsUpdateMode.KeepLast)));
    
    TrainingPartitionParameter.Hidden = true;
    TestPartitionParameter.Hidden = true;
  }
  
  public DynamicRegressionProblemData() : base() {
    Parameters.Add(new ValueParameter<IntMatrix>("TrainingPartitions", new IntMatrix(new int[4, 2] { { 0,  5}, { 5, 10}, {10, 15}, {15, 20} })));
    Parameters.Add(new ValueParameter<IntMatrix>("TestPartitions",     new IntMatrix(new int[4, 2] { { 5, 10}, {10, 15}, {15, 20}, {20, 25} })));
    Parameters.Add(new FixedValueParameter<EnumValue<PartitionsUpdateMode>>("PartitionsUpdate", new EnumValue<PartitionsUpdateMode>(PartitionsUpdateMode.KeepLast)));

    TrainingPartitionParameter.Hidden = true;
    TestPartitionParameter.Hidden = true;
  }
  
  [StorableConstructor] protected DynamicRegressionProblemData(StorableConstructorFlag _) : base(_) { }
  protected DynamicRegressionProblemData(RegressionProblemData original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new DynamicRegressionProblemData(this, cloner); }
}
