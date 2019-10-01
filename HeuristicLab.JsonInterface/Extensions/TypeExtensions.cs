using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public static class TypeExtensions {

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
