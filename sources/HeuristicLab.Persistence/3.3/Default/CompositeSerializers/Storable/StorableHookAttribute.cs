using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  public enum HookType { BeforeSerialization, AfterDeserialization };

  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
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
    public HookType HookType {
      get { return hookType; }
    }

    public StorableHookAttribute(HookType hookType) {
      this.hookType = hookType;
    }

    private static readonly BindingFlags instanceMembers =
      BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private static readonly object[] emptyArgs = new object[] { };

    private static Dictionary<HookDesignator, List<MethodInfo>> hookCache =
      new Dictionary<HookDesignator, List<MethodInfo>>();

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
      foreach (MemberInfo memberInfo in type.GetMembers(instanceMembers)) {
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