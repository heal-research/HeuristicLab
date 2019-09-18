using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace ParameterTest.ParameterTransformer {
  public class ConstrainedValueParameterTransformer : ParameterBaseTransformer {
    public override void SetValue(IParameter parameter, ParameterData data) {
      foreach (var x in parameter.Cast<dynamic>().ValidValues)
        if (x.GetType().Name == CastValue<string>(data.Default))
          parameter.ActualValue = x;
    }
  }
}
