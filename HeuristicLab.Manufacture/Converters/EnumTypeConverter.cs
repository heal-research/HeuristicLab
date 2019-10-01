using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Manufacture {
  public class EnumTypeConverter : BaseConverter {
    public override void InjectData(IItem item, JsonItem data) =>
      item.Cast<dynamic>().Value = Enum.Parse(
        item.GetType().GenericTypeArguments.First(), 
        CastValue<string>(data.Default));

    public override JsonItem ExtractData(IItem value) {
      JsonItem data = new JsonItem();
      object val = ((dynamic)value).Value;
      Type enumType = val.GetType();
      data.Default = Enum.GetName(enumType, val);
      data.Range = Enum.GetNames(enumType);
      return data;
    }
  }
}
