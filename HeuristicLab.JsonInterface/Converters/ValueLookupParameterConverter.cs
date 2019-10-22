using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ValueLookupParameterConverter : ParameterBaseConverter {
    public override JsonItem ExtractData(IParameter value) {
      IValueLookupParameter param = value.Cast<IValueLookupParameter>();
      object actualValue = null;
      IList<object> actualRange = null;
      if(param.Value != null) {
        JsonItem tmp = JsonItemConverter.Extract(param.Value);
        actualValue = tmp.Value;
        actualRange = tmp.Range;
      }
      return new JsonItem() {
        Name = value.Name,
        ActualName = param.ActualName,
        Value = actualValue,
        Range = actualRange
      };
    }

    public override void InjectData(IParameter parameter, JsonItem data) {
      IValueLookupParameter param = parameter.Cast<IValueLookupParameter>();
      param.ActualName = CastValue<string>(data.ActualName);
      if (param.Value != null)
        JsonItemConverter.Inject(param.Value, data);
    }
  }
}
