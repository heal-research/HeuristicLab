using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Manufacture {
  public class ValueTypeValueTransformer<ValueType, T> : BaseTransformer
    where ValueType : ValueTypeValue<T>
    where T : struct {

    public override void InjectData(IItem item, Component data) =>
      item.Cast<ValueType>().Value = CastValue<T>(data.Default);

    public override Component ExtractData(IItem value) => 
      new Component() {
        Default = value.Cast<ValueType>().Value,
        Range = new object[] { default(T), GetMaxValue() }
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
    #endregion
  }
}
