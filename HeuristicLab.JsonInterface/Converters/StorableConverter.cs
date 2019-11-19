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


  public class StorableConverter : BaseConverter {
    private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private bool ContainsOnlyPrimitives(IEnumerable enumerable) {
      
      foreach (var i in enumerable) {
        if (!i.GetType().IsPrimitive) return false;
      }
      return true;
    }

    private void ExtractFromMemberInfo(MemberInfo info, object obj, JsonItem item) {
      string name = (obj is IItem) ? ((IItem)obj).ItemName : info.Name;
      if (StorableAttribute.IsStorable(info)) {
        object tmp = GetValue(info, obj);
        if (tmp is IItem)
          item.Parameters.Add(JsonItemConverter.Extract((IItem)tmp));
        else if (tmp is IDictionary) { 
        } 
        else if(tmp is IEnumerable) {
          IEnumerable c = (IEnumerable)tmp;
          List<object> objs = new List<object>();
          foreach (var i in c) {
            if (i is IItem)
              item.Parameters.Add(JsonItemConverter.Extract((IItem)i));
            else
              objs.Add(i);
          }
          item.Value = objs;
        }
      }
    }

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


    private object GetValue(MemberInfo info, object obj) {
      switch(info.MemberType) {
        case MemberTypes.Field:
          return ((FieldInfo)info).GetValue(obj);
        case MemberTypes.Property:
          return ((PropertyInfo)info).CanRead ? ((PropertyInfo)info).GetValue(obj) : null;
        default: return null;
      }
    }      
  }
}
