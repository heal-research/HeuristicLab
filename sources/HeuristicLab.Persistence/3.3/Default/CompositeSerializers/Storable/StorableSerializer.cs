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
  public sealed class StorableSerializer : ICompositeSerializer {

    #region ICompositeSerializer implementation

    /// <summary>
    /// Priority 200, one of the first default composite serializers to try.
    /// </summary>
    /// <value></value>
    public int Priority {
      get { return 200; }
    }

    /// <summary>
    /// Determines for every type whether the composite serializer is applicable.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// 	<c>true</c> if this instance can serialize the specified type; otherwise, <c>false</c>.
    /// </returns>
    public bool CanSerialize(Type type) {
      if (!ReflectionTools.HasDefaultConstructor(type) &&
        GetStorableConstructor(type) == null)
        return false;
      return StorableReflection.IsEmptyOrStorableType(type, true);
    }

    /// <summary>
    /// Give a reason if possibly why the given type cannot be serialized by this
    /// ICompositeSerializer.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// A string justifying why type cannot be serialized.
    /// </returns>
    public string JustifyRejection(Type type) {
      if (!ReflectionTools.HasDefaultConstructor(type) &&
        GetStorableConstructor(type) == null)
        return "no default constructor and no storable constructor";
      if (!StorableReflection.IsEmptyOrStorableType(type, true))
        return "class is not marked with the storable class attribute";
      return "no reason";
    }

    /// <summary>
    /// Creates the meta info.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <returns>A list of storable components.</returns>
    public IEnumerable<Tag> CreateMetaInfo(object o) {
      InvokeHook(HookType.BeforeSerialization, o);
      return new Tag[] { };
    }

    /// <summary>
    /// Decompose an object into <see cref="Tag"/>s, the tag name can be null,
    /// the order in which elements are generated is guaranteed to be
    /// the same as they will be supplied to the Populate method.
    /// </summary>
    /// <param name="obj">An object.</param>
    /// <returns>An enumerable of <see cref="Tag"/>s.</returns>
    public IEnumerable<Tag> Decompose(object obj) {
      foreach (var accessor in GetStorableAccessors(obj)) {
        yield return new Tag(accessor.Name, accessor.Get());
      }
    }

    private static readonly object[] defaultArgs = new object[] { true };

    /// <summary>
    /// Create an instance of the object using the provided meta information.
    /// </summary>
    /// <param name="type">A type.</param>
    /// <param name="metaInfo">The meta information.</param>
    /// <returns>A fresh instance of the provided type.</returns>
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

    /// <summary>
    /// Populates the specified instance.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="objects">The objects.</param>
    /// <param name="type">The type.</param>
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
      InvokeHook(HookType.AfterDeserialization, instance);
    }

    #endregion

    #region constants & private data types

    private const BindingFlags ALL_CONSTRUCTORS =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private static readonly object[] emptyArgs = new object[] { };

    private sealed class TypeQuery {
      public Type Type { get; private set; }
      public bool Inherited { get; private set; }
      public TypeQuery(Type type, bool inherited) {
        this.Type = type;
        this.Inherited = inherited;
      }
    }

    private sealed class HookDesignator {
      public Type Type { get; set; }
      public HookType HookType { get; set; }
      public HookDesignator() { }
      public HookDesignator(Type type, HookType hookType) {
        Type = type;
        HookType = HookType;
      }
    }

    private sealed class MemberCache : Dictionary<TypeQuery, IEnumerable<StorableMemberInfo>> { }

    #endregion

    #region caches

    private MemberCache storableMemberCache = new MemberCache();
    private Dictionary<Type, ConstructorInfo> constructorCache =
      new Dictionary<Type, ConstructorInfo>();
    
    private Dictionary<HookDesignator, List<MethodInfo>> hookCache =
      new Dictionary<HookDesignator, List<MethodInfo>>();

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

    private void InvokeHook(HookType hookType, object obj) {
      if (obj == null)
        throw new ArgumentNullException("Cannot invoke hooks on null");
      foreach (MethodInfo mi in GetHooks(hookType, obj.GetType())) {
        mi.Invoke(obj, emptyArgs);
      }
    }

    private IEnumerable<MethodInfo> GetHooks(HookType hookType, Type type) {
      lock (hookCache) {
        List<MethodInfo> hooks;
        var designator = new HookDesignator(type, hookType);
        hookCache.TryGetValue(designator, out hooks);
        if (hooks != null)
          return hooks;
        hooks = new List<MethodInfo>(StorableReflection.CollectHooks(hookType, type));
        hookCache.Add(designator, hooks);
        return hooks;
      }
    }

    #endregion

    
    
  }
  
}