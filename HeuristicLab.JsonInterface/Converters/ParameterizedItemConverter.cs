using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ParameterizedItemConverter : BaseConverter {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(IParameterizedItem);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IParameterizedItem pItem = item as IParameterizedItem;

      if(data.Children != null) {
        foreach (var sp in data.Children)
          if (pItem.Parameters.TryGetValue(sp.Name, out IParameter param) && param != null)
            root.Inject(param, sp, root);
      }
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IJsonItem item = new JsonItem() { Name = value.ItemName };

      var parameterizedItem = value as IParameterizedItem;
      foreach (var param in parameterizedItem.Parameters) {
        if (!param.Hidden) {
          IJsonItem tmp = root.Extract(param, root);
          if (!(tmp is UnsupportedJsonItem))
            item.AddChilds(tmp);
        }
      }

      return item;
    }
  }
}
