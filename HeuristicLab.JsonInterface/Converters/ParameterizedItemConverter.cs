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

      if(data.Parameters != null) {
        foreach (var sp in data.Parameters)
          if (pItem.Parameters.TryGetValue(sp.Name, out IParameter param))
            JsonItemConverter.Inject(param, sp);
      }
    }

    public override JsonItem ExtractData(IItem value) {
      JsonItem item = new JsonItem();
      item.Name = value.ItemName;
      item.Type = value.GetType().AssemblyQualifiedName;
      item.Path = value.ItemName;

      foreach (var param in value.Cast<IParameterizedItem>().Parameters) {
        JsonItem data = JsonItemConverter.Extract(param);
        data.Name = param.Name;
        data.Path = param.Name;
        data.PrependPath(item.Path);
        data.UpdatePaths();
        
        if (item.Parameters == null)
          item.Parameters = new List<JsonItem>();
        item.Parameters.Add(data);
      }
      return item;
    }
  }
}
