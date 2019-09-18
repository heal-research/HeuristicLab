using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace ParameterTest {
  public class ValueParameterTransformer : ParameterBaseTransformer {

    public override void SetValue(IParameter parameter, ParameterData data) => 
      Transformer.SetValue(parameter.ActualValue, data);

  }
}
