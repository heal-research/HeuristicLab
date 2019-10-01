using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public abstract class ParameterBaseConverter : BaseConverter {
    public override JsonItem ExtractData(IItem value) {
      IParameter param = value.Cast<IParameter>();
      JsonItem comp = ExtractData(param);
      //comp.Path = value.Cast<IParameter>().ActualValue?.ItemName;
      /*comp.Path = value.Cast<IParameter>().Name + "." + comp.Path;
      if(comp.ParameterizedItems != null) {
        foreach (var item in comp.ParameterizedItems) {
          item.Path = value.Cast<IParameter>().Name + "." + item.Path;
        }
      }*/
      return comp;
    }
    public abstract JsonItem ExtractData(IParameter value);

    public override void InjectData(IItem item, JsonItem data) => InjectData(item.Cast<IParameter>(), data);

    public abstract void InjectData(IParameter parameter, JsonItem data);
  }
}
