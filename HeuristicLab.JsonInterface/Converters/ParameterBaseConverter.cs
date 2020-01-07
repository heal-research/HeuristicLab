using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public abstract class ParameterBaseConverter : BaseConverter {
    public override void Populate(IItem value, JsonItem item, IJsonItemConverter root) => 
      Populate(value as IParameter, item, root);

    public abstract void Populate(IParameter value, JsonItem item, IJsonItemConverter root);

    public override void InjectData(IItem item, JsonItem data, IJsonItemConverter root) => 
      InjectData(item as IParameter, data, root);

    public abstract void InjectData(IParameter parameter, JsonItem data, IJsonItemConverter root);
  }
}
