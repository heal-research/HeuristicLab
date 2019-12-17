using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {
  public class RegressionProblemDataConverter : BaseConverter {
    private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    public override JsonItem ExtractData(IItem value) {
      JsonItem item = new JsonItem() {
        Path = value.ItemName,
        Children = new List<JsonItem>()
      };
      
      dynamic val = (dynamic)value;
      object dataset = (object)val.Dataset;
      dynamic targetVariable = val.TargetVariable;
      FieldInfo dataInfo = dataset.GetType().GetField("storableData", flags);
      // TODO: aufteilen in trainings und test daten abschnitte
      item.Children.Add(new JsonItem() {
        Name = "Dataset",
        Value = dataInfo.GetValue(dataset),
        Path = "Dataset"
      });

      IEnumerable<StringValue> variables = (IEnumerable<StringValue>)val.InputVariables;
      item.Children.Add(new JsonItem() {
        Name = "TargetVariable",
        Value = (object)targetVariable,
        Range = variables.Select(x => x.Value),
        Path = "TargetVariable"
      });


      item.Children.Add(new JsonItem() {
        Name = "AllowedInputVariables",
        Value = (object)val.AllowedInputVariables,
        Range = variables.Select(x => x.Value),
        Path = "AllowedInputVariables"
      });

      item.UpdatePath();

      return item;
    }

    public override void InjectData(IItem item, JsonItem data) {
      // TODO: inject data
      throw new NotImplementedException();
    }
  }
}
