using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ConstrainedValueParameterConverter : BaseConverter {
    public override int Priority => 3;
    public override Type ConvertableType => typeof(IConstrainedValueParameter<>);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      StringJsonItem cdata = data as StringJsonItem;
      IParameter parameter = item as IParameter;
      foreach (var x in GetValidValues(parameter))
        if(x.ToString() == CastValue<string>(cdata.Value))
          parameter.ActualValue = x;

      if (parameter.ActualValue is IParameterizedItem && cdata.Children != null) {
        foreach(var param in cdata.Children) {
          if(param.Name == parameter.ActualValue.ItemName)
            root.Inject(parameter.ActualValue, param, root);
        }
      }
    }
    
    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IParameter parameter = value as IParameter;

      IJsonItem item = new StringJsonItem() {
        Name = parameter.Name,
        Description = value.ItemDescription,
        Value = parameter.ActualValue?.ToString(),
        Range = GetValidValues(parameter).Select(x => x.ToString())
      };
      item.AddChildren(GetParameterizedChilds(parameter));

      return item;
    }

    #region Helper
    private IItem[] GetValidValues(IParameter value) {
      List<IItem> list = new List<IItem>();
      var values = ((dynamic)value).ValidValues;
      foreach (var x in values) list.Add((IItem)x);
      return list.ToArray();
    }

    private IList<IJsonItem> GetParameterizedChilds(IParameter value) {
      List<IJsonItem> list = new List<IJsonItem>();
      var values = ((dynamic)value).ValidValues;
      foreach(var x in values) {
        if (x is IParameterizedItem) {
          IJsonItem tmp = JsonItemConverter.Extract(x);
          if(!(tmp is UnsupportedJsonItem))
            list.Add(tmp);
        }
      }
      return list.Count == 0 ? null : list;
    }
    #endregion
  }
}
