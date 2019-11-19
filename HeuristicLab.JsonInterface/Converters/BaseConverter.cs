using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public abstract class BaseConverter : IJsonItemConverter
  {
    public void Inject(IItem item, JsonItem data) {
      if (data.Reference != null) {
        JsonItem.Merge(data, data.Reference);
      }
      InjectData(item, data);
    }

    public JsonItem Extract(IItem value) {
      JsonItem data = ExtractData(value);
      data.Name = String.IsNullOrEmpty(data.Name) ? value.ItemName : data.Name;
      return data;
    }
    
    public abstract void InjectData(IItem item, JsonItem data);
    public abstract JsonItem ExtractData(IItem value);

    #region Helper
    protected ValueType CastValue<ValueType>(object obj) {
      if (obj is JToken)
        return (obj.Cast<JToken>()).ToObject<ValueType>();
      else if (obj is IConvertible)
        return Convert.ChangeType(obj, typeof(ValueType)).Cast<ValueType>();
      else return (ValueType)obj;
    }

    protected IItem Instantiate(Type type, params object[] args) =>
      (IItem)Activator.CreateInstance(type,args);

    protected T Instantiate<T>(params object[] args) => (T)Instantiate(typeof(T), args);

    protected object GetMaxValue(Type t) {
      TypeCode typeCode = Type.GetTypeCode(t);

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
        default: return GetDefaultValue(t);
      }
    }

    protected object GetMinValue(Type t) {
      TypeCode typeCode = Type.GetTypeCode(t);

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
        default: return GetDefaultValue(t);
      }
    }

    protected object GetDefaultValue(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    #endregion
  }
}
