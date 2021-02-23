using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class RegressionProblemDataConverter : BaseConverter {
    #region Constants
    private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private const string TestPartition = "TestPartition";
    private const string TrainingPartition = "TrainingPartition";
    private const string TargetVariable = "TargetVariable";
    private const string AllowedInputVariables = "AllowedInputVariables";
    private const string Dataset = "Dataset";
    private const string VariableValues = "variableValues";
    private const string VariableNames = "variableNames";
    private const string InputVariables = "InputVariables";
    private const string Rows = "rows";
    private const string Value = "value";
    private const string Parameters = "parameters";
    private const string CheckedItemList = "CheckedItemList";
    #endregion

    public override int Priority => 20;

    public override bool CanConvertType(Type t) =>
      HEAL.Attic.Mapper.StaticCache.GetType(new Guid("EE612297-B1AF-42D2-BF21-AF9A2D42791C")).IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {

      dynamic regressionProblemData = (dynamic)item;

      DoubleMatrixJsonItem dataset = null;
      StringJsonItem targetVariable = null;
      IntRangeJsonItem testPartition = null;
      IntRangeJsonItem trainingPartition = null;
      StringArrayJsonItem allowedInputVariables = null;


      // search first for the items (cache them, because the 
      // order is important for injection)
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

      // inject the value of the items
      SetDataset(regressionProblemData, dataset);
      SetTargetVariable(regressionProblemData, targetVariable);
      SetAllowedInputVariables(regressionProblemData, allowedInputVariables, dataset);
      SetTestPartition(regressionProblemData, testPartition);
      SetTrainingPartition(regressionProblemData, trainingPartition);

    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IJsonItem item = new EmptyJsonItem() { 
        Name = value.ItemName,
        Description = value.ItemDescription
      };

      IJsonItem ds = GetDataset(value);
      if(ds != null)
        item.AddChildren(ds);

      item.AddChildren(GetTestPartition(value));
      item.AddChildren(GetTrainingPartition(value));
      item.AddChildren(GetTargetVariable(value));
      item.AddChildren(GetAllowedInputVariables(value));
      return item;
    }

    #region Inject Helper
    private void SetDataset(dynamic regressionProblemData, DoubleMatrixJsonItem item) {
      if (item != null) {
        var dictTmp = new Dictionary<string, IList>();
        int c = 0;
        foreach (var col in item.ColumnNames) {
          dictTmp.Add(col, new List<double>(item.Value[c]));
          ++c;
        }

        object dataset = (object)regressionProblemData.Dataset;
        var rows = dataset.GetType().GetField(Rows, flags);
        rows.SetValue(dataset, item.Value[0].Length);

        var variableNames = dataset.GetType().GetField(VariableNames, flags);
        variableNames.SetValue(dataset, item.ColumnNames);

        var dataInfo = dataset.GetType().GetField(VariableValues, flags);
        dataInfo.SetValue(dataset, dictTmp);
      }
    }

    private void SetTestPartition(dynamic regressionProblemData, IntRangeJsonItem item) {
      if (item != null) {
        regressionProblemData.TestPartition.Start = item.MinValue;
        regressionProblemData.TestPartition.End = item.MaxValue;
      }
    }

    private void SetTrainingPartition(dynamic regressionProblemData, IntRangeJsonItem item) {
      if (item != null) {
        regressionProblemData.TrainingPartition.Start = item.MinValue;
        regressionProblemData.TrainingPartition.End = item.MaxValue;
      }
    }

    private void SetTargetVariable(dynamic regressionProblemData, StringJsonItem item) {
      if (item != null) {
        var param = (IConstrainedValueParameter<StringValue>)regressionProblemData.TargetVariableParameter;
        StringValue v = param.Value;
        FieldInfo fi = v.GetType().GetField(Value, flags);
        fi.SetValue(v, item.Value);
      }
    }

    private void SetAllowedInputVariables(dynamic regressionProblemData, StringArrayJsonItem item, IMatrixJsonItem matrix) {
      if (item != null && regressionProblemData is IParameterizedItem p) {
        var regProbDataType = ((ParameterizedNamedItem)regressionProblemData).GetType(); //RegressionProblemData

        var parameterizedNamedItemType = regProbDataType.BaseType.BaseType;

        // reset parameter
        var parametersInfo = parameterizedNamedItemType.GetField(Parameters, flags);
        ParameterCollection col = (ParameterCollection)parametersInfo.GetValue((object)regressionProblemData);
        var oldParam = (FixedValueParameter<ReadOnlyCheckedItemList<StringValue>>)col[InputVariables];
        var value = oldParam.Value;
        PropertyInfo listInfo = value.GetType().GetProperty(CheckedItemList, flags);
        CheckedItemList<StringValue> checkedItemList = (CheckedItemList<StringValue>)listInfo.GetValue(value);
        checkedItemList.Clear();

        // add list items and set their check state (based on allowed input variables)
        foreach(var i in matrix.ColumnNames) {
          bool isChecked = false;
          foreach(var x in item.Value)
            isChecked = isChecked || (x == i);
          checkedItemList.Add(new StringValue(i).AsReadOnly(), isChecked);
        }
      }
    }
    #endregion

    #region Extract Helper
    private IJsonItem GetDataset(IItem item) {
      dynamic val = (dynamic)item;
      object dataset = (object)val.Dataset;
      FieldInfo dataInfo = dataset.GetType().GetField(VariableValues, flags);

      if (dataInfo.GetValue(dataset) is Dictionary<string, IList> dict) {
        IEnumerator it = dict.Values.First()?.GetEnumerator();

        if(it != null) {
          if(it.MoveNext() && it.Current is double) {
            CreateMatrix(dict, out IList<string> columnNames, out double[][] mat);
            return new DoubleMatrixJsonItem() {
              Name = Dataset,
              Value = mat,
              ColumnNames = columnNames,
              Minimum = double.MinValue,
              Maximum = double.MaxValue
            };
          } else if(it.Current is int) {
            CreateMatrix(dict, out IList<string> columnNames, out int[][] mat);
            return new IntMatrixJsonItem() {
              Name = Dataset,
              Value = mat,
              ColumnNames = columnNames,
              Minimum = int.MinValue,
              Maximum = int.MaxValue
            };
          } else if (it.Current is bool) {
            CreateMatrix(dict, out IList<string> columnNames, out bool[][] mat);
            return new BoolMatrixJsonItem() {
              Name = Dataset,
              Value = mat,
              ColumnNames = columnNames
            };
          }
        }
      }
      return null;
    }
    
    private void CreateMatrix<T>(Dictionary<string, IList> dict, out IList<string> columnNames, out T[][] matrix) {
      int cols = dict.Count, rows = 0, c = 0;
      columnNames = new List<string>();
      matrix = new T[cols][];
      foreach (var x in dict) {
        rows = Math.Max(rows, x.Value.Count);
        columnNames.Add(x.Key);

        matrix[c] = new T[rows];

        int r = 0;
        foreach (var callValue in x.Value) {
          matrix[c][r] = (T)callValue;
          ++r;
        }
        ++c;
      }
    }

    private IJsonItem GetTestPartition(IItem item) {
      dynamic val = (dynamic)item;
      var trainingPartition = (IntRange)val.TrainingPartition;
      var testPartition = (IntRange)val.TestPartition;
      return new IntRangeJsonItem() {
        Name = TestPartition,
        MinValue = testPartition.Start,
        MaxValue = testPartition.End,
        Minimum = 0,
        Maximum = int.MaxValue //Math.Max(testPartition.End, trainingPartition.End)
      };
    }

    private IJsonItem GetTrainingPartition(IItem item) {
      dynamic val = (dynamic)item;
      var trainingPartition = (IntRange)val.TrainingPartition;
      var testPartition = (IntRange)val.TestPartition;
      return new IntRangeJsonItem() {
        Name = TrainingPartition,
        MinValue = trainingPartition.Start,
        MaxValue = trainingPartition.End,
        Minimum = 0,
        Maximum = int.MaxValue //Math.Max(testPartition.End, trainingPartition.End)
      };
    }


    private IJsonItem GetTargetVariable(IItem item) {
      var vars = (IEnumerable<StringValue>)((dynamic)item).InputVariables;
      return new StringJsonItem() {
        Name = TargetVariable,
        Value = (string)((dynamic)item).TargetVariable,
        ConcreteRestrictedItems = vars.Select(x => x.Value)
      };
    }

    private IJsonItem GetAllowedInputVariables(IItem item) {
      var vars = (IEnumerable<StringValue>)((dynamic)item).InputVariables;
      return new StringArrayJsonItem() {
        Name = AllowedInputVariables,
        Value = ((IEnumerable<string>)((dynamic)item).AllowedInputVariables).ToArray(),
        ConcreteRestrictedItems = vars.Select(x => x.Value)
      };
    }
    #endregion
  }
}
