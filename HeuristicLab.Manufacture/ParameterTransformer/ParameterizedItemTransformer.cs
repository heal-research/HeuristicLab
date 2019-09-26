using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture
{
  public class ParameterizedItemTransformer : BaseTransformer {
    
    public override void InjectData(IItem item, Component data) {
      IParameterizedItem pItem = item.Cast<IParameterizedItem>();

      if(data.Parameters != null) {
        foreach (var sp in data.Parameters)
          if (pItem.Parameters.TryGetValue(sp.Name, out IParameter param))
            Transformer.Inject(param, sp);
      }
    }

    public override Component ExtractData(IItem value) {
      List<Component> list = new List<Component>();

      Component obj = new Component();
      obj.Name = value.ItemName;
      obj.Type = value.GetType().AssemblyQualifiedName;
      obj.ParameterizedItems = list;
      obj.Parameters = new List<Component>();
      list.Add(obj);

      foreach (var param in value.Cast<IParameterizedItem>().Parameters) {
        if (!param.Hidden) {
          Component data = Transformer.Extract(param);
          obj.Parameters.Add(data);
          if(data.ParameterizedItems != null)
            list.AddRange(data.ParameterizedItems);
        }
      }
      return obj;
    }
  }
}
