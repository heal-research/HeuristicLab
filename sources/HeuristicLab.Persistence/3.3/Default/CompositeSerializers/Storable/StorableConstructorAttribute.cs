using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
  public sealed class StorableConstructorAttribute : Attribute {

    private static readonly BindingFlags allConstructors =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;    

    private static Dictionary<Type, ConstructorInfo> constructorCache =
      new Dictionary<Type, ConstructorInfo>();

    public static ConstructorInfo GetStorableConstructor(Type type) {
      lock (constructorCache) {
        if (constructorCache.ContainsKey(type))
          return constructorCache[type];
        foreach (ConstructorInfo ci in type.GetConstructors(allConstructors)) {
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
  }
}
