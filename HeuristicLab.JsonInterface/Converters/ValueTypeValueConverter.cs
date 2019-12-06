using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {
  public class ValueTypeValueConverter<ValueType, T> : BaseConverter
    where ValueType : ValueTypeValue<T>
    where T : struct {

    public override void InjectData(IItem item, JsonItem data) =>
      item.Cast<ValueType>().Value = CastValue<T>(data.Value);

    public override JsonItem ExtractData(IItem value) => 
      new JsonItem() {
        Name = "[OverridableParamName]",
        Value = value.Cast<ValueType>().Value,
        Range = new object[] { GetMinValue(typeof(T)), GetMaxValue(typeof(T)) }
      };
  }
}
