using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking._3D.Utils.Tests {
  internal static class TestUtils {
    public static object InvokeStaticMethod(Type type, string methodName, object[] parameters) {
      var method = type.GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
      return method.Invoke(null, parameters);
    }

    /// <summary>
    /// Invokes a private method of an given object by using reflection
    /// </summary>
    /// <param name="type"></param>
    /// <param name="o"></param>
    /// <param name="methodName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static object InvokeMethod(Type type, object o, string methodName, object[] parameters) {
      var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
      return method.Invoke(o, parameters);
    }
  }
}
