using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false)]
  public class StorableAttribute : Attribute {

    public string Name { get; set; }
    public object DefaultValue { get; set; }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[Storable");
      if (Name != null || DefaultValue != null)
        sb.Append('(');
      if (Name != null) {
        sb.Append("Name = \"").Append(Name).Append("\"");
        if (DefaultValue != null)
          sb.Append(", ");
      }
      if (DefaultValue != null)
        sb.Append("DefaultValue = \"").Append(DefaultValue).Append("\"");
      if (Name != null || DefaultValue != null)
        sb.Append(')');
      sb.Append(']');
      return sb.ToString();
    }

    private const BindingFlags instanceMembers =
      BindingFlags.Instance |
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.DeclaredOnly;

    public sealed class StorableMemberInfo {
      public StorableAttribute Attribute { get; private set; }
      public MemberInfo MemberInfo { get; private set; }
      public StorableMemberInfo(StorableAttribute attribute, MemberInfo memberInfo) {
        this.Attribute = attribute;
        this.MemberInfo = memberInfo;
      }
      public override string ToString() {
        return new StringBuilder()
          .Append('[').Append(Attribute).Append(", ")
          .Append(MemberInfo).Append('}').ToString();
      }
    }

    sealed class TypeQuery {
      public Type Type { get; private set; }
      public bool Inherited { get; private set; }
      public TypeQuery(Type type, bool inherited) {
        this.Type = type;
        this.Inherited = inherited;
      }
    }

    sealed class MemberCache : Dictionary<TypeQuery, IEnumerable<StorableMemberInfo>> { }

    private static MemberCache memberCache = new MemberCache();

    public static IEnumerable<StorableMemberInfo> GetStorableMembers(Type type) {
      return GetStorableMembers(type, true);
    }

    public static IEnumerable<StorableMemberInfo> GetStorableMembers(Type type, bool inherited) {
      var query = new TypeQuery(type, inherited);
      if (memberCache.ContainsKey(query))
        return memberCache[query];
      var storablesMembers = GenerateStorableMembers(type, inherited);
      memberCache[query] = storablesMembers;
      return storablesMembers;
    }

    private static IEnumerable<StorableMemberInfo> GenerateStorableMembers(Type type, bool inherited) {
      var storableMembers = new List<StorableMemberInfo>();
      if (inherited && type.BaseType != null)
        storableMembers.AddRange(GenerateStorableMembers(type.BaseType, true));
      foreach (MemberInfo memberInfo in type.GetMembers(instanceMembers)) {
        foreach (object attribute in memberInfo.GetCustomAttributes(false)) {
          StorableAttribute storableAttribute = attribute as StorableAttribute;
          if (storableAttribute != null) {
            storableMembers.Add(new StorableMemberInfo(storableAttribute, memberInfo));
          }
        }
      }
      return storableMembers;
    }

    public static Dictionary<string, DataMemberAccessor> GetStorableAccessors(object obj) {
      var storableAccessors = new Dictionary<string, DataMemberAccessor>();
      var nameMapping = createNameMapping(obj.GetType());
      var finalNameMapping = analyzeNameMapping(nameMapping);
      foreach (var mapping in finalNameMapping) {
        storableAccessors.Add(mapping.Value.Attribute.Name ?? mapping.Key,
          new DataMemberAccessor(
            mapping.Value.MemberInfo,
            mapping.Value.Attribute.Name ?? mapping.Key,
            mapping.Value.Attribute.DefaultValue,
            obj));
      }
      return storableAccessors;
    }

    private static Dictionary<string, StorableMemberInfo> analyzeNameMapping(
        Dictionary<string, List<StorableMemberInfo>> nameMapping) {
      var finalNameMapping = new Dictionary<string, StorableMemberInfo>();
      foreach (var attributes in nameMapping) {
        if (attributes.Value.Count == 1) {
          finalNameMapping[attributes.Key] = attributes.Value[0];
        } else if (attributes.Value[0].MemberInfo.MemberType == MemberTypes.Field) {
          foreach (var attribute in attributes.Value) {
            StringBuilder sb = new StringBuilder();
            sb.Append(attribute.MemberInfo.ReflectedType.FullName).Append('.')
              .Append(attribute.MemberInfo.Name);
            finalNameMapping[sb.ToString()] = attribute;
          }
        } else {
          var uniqueAccessors = new Dictionary<Type, StorableMemberInfo>();
          foreach (var attribute in attributes.Value) {
            uniqueAccessors[((PropertyInfo)attribute.MemberInfo).GetGetMethod(true).GetBaseDefinition().DeclaringType] =
              attribute;
          }
          if (uniqueAccessors.Count == 1) {
            var it = uniqueAccessors.Values.GetEnumerator();
            it.MoveNext();
            finalNameMapping[attributes.Key] = it.Current;
          } else {
            foreach (var attribute in uniqueAccessors.Values) {
              StringBuilder sb = new StringBuilder();
              sb.Append(attribute.MemberInfo.DeclaringType.FullName).Append('.')
                .Append(attribute.MemberInfo.Name);
              finalNameMapping[sb.ToString()] = attribute;
            }
          }
        }
      }
      return finalNameMapping;
    }

    private static Dictionary<string, List<StorableMemberInfo>> createNameMapping(Type type) {
      var nameMapping = new Dictionary<string, List<StorableMemberInfo>>();
      foreach (StorableMemberInfo storable in GetStorableMembers(type)) {
        if (!nameMapping.ContainsKey(storable.MemberInfo.Name))
          nameMapping[storable.MemberInfo.Name] = new List<StorableMemberInfo>();
        nameMapping[storable.MemberInfo.Name].Add(storable);
      }
      return nameMapping;
    }
  }
}
