using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Manufacture {
  public static class TypeExtensions {
    public static int GetInterfaceDistance(this Type type, Type interfaceType) {
      if (!interfaceType.IsInterface) return -1;
      int distance = 0;
      Type baseType = type;
      while (baseType != typeof(object)) {
        var interfaces = baseType.GetInterfaces();
        var minimalInterfaces = interfaces.Except(interfaces.SelectMany(i => i.GetInterfaces()));
        if (baseType == interfaceType && minimalInterfaces.Any(i => i == interfaceType))
          ++distance;
        baseType = baseType.BaseType;
      }
      return distance;
    }
    public static bool IsEqualTo(this Type type, Type other) {
      if (other == null) throw new ArgumentNullException("other");
      if (type == other) return true;
      if (other.IsInterface && other.IsGenericType)
        return
          type.IsAssignableFrom(other) ||
          type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .Any(i => i.GetGenericTypeDefinition() == other);
      else if (other.IsInterface) {
        /*
        Type baseType = type;
        while (baseType != typeof(object)) {
          var interfaces = baseType.GetInterfaces();
          var minimalInterfaces = interfaces.Except(interfaces.SelectMany(i => i.GetInterfaces()));
          if (baseType == other && minimalInterfaces.Any(i => i == other))
            return true;
          baseType = baseType.BaseType;
        }
        */
        return type.GetInterfaces().Any(i => i == other);
      }
        
      else if (other.IsGenericType) {
        Type baseType = type;
        while (baseType != typeof(object)) {
          if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == other)
            return true;
          baseType = baseType.BaseType;
        }
      }
      return type.IsAssignableFrom(other);
    }
  }
}
