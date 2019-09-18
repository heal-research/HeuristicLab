using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParameterTest {
  public static class ObjectExtensions {
    public static T Cast<T>(this object obj) => (T)obj;
  }
}
