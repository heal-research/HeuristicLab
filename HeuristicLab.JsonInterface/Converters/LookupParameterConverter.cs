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

    public override bool CanConvertType(Type t) =>
      t.GetInterfaces().Any(x => x == typeof(ILookupParameter));

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((ILookupParameter)item).ActualName = ((ILookupJsonItem)data).ActualName as string;

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IParameter parameter = value as IParameter;

      IJsonItem item = new LookupJsonItem() {
        Name = parameter.Name,
        Description = parameter.Description,
        ActualName = ((ILookupParameter)parameter).ActualName
      };
      return item;
    }
  }
}
