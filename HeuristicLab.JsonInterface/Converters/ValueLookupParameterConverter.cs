using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ValueLookupParameterConverter : BaseConverter {
    public override int Priority => 4;
    public override Type ConvertableType => typeof(IValueLookupParameter);

    public override bool CanConvertType(Type t) =>
      t.GetInterfaces().Any(x => x == typeof(IValueLookupParameter));

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IValueLookupParameter param = item as IValueLookupParameter;
      IValueLookupJsonItem lookupItem = data as IValueLookupJsonItem;
      param.ActualName = lookupItem.ActualName;
      if (param.Value != null && lookupItem.ActualValue != null)
        root.Inject(param.Value, lookupItem.ActualValue, root);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IValueLookupParameter param = value as IValueLookupParameter;

      IValueLookupJsonItem item = new ValueLookupJsonItem();

      if (param.Value != null) {
        IJsonItem tmp = root.Extract(param.Value, root);
        tmp.Parent = item;
        item.ActualValue = tmp;
      }
      item.Name = param.Name;
      item.Description = param.Description;
      item.ActualName = param.ActualName;
      return item;
    }
  }
}
