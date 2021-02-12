using System;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface {
  public class ResultConverter : BaseConverter {
    public override int Priority => 1;

    public override Type ConvertableType => typeof(IResult);

    public override bool CanConvertType(Type t) =>
      t.GetInterfaces().Any(x => x == ConvertableType);

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IResult res = value as IResult;
      var formatter = ResultFormatter.ForType(res.Value.GetType()).Last();
      return new ResultJsonItem() {
        Name = res.Name,
        Description = res.Description,
        ResultFormatterType = formatter.GetType().FullName,
        ValueType = res.DataType
      };
    }

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IResult res = item as IResult;
      res.Name = data.Name;
    }
  }
}
