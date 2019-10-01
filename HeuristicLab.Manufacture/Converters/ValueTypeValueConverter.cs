using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Manufacture {
  public class ValueTypeValueConverter<ValueType, T> : BaseConverter
    where ValueType : ValueTypeValue<T>
    where T : struct {

    public override void InjectData(IItem item, JsonItem data) =>
      item.Cast<ValueType>().Value = CastValue<T>(data.Default);

    public override JsonItem ExtractData(IItem value) => 
      new JsonItem() {
        Default = value.Cast<ValueType>().Value,
        Range = new object[] { GetMinValue(), GetMaxValue() }
      };

    #region Helper
    private object GetMaxValue() {
      TypeCode typeCode = Type.GetTypeCode(typeof(T));

      if (typeof(ValueType).IsEqualTo(typeof(PercentValue)))
        return 1.0d;

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

    private object GetMinValue() {
      TypeCode typeCode = Type.GetTypeCode(typeof(T));

      if (typeof(ValueType).IsEqualTo(typeof(PercentValue)))
        return 0.0d;

      switch (typeCode) {
        case TypeCode.Int16: return Int16.MinValue;
        case TypeCode.Int32: return Int32.MinValue;
        case TypeCode.Int64: return Int64.MinValue;
        case TypeCode.UInt16: return UInt16.MinValue;
        case TypeCode.UInt32: return UInt32.MinValue;
        case TypeCode.UInt64: return UInt64.MinValue;
        case TypeCode.Single: return Single.MinValue;
        case TypeCode.Double: return Double.MinValue;
        case TypeCode.Decimal: return Decimal.MinValue;
        case TypeCode.Byte: return Byte.MinValue;
        case TypeCode.Boolean: return false;
        default: return default(T);
      }
    }
    #endregion
  }
}
