using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {
  public class StringValueConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(StringValue);

    public override void InjectData(IItem item, JsonItem data, IJsonItemConverter root) =>
      ((StringValue)item).Value = CastValue<string>(data.Value);

    public override void Populate(IItem value, JsonItem item, IJsonItemConverter root) =>
      item.Value = ((StringValue)value).Value;
  }
}
