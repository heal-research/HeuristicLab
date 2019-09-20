using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class ConstrainedValueParameterTransformer : ParameterBaseTransformer {
    public override void InjectData(IParameter parameter, ParameterData data) {
      foreach (var x in parameter.Cast<dynamic>().ValidValues)
        if (x.GetType().Name == CastValue<string>(data.Default))
          parameter.ActualValue = x;
    }

    public override ParameterData ExtractData(IParameter value) {
      return new ParameterData() {
        Name = value.Name,
        Default = value.ActualValue?.GetType().Name,
        Range = GetValidValues(value)
      };
    }

    #region Helper
    private object[] GetValidValues(IParameter value) {
      List<object> list = new List<object>();
      var values = value.Cast<dynamic>().ValidValues;
      foreach (var x in values) list.Add(x);
      return list.Select(x => x.GetType().Name).ToArray();
    }
    #endregion
  }
}
