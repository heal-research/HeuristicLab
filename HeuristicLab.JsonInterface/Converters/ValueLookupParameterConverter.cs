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
      param.ActualName = CastValue<string>(((IValueLookupJsonItem)data).ActualName);
      if (param.Value != null)
        root.Inject(param.Value, data, root);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IValueLookupParameter param = value as IValueLookupParameter;

      IValueLookupJsonItem item = new ValueLookupJsonItem() {};

      if (param.Value != null) {
        IJsonItem tmp = root.Extract(param.Value, root);
        item.Value = tmp.Value;
        item.Range = tmp.Range;
        item.Name = tmp.Name;
        item.Description = tmp.Description;
        item.AddChildren(tmp.Children);
        item.Active = tmp.Active;
        item.JsonItemReference = tmp;
      } else {
        var min = GetMinValue(param.DataType);
        var max = GetMaxValue(param.DataType);
        if (min != null && max != null)
          item.Range = new object[] { min, max };
        else
          item.Range = null;
      }
      item.Name = param.Name;
      item.Description = param.Description;
      item.ActualName = param.ActualName;
      return item;
    }
  }
}
