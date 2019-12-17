using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {

  public class IntRangeConverter : ValueRangeConverter<IntRange, IntValue, int> { }
  public class DoubleRangeConverter : ValueRangeConverter<DoubleRange, DoubleValue, double> {}

  public class ValueRangeConverter<RangeType, T, TType> : BaseConverter 
    where RangeType : StringConvertibleValueTuple<T, T>
    where T : ValueTypeValue<TType>, IDeepCloneable, IStringConvertibleValue
    where TType : struct {

    private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;

    public override JsonItem ExtractData(IItem value) {
      var field = value.GetType().GetField("values", Flags);
      Tuple<T,T> tuple = (Tuple<T,T>)field.GetValue(value);

      return new JsonItem() {
        Name = "[OverridableParamName]",
        Value = new object[] { tuple.Item1.Value, tuple.Item2.Value },
        Range = new object[] { GetMinValue(tuple.Item1.Value.GetType()), 
                               GetMaxValue(tuple.Item2.Value.GetType()) }
      };
    }

    public override void InjectData(IItem item, JsonItem data) {
      object[] arr = (object[])data.Value;
      Tuple<T,T> tuple = new Tuple<T,T>(Instantiate<T>(arr[0]), Instantiate<T>(arr[1]));
      var field = item.GetType().GetField("values", Flags);
      field.SetValue(tuple, item);
    }
  }
}
