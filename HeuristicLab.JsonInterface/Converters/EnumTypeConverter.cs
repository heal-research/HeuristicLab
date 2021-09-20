using System;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {
  public class EnumTypeConverter : BaseConverter {
    public override int Priority => 1;

    public override bool CanConvertType(Type t) =>
      typeof(EnumValue<>).IsAssignableFrom(t) || 
      (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(EnumValue<>));

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      ((dynamic)item).Value = (dynamic)Enum.Parse(
        item.GetType().GenericTypeArguments.First(),
        ((StringJsonItem)data).Value);
    }
      
    
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
