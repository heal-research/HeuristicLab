using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ParameterizedItemConverter : BaseConverter {
    public override int Priority => 2;

    public override bool CanConvertType(Type t) =>
      t.GetInterfaces().Any(x => x == typeof(IParameterizedItem));

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IParameterizedItem pItem = item as IParameterizedItem;

      if(data.Children != null) {
        foreach (var sp in data.Children)
          if (pItem.Parameters.TryGetValue(sp.Name, out IParameter param) && param != null)
            root.Inject(param, sp, root);
      }
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      var parameterizedItem = value as IParameterizedItem;

      IJsonItem item = new EmptyJsonItem() { 
        Name = value.ItemName,
        Description = value.ItemDescription
      };

      foreach (var param in parameterizedItem.Parameters) {
        if (!param.Hidden) {
          IJsonItem tmp = root.Extract(param, root);
          if (!(tmp is UnsupportedJsonItem))
            item.AddChildren(tmp);
        }
      }

      return item;
    }

  }
}
