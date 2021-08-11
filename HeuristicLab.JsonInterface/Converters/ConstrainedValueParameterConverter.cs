using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ConstrainedValueParameterConverter : BaseConverter {
    public override int Priority => 3;

    public override bool CanConvertType(Type t) =>
      t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConstrainedValueParameter<>));

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      StringJsonItem cdata = data as StringJsonItem;
      IParameter parameter = item as IParameter;
      foreach (var x in GetValidValues(parameter))
        if(x.ToString() == cdata.Value)
          parameter.ActualValue = x;

      if (parameter.ActualValue != null && parameter.ActualValue is IParameterizedItem && cdata.Children != null) {
        foreach(var child in cdata.Children) {
          if(child.Name == cdata.Value || child.Path.EndsWith(cdata.Value) || child.Name == parameter.ActualValue.ItemName)
            root.Inject(parameter.ActualValue, child, root);
        }
      }
    }
    
    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IParameter parameter = value as IParameter;

      IJsonItem item = new StringJsonItem() {
        Name = parameter.Name,
        Description = value.ItemDescription,
        Value = parameter.ActualValue?.ToString(),
        ConcreteRestrictedItems = GetValidValues(parameter).Select(x => x.ToString())
      };
      item.AddChildren(GetParameterizedChilds(parameter, root));

      return item;
    }

    #region Helper
    private IItem[] GetValidValues(IParameter value) {
      List<IItem> list = new List<IItem>();
      var values = ((dynamic)value).ValidValues;
      foreach (var x in values) list.Add((IItem)x);
      return list.ToArray();
    }

    private IList<IJsonItem> GetParameterizedChilds(IParameter value, IJsonItemConverter root) {
      List<IJsonItem> list = new List<IJsonItem>();
      var values = ((dynamic)value).ValidValues;
      foreach(var x in values) {
        if (x is IParameterizedItem) { // only makes sense for IParameterizedItems to go deeper
          IJsonItem tmp = root.Extract(x, root);
          if(!(tmp is UnsupportedJsonItem))
            list.Add(tmp);
        }
      }
      return list.Count == 0 ? null : list;
    }
    #endregion
  }
}
