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
      return new JsonItem() {
        Name = value.Name,
        Default = param.ActualName,
        Reference = param.Value != null ? JsonItemConverter.Extract(param.Value) : null
      };
    }

    public override void InjectData(IParameter parameter, JsonItem data) {
      IValueLookupParameter param = parameter.Cast<IValueLookupParameter>();
      param.ActualName = CastValue<string>(data.Default);
      if (param.Value != null && data.Reference != null)
        JsonItemConverter.Inject(param.Value, data.Reference);
    }
  }
}
