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

    private static readonly object[] defaultArgs = new object[] { true };

    public static object CallStorableConstructor(Type type) {
      foreach (ConstructorInfo constructorInfo in type.GetConstructors(allConstructors)) {
        if (constructorInfo
          .GetCustomAttributes(typeof(StorableConstructorAttribute), false).Length > 0) {
          if (constructorInfo.GetParameters().Length != 1 ||
              constructorInfo.GetParameters()[0].ParameterType != typeof(bool))
            throw new PersistenceException("StorableConstructor must have exactly one argument of type bool");
          return constructorInfo.Invoke(defaultArgs);
        }
      }
      return null;
    }

  }
}
