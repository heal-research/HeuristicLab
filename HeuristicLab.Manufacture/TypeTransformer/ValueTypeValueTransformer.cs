using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace ParameterTest {
  public class ValueTypeValueTransformer<ValueType, T> : BaseTransformer
    where ValueType : ValueTypeValue<T>
    where T : struct {
    public override IItem FromData(ParameterData obj, Type targetType) =>
      //item.Cast<ValueType>().Value = CastValue<T>(obj.Default);
      Instantiate<ValueType>(CastValue<T>(obj.Default));
    public override void SetValue(IItem item, ParameterData data) =>
      item.Cast<ValueType>().Value = CastValue<T>(data.Default);

    public override ParameterData ToData(IItem value) {
      ParameterData data = base.ToData(value);
      data.Default = value.Cast<ValueType>().Value;
      data.Range = new object[] { default(T), GetMaxValue() };
      return data;
    }

    private object GetMaxValue() {
      TypeCode typeCode = Type.GetTypeCode(typeof(T));
      switch (typeCode) {
        case TypeCode.Int16: return Int16.MaxValue;
        case TypeCode.Int32: return Int32.MaxValue;
        case TypeCode.Int64: return Int64.MaxValue;
        case TypeCode.UInt16: return UInt16.MaxValue;
        case TypeCode.UInt32: return UInt32.MaxValue;
        case TypeCode.UInt64: return UInt64.MaxValue;
        case TypeCode.Single: return Single.MaxValue;
        case TypeCode.Double: return Double.MaxValue;
        case TypeCode.Decimal: return Decimal.MaxValue;
        case TypeCode.Byte: return Byte.MaxValue;
        case TypeCode.Boolean: return true;
        default: return default(T);
      }
    }
  }
}
