using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace ParameterTest {
  public class EnumTypeTransformer : BaseTransformer {
    public override IItem FromData(ParameterData obj, Type targetType) {
      Type enumType = targetType.GenericTypeArguments.First();
      var data = Enum.Parse(enumType, CastValue<string>(obj.Default));
      //item.Cast<dynamic>().Value = data;
      return Instantiate(targetType, data);
    }

    public override void SetValue(IItem item, ParameterData data) {
      Type enumType = item.GetType().GenericTypeArguments.First();
      item.Cast<dynamic>().Value = Enum.Parse(enumType, CastValue<string>(data.Default));
    }

    public override ParameterData ToData(IItem value) {
      ParameterData data = base.ToData(value);
      object val = ((dynamic)value).Value;
      Type enumType = val.GetType();
      data.Default = Enum.GetName(enumType, val);
      data.Range = Enum.GetNames(enumType);
      return data;
    }
  }
}
