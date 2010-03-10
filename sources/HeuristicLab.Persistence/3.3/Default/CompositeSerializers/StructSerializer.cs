using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Reflection;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [EmptyStorableClass]
  public class StructSerializer : ICompositeSerializer {    

    public int Priority {
      get { return 50; }
    }

    public bool CanSerialize(Type type) {
      return type.IsValueType && !type.IsPrimitive && !type.IsEnum && type.IsSealed;      
    }

    public string JustifyRejection(Type type) {
      if (!type.IsValueType)
        return "not a value type";
      if (type.IsPrimitive)
        return "type is primitive (int, float, ...)";
      if (type.IsEnum)
        return "type is enum";
      return "type is not sealed";
    }

    public IEnumerable<Tag> CreateMetaInfo(object obj) {
      return null;
    }

    private readonly BindingFlags AllInstanceMembers =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public IEnumerable<Tag> Decompose(object obj) {
      Type t = obj.GetType();
      foreach (MemberInfo mi in t.GetMembers(AllInstanceMembers)) {
        if (mi.MemberType == MemberTypes.Field) {
          string name = mi.Name.Replace("<", "&lt;").Replace(">", "&gt;");          
          yield return new Tag(name, ((FieldInfo)mi).GetValue(obj));
        }
      }
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, true);
    }

    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {
      foreach (Tag t in tags) {
        string name = t.Name.Replace("&lt;", "<").Replace("&gt;", ">");
        MemberInfo[] mis = type.GetMember(name, AllInstanceMembers);
        if (mis.Length != 1)
          throw new PersistenceException("ambiguous struct member name " + name);
        MemberInfo mi = mis[0];
        if (mi.MemberType == MemberTypes.Field)
          ((FieldInfo)mi).SetValue(instance, t.Value);        
        else
          throw new PersistenceException("invalid struct member type " + mi.MemberType.ToString());
      }
    }
    
  }
}
