using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class ValueParameterConverter : ParameterBaseConverter {

    public override void InjectData(IParameter parameter, Component data) => 
      JsonItemConverter.Inject(parameter.ActualValue, data);

    public override Component ExtractData(IParameter value) {
      Component data = null;
      if (value.ActualValue == null)
        data = new Component();
      else
        data = JsonItemConverter.Extract(value.ActualValue);
      data.Name = value.Name;
      return data;
    }
  }
}
