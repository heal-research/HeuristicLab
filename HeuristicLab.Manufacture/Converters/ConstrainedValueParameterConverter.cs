using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class ConstrainedValueParameterConverter : ParameterBaseConverter {
    public override void InjectData(IParameter parameter, JsonItem data) {
      foreach (var x in parameter.Cast<dynamic>().ValidValues)
        if (x.GetType().Name == CastValue<string>(data.Default))
          parameter.ActualValue = x;

      if (parameter.ActualValue is IParameterizedItem && data.Reference != null)
        JsonItemConverter.Inject(parameter.ActualValue, data.Reference);
    }

    public override JsonItem ExtractData(IParameter value) =>
      new JsonItem() {
        Name = value.Name,
        Default = value.ActualValue?.GetType().Name,
        Range = GetValidValues(value),
        Parameters = GetParameterizedChilds(value)
      };

    #region Helper
    private object[] GetValidValues(IParameter value) {
      List<object> list = new List<object>();
      var values = value.Cast<dynamic>().ValidValues;
      foreach (var x in values) list.Add(x.GetType().Name);
      return list.ToArray();
    }
    // id = kombi aus path + default 
    private IList<JsonItem> GetParameterizedChilds(IParameter value) {
      List<JsonItem> list = new List<JsonItem>();
      var values = value.Cast<dynamic>().ValidValues;
      foreach(var x in values) {
        if (x is IParameterizedItem) {
          JsonItem tmp = JsonItemConverter.Extract(x);
          tmp.PrependPath(value.Name);
          list.Add(tmp);
        }
      }
      return list.Count == 0 ? null : list;
    }
    #endregion
  }
}
