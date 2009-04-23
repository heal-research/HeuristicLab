using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.Decomposers {

  [EmptyStorableClass]
  public class TypeDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }

    public bool CanDecompose(Type type) {
      return type == typeof(Type) ||
             type.VersionInvariantName() == "System.RuntimeType, mscorlib";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      yield return new Tag("VersionInvariantName", ((Type)o).VersionInvariantName());
    }

    public IEnumerable<Tag> Decompose(object obj) {
      return new Tag[] { };
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      IEnumerator<Tag> it = metaInfo.GetEnumerator();
      try {
        it.MoveNext();
      } catch (InvalidOperationException e) {
        throw new PersistenceException("Insufficient meta information to instantiate Type object", e);
      }
      try {
        return Type.GetType((string)it.Current.Value, true);
      } catch (InvalidCastException e) {
        throw new PersistenceException("Invalid meta information during reconstruction of Type object", e);
      } catch (TypeLoadException e) {
        throw new PersistenceException(String.Format(
          "Cannot load Type {0}, make sure all required assemblies are available.",
          (string)it.Current.Value), e);
      }
    }

    public void Populate(object instance, IEnumerable<Tag> objects, Type type) {
      // Type ojects are populated during instance creation.
    }
  }
}
