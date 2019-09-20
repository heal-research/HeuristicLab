using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Manufacture {
  public class StringValueTransformer : BaseTransformer {
    public override void InjectData(IItem item, ParameterData data) =>
      item.Cast<StringValue>().Value = CastValue<string>(data.Default);

    public override ParameterData ExtractData(IItem value) => 
      new ParameterData() {
        Default = value.Cast<StringValue>().Value
      };
  }
}
