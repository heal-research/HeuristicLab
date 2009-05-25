using System;
using System.Collections.Generic;
using System.Reflection;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false)]
  public class StorableAttribute : Attribute {

    public string Name { get; set; }
    public object DefaultValue { get; set; }

    private const BindingFlags instanceMembers =
      BindingFlags.Instance |
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.DeclaredOnly;

    private static Dictionary<KeyValuePair<Type, bool>, IEnumerable<KeyValuePair<StorableAttribute, MemberInfo>>> memberCache =
      new Dictionary<KeyValuePair<Type, bool>, IEnumerable<KeyValuePair<StorableAttribute, MemberInfo>>>();

    public static IEnumerable<KeyValuePair<StorableAttribute, MemberInfo>> GetStorableMembers(Type type) {
      return GetStorableMembers(type, true);
    }

    public static IEnumerable<KeyValuePair<StorableAttribute, MemberInfo>>
        GetStorableMembers(Type type, bool inherited) {
      var query = new KeyValuePair<Type, bool>(type, inherited);
      if (memberCache.ContainsKey(query))
        return memberCache[query];
      IEnumerable<KeyValuePair<StorableAttribute, MemberInfo>> storablesMembers = GenerateStorableMembers(type, inherited);
      memberCache[query] = storablesMembers;
      return storablesMembers;
    }

    public static IEnumerable<KeyValuePair<StorableAttribute, MemberInfo>>
        GenerateStorableMembers(Type type, bool inherited) {
      List<KeyValuePair<StorableAttribute, MemberInfo>> storableMembers =
        new List<KeyValuePair<StorableAttribute, MemberInfo>>();
      if (inherited && type.BaseType != null)
        storableMembers.AddRange(GenerateStorableMembers(type.BaseType, true));        
      foreach (MemberInfo memberInfo in type.GetMembers(instanceMembers)) {
        foreach (object attribute in memberInfo.GetCustomAttributes(false)) {
          StorableAttribute storableAttribute =
            attribute as StorableAttribute;
          if (storableAttribute != null) {
            storableMembers.Add(new KeyValuePair<StorableAttribute, MemberInfo>(storableAttribute, memberInfo));
          }
        }
      }
      return storableMembers;
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
