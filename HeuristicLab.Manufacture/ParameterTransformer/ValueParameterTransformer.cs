using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class ValueParameterTransformer : ParameterBaseTransformer {

    public override void InjectData(IParameter parameter, ParameterData data) => 
      Transformer.Inject(parameter.ActualValue, data);

    public override ParameterData ExtractData(IParameter value) {
      ParameterData data = null;
      if (value.ActualValue == null)
        data = new ParameterData();
      else
        data = Transformer.Extract(value.ActualValue);
      data.Name = value.Name;
      return data;
    }
  }
}
