using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ParameterizedItemConverter : BaseConverter {
    public override void InjectData(IItem item, JsonItem data) {
      IParameterizedItem pItem = item.Cast<IParameterizedItem>();

      if(data.Children != null) {
        foreach (var sp in data.Children)
          if (pItem.Parameters.TryGetValue(sp.Name, out IParameter param))
            JsonItemConverter.Inject(param, sp);
      }
    }

    public override JsonItem ExtractData(IItem value) {
      JsonItem item = new JsonItem();
      var parameterizedItem = value as IParameterizedItem;

      foreach (var param in parameterizedItem.Parameters) {
        if(!param.Hidden) {
          JsonItem data = JsonItemConverter.Extract(param);
          //data.Name = param.Name;

          if (item.Children == null)
            item.Children = new List<JsonItem>();
          item.Children.Add(data);
        }
      }
      return item;
    }
  }
}
