using System;
using HEAL.Attic;
using System.Collections.Generic;
using HeuristicLab.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using System.Reflection;
using HeuristicLab.Data;
using System.Collections;

namespace HeuristicLab.JsonInterface {


  public class StorableConverter {
    private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    //private IDictionary<Type, >


    private bool Filter(MemberInfo info) => info.Name != "ItemName" && info.Name != "ItemDescription" && info.Name != "name" && info.Name != "description";

    private Stack<Tuple<JsonItem, object, Type, string>> stack = new Stack<Tuple<JsonItem, object, Type, string>>();
    private IDictionary<int, JsonItem> hashToObject = new Dictionary<int, JsonItem>();

    public JsonItem ExtractHelper(object obj, Type type, string name = "") {
      JsonItem item = new JsonItem() {
        Parameters = new List<JsonItem>()
      };

      

      item.Type = type.AssemblyQualifiedName;

      if (obj != null && obj is IItem)
        item.Name = (obj as IItem).ItemName;
      else if (!string.IsNullOrEmpty(name))
        item.Name = name;
      else
        item.Name = type.Name;

      hashToObject.Add(obj.GetHashCode(), item);

      item.Path = item.Name;

      if (obj == null) return item;

      if (StorableTypeAttribute.IsStorableType(type)) {
        do {
          foreach (var property in type.GetProperties(flags)) {
            if (StorableAttribute.IsStorable(property) && Filter(property)) {
              object tmp = GetValue(property, obj);
              if (tmp != null && !hashToObject.ContainsKey(tmp.GetHashCode()))
                item.Parameters.Add(ExtractHelper(tmp, tmp != null ? tmp.GetType() : property.PropertyType, property.Name));
            }
          }

          foreach (var field in type.GetFields(flags)) {
            if (StorableAttribute.IsStorable(field) && Filter(field)) {
              object tmp = GetValue(field, obj);
              if (tmp != null && !hashToObject.ContainsKey(tmp.GetHashCode()))
                item.Parameters.Add(ExtractHelper(tmp, tmp != null ? tmp.GetType() : field.FieldType, field.Name));
            }
          }

          type = type.BaseType;
        } while (type != null);
      } else if (
          type.IsEqualTo(typeof(short))
          || type.IsEqualTo(typeof(int))
          || type.IsEqualTo(typeof(long))
          || type.IsEqualTo(typeof(ushort))
          || type.IsEqualTo(typeof(uint))
          || type.IsEqualTo(typeof(ulong))
          || type.IsEqualTo(typeof(float))
          || type.IsEqualTo(typeof(double))
          || type.IsEqualTo(typeof(bool))
      ) {
        item.Value = obj;
        item.Range = new object[] { GetMinValue(type), GetMaxValue(type) };
      } else if (type.IsEqualTo(typeof(IDictionary))) {
        if (obj != null) {
          foreach (DictionaryEntry i in (IDictionary)obj) {
            if (i.Key is string) {
              if (!hashToObject.ContainsKey(i.Value.GetHashCode()))
                item.Parameters.Add(ExtractHelper(i.Value, i.Value.GetType()));
            }
          }
        }
      } else if(type.IsEqualTo(typeof(IList))) {
        foreach (var i in (IList)obj) {
          if (!hashToObject.ContainsKey(i.GetHashCode()))
            item.Parameters.Add(ExtractHelper(i, i.GetType()));
        }
      } else if(type.IsEqualTo(typeof(KeyValuePair<,>))) {
        dynamic tmp = (dynamic)obj;
        object key = (object)tmp.Key;
        object value = (object)tmp.Value;
        if (!hashToObject.ContainsKey(key.GetHashCode()))
          item.Parameters.Add(ExtractHelper(key, key.GetType()));

        if (!hashToObject.ContainsKey(value.GetHashCode()))
          item.Parameters.Add(ExtractHelper(value, value.GetType()));

      } else if(type.IsEqualTo(typeof(string))) {
        item.Value = obj;

      } else if(type.IsEqualTo(typeof(Array))) {
        foreach (var i in (Array)obj) {
          if (!hashToObject.ContainsKey(i.GetHashCode()))
            item.Parameters.Add(ExtractHelper(i, i.GetType()));
        }
      } else if(type.IsEqualTo(typeof(Tuple<,>))) {
        dynamic tmp = (dynamic)obj;
        object key = (object)tmp.Item1;
        object value = (object)tmp.Item2;
        if (!hashToObject.ContainsKey(key.GetHashCode()))
          item.Parameters.Add(ExtractHelper(key, key.GetType()));

        if (!hashToObject.ContainsKey(value.GetHashCode()))
          item.Parameters.Add(ExtractHelper(value, value.GetType()));

      } else if (type.IsEqualTo(typeof(IEnumerable))) {
        foreach (var i in (IEnumerable)obj) {
          if (!hashToObject.ContainsKey(i.GetHashCode()))
            item.Parameters.Add(ExtractHelper(i, i.GetType()));
        }
      }


      item.UpdatePath();
      return item;
    }

    public JsonItem Extract(object obj) {
      return ExtractHelper(obj, obj.GetType());
    }

    /*
    public override JsonItem ExtractData(IItem value) {
      Type type = value.GetType();

      JsonItem item = new JsonItem() {
        Name = value.ItemName,
        Path = value.ItemName,
        Value = ".",
        Type = value.GetType().AssemblyQualifiedName
      };
      item.Parameters = new List<JsonItem>();

      
      do {
        foreach (var property in type.GetProperties(flags)) {
          ExtractFromMemberInfo(property, value, item);
        }

        foreach (var field in type.GetFields(flags)) {
          ExtractFromMemberInfo(field, value, item);
        }

        type = type.BaseType;
      } while (type != null);


      return item;
    }
    
    public override void InjectData(IItem item, JsonItem data) {
      throw new NotImplementedException();
    }
    */

    private object GetValue(MemberInfo info, object obj) {
      switch(info.MemberType) {
        case MemberTypes.Field:
          return ((FieldInfo)info).GetValue(obj);
        case MemberTypes.Property:
          return ((PropertyInfo)info).CanRead ? ((PropertyInfo)info).GetValue(obj) : null;
        default: return null;
      }
    }

    private object GetMaxValue(Type t) {
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

    private object GetMinValue(Type t) {
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

    private object GetDefaultValue(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;

  }
}
