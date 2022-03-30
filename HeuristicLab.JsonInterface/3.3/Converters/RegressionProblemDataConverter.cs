//using System;
//using System.Collections.Generic;
//using System.Linq;
//using HeuristicLab.Core;
//using HeuristicLab.Data;
//using HeuristicLab.Parameters;
//using HeuristicLab.Problems.DataAnalysis;

//namespace HeuristicLab.JsonInterface {
//  public class RegressionProblemDataConverter : BaseConverter {
//    #region Constants
//    private const string TestPartition = "TestPartition";
//    private const string TrainingPartition = "TrainingPartition";
//    private const string TargetVariable = "TargetVariable";
//    private const string AllowedInputVariables = "AllowedInputVariables";
//    private const string Dataset = "Dataset";
//    #endregion

//    public override int Priority => 20;

//    public override bool CanConvertType(Type t) => t == typeof(ValueParameter<IRegressionProblemData>);

//    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
//      if (data.Children.All(x => !x.Active))
//        return;

//      var parameter = item as ValueParameter<IRegressionProblemData>;

//      DoubleMatrixJsonItem datasetItem = null;
//      StringJsonItem targetVariableItem = null;
//      IntRangeJsonItem testPartitionItem = null;
//      IntRangeJsonItem trainingPartitionItem = null;
//      StringArrayJsonItem allowedInputVariablesItem = null;

//      // get all child items
//      foreach (var child in data.Children) {
//        if (!child.Active)
//          continue;

//        if (child.Path.EndsWith(Dataset))
//          datasetItem = child as DoubleMatrixJsonItem;
//        else if (child.Path.EndsWith(TargetVariable))
//          targetVariableItem = child as StringJsonItem;
//        else if (child.Path.EndsWith(TestPartition))
//          testPartitionItem = child as IntRangeJsonItem;
//        else if (child.Path.EndsWith(TrainingPartition))
//          trainingPartitionItem = child as IntRangeJsonItem;
//        else if (child.Path.EndsWith(AllowedInputVariables))
//          allowedInputVariablesItem = child as StringArrayJsonItem;
//      }

//      // check data
//      if(!datasetItem.ColumnNames.Any(x => x == targetVariableItem.Value)) {
//        throw new Exception($"The value of the target variable ('{targetVariableItem.Value}') has no matching row name value of the dataset.");
//      }

//      foreach(var v in allowedInputVariablesItem.Value) {
//        if(!datasetItem.ColumnNames.Any(x => x == v))
//          throw new Exception($"The value of the input variable ('{v}') has no matching row name value of the dataset.");
//      }

//      // create the new problemData object
//      var dataset = datasetItem == null ? 
//        parameter.Value.Dataset : 
//        new Dataset(datasetItem.ColumnNames, datasetItem.Value);

//      var allowedInputVariables = allowedInputVariablesItem == null ? 
//        parameter.Value.AllowedInputVariables : 
//        allowedInputVariablesItem.Value;

//      var targetVariable = targetVariableItem == null ?
//        parameter.Value.TargetVariable :
//        targetVariableItem.Value;

//      var problemData = new RegressionProblemData(
//        dataset,
//        allowedInputVariables,
//        targetVariable);

//      // set the new problemData
//      parameter.Value = problemData;
//    }

//    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
//      var parameter = value as ValueParameter<IRegressionProblemData>;
//      var problemData = parameter.Value;

//      IJsonItem item = new EmptyJsonItem() { 
//        Name = parameter.Name,
//        Description = problemData.ItemDescription
//      };

//      item.AddChildren(GetDataset(parameter.Value.Dataset)); 
//      item.AddChildren(GetTestPartition(problemData.TestPartition));
//      item.AddChildren(GetTrainingPartition(problemData.TrainingPartition));
//      item.AddChildren(GetTargetVariable(problemData.TargetVariable, problemData.Dataset.VariableNames));
//      item.AddChildren(GetAllowedInputVariables(problemData.AllowedInputVariables, problemData.Dataset.VariableNames));

//      return item;
//    }

//    #region Extract Helper
//    private IJsonItem GetDataset(IDataset dataset) {
//      int variableCount = dataset.VariableNames.Count();
//      var matrix = new double[variableCount][];
//      int count = 0;

//      foreach(var variable in dataset.VariableNames) {
//        if(count < variableCount) {
//          matrix[count] = dataset.GetDoubleValues(variable).ToArray();
//          count++;
//        }
//      }

//      return new DoubleMatrixJsonItem() {
//        Name = Dataset,
//        Value = matrix,
//        ColumnNames = dataset.ColumnNames,
//        Minimum = double.MinValue,
//        Maximum = double.MaxValue
//      };
//    }

//    private IJsonItem GetTestPartition(IntRange testPartition) =>
//      new IntRangeJsonItem() {
//        Name = TestPartition,
//        Start = testPartition.Start,
//        End = testPartition.End,
//        Minimum = 0,
//        Maximum = int.MaxValue
//      };

//    private IJsonItem GetTrainingPartition(IntRange trainingPartition) =>
//      new IntRangeJsonItem() {
//        Name = TrainingPartition,
//        Start = trainingPartition.Start,
//        End = trainingPartition.End,
//        Minimum = 0,
//        Maximum = int.MaxValue
//      };


//    private IJsonItem GetTargetVariable(string targetVariable, IEnumerable<string> variables) =>
//      new StringJsonItem() {
//        Name = TargetVariable,
//        Value = targetVariable,
//        ConcreteRestrictedItems = variables
//      };

//    private IJsonItem GetAllowedInputVariables(IEnumerable<string> allowedVariables, IEnumerable<string> variables) =>
//      new StringArrayJsonItem() {
//        Name = AllowedInputVariables,
//        Value = allowedVariables.ToArray(),
//        ConcreteRestrictedItems = variables
//      };
//    #endregion
//  }
//}