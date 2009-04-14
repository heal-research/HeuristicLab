using System;
using System.Collections.Generic;
using System.Reflection;

namespace HeuristicLab.Persistence.Core {

  [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple=false,
    Inherited=false)]
  public class StorableAttribute : Attribute {

    public string Name { get; set; }
    public object DefaultValue { get; set; }

    private const BindingFlags instanceMembers =
      BindingFlags.Instance |
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.DeclaredOnly;
    
    public static IEnumerable<KeyValuePair<StorableAttribute, MemberInfo>> GetStorableMembers(Type type) {
      return GetStorableMembers(type, true);      
    }

    public static IEnumerable<KeyValuePair<StorableAttribute, MemberInfo>>
        GetStorableMembers(Type type, bool inherited) {
      if (type.BaseType != null)
        foreach ( var pair in GetStorableMembers(type.BaseType) )
          yield return pair;
      foreach (MemberInfo memberInfo in type.GetMembers(instanceMembers)) {
        foreach (object attribute in memberInfo.GetCustomAttributes(false)) {          
          StorableAttribute storableAttribute =
            attribute as StorableAttribute;
          if (storableAttribute != null) {
            yield return new KeyValuePair<StorableAttribute, MemberInfo>(storableAttribute, memberInfo);            
          }
        }
      }      
    }

    public static Dictionary<string, DataMemberAccessor> GetStorableAccessors(object obj) {
      Dictionary<string, DataMemberAccessor> storableAccessors =
        new Dictionary<string, DataMemberAccessor>();
      foreach (KeyValuePair<StorableAttribute, MemberInfo> pair in GetStorableMembers(obj.GetType())) {
        storableAccessors.Add(pair.Value.Name,
          new DataMemberAccessor(pair.Value, pair.Key, obj));
      }            
      return storableAccessors;
    }    
  }
}
