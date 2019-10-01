using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Manufacture {
  public class StringValueConverter : BaseConverter {
    public override void InjectData(IItem item, JsonItem data) =>
      item.Cast<StringValue>().Value = CastValue<string>(data.Default);

    public override JsonItem ExtractData(IItem value) => 
      new JsonItem() {
        Default = value.Cast<StringValue>().Value
      };
  }
}
