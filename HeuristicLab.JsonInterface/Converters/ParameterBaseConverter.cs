using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public abstract class ParameterBaseConverter : BaseConverter {
    public override void Populate(IItem value, IJsonItem item, IJsonItemConverter root) => 
      Populate(value as IParameter, item, root);

    public abstract void Populate(IParameter value, IJsonItem item, IJsonItemConverter root);

    public override void InjectData(IItem item, IJsonItem data, IJsonItemConverter root) => 
      InjectData(item as IParameter, data, root);

    public abstract void InjectData(IParameter parameter, IJsonItem data, IJsonItemConverter root);
  }
}
