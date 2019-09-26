using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class ConstrainedValueParameterTransformer : ParameterBaseTransformer {
    public override void InjectData(IParameter parameter, Component data) {
      foreach (var x in parameter.Cast<dynamic>().ValidValues)
        if (x.GetType().Name == CastValue<string>(data.Default))
          parameter.ActualValue = x;

      if (parameter.ActualValue is IParameterizedItem && data.Reference != null)
        Transformer.Inject(parameter.ActualValue, data.Reference);
    }

    public override Component ExtractData(IParameter value) {

      return new Component() {
        Name = value.Name,
        Default = value.ActualValue?.GetType().Name,
        Range = GetValidValues(value),
        ParameterizedItems = GetParameterizedChilds(value)
      };
    }

    #region Helper
    private object[] GetValidValues(IParameter value) {
      List<object> list = new List<object>();
      var values = value.Cast<dynamic>().ValidValues;
      foreach (var x in values) list.Add(x.GetType().Name);
      return list.ToArray();
    }

    private IList<Component> GetParameterizedChilds(IParameter value) {
      List<Component> list = new List<Component>();
      var values = value.Cast<dynamic>().ValidValues;
      foreach(var x in values) {
        if (x is IParameterizedItem &&
            ((IParameterizedItem)x).Parameters.Any(p => !p.Hidden)) {
          Component tmp = Transformer.Extract(x);
          if (tmp.ParameterizedItems != null)
            list.AddRange(tmp.ParameterizedItems);
          else
            list.Add(tmp);
        }
      }
      return list.Count == 0 ? null : list;
    }
    #endregion
  }
}
