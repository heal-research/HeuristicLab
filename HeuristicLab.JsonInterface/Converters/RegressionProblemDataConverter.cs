using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class RegressionProblemDataConverter : BaseConverter {
    private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    public override int Priority => 20;
    public override Type ConvertableType => HEAL.Attic.Mapper.StaticCache.GetType(new Guid("EE612297-B1AF-42D2-BF21-AF9A2D42791C"));

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      var dictTmp = new Dictionary<string, IList>();
      DoubleMatrixJsonItem matrix = data.Children[0] as DoubleMatrixJsonItem;
      if(matrix != null) {
        int c = 0;
        foreach(var col in matrix.RowNames) {
          dictTmp.Add(col, new List<double>(matrix.Value[c]));
          ++c;
        }
      }

      dynamic val = (dynamic)item;
      object dataset = (object)val.Dataset;
      var rows = dataset.GetType().GetField("rows", flags);
      rows.SetValue(dataset, matrix.Value[0].Length);

      var variableNames = dataset.GetType().GetField("variableNames", flags);
      variableNames.SetValue(dataset, matrix.RowNames);

      var dataInfo = dataset.GetType().GetField("variableValues", flags);
      dataInfo.SetValue(dataset, dictTmp);
      val.TargetVariable = ((StringJsonItem)data.Children[3]).Value;
      val.TrainingPartition.Start = ((IntRangeJsonItem)data.Children[2]).MinValue;
      val.TrainingPartition.End = ((IntRangeJsonItem)data.Children[2]).MaxValue;
      val.TestPartition.Start = ((IntRangeJsonItem)data.Children[1]).MinValue;
      val.TestPartition.End = ((IntRangeJsonItem)data.Children[1]).MaxValue;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IJsonItem item = new EmptyJsonItem() { 
        Name = value.ItemName,
        Description = value.ItemDescription
      };

      dynamic val = (dynamic)value;
      object dataset = (object)val.Dataset;
      dynamic targetVariable = val.TargetVariable;
      
      FieldInfo dataInfo = dataset.GetType().GetField("variableValues", flags);
      
      if(dataInfo.GetValue(dataset) is Dictionary<string, IList> dict) {
        int cols = dict.Count;
        int rows = 0;
        IList<string> rowNames = new List<string>();
        double[][] mat = new double[cols][];
        int c = 0;
        foreach(var x in dict) {
          rows = Math.Max(rows, x.Value.Count);
          rowNames.Add(x.Key);
          mat[c] = new double[rows];
          int r = 0;
          foreach(var rowValue in x.Value) {
            // TODO: for integers and bools aswell
            mat[c][r] = (double)rowValue; 
            ++r;
          }
          ++c;
        }
        item.AddChildren(new DoubleMatrixJsonItem() {
          Name = "Dataset",
          Value = mat,
          RowNames = rowNames,
          Minimum = double.MinValue,
          Maximum = double.MaxValue
        });
      }

      var trainingPartition = ((IntRange)val.TrainingPartition);
      var testPartition = ((IntRange)val.TestPartition);

      item.AddChildren(new IntRangeJsonItem() {
        Name = "TestPartition",
        MinValue = testPartition.Start, 
        MaxValue = testPartition.End,
        Minimum = 0, 
        Maximum = Math.Max(testPartition.End, trainingPartition.End)
      });

      
      item.AddChildren(new IntRangeJsonItem() {
        Name = "TrainingPartition",
        MinValue = trainingPartition.Start,
        MaxValue = trainingPartition.End,
        Minimum = 0,
        Maximum = Math.Max(testPartition.End, trainingPartition.End)
      });

      IEnumerable<StringValue> variables = (IEnumerable<StringValue>)val.InputVariables;
      item.AddChildren(new StringJsonItem() {
        Name = "TargetVariable",
        Value = (string)targetVariable,
        ConcreteRestrictedItems = variables.Select(x => x.Value)
      });

      /*
      item.AddChildren(new StringArrayJsonItem() {
        Name = "AllowedInputVariables",
        Value = (string[])val.AllowedInputVariables,
        Range = variables.Select(x => x.Value)
      });*/
      return item;
    }
  }
}
