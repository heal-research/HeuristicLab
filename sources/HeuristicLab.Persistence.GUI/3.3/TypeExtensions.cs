using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Persistence.GUI {

  internal static class TypeExtensions {

    public static string SimpleFullName(this Type type) {
      StringBuilder sb = new StringBuilder();
      SimpleFullName(type, sb);
      return sb.ToString();
    }

    private static void SimpleFullName(Type type, StringBuilder sb) {
      if (type.IsGenericType) {
        sb.Append(type.Name, 0, type.Name.LastIndexOf('`'));
        sb.Append("<");
        foreach (Type t in type.GetGenericArguments()) {
          SimpleFullName(t, sb);
          sb.Append(", ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(">");
      } else {
        sb.Append(type.Name);
      }
    }

  }
  
}
