using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Default.Decomposers {
  
  public class TypeDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }

    public bool CanDecompose(Type type) {
      return type == typeof (Type) ||
             type.VersionInvariantName() == "System.RuntimeType, mscorlib";
    }

    public IEnumerable<Tag> DeCompose(object obj) {
      Type t = (Type) obj;
      yield return new Tag("VersionInvariantName", t.VersionInvariantName());
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> objects, Type type) {
      foreach ( var typeName in objects ) {
        return Type.GetType((string)typeName.Value);
      }
      return null;
    }
  }
}
