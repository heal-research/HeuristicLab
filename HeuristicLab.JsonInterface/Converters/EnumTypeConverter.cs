using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {
  public class EnumTypeConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(EnumValue<>);

    public override void InjectData(IItem item, JsonItem data, IJsonItemConverter root) =>
      ((dynamic)item).Value = Enum.Parse(
        item.GetType().GenericTypeArguments.First(), 
        CastValue<string>(data.Value));

    public override void Populate(IItem value, JsonItem item, IJsonItemConverter root) {
      object val = ((dynamic)value).Value;
      Type enumType = val.GetType();
      item.Value = Enum.GetName(enumType, val);
      item.Range = Enum.GetNames(enumType);
    }
  }
}
