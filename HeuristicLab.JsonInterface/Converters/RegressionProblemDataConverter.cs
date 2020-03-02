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
      DoubleNamedMatrixJsonItem matrix = data.Children[0] as DoubleNamedMatrixJsonItem;
      if(matrix != null) {
        int c = 0;
        foreach(var col in matrix.RowNames) {
          dictTmp.Add(col, new List<double>(matrix.Value[c]));
          ++c;
        }
      }

      dynamic val = (dynamic)item;
      object dataset = (object)val.Dataset;
      var dataInfo = dataset.GetType().GetField("variableValues", flags);
      dataInfo.SetValue(dataset, dictTmp);
      val.TargetVariable = (string)data.Children[3].Value;
      val.TrainingPartition.Start = ((IntRangeJsonItem)data.Children[2]).Value.First();
      val.TrainingPartition.End = ((IntRangeJsonItem)data.Children[2]).Value.Last();
      val.TestPartition.Start = ((IntRangeJsonItem)data.Children[1]).Value.First();
      val.TestPartition.End = ((IntRangeJsonItem)data.Children[1]).Value.Last();
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IJsonItem item = new JsonItem() { 
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
        item.AddChildren(new DoubleNamedMatrixJsonItem() {
          Name = "Dataset",
          Value = mat,
          RowNames = rowNames
        });
      }

      var trainingPartition = ((IntRange)val.TrainingPartition);
      var testPartition = ((IntRange)val.TestPartition);

      item.AddChildren(new IntRangeJsonItem() {
        Name = "TestPartition",
        Value = new int[] { testPartition.Start, testPartition.End },
        Range = new int[] { 0, Math.Max(testPartition.End, trainingPartition.End) }
      });

      
      item.AddChildren(new IntRangeJsonItem() {
        Name = "TrainingPartition",
        Value = new int[] { trainingPartition.Start, trainingPartition.End },
        Range = new int[] { 0, Math.Max(testPartition.End, trainingPartition.End)}
      });

      IEnumerable<StringValue> variables = (IEnumerable<StringValue>)val.InputVariables;
      item.AddChildren(new JsonItem() {
        Name = "TargetVariable",
        Value = (object)targetVariable,
        Range = variables.Select(x => x.Value)
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
