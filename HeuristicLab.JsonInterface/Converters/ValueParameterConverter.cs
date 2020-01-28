using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ValueParameterConverter : BaseConverter {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(IValueParameter);

    public override void Inject(IItem value, IJsonItem data, IJsonItemConverter root) {
      IParameter parameter = value as IParameter;

      if (parameter.ActualValue == null && data.Value != null)
        parameter.ActualValue = Instantiate(parameter.DataType);
      root.Inject(parameter.ActualValue, data, root);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IParameter parameter = value as IParameter;

      IJsonItem item = new JsonItem() {
        Name = parameter.Name
      };

      if (parameter.ActualValue != null) {
        IJsonItem tmp = root.Extract(parameter.ActualValue, root);
        if (!(tmp is UnsupportedJsonItem)) {
          if (tmp.Name == "[OverridableParamName]") {
            tmp.Name = parameter.Name;
            item = tmp;
            //JsonItem.Merge(item as JsonItem, tmp as JsonItem);
          } else
            item.AddChilds(tmp);
        }
      }
      return item;
    }
  }
}
