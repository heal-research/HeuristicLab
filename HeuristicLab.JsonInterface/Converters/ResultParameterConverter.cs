using System;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface {
  public class ResultParameterConverter : BaseConverter {
    public override int Priority => 5;

    public override bool CanConvertType(Type t) =>
      t.GetInterfaces().Any(x => x == typeof(IResultParameter));

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IResultParameter res = value as IResultParameter;
      var formatter = ResultFormatter.ForType(res.DataType).Last();
      return new ResultJsonItem() {
        Name = res.ActualName,
        Description = res.Description,
        ResultFormatterType = formatter.GetType().FullName,
        ValueType = res.DataType
      };
    }

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IResultParameter res = item as IResultParameter;
      res.ActualName = data.Name;
    }
  }
}
