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
    public override int Priority => 20;
    public override Type ConvertableType => HEAL.Attic.Mapper.StaticCache.GetType(new Guid("EE612297-B1AF-42D2-BF21-AF9A2D42791C"));

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      // TODO: inject data
      throw new NotImplementedException();
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IJsonItem item = new JsonItem() { Name = value.ItemName };

      dynamic val = (dynamic)value;
      object dataset = (object)val.Dataset;
      dynamic targetVariable = val.TargetVariable;
      FieldInfo dataInfo = dataset.GetType().GetField("storableData", flags);
      // TODO: aufteilen in trainings und test daten abschnitte
      item.AddChilds(new JsonItem() {
        Name = "Dataset",
        Value = dataInfo.GetValue(dataset)
      });

      IEnumerable<StringValue> variables = (IEnumerable<StringValue>)val.InputVariables;
      item.AddChilds(new JsonItem() {
        Name = "TargetVariable",
        Value = (object)targetVariable,
        Range = variables.Select(x => x.Value)
      });


      item.AddChilds(new JsonItem() {
        Name = "AllowedInputVariables",
        Value = (object)val.AllowedInputVariables,
        Range = variables.Select(x => x.Value)
      });
      return item;
    }
  }
}
