using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Reflection;
using HeuristicLab.Persistence.Auxiliary;
using System.Text;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  /// <summary>
  /// Intended for serialization of all custom classes. Classes should have the
  /// <c>[StorableClass]</c> attribute set and a serialization mode set.
  /// Optionally selected fields and properties can be marked with the
  /// <c>[Storable]</c> attribute.
  /// </summary>
  [StorableClass]    
  public class StorableSerializer : ICompositeSerializer {


    #region ICompositeSerializer implementation

    public int Priority {
      get { return 200; }
    }

    public bool CanSerialize(Type type) {
      if (!ReflectionTools.HasDefaultConstructor(type) &&
        GetStorableConstructor(type) == null)
        return false;
      return IsEmptyOrStorableType(type, true);
    }

    public string JustifyRejection(Type type) {
      if (!ReflectionTools.HasDefaultConstructor(type) &&
        GetStorableConstructor(type) == null)
        return "no default constructor and no storable constructor";
      if (!IsEmptyOrStorableType(type, true))
        return "class is not marked with the storable class attribute";
      return "no reason";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      StorableHookAttribute.InvokeHook(HookType.BeforeSerialization, o);
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object obj) {
      foreach (var accessor in GetStorableAccessors(obj)) {
        yield return new Tag(accessor.Name, accessor.Get());
      }
    }

    private static readonly object[] defaultArgs = new object[] { true };

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      try {
        ConstructorInfo constructor = GetStorableConstructor(type);
        return constructor != null ? constructor.Invoke(defaultArgs) : Activator.CreateInstance(type, true);
      } catch (TargetInvocationException x) {
        throw new PersistenceException(
          "Could not instantiate storable object: Encountered exception during constructor call",
          x.InnerException);
      }
    }

    public void Populate(object instance, IEnumerable<Tag> objects, Type type) {
      var memberDict = new Dictionary<string, Tag>();
      IEnumerator<Tag> iter = objects.GetEnumerator();
      while (iter.MoveNext()) {
        memberDict.Add(iter.Current.Name, iter.Current);
      }      
      foreach (var accessor in GetStorableAccessors(instance)) {
        if (memberDict.ContainsKey(accessor.Name)) {
          accessor.Set(memberDict[accessor.Name].Value);
        } else if (accessor.DefaultValue != null) {
          accessor.Set(accessor.DefaultValue);
        }
      }
      StorableHookAttribute.InvokeHook(HookType.AfterDeserialization, instance);
    }

    #endregion

    #region constances & private data types

    private const BindingFlags ALL_CONSTRUCTORS =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private const BindingFlags DECLARED_INSTANCE_MEMBERS =
      BindingFlags.Instance |
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.DeclaredOnly;

    private sealed class StorableMemberInfo {
      public StorableAttribute Attribute { get; private set; }
      public MemberInfo MemberInfo { get; private set; }
      public string DisentangledName { get; private set; }
      public string FullyQualifiedMemberName {
        get {
          return new StringBuilder()
            .Append(MemberInfo.ReflectedType.FullName)
            .Append('.')
            .Append(MemberInfo.Name)
            .ToString();
        }
      }
      public StorableMemberInfo(StorableAttribute attribute, MemberInfo memberInfo) {
        this.Attribute = attribute;
        this.MemberInfo = memberInfo;
      }
      public override string ToString() {
        return new StringBuilder()
          .Append('[').Append(Attribute).Append(", ")
          .Append(MemberInfo).Append('}').ToString();
      }
      public void SetDisentangledName(string name) {
        DisentangledName = Attribute.Name ?? name;
      }
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

    #endregion

    #region caches

    private MemberCache storableMemberCache = new MemberCache();
    private Dictionary<Type, ConstructorInfo> constructorCache = 
      new Dictionary<Type, ConstructorInfo>();

    #endregion

    #region auxiliary attribute reflection tools

    private IEnumerable<StorableMemberInfo> GetStorableMembers(Type type) {
      return GetStorableMembers(type, true);
    }

    private IEnumerable<StorableMemberInfo> GetStorableMembers(Type type, bool inherited) {
      lock (storableMemberCache) {
        var query = new TypeQuery(type, inherited);
        if (storableMemberCache.ContainsKey(query))
          return storableMemberCache[query];
        var storablesMembers = GenerateStorableMembers(type, inherited);
        storableMemberCache[query] = storablesMembers;
        return storablesMembers;
      }
    }

    private static IEnumerable<StorableMemberInfo> GenerateStorableMembers(Type type, bool inherited) {
      var storableMembers = new List<StorableMemberInfo>();
      if (inherited && type.BaseType != null)
        storableMembers.AddRange(GenerateStorableMembers(type.BaseType, true));
      foreach (MemberInfo memberInfo in type.GetMembers(DECLARED_INSTANCE_MEMBERS)) {
        foreach (StorableAttribute attribute in memberInfo.GetCustomAttributes(typeof(StorableAttribute), false)) {
          storableMembers.Add(new StorableMemberInfo(attribute, memberInfo));
        }
      }
      return DisentangleNameMapping(storableMembers);
    }

    private IEnumerable<DataMemberAccessor> GetStorableAccessors(object obj) {
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

    private static bool IsEmptyOrStorableType(Type type, bool recusrive) {
      if (IsEmptyType(type, recusrive)) return true;
      if (!HastStorableClassAttribute(type)) return false;
      return !recusrive || type.BaseType == null || IsEmptyOrStorableType(type.BaseType, true);
    }

    private static bool HastStorableClassAttribute(Type type) {
      return type.GetCustomAttributes(typeof(StorableClassAttribute), false).Length > 0;
    }

    private static bool IsEmptyType(Type type, bool recursive) {
      foreach (MemberInfo memberInfo in type.GetMembers(DECLARED_INSTANCE_MEMBERS)) {
        if (IsModifiableMember(memberInfo)) return false;
      }
      return !recursive || type.BaseType == null || IsEmptyType(type.BaseType, true);
    }

    private static bool IsModifiableMember(MemberInfo memberInfo) {
      return memberInfo.MemberType == MemberTypes.Field && IsModifiableField((FieldInfo)memberInfo) ||
                memberInfo.MemberType == MemberTypes.Property && IsModifiableProperty((PropertyInfo)memberInfo);
    }

    private static bool IsModifiableField(FieldInfo fi) {
      return !fi.IsLiteral && !fi.IsInitOnly;
    }

    private static bool IsModifiableProperty(PropertyInfo pi) {
      return pi.CanWrite;
    }

    private ConstructorInfo GetStorableConstructor(Type type) {
      lock (constructorCache) {
        if (constructorCache.ContainsKey(type))
          return constructorCache[type];
        foreach (ConstructorInfo ci in type.GetConstructors(ALL_CONSTRUCTORS)) {
          if (ci.GetCustomAttributes(typeof(StorableConstructorAttribute), false).Length > 0) {
            if (ci.GetParameters().Length != 1 ||
                ci.GetParameters()[0].ParameterType != typeof(bool))
              throw new PersistenceException("StorableConstructor must have exactly one argument of type bool");
            constructorCache[type] = ci;
            return ci;
          }
        }
        constructorCache[type] = null;
        return null;
      }
    }

    #endregion
  }
}