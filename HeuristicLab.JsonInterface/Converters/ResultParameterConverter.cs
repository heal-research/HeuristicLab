using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface {
  public class ResultParameterConverter : BaseConverter {
    public override int Priority => 5;

    public override Type ConvertableType => typeof(IResultParameter);

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IResultParameter res = value as IResultParameter;
      return new ResultJsonItem() {
        Name = res.ActualName,
        ActualName = res.ActualName,
        Value = res.ActualName,
        Description = res.Description
      };
    }

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IResultParameter res = item as IResultParameter;
      res.ActualName = (string)data.Value;
    }
  }
}
