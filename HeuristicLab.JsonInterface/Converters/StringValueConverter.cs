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

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((StringValue)item).Value = ((StringJsonItem)data).Value;

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new StringJsonItem() {
        Name = value.ItemName,
        Value = ((StringValue)value).Value
      };
  }
}
