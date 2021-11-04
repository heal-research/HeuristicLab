using System;
using System.Linq;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ValueParameterConverter : BaseConverter {
    public override int Priority => 2;

    public override bool CanConvertType(Type t) =>
      t.GetInterfaces().Any(x => x == typeof(IValueParameter));

    public override void Inject(IItem value, IJsonItem data, IJsonItemConverter root) {
      IParameter parameter = value as IParameter;

      if (parameter.ActualValue == null && data.Active)
        parameter.ActualValue = Instantiate(parameter.DataType);
      root.Inject(parameter.ActualValue, data, root);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IParameter parameter = value as IParameter;

      IJsonItem item = new EmptyJsonItem() {
        Name = parameter.Name,
        Description = parameter.Description
      };

      if (parameter.ActualValue != null) {
        IJsonItem tmp = root.Extract(parameter.ActualValue, root);
        if (!(tmp is UnsupportedJsonItem)) {
          tmp.Name = parameter.Name;
          tmp.Description = parameter.Description;
          item = tmp;
        }
      }
      return item;
    }
  }
}
