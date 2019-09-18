using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace ParameterTest {
  public abstract class ParameterBaseTransformer : BaseTransformer {
    public override IItem FromData(ParameterData obj, Type targetType) {
      throw new NotImplementedException();
    }



    public override void SetValue(IItem item, ParameterData data) => SetValue(item.Cast<IParameter>(), data);

    public abstract void SetValue(IParameter parameter, ParameterData data);
  }
}
