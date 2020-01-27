using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class LookupParameterConverter : BaseConverter {
    public override int Priority => 3;
    public override Type ConvertableType => typeof(ILookupParameter);
    
    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((ILookupParameter)item).ActualName = data.ActualName as string;

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IParameter parameter = value as IParameter;

      IJsonItem item = new JsonItem() {
        Name = parameter.Name,
        ActualName = ((ILookupParameter)parameter).ActualName
      };
      return item;
    }
  }
}
