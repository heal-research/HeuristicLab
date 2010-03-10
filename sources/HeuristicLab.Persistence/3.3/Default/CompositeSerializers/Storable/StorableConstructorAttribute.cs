using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {


  /// <summary>
  /// Indicate that this constructor should be used instead of the default constructor
  /// when the <code>StorableSerializer</code> instantiates this class during
  /// deserialization.
  /// 
  /// The constructor must take exactly one <code>bool</code> argument that will be
  /// set to <code>true</code> during deserialization.
  /// </summary>
  [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
  public sealed class StorableConstructorAttribute : Attribute {

    private static readonly BindingFlags allConstructors =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;    

    private static Dictionary<Type, ConstructorInfo> constructorCache =
      new Dictionary<Type, ConstructorInfo>();


    /// <summary>
    /// Get a designated storable constructor for a type or <code>null</code>.
    /// </summary>    
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
