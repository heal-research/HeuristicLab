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

    public override bool CanConvertType(Type t) =>
      typeof(EnumValue<>).IsAssignableFrom(t) || 
      (t.IsGenericType && t.GetGenericTypeDefinition() == ConvertableType);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((dynamic)item).Value = Enum.Parse(
        item.GetType().GenericTypeArguments.First(), 
        ((StringJsonItem)data).Value);
    
    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      object val = ((dynamic)value).Value;
      Type enumType = val.GetType();
      return new StringJsonItem() { 
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = Enum.GetName(enumType, val),
        ConcreteRestrictedItems = Enum.GetNames(enumType)
      };
    }
  }
}
