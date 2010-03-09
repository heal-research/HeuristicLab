using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  public enum HookType { BeforeSerialization, AfterDeserialization };

  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
  public sealed class StorableHookAttribute : Attribute {

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

    public static void InvokeHook(HookType hookType, object obj) {
      if (obj == null)
        throw new ArgumentNullException("Cannot invoke hooks on null");
      Type type = obj.GetType();
      foreach (MemberInfo memberInfo in type.GetMembers(instanceMembers)) {
        foreach (object attribute in memberInfo.GetCustomAttributes(false)) {
          StorableHookAttribute hook = attribute as StorableHookAttribute;
          if (hook != null && hook.HookType == hookType) {
            MethodInfo methodInfo = memberInfo as MethodInfo;
            if (memberInfo.MemberType != MemberTypes.Method || memberInfo == null)
              throw new ArgumentException("Storable hooks must be methods");            
            methodInfo.Invoke(obj, emptyArgs);            
          }
        }
      }
    }

  }
}
