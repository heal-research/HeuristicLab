using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Default.Decomposers {
  
  [EmptyStorableClass]
  public class TypeDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }

    public bool CanDecompose(Type type) {
      return type == typeof (Type) ||
             type.VersionInvariantName() == "System.RuntimeType, mscorlib";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      yield return new Tag("VersionInvariantName", ((Type)o).VersionInvariantName());
    }

    public IEnumerable<Tag> Decompose(object obj) {
      return new Tag[] { };
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      foreach (var typeName in metaInfo) {
        return Type.GetType((string)typeName.Value);
      }
      return null;      
    }

    public void Populate(object instance, IEnumerable<Tag> objects, Type type) {      
    }
  }
}
