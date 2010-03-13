using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {


  /// <summary>
  /// Indicate that this constructor should be used instead of the default constructor
  /// when the <c>StorableSerializer</c> instantiates this class during
  /// deserialization.
  /// 
  /// The constructor must take exactly one <c>bool</c> argument that will be
  /// set to <c>true</c> during deserialization.
  /// </summary>
  [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
  public sealed class StorableConstructorAttribute : Attribute {

    private static readonly BindingFlags allConstructors =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;    

    private static Dictionary<Type, ConstructorInfo> constructorCache =
      new Dictionary<Type, ConstructorInfo>();


    /// <summary>
    /// Get a designated storable constructor for a type or <c>null</c>.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The <see cref="ConstructorInfo"/> of the storable constructor or <c>null</c></returns>
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
