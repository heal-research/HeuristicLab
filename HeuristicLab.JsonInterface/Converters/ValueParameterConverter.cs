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
      JsonItem data = new JsonItem() { Name = value.Name };
      if (value.ActualValue != null) {
        JsonItem tmp = JsonItemConverter.Extract(value.ActualValue);
        if(tmp.Name == "[OverridableParamName]") {
          tmp.Name = value.Name;
          data = tmp;
        }
        else
          data.AddParameter(tmp);
      }
      return data;
    }
  }
}
