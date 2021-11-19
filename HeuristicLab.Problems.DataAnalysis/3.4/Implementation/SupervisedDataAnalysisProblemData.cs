using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis {
  public abstract class SupervisedDataAnalysisProblemData : DataAnalysisProblemData {
    protected const string TargetVariableParameterName = "TargetVariable";

    public IConstrainedValueParameter<StringValue> TargetVariableParameter => (IConstrainedValueParameter<StringValue>)Parameters[TargetVariableParameterName];

    public string TargetVariable {
      get { return TargetVariableParameter.Value.Value; }
      set {
        if (value == null) throw new ArgumentNullException("targetVariable", "The provided value for the targetVariable is null.");
        if (value == TargetVariable) return;

        var matchingParameterValue = TargetVariableParameter.ValidValues.FirstOrDefault(v => v.Value == value);
        if (matchingParameterValue == null) throw new ArgumentException("The provided value is not valid as the targetVariable.", "targetVariable");
        TargetVariableParameter.Value = matchingParameterValue;
      }
    }
    public IEnumerable<double> TargetVariableValues => Dataset.GetDoubleValues(TargetVariable);
    public IEnumerable<double> TargetVariableTrainingValues => Dataset.GetDoubleValues(TargetVariable, TrainingIndices);
    public IEnumerable<double> TargetVariableTestValues => Dataset.GetDoubleValues(TargetVariable, TestIndices);


    public SupervisedDataAnalysisProblemData(IDataset dataset, IEnumerable<string> allowedInputVariables, IEnumerable<ITransformation> transformations = null, IntervalCollection variableRanges = null) : base(dataset, allowedInputVariables, transformations, variableRanges) { }
    protected SupervisedDataAnalysisProblemData(SupervisedDataAnalysisProblemData original, Cloner cloner) : base(original, cloner) { }

    [StorableConstructor]
    protected SupervisedDataAnalysisProblemData(StorableConstructorFlag _) : base(_) { }

  }
}
