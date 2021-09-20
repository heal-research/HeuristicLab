using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.JsonInterface {
  public class RegressionProblemDataConverter : BaseConverter {
    #region Constants
    private const string TestPartition = "TestPartition";
    private const string TrainingPartition = "TrainingPartition";
    private const string TargetVariable = "TargetVariable";
    private const string AllowedInputVariables = "AllowedInputVariables";
    private const string Dataset = "Dataset";
    #endregion

    public override int Priority => 20;

    public override bool CanConvertType(Type t) => t == typeof(ValueParameter<IRegressionProblemData>);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      var parameter = item as ValueParameter<IRegressionProblemData>;

      DoubleMatrixJsonItem dataset = null;
      StringJsonItem targetVariable = null;
      IntRangeJsonItem testPartition = null;
      IntRangeJsonItem trainingPartition = null;
      StringArrayJsonItem allowedInputVariables = null;

      // get all child items
      foreach (var child in data.Children) {
        if (child.Path.EndsWith(Dataset))
          dataset = child as DoubleMatrixJsonItem;
        else if (child.Path.EndsWith(TargetVariable))
          targetVariable = child as StringJsonItem;
        else if (child.Path.EndsWith(TestPartition))
          testPartition = child as IntRangeJsonItem;
        else if (child.Path.EndsWith(TrainingPartition))
          trainingPartition = child as IntRangeJsonItem;
        else if (child.Path.EndsWith(AllowedInputVariables))
          allowedInputVariables = child as StringArrayJsonItem;
      }

      // check data
      if(!dataset.ColumnNames.Any(x => x == targetVariable.Value)) {
        throw new Exception($"The value of the target variable ('{targetVariable.Value}') has no matching row name value of the dataset.");
      }

      foreach(var v in allowedInputVariables.Value) {
        if(!dataset.ColumnNames.Any(x => x == v))
          throw new Exception($"The value of the input variable ('{v}') has no matching row name value of the dataset.");
      }

      // create the new problemData object
      var problemData = new RegressionProblemData(
        new Dataset(dataset.ColumnNames, dataset.Value), 
        allowedInputVariables.Value, 
        targetVariable.Value);

      // set the new problemData
      parameter.Value = problemData;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      var parameter = value as ValueParameter<IRegressionProblemData>;
      var problemData = parameter.Value;

      IJsonItem item = new EmptyJsonItem() { 
        Name = parameter.Name,
        Description = problemData.ItemDescription
      };

      item.AddChildren(GetDataset(parameter.Value.Dataset)); 
      item.AddChildren(GetTestPartition(problemData.TestPartition));
      item.AddChildren(GetTrainingPartition(problemData.TrainingPartition));
      item.AddChildren(GetTargetVariable(problemData.TargetVariable, problemData.Dataset.VariableNames));
      item.AddChildren(GetAllowedInputVariables(problemData.AllowedInputVariables, problemData.Dataset.VariableNames));

      return item;
    }

    #region Extract Helper
    private IJsonItem GetDataset(IDataset dataset) {
      int variableCount = dataset.VariableNames.Count();
      var matrix = new double[variableCount][];
      int count = 0;

      foreach(var variable in dataset.VariableNames) {
        if(count < variableCount) {
          matrix[count] = dataset.GetDoubleValues(variable).ToArray();
          count++;
        }
      }

      return new DoubleMatrixJsonItem() {
        Name = Dataset,
        Value = matrix,
        ColumnNames = dataset.ColumnNames,
        Minimum = double.MinValue,
        Maximum = double.MaxValue
      };
    }

    private IJsonItem GetTestPartition(IntRange testPartition) =>
      new IntRangeJsonItem() {
        Name = TestPartition,
        MinValue = testPartition.Start,
        MaxValue = testPartition.End,
        Minimum = 0,
        Maximum = int.MaxValue
      };

    private IJsonItem GetTrainingPartition(IntRange trainingPartition) =>
      new IntRangeJsonItem() {
        Name = TrainingPartition,
        MinValue = trainingPartition.Start,
        MaxValue = trainingPartition.End,
        Minimum = 0,
        Maximum = int.MaxValue
      };


    private IJsonItem GetTargetVariable(string targetVariable, IEnumerable<string> variables) =>
      new StringJsonItem() {
        Name = TargetVariable,
        Value = targetVariable,
        ConcreteRestrictedItems = variables
      };

    private IJsonItem GetAllowedInputVariables(IEnumerable<string> allowedVariables, IEnumerable<string> variables) =>
      new StringArrayJsonItem() {
        Name = AllowedInputVariables,
        Value = allowedVariables.ToArray(),
        ConcreteRestrictedItems = variables
      };
    #endregion
  }
}