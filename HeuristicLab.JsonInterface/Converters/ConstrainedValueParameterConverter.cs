using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ConstrainedValueParameterConverter : ParameterBaseConverter {
    public override void InjectData(IParameter parameter, JsonItem data) {
      foreach(var x in GetValidValues(parameter))
        if(x.ToString() == CastValue<string>(data.Value))
          parameter.ActualValue = x;

      if (parameter.ActualValue is IParameterizedItem && data.Children != null) {
        foreach(var param in data.Children) {
          if(param.Name == parameter.ActualValue.ItemName)
            JsonItemConverter.Inject(parameter.ActualValue, param);
        }
      }
    }

    public override JsonItem ExtractData(IParameter value) =>
      new JsonItem() {
        Name = value.Name,
        Value = value.ActualValue?.ToString(),
        Range = GetValidValues(value).Select(x => x.ToString()),
        Children = GetParameterizedChilds(value)
      };

    #region Helper
    private IItem[] GetValidValues(IParameter value) {
      List<IItem> list = new List<IItem>();
      var values = value.Cast<dynamic>().ValidValues;
      foreach (var x in values) list.Add((IItem)x);
      return list.ToArray();
    }

    private IList<JsonItem> GetParameterizedChilds(IParameter value) {
      List<JsonItem> list = new List<JsonItem>();
      var values = value.Cast<dynamic>().ValidValues;
      foreach(var x in values) {
        if (x is IParameterizedItem) {
          JsonItem tmp = JsonItemConverter.Extract(x);
          list.Add(tmp);
        }
      }
      return list.Count == 0 ? null : list;
    }
    #endregion
  }
}
