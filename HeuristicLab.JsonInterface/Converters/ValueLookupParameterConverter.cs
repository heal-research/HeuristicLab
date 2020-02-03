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
      param.ActualName = CastValue<string>(data.ActualName);
      if (param.Value != null)
        root.Inject(param.Value, data, root);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IValueLookupParameter param = value as IValueLookupParameter;

      IJsonItem item = new JsonItem() {};

      if (param.Value != null) {
        IJsonItem tmp = root.Extract(param.Value, root);
        /*tmp.Parent = item;
        actualValue = tmp.Value;
        actualRange = tmp.Range;
        */
        item = tmp;
      } else {
        item.Range = new object[] { GetMinValue(param.DataType), GetMaxValue(param.DataType) };
      }
      item.Name = param.Name;
      item.ActualName = param.ActualName;
      return item;
    }
  }
}
