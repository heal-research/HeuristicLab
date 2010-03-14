using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {


  /// <summary>
  /// Mark the member of a class to be considered by the <c>StorableSerializer</c>.
  /// The class must be marked as <c>[StorableClass]</c> and the
  /// <c>StorableClassType</c> should be set to <c>MarkedOnly</c> for
  /// this attribute to kick in.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false)]    
  public class StorableAttribute : Attribute {

    /// <summary>
    /// An optional name for this member that will be used during serialization.
    /// This allows to rename a field/property in code but still be able to read
    /// the old serialized format.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }


    /// <summary>
    /// A default value in case the field/property was not present or not serialized
    /// in a previous version of the class and could therefore be absent during
    /// deserialization.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
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

    /// <summary>
    /// Encapsulate information about storable members of a class
    /// that have the storable attribute set.
    /// </summary>
    public sealed class StorableMemberInfo {

      /// <summary>
      /// Gets the [Storable] attribute itself.
      /// </summary>
      /// <value>The [Storable] attribute.</value>
      public StorableAttribute Attribute { get; private set; }

      /// <summary>
      /// Gets the .NET reflection MemberInfo.
      /// </summary>
      /// <value>The member info.</value>
      public MemberInfo MemberInfo { get; private set; }


      /// <summary>
      /// Gets disentangled name (i.e. unique access name regardless of
      /// type hierarchy.
      /// </summary>
      /// <value>The disentangled name.</value>
      public string DisentangledName { get; private set; }


      /// <summary>
      /// Gets the fully qualified member name.
      /// </summary>
      /// <value>The the fully qualified member name.</value>
      public string FullyQualifiedMemberName {
        get {
          return new StringBuilder()
            .Append(MemberInfo.ReflectedType.FullName)
            .Append('.')
            .Append(MemberInfo.Name)
            .ToString();
        }
      }

      internal StorableMemberInfo(StorableAttribute attribute, MemberInfo memberInfo) {
        this.Attribute = attribute;
        this.MemberInfo = memberInfo;
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public override string ToString() {
        return new StringBuilder()
          .Append('[').Append(Attribute).Append(", ")
          .Append(MemberInfo).Append('}').ToString();
      }

      internal void SetDisentangledName(string name) {
        DisentangledName = Attribute.Name ?? name;
      }

      /// <summary>
      /// Gets the delcaring type of the property.
      /// </summary>
      /// <returns></returns>
      public Type GetPropertyDeclaringBaseType() {
        return ((PropertyInfo)MemberInfo).GetGetMethod(true).GetBaseDefinition().DeclaringType;
      }
    }

    private sealed class TypeQuery {
      public Type Type { get; private set; }
      public bool Inherited { get; private set; }
      public TypeQuery(Type type, bool inherited) {
        this.Type = type;
        this.Inherited = inherited;
      }
    }

    private sealed class MemberCache : Dictionary<TypeQuery, IEnumerable<StorableMemberInfo>> { }

    private static MemberCache memberCache = new MemberCache();


    /// <summary>
    /// Get all fields and properties of a class that have the
    /// <c>[Storable]</c> attribute set.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>An enumerable of StorableMemberInfos.</returns>
    public static IEnumerable<StorableMemberInfo> GetStorableMembers(Type type) {
      return GetStorableMembers(type, true);
    }

    /// <summary>
    /// Get all fields and properties of a class that have the
    /// <c>[Storable]</c> attribute set.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="inherited">should storable members from base classes be included</param>
    /// <returns>An enumerable of StorableMemberInfos</returns>
    public static IEnumerable<StorableMemberInfo> GetStorableMembers(Type type, bool inherited) {
      lock (memberCache) {
        var query = new TypeQuery(type, inherited);
        if (memberCache.ContainsKey(query))
          return memberCache[query];
        var storablesMembers = GenerateStorableMembers(type, inherited);
        memberCache[query] = storablesMembers;
        return storablesMembers;
      }
    }

    private static IEnumerable<StorableMemberInfo> GenerateStorableMembers(Type type, bool inherited) {
      var storableMembers = new List<StorableMemberInfo>();
      if (inherited && type.BaseType != null)
        storableMembers.AddRange(GenerateStorableMembers(type.BaseType, true));
      foreach (MemberInfo memberInfo in type.GetMembers(instanceMembers)) {
        foreach (StorableAttribute attribute in memberInfo.GetCustomAttributes(typeof(StorableAttribute), false)) {          
          storableMembers.Add(new StorableMemberInfo(attribute, memberInfo));          
        }
      }
      return DisentangleNameMapping(storableMembers);
    }


    /// <summary>
    /// Get the associated accessors for all storable memebrs.
    /// </summary>
    /// <param name="obj">The object</param>
    /// <returns>An enumerable of storable accessors.</returns>
    public static IEnumerable<DataMemberAccessor> GetStorableAccessors(object obj) {      
      foreach (var memberInfo in GetStorableMembers(obj.GetType()))
        yield return new DataMemberAccessor(
          memberInfo.MemberInfo,
          memberInfo.DisentangledName,
          memberInfo.Attribute.DefaultValue,
          obj);      
    }

    private static IEnumerable<StorableMemberInfo> DisentangleNameMapping(
        IEnumerable<StorableMemberInfo> storableMemberInfos) {
      var nameGrouping = new Dictionary<string, List<StorableMemberInfo>>();
      foreach (StorableMemberInfo storable in storableMemberInfos) {
        if (!nameGrouping.ContainsKey(storable.MemberInfo.Name))
          nameGrouping[storable.MemberInfo.Name] = new List<StorableMemberInfo>();
        nameGrouping[storable.MemberInfo.Name].Add(storable);
      }
      var memberInfos = new List<StorableMemberInfo>();
      foreach (var storableMemberInfoGroup in nameGrouping.Values) {        
        if (storableMemberInfoGroup.Count == 1) {
          storableMemberInfoGroup[0].SetDisentangledName(storableMemberInfoGroup[0].MemberInfo.Name);
          memberInfos.Add(storableMemberInfoGroup[0]);
        } else if (storableMemberInfoGroup[0].MemberInfo.MemberType == MemberTypes.Field) {
          foreach (var storableMemberInfo in storableMemberInfoGroup) {            
            storableMemberInfo.SetDisentangledName(storableMemberInfo.FullyQualifiedMemberName);
            memberInfos.Add(storableMemberInfo);
          }
        } else {          
          memberInfos.AddRange(MergePropertyAccessors(storableMemberInfoGroup));
        }
      }
      return memberInfos;
    }
    
    private static IEnumerable<StorableMemberInfo> MergePropertyAccessors(List<StorableMemberInfo> members) {
      var uniqueAccessors = new Dictionary<Type, StorableMemberInfo>();
      foreach (var member in members)
        uniqueAccessors[member.GetPropertyDeclaringBaseType()] = member;                  
      if (uniqueAccessors.Count == 1) {
        var storableMemberInfo = uniqueAccessors.Values.First();
        storableMemberInfo.SetDisentangledName(storableMemberInfo.MemberInfo.Name);
        yield return storableMemberInfo;
      } else {
        foreach (var attribute in uniqueAccessors.Values) {
          attribute.SetDisentangledName(attribute.FullyQualifiedMemberName);
          yield return attribute;
        }
      }
    }    
  }
}
