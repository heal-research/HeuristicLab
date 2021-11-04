using System;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {
  public class StringValueConverter : BaseConverter {
    public override int Priority => 1;

    public override bool CanConvertType(Type t) =>
      typeof(StringValue).IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      if(data.Active)
        ((StringValue)item).Value = ((StringJsonItem)data).Value;
    }
      

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new StringJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((StringValue)value).Value
      };
  }
}
