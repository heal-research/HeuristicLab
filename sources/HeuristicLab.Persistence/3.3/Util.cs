using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace HeuristicLab.Persistence {  

  public class InterfaceInstantiatior {
    public static List<T> InstantiateAll<T>() {
      return InstantiateAll<T>(Assembly.GetExecutingAssembly());
    }
    public static List<T> InstantiateAll<T>(Assembly a) {
      List<T> instances = new List<T>();
      foreach (Type t in a.GetTypes()) {
        if (t.GetInterface(typeof(T).FullName) != null) {
          ConstructorInfo ci = t.GetConstructor(new Type[] { });
          if (ci != null) {
            instances.Add((T)ci.Invoke(new object[] { }));
          }
        }
      }
      return instances;
    }
  }
  public class Util {
    public static string AutoFormat(object o) {
      return AutoFormat(o, false);
    }
    public static string AutoFormat(object o, bool recursive) {
      Dictionary<object, int> visitedObjects = new Dictionary<object,int>();
      return AutoFormat(o, recursive, visitedObjects);
    }
    private static string AutoFormat(object o, bool recursive, IDictionary<object, int> visitedObjects) {
      string s = o as string;
      if (s != null)
        return s;
      if (o == null) {
        return "<null>";
      }
      if (visitedObjects.ContainsKey(o)) {
        return o.ToString();
      }
      visitedObjects.Add(o, 0);
      if (o.ToString() != o.GetType().ToString()) {
        return o.ToString();
      }
      StringBuilder sb = new StringBuilder();
      sb.Append(o.GetType().Name);
      sb.Append("(");
      foreach (MemberInfo mInfo in o.GetType().GetMembers(
        BindingFlags.Public |
        BindingFlags.NonPublic |
        BindingFlags.Instance)) {
        if (mInfo.MemberType == MemberTypes.Field) {
          FieldInfo fInfo = (FieldInfo)mInfo;          
          sb.Append(mInfo.Name);
          sb.Append("=");          
          if (recursive) {
            sb.Append(AutoFormat(fInfo.GetValue(o), true, visitedObjects));
          } else {
            sb.Append(fInfo.GetValue(o));
          }
          sb.Append(", ");
        }
      }
      sb.Remove(sb.Length - 2, 2);
      sb.Append(")");
      return sb.ToString();
    }
  }
}
