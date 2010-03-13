using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {


  /// <summary>
  /// Indicates the time at which the hook should be invoked.
  /// </summary>
  public enum HookType { BeforeSerialization, AfterDeserialization };


  /// <summary>
  /// Mark methods that should be called at certain times during
  /// serialization/deserialization by the <c>StorableSerializer</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
  public sealed class StorableHookAttribute : Attribute {

    private sealed class HookDesignator {
      public Type Type { get; set; }
      public HookType HookType { get; set; }
      public HookDesignator() { }
      public HookDesignator(Type type, HookType hookType) {
        Type = type;
        HookType = HookType;
      }
    }

    private readonly HookType hookType;
    /// <summary>
    /// Gets the type of the hook.
    /// </summary>
    /// <value>The type of the hook.</value>
    public HookType HookType {
      get { return hookType; }
    }


    /// <summary>
    /// Mark method as <c>StorableSerializer</c> hook to be run
    /// at the <c>HookType</c> time.
    /// </summary>
    /// <param name="hookType">Type of the hook.</param>
    public StorableHookAttribute(HookType hookType) {
      this.hookType = hookType;
    }

    private static readonly BindingFlags declaredInstanceMembers =
      BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

    private static readonly object[] emptyArgs = new object[] { };

    private static Dictionary<HookDesignator, List<MethodInfo>> hookCache =
      new Dictionary<HookDesignator, List<MethodInfo>>();


    /// <summary>
    /// Invoke <c>hookType</c> hook on <c>obj</c>.
    /// </summary>
    /// <param name="hookType">Type of the hook.</param>
    /// <param name="obj">The object.</param>
    public static void InvokeHook(HookType hookType, object obj) {
      if (obj == null)
        throw new ArgumentNullException("Cannot invoke hooks on null");
      foreach (MethodInfo mi in GetHooks(hookType, obj.GetType())) {
        mi.Invoke(obj, emptyArgs);
      }
    }    

    private static IEnumerable<MethodInfo> GetHooks(HookType hookType, Type type) {
      lock (hookCache) {
        List<MethodInfo> hooks;
        var designator = new HookDesignator(type, hookType);
        hookCache.TryGetValue(designator, out hooks);
        if (hooks != null)
          return hooks;
        hooks = new List<MethodInfo>(CollectHooks(hookType, type));
        hookCache.Add(designator, hooks);
        return hooks;
      }
    }

    private static IEnumerable<MethodInfo> CollectHooks(HookType hookType, Type type) {  
      if (type.BaseType != null)
        foreach (var mi in CollectHooks(hookType, type.BaseType))
          yield return mi;
      foreach (MemberInfo memberInfo in type.GetMembers(declaredInstanceMembers)) {
        foreach (StorableHookAttribute hook in memberInfo.GetCustomAttributes(typeof(StorableHookAttribute), false)) {
          if (hook != null && hook.HookType == hookType) {
            MethodInfo methodInfo = memberInfo as MethodInfo;
            if (memberInfo.MemberType != MemberTypes.Method || memberInfo == null)
              throw new ArgumentException("Storable hooks must be methods");
            yield return methodInfo;
          }
        }
      }
    }

  }
}