﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ConstrainedValueParameterConverter : ParameterBaseConverter {
    public override int Priority => 3;
    public override Type ConvertableType => typeof(IConstrainedValueParameter<>);

    public override void InjectData(IParameter parameter, IJsonItem data, IJsonItemConverter root) {
      foreach(var x in GetValidValues(parameter))
        if(x.ToString() == CastValue<string>(data.Value))
          parameter.ActualValue = x;

      if (parameter.ActualValue is IParameterizedItem && data.Children != null) {
        foreach(var param in data.Children) {
          if(param.Name == parameter.ActualValue.ItemName)
            root.Inject(parameter.ActualValue, param, root);
        }
      }
    }

    public override void Populate(IParameter value, IJsonItem item, IJsonItemConverter root) {
      item.AddChilds(GetParameterizedChilds(value));
      item.Name = value.Name;
      item.Value = value.ActualValue?.ToString();
      item.Range = GetValidValues(value).Select(x => x.ToString());
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
