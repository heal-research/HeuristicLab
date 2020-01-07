using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ValueLookupParameterConverter : ParameterBaseConverter {
    public override int Priority => 4;
    public override Type ConvertableType => typeof(IValueLookupParameter);

    public override void Populate(IParameter value, JsonItem item, IJsonItemConverter root) {
      IValueLookupParameter param = value as IValueLookupParameter;
      
      item.Name = value.Name;
      item.ActualName = param.ActualName;

      object actualValue = null;
      IEnumerable<object> actualRange = null;
      if(param.Value != null) {
        JsonItem tmp = root.Extract(param.Value, root);
        tmp.Parent = item;
        actualValue = tmp.Value;
        actualRange = tmp.Range;
      } else {
        actualRange = new object[] { GetMinValue(param.DataType), GetMaxValue(param.DataType) };
      }
      item.Value = actualValue;
      item.Range = actualRange;
    }

    public override void InjectData(IParameter parameter, JsonItem data, IJsonItemConverter root) {
      IValueLookupParameter param = parameter as IValueLookupParameter;
      param.ActualName = CastValue<string>(data.ActualName);
      if (param.Value != null)
        root.Inject(param.Value, data, root);
    }
  }
}
