using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture
{
  public class ParameterizedItemConverter : BaseConverter {
    public override void InjectData(IItem item, Component data) {
      IParameterizedItem pItem = item.Cast<IParameterizedItem>();

      if(data.Parameters != null) {
        foreach (var sp in data.Parameters)
          if (pItem.Parameters.TryGetValue(sp.Name, out IParameter param))
            JsonItemConverter.Inject(param, sp);
      }
    }

    public override Component ExtractData(IItem value) {
      Component obj = new Component();
      obj.Name = value.ItemName;
      obj.Type = value.GetType().AssemblyQualifiedName;
      obj.Path = value.ItemName;

      foreach (var param in value.Cast<IParameterizedItem>().Parameters) {
        Component data = JsonItemConverter.Extract(param);
        data.Name = param.Name;
        data.Path = param.Name;
        data.PrependPath(obj.Path);
        data.UpdatePaths();
        
        if (obj.Parameters == null)
          obj.Parameters = new List<Component>();
        obj.Parameters.Add(data);
      }
      return obj;
    }
  }
}
