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

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IValueLookupParameter param = item as IValueLookupParameter;
      IValueLookupJsonItem lookupItem = data as IValueLookupJsonItem;
      param.ActualName = lookupItem.ActualName;
      if (param.Value != null)
        root.Inject(param.Value, lookupItem.JsonItemReference, root);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IValueLookupParameter param = value as IValueLookupParameter;

      IValueLookupJsonItem item = new ValueLookupJsonItem() {};

      if (param.Value != null) {
        IJsonItem tmp = root.Extract(param.Value, root);
        item.AddChildren(tmp.Children);
        item.JsonItemReference = tmp;
      }
      item.Name = param.Name;
      item.Description = param.Description;
      item.ActualName = param.ActualName;
      return item;
    }
  }
}
