using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ValueParameterConverter : ParameterBaseConverter {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(IValueParameter);

    public override void InjectData(IParameter parameter, IJsonItem data, IJsonItemConverter root) {
      if (parameter.ActualValue == null && data.Value != null)
        parameter.ActualValue = Instantiate(parameter.DataType);
      root.Inject(parameter.ActualValue, data, root);
    }

    public override void Populate(IParameter value, IJsonItem item, IJsonItemConverter root) {
      item.Name = value.Name;
      if (value.ActualValue != null) {
        IJsonItem tmp = root.Extract(value.ActualValue, root);
        if(!(tmp is UnsupportedJsonItem)) {
          if (tmp.Name == "[OverridableParamName]") {
            tmp.Name = value.Name;
            JsonItem.Merge(item as JsonItem, tmp as JsonItem);
          } else
            item.AddChilds(tmp);
        }
      }
    }
  }
}
