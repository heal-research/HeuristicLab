using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class Util {
    public static bool IsTypeOf(object obj, Type t) {
      if (obj == null || t == null) return false;
      Type objType = obj.GetType();
      if (t.IsInterface && t.IsGenericType)
        return (objType == t || objType.IsAssignableFrom(t) || objType.GetInterfaces().Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == t));
      if (t.IsGenericType) {
        Type baseType = objType;
        while (baseType != typeof(object)) {
          if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == t) return true;
          baseType = baseType.BaseType;
        }
      }
      return objType == t;
    }
  }
}
