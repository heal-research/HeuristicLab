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

    public override void InjectData(IItem item, JsonItem data, IJsonItemConverter root) {
      IParameterizedItem pItem = item as IParameterizedItem;

      if(data.Children != null) {
        foreach (var sp in data.Children)
          if (pItem.Parameters.TryGetValue(sp.Name, out IParameter param))
            root.Inject(param, sp, root);
      }
    }

    public override void Populate(IItem value, JsonItem item, IJsonItemConverter root) {
      var parameterizedItem = value as IParameterizedItem;

      foreach (var param in parameterizedItem.Parameters) {
        if (!param.Hidden) {
          JsonItem tmp = root.Extract(param, root);
          if(!(tmp is UnsupportedJsonItem))
            item.AddChilds(tmp);
        }
          
      }
    }
  }
}
