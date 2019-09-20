using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public abstract class ParameterBaseTransformer : BaseTransformer {
    public override ParameterData ExtractData(IItem value) => ExtractData(value.Cast<IParameter>());
    public abstract ParameterData ExtractData(IParameter value);

    public override void InjectData(IItem item, ParameterData data) => InjectData(item.Cast<IParameter>(), data);

    public abstract void InjectData(IParameter parameter, ParameterData data);
  }
}
