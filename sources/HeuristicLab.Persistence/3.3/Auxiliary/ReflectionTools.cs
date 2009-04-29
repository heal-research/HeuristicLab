using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace HeuristicLab.Persistence.Auxiliary {

  internal class ReflectionTools {

    public static bool HasDefaultConstructor(Type t) {
      return t.GetConstructor(
        BindingFlags.Instance |
        BindingFlags.NonPublic |
        BindingFlags.Public,
        null, Type.EmptyTypes, null) != null;
    }

  }
}
