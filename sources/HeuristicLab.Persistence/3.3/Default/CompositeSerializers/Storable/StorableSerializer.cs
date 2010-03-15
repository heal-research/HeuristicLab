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
  /// <c>[StorableClass]</c> attribute set. The default mode is to serialize
  /// members with the <c>[Storable]</c> attribute set. Alternatively the
  /// storable mode can be set to <c>AllFields</c>, <c>AllProperties</c>
  /// or <c>AllFieldsAndAllProperties</c>.
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
      return StorableReflection.IsEmptyOrStorableType(type, true);
    }

    public string JustifyRejection(Type type) {
      if (!ReflectionTools.HasDefaultConstructor(type) &&
        GetStorableConstructor(type) == null)
        return "no default constructor and no storable constructor";
      if (!StorableReflection.IsEmptyOrStorableType(type, true))
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

    #region constants & private data types

    private const BindingFlags ALL_CONSTRUCTORS =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

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

    #region attribute access

    private IEnumerable<StorableMemberInfo> GetStorableMembers(Type type) {
      return GetStorableMembers(type, true);
    }

    private IEnumerable<StorableMemberInfo> GetStorableMembers(Type type, bool inherited) {
      lock (storableMemberCache) {
        var query = new TypeQuery(type, inherited);
        if (storableMemberCache.ContainsKey(query))
          return storableMemberCache[query];
        var storablesMembers = StorableReflection.GenerateStorableMembers(type, inherited);
        storableMemberCache[query] = storablesMembers;
        return storablesMembers;
      }
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

    private IEnumerable<DataMemberAccessor> GetStorableAccessors(object obj) {
      return GetStorableMembers(obj.GetType())
        .Select(mi => new DataMemberAccessor(mi.MemberInfo, mi.DisentangledName, mi.DefaultValue, obj));
    }

    #endregion
    
  }
  
}