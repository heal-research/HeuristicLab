using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ValueParameterConverter : ParameterBaseConverter {

    public override void InjectData(IParameter parameter, JsonItem data) {
      if (parameter.ActualValue == null && data.Value != null)
        parameter.ActualValue = Instantiate(parameter.DataType);
      JsonItemConverter.Inject(parameter.ActualValue, data);
    }

    public override JsonItem ExtractData(IParameter value) {
      JsonItem data = null;
      if (value.ActualValue == null)
        data = new JsonItem();
      else
        data = JsonItemConverter.Extract(value.ActualValue);
      data.Name = value.Name;
      return data;
    }
  }
}
